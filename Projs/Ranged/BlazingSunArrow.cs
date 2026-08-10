using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class BlazingSunArrow : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(35);
            Projectile.penetrate = 4;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), Projectile.SafeDir() * Main.rand.NextFloat(.1f, 7f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 1, 0.27f, .2f);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6), Projectile.SafeDir() * Main.rand.NextFloat(.1f, 13f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, RandRotTwoPi, 1, 0.24f, blendstate: BlendState.Additive);
            }
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(4), DustID.Torch);
                d.velocity = Projectile.velocity / 8f;
                d.noGravity = true;
                d.scale = 1.02f;
            }
            if (Main.rand.NextBool(3))
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8f, RandLerpColor(Color.White, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.20f, blendstate: BlendState.Additive);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 4.2f), Color.OrangeRed, 40, 1, 0.36f);
            }
            for (int i = 0; i < 12; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 6.2f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, 1, 0.9f, 0.31f, blendstate: BlendState.Additive);
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, Color.OrangeRed, .75f, offsetHeight: 1.25f);
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, Color.Orange, .75f, offsetHeight: 1.5f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.OrangeRed, 0.55f, .84f, offsetHeight: 2);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.45f, .85f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 200;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50.2f * offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.6f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();
            //做掉可能存在的零向量
            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Projectile.Opacity);
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + Projectile.Size / 2f + Projectile.SafeDir() * 10f - new Vector2(0, -1.15f).RotatedBy(Projectile.rotation);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 20 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float drawRot = Projectile.rotation - PiOver2;
            int length = Projectile.oldPos.Length - 6;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1 - i / (float)(length);
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float oldRot = Projectile.oldRot[i] - PiOver2;
                Color c = Color.Lerp(Color.OrangeRed, Color.White, ratios).ToAddColor(100);
                float scale = Lerp(.2f, 1f, ratios);
                SB.FastDraw(projTex, oldPos, c * ratios, oldRot, projTex.Size() / 2f, scale, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.35f, Color.White.ToAddColor(), drawRot, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, drawRot, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
    }
}
