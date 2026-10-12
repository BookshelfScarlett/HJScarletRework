using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReLiquidBowBolt : ReVisualProjectile 
    {
        protected override int ApplyProjectile => ProjectileType<LiquidBow.LiquidBowBolt>();
        protected override int TrailLength => 12;
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.GetReVisualPlayer().reVisualLiquidBow)
            {
                if (TrailLength > 0)
                {
                    projectile.rotation = projectile.velocity.ToRotation();
                    OldRotationList.Add(projectile.rotation);
                    OldPositionList.Add(projectile.Center);
                    if (OldRotationList.Count > TrailLength)
                        OldRotationList.RemoveAt(0);
                    if (OldPositionList.Count > TrailLength)
                        OldPositionList.RemoveAt(0);
                }
                //复制旅人归途的AI，但做出了一定的改动
                if (Main.player[projectile.owner].magicQuiver)
                {
                    projectile.extraUpdates = 1;
                }
                else
                {
                    projectile.extraUpdates = 0;
                }

                ECSParticle.ShrinkParticle(projectile.Center.ToRandCirclePosEdge(6)-projectile.SafeDir()*Main.rand.NextFloat(.2f,1f)*10f, projectile.velocity / 8f, RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .12f, 1);
                if (Main.rand.NextBool())
                {
                    Vector2 orbVel = projectile.SafeDir().RotatedBy(Main.rand.NextFloat(ToRadians(1.5f), ToRadians(3.5f)) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(.4f, 1.1f) * 2f;
                    ECSParticle.HRShinyOrb(projectile.Center.ToRandCirclePos(4) -projectile.SafeDir()*Main.rand.NextFloat(.2f,1f)*10f, orbVel,
                        RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 40, 1, 0.051f, 0.5f);
                }
                return false;

            }
            return base.PreAI(projectile);
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if(projectile.GetReVisualPlayer().reVisualLiquidBow)
            {
                for (int i = 0; i < 6; i++)
                {
                    ECSParticle.ShrinkParticle(projectile.Center.ToRandCirclePos(13), RandVelTwoPi(2,6)*1.3f, RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 80, 1,
                        RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .12f, 1);
                    ECSParticle.HRShinyOrb(projectile.Center.ToRandCirclePos(10), RandVelTwoPi(2,6)*1.3f,
                        RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 80, 1, 0.051f, 0.5f);
                }
                            SoundEngine.PlaySound( SoundID.Item10, projectile.position);

                return false;
            }
            return base.PreKill(projectile, timeLeft);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.GetReVisualPlayer().reVisualLiquidBow)
            {
                Texture2D starShape = HJScarletTexture.Particle_SharpTear;
                int length = OldPositionList.Count;
                for (int i = 0; i < length; i++)
                {
                    float progress =1- i / (float)length;
                    Vector2 oldPos = OldPositionList[i] - Main.screenPosition;
                    float oldRot = OldRotationList[i] + PiOver2;
                    Vector2 scale = new Vector2(Lerp(0.25f, 0.08f, progress), Lerp(1.05f, 0.30f, progress));
                    Vector2 pos = oldPos;
                    SB.Draw(starShape, pos, null, Color.Lerp(Color.DeepSkyBlue, Color.MidnightBlue, progress/2f).ToAddColor(0), oldRot, starShape.Size() / 2f, scale, 0, 0);
                    SB.Draw(starShape, pos, null, Color.White.ToAddColor(0) *(1- progress), oldRot, starShape.Size() / 2f, scale * 0.75f, 0, 0);
                }
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
    public class ReLiquidBowProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<LiquidBow>();
        protected override int TrailLength => 18;
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.GetReVisualPlayer().reVisualLiquidBow)
            {
                if (TrailLength > 0)
                {
                    projectile.rotation = projectile.velocity.ToRotation();
                    OldRotationList.Add(projectile.rotation);
                    OldPositionList.Add(projectile.Center);
                    if (OldRotationList.Count > TrailLength)
                        OldRotationList.RemoveAt(0);
                    if (OldPositionList.Count > TrailLength)
                        OldPositionList.RemoveAt(0);
                }
                //复制旅人归途的AI，但做出了一定的改动
                if (projectile.localAI[1] > 1f)

                {
                    projectile.penetrate = 0;
                    return false;
                }
                //这里的改动是，在原AI的情况下，发射出去的箭矢直接不发光导致的违和
                //这里提高了点亮度
                Lighting.AddLight(projectile.Center,Color.SkyBlue.ToVector3()*1);
                Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
                if (projectile2.numHits > 0)
                {
                    projectile.localAI[1] = 5f;
                }
                else if (projectile2.active && projectile2.friendly && (float)projectile2.type == projectile.ai[1])
                {
                    projectile.Center = projectile2.Center - projectile2.velocity;
                    projectile.velocity = projectile2.velocity;
                    projectile.width = projectile2.width;
                    projectile.height = projectile2.height;
                }
                else
                {
                    projectile.localAI[1] = 5f;
                }
                //粒子
                ECSParticle.ShrinkParticle(projectile.Center.ToRandCirclePosEdge(6), projectile.velocity / 8f, RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .12f, 1);
                if (Main.rand.NextBool())
                {
                    Vector2 orbVel = projectile.SafeDir().RotatedBy(Main.rand.NextFloat(ToRadians(5), ToRadians(10)) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(.4f, 1.1f) * 15f;
                    ECSParticle.HRShinyOrb(projectile.Center.ToRandCirclePos(4), orbVel,
                        RandLerpColor(Color.DeepSkyBlue, Color.LightSkyBlue), 40, 1, 0.051f, .5f);
                }
                return false;

            }
            return base.PreAI(projectile);
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if(projectile.GetReVisualPlayer().reVisualLiquidBow)
            {
                for (int i = 0; i < 6; i++)
                {
                    ECSParticle.ShrinkParticle(projectile.Center.ToRandCirclePos(13), RandVelTwoPi(2,6)*1.3f, RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 80, 1,
                        RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .12f, 1);

                    ECSParticle.HRShinyOrb(projectile.Center.ToRandCirclePos(10), RandVelTwoPi(2,6)*1.3f,
                        RandLerpColor(Color.DeepSkyBlue, Color.LightSkyBlue), 80, 1, 0.051f, .5f);
                }

                return true;
            }
            return base.PreKill(projectile, timeLeft);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.GetReVisualPlayer().reVisualLiquidBow)
            {
                PixelatedRenderManager.BeginDrawProj = true;
                Texture2D line = HJScarletTexture.Particle_SharpTear;
                int count = OldPositionList.Count;
                float overallScale = 1f;
                float overallAlpha = 1f;

                //Copy-right:VFX
                for (int i = 0; i < count; i++)
                {
                    float progress = (float)i / count;

                    float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.25f) * 0.1f;

                    Vector2 AfterImagePos = OldPositionList[i] - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f)*progress; //6f

                    float startScale = 1.1f + sineScale;

                    float rot = OldRotationList[i] + PiOver2;
                    Color between = Color.Lerp(Color.DeepSkyBlue, Color.Blue, 0.15f);
                    Color col = Color.Lerp(between, Color.LightSkyBlue, 1f - progress);

                    float easedFadeValue = progress * progress * overallAlpha;
                    Vector2 lineScale = new Vector2(0.13f + 0.4f * progress, 1.25f);
                    lineScale.Y *= overallScale;
                    Vector2 lineScale2 = new Vector2(0.05f + 0.1f * progress, 1.25f);
                    lineScale2.Y *= overallScale;
                    SB.FastDraw(line, AfterImagePos, Color.Black * .24f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                    SB.FastDraw(line, AfterImagePos, col.ToAddColor() * .985f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                    SB.FastDraw(line, AfterImagePos, Color.White.ToAddColor() * .95f * easedFadeValue, rot, line.Size() / 2f, lineScale2 * startScale, SpriteEffects.None);
                }
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }

    }
}
