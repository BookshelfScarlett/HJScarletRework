using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class RitualofReposeStar : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.DivingProjectile[Type] = true;
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = GetSeconds(5) * Projectile.MaxUpdates;
        }

        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.damage != 0 && Projectile.timeLeft > 50)
            {
                Projectile.scale = Lerp(Projectile.scale, 1.01f, .12f);
                if (Projectile.scale > 1 && Timer > 45)
                {
                    NPC CurTarget = Projectile.HJScarlet().CurStoredTarget;
                    if (CurTarget.IsLegal())
                    {
                        float speedValue = Projectile.velocity.Length();
                        float rotation = Projectile.velocity.ToRotation();
                        float angleTo = Projectile.AngleTo(CurTarget.Center);
                        float dist = Projectile.Distance(CurTarget.Center);
                        float r = dist * 0.30f / (float)Math.Abs(Math.Sin(rotation - angleTo));
                        if (Vector2.Dot(Projectile.velocity, Projectile.DirectionTo(CurTarget.Center)) < 0)
                        {
                            r = Clamp(r, 1, 240);
                        }
                        Projectile.velocity = Projectile.velocity.RotatedBy(-Math.Sign(WrapAngle(rotation - angleTo)) * speedValue / r);
                        if (Projectile.velocity.LengthSquared() < 11f * 11f)
                            Projectile.velocity *= 1.01f;
                        else
                            Projectile.velocity *= 0.9f;
                    }
                    else
                    {
                        if (Projectile.GetTargetSafe(out NPC tar, true, 1200f, true))
                        {
                            Projectile.HJScarlet().CurStoredTarget = tar;
                        }
                    }
                }
                else
                {
                    if (Projectile.velocity.LengthSquared() >= 12f * 12f)
                        Projectile.velocity *= 0.9f;
                }
            }
            else
            {
                Projectile.scale = Lerp(Projectile.scale, 0f, .12f);
                if (Projectile.scale <= 0.2f)
                    Projectile.Kill();
            }

            Timer++;

            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), -Vector2.UnitY, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            if (Main.rand.NextBool())
                ECSParticle.CrossGlow(Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(.1f, .3f), RandLerpColor(Color.LightGoldenrodYellow, Color.Goldenrod), 40, 1f, 0, Main.rand.NextFloat(.85f, 1.15f) * Projectile.scale * .041f, fadinTime: .2f);

        }
        public override bool? CanDamage()
        {
            return Timer > 45;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Projectile.ApplyDirectionOverride(target);
        }


        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = Color.Lerp(Color.LightGoldenrodYellow, Color.Gold, Projectile.localAI[1] / 6f);
            float generalScale = 8f * Projectile.Opacity;
            Vector2 scale = new Vector2(1.02f, 1.72f) * .024f * generalScale * Projectile.scale;
            Vector2 orig = tex.Size() / 2;

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkGoldenrod, 1.26f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Goldenrod, 0.8f, 1f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f, 1f, 0.95f);
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            for (int i = 0; i < 2; i++)
                SB.Draw(tex, pos, null, c * Projectile.Opacity, PiOver2 * i, orig, scale, 0, 0);
            Texture2D orb = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            SB.Draw(orb, pos, null, Color.Gold * .64f, 0, orb.Size() / 2f, .125f * generalScale, 0, 0);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawCoreStar(SpriteBatch sb)
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i) * Projectile.scale;
                float colorAlpha = GetAlphaFade(1 - i);
                Color drawColor = Color.Lerp(Color.DarkGoldenrod * colorAlpha, Color.LightGoldenrodYellow * colorAlpha, colorAlpha);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation + PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.LightGoldenrodYellow * colorAlpha, Projectile.rotation, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.LightGoldenrodYellow * colorAlpha, Projectile.rotation + PiOver2, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
            }
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.05f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 16 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }

        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(0.9f, 1.4f);
            Vector2 beginScale = new(0.1f, 0.2f);
            return Vector2.Lerp(beginScale, starScale, t) * 1f;
        }
    }
}
