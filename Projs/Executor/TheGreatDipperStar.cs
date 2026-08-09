using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Misc;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheGreatDipperStar : HJScarletProj,IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.Particle_GlowStar.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public float OriginalSpeed = 0;
        public ref float Osci => ref Projectile.ai[1];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.scale = 0f;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            OriginalSpeed = Projectile.velocity.Length();
        }
        public override void ProjAI()
        {
            if (Projectile.damage != 0 && Projectile.timeLeft >50)
            {
                Projectile.scale = Lerp(Projectile.scale, 1.01f, .12f);
            }
            else
                Projectile.scale = Lerp(Projectile.scale, 0f, .12f);
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), -Vector2.UnitY, RandLerpColor(Color.SkyBlue, Color.DodgerBlue), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            for (int i = 0; i < 3; i++)
                ECSParticle.CrossGlow(Projectile.Center.ToRandCirclePos(6) + i * Projectile.SafeDir() * 5f, RandVelTwoPi(.1f, .3f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1f, 0, Main.rand.NextFloat(.85f, 1.15f) * Projectile.scale * .041f, fadinTime: .2f);
        }
        public override void OnKill(int timeLeft)
        {
            float centerGlowScale = .12f;
            ECSParticle.CrossGlow(Projectile.Center, Color.SkyBlue, 45, 1, centerGlowScale);
            ECSParticle.CrossGlow(Projectile.Center, Color.LightSkyBlue, 45, 1, centerGlowScale * .98f);
            ECSParticle.CrossGlow(Projectile.Center, Color.White, 45, 1, centerGlowScale * .96f);

            for (int i = 0; i < 3; i++)
            {
                Color color = RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue);
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .05f + i * 0.042f, Projectile.whoAmI, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 10; i++)
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(8), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate<TheGreatDipper>();
            Projectile.velocity *= .001f;
            Projectile.timeLeft = 5;
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = Color.Lerp(Color.LightSkyBlue, Color.SkyBlue, Projectile.localAI[1] / 6f);
            float generalScale = 8f * Projectile.Opacity;
            Vector2 scale = new Vector2(1.02f, 1.72f) * .024f * generalScale *Projectile.scale;
            Vector2 orig = tex.Size() / 2;

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DeepSkyBlue, 1.26f, 1f,1.1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.SkyBlue, 0.8f, 1f,1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f,1f,0.95f);
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            for (int i = 0; i < 2; i++)
                SB.Draw(tex, pos, null, c * Projectile.Opacity, PiOver2 * i, orig, scale, 0, 0);
            Texture2D orb = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            SB.Draw(orb, pos, null, Color.White * .4f, 0, orb.Size() / 2f, .125f * generalScale, 0, 0);
            HJScarletMethods.EndShaderAreaPixel();

        }
        public void DrawCoreStar(SpriteBatch sb)
        {
            Texture2D star = HJScarletTexture.Particle_GlowStar.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i) * Projectile.scale;
                float colorAlpha = GetAlphaFade(1 - i);
                Color drawColor = Color.Lerp(Color.DeepSkyBlue * colorAlpha, Color.SkyBlue * colorAlpha, colorAlpha);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation + PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.White * colorAlpha, Projectile.rotation, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.White * colorAlpha, Projectile.rotation + PiOver2, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
            }
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -40.2f * offsetHeight);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)((Projectile.oldPos.Length-6) * Clamp(Projectile.velocity.Length(), 0, 1));
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] == Vector2.Zero)
                    continue;
                float rot = Projectile.oldRot[j];
                trailDrawDates.Add(new(Projectile.oldPos[j] + Projectile.Size / 2 +Projectile.SafeDir() * 10f, drawColor, new Vector2(0, 13 * multipleSize * Projectile.scale), rot));
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
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
    }
}
