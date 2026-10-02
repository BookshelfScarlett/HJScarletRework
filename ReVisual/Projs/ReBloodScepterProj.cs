using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using System;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReBloodScepterProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<BloodScepter>();
        protected override int TrailLength => 45;
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.GetReVisualPlayer().reVisualBloodScepter)
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

                projectile.velocity.Y += 0.2f;
                if (projectile.velocity.Y > 16f)
                {
                    projectile.velocity.Y = 16f;
                }
                ECSParticle.ShrinkParticle(projectile.Center.ToRandCirclePosEdge(6), projectile.velocity / 8f, RandLerpColor(Color.Red, Color.Crimson), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .12f, 1);
                if (Main.rand.NextBool())
                {
                    Vector2 orbVel = projectile.SafeDir().RotatedBy(Main.rand.NextFloat(ToRadians(5), ToRadians(10)) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(.4f, 1.1f) * 15f;
                    ECSParticle.HRShinyOrb(projectile.Center.ToRandCirclePos(4), orbVel,
                        RandLerpColor(Color.Red, Color.Crimson), 40, 1, 0.031f, 0);
                }

                
                return false;
            }
            return base.PreAI(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.GetReVisualPlayer().reVisualBloodScepter)
            {
                Texture2D line = HJScarletTexture.Particle_SharpTear;
                int count = OldPositionList.Count;
                float overallScale = 1f;
                float overallAlpha = 1f;

                //Copy-right:VFX
                for (int i = 0; i < count; i++)
                {
                    float progress = (float)i / count;

                    float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.45f) * 0.1f;

                    Vector2 AfterImagePos = OldPositionList[i] - Main.screenPosition + Main.rand.NextVector2Circular(4.5f, 4.5f); //6f

                    float startScale = 1.1f + sineScale;

                    float rot = OldRotationList[i] + PiOver2;
                    Color between = Color.Lerp(Color.OrangeRed, Color.Crimson, 0.15f);
                    Color col = Color.Lerp(between, Color.DarkRed, 1f - progress);

                    float easedFadeValue = progress * progress * overallAlpha;
                    Vector2 lineScale = new Vector2(0.3f + 0.4f * progress, 1.25f);
                    lineScale.Y *= overallScale;
                    Vector2 lineScale2 = new Vector2(0.05f + 0.1f * progress, 1.25f);
                    lineScale2.Y *= overallScale;
                    SB.FastDraw(line, AfterImagePos, Color.Black * .4f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                    SB.FastDraw(line, AfterImagePos, col.ToAddColor() * .985f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                    SB.FastDraw(line, AfterImagePos, Color.White.ToAddColor() * .95f * easedFadeValue, rot, line.Size() / 2f, lineScale2 * startScale, SpriteEffects.None);
                }
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
    public class ReBloodScepterProj2 : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<BloodScepter_2>();
        protected override int TrailLength => 45;
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.GetReVisualPlayer().reVisualBloodScepter)
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
                ECSParticle.ShrinkParticle(projectile.Center.ToRandCirclePosEdge(4), projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.White), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .12f, 1);
                if (Main.rand.NextBool())
                {
                    Vector2 orbVel = projectile.SafeDir().RotatedBy(Main.rand.NextFloat(ToRadians(5), ToRadians(10)) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(.4f, 1.1f) * 15f;
                    ECSParticle.HRShinyOrb(projectile.Center.ToRandCirclePos(4), orbVel,
                        RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1, 0.031f, 0);
                }

                projectile.velocity.Y += 0.2f;
                if (projectile.velocity.Y > 16f)
                {
                    projectile.velocity.Y = 16f;
                }

                return false;
            }
            return base.PreAI(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.GetReVisualPlayer().reVisualBloodScepter)
            {
                Texture2D line = HJScarletTexture.Particle_SharpTear;
                int count = OldPositionList.Count;
                float overallScale = 1f;
                float overallAlpha = 1f;

                //Copy-right:VFX
                for (int i = 0; i < count; i++)
                {
                    float progress = (float)i / count;

                    float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.45f) * 0.1f;

                    Vector2 AfterImagePos = OldPositionList[i] - Main.screenPosition + Main.rand.NextVector2Circular(4.5f, 4.5f); //6f

                    float startScale = 1.1f + sineScale;

                    float rot = OldRotationList[i] + PiOver2;
                    Color between = Color.Lerp(Color.Silver, Color.WhiteSmoke, 0.15f);
                    Color col = Color.Lerp(between, Color.White, 1f - progress);

                    float easedFadeValue = progress * progress * overallAlpha;
                    Vector2 lineScale = new Vector2(0.3f + 0.4f * progress, 1.25f);
                    lineScale.Y *= overallScale;
                    Vector2 lineScale2 = new Vector2(0.05f + 0.1f * progress, 1.25f);
                    lineScale2.Y *= overallScale;
                    SB.FastDraw(line, AfterImagePos, Color.Black * .4f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                    SB.FastDraw(line, AfterImagePos, col.ToAddColor() * .985f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                    SB.FastDraw(line, AfterImagePos, Color.White.ToAddColor() * .95f * easedFadeValue, rot, line.Size() / 2f, lineScale2 * startScale, SpriteEffects.None);
                }
                Texture2D crossGlow = HJScarletTexture.Particle_HRShinyOrbSmall.Value;

                Vector2 pos = projectile.Center - Main.screenPosition;
                float glowScale = overallScale * .4f * projectile.scale;
                Vector2 glowVec = glowScale * new Vector2(1f, 1.5f);
                SB.EnterShaderArea();
                SB.FastDraw(crossGlow, pos + projectile.rotation.ToRotationVector2() * 10f, Color.White*overallAlpha*.945f, projectile.rotation+PiOver2, crossGlow.Size() / 2f, glowVec, 0);
                SB.FastDraw(crossGlow, pos + projectile.SafeDir() * 20f, Color.White*overallAlpha*.945f, projectile.rotation+PiOver2, crossGlow.Size() / 2f, glowVec, 0);
                SB.EndShaderArea();
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
