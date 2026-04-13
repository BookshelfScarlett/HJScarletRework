using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class ClimaticHawstringArrow : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public Vector2 DrawOffset => Projectile.SafeDirByRot() * -10f;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforePlayer;
        public BlendState BlendState => BlendState.Additive;
        public List<Vector2> PosList = [];
        public bool ShouldAdd = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.SetupImmnuity(-1, 3);
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextFloat() < Projectile.scale && Projectile.FinalUpdateNextBool(3))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePosEdge(6f), Projectile.velocity / 6f, RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, Main.rand.NextFloat(0.24f, 0.35f) * Projectile.scale).Spawn();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HJScarlet().climaticHawstringLaserCounter < 20 && !ShouldAdd)
            {
                Owner.HJScarlet().climaticHawstringLaserCounter++;
                ShouldAdd = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = -Projectile.velocity.ToRandVelocity(ToRadians(10f), 6.1f, 9.8f);
                new StarShape(Projectile.Center.ToRandCirclePos(10f), vel, RandLerpColor(Color.DarkOrange, Color.LightGoldenrodYellow), Main.rand.NextFloat(0.385f, .401f) * 2f, 40).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            float rotFix = -PiOver2;
            Projectile.DrawGlowEdge(Color.White, rotFix: rotFix, drawPosOffset: Projectile.SafeDir() * 10f);
            Projectile.DrawProj(Color.White, rotFix: rotFix, drawPosOffset: Projectile.SafeDir() * 10f);
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            //给箭矢尖端画一个星星。
            SB.EnterShaderArea();
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i);
                float colorAlpha = GetAlphaFade(1 - i);
                SB.Draw(star, Projectile.Center - Main.screenPosition + Projectile.SafeDir() * 10f, null, Color.DarkGoldenrod * colorAlpha, Projectile.rotation+ PiOver4, star.Size() / 2, starScale, SpriteEffects.None, 0);
                SB.Draw(star, Projectile.Center - Main.screenPosition + Projectile.SafeDir() * 10f, null, Color.DarkGoldenrod* colorAlpha,Projectile.rotation+ PiOver4+ PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
            }
            SB.EndShaderArea();
            return false;
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.25f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(1.2f, 0.8f);
            Vector2 beginScale = new(0.2f, 0.05f);
            return Vector2.Lerp(beginScale, starScale, t) * Projectile.scale * 0.75f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            if (!Projectile.HJScarlet().FirstFrame)
                return;
            DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.DarkGoldenrod, 1.26f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.Goldenrod, 0.8f, 1f);
            DrawTrails(HJScarletTexture.Trail_MegaBeam.Texture, Color.White, 0.58f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -10.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            shader.Parameters["uFadeoutLength"].SetValue(0.80f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> _, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight * 0f;
                float ratios = 1;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, Color.White, new Vector2(0, 10 * ratios * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

        public void DrawTrail()
        {
            Vector2 projDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + projDir * 10f;
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            int length = 16;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Vector2 lerpPos = drawPos;
                Color drawColor = (Color.Lerp(Color.DarkGoldenrod, Color.Gold, rads) with { A = 255 }) * 0.9f * (1 - rads) * Projectile.scale * Projectile.Opacity;
                SB.Draw(star, lerpPos - projDir * i * 15f, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.rotation - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1.0f, 1.5f) * 0.85f, 0, 0);
                SB.Draw(star, lerpPos - projDir * 5f - projDir * i * 15f, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.rotation - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1.0f, 1.5f) * 0.85f, 0, 0);
            }
        }
    }
}
