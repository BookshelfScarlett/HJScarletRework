using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class ClimaticHawstringBeam : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;

        public float BeamLength = 200;
        public AnimationStruct Helper = new(3);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(-1, 4, true);
            Projectile.extraUpdates = 2;
            Projectile.scale = Projectile.Opacity = 0;
            Projectile.tileCollide = false;
            Projectile.timeLeft =300;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 10;
            Helper.MaxProgress[1] = 5;
        }
        public override bool ShouldUpdatePosition()
        {
            return true;
        }
        public override void ProjAI()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.scale = Lerp(Projectile.scale, 1.1f, 0.2f);
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.2f);
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextFloat() < Projectile.scale && Projectile.FinalUpdateNextBool(3))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePosEdge(6f), Projectile.velocity / 6f, RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, Main.rand.NextFloat(0.24f, 0.35f) * Projectile.scale).Spawn();

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = -Projectile.velocity.ToRandVelocity(ToRadians(10f), 6.1f, 9.8f);
                new StarShape(Projectile.Center.ToRandCirclePos(10f), vel, RandLerpColor(Color.DarkOrange, Color.LightGoldenrodYellow), Main.rand.NextFloat(0.385f, .401f) * 2f, 40).Spawn();
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D starShape = HJScarletTexture.Particle_ShinyOrb.Value;
            Vector2 scale = new Vector2(0.35f, 3.05f) * 0.825f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (int i  =0;i< 10;i++)
            {
                float ratios = (float)i / 10;
                Color lerpColor = Color.Lerp(Color.DarkGoldenrod, Color.Goldenrod, ratios) * 0.85f;
                if (i < 2 || i > 8)
                    scale = new Vector2(0.35f, 2.80f) * 0.825f;
                SB.Draw(starShape, drawPos - Projectile.SafeDir() * i * 3f, null, lerpColor, Projectile.rotation + PiOver2, starShape.ToOrigin(), Projectile.scale * scale, 0, 0);
                lerpColor = Color.Lerp(Color.Goldenrod, Color.LightGoldenrodYellow, ratios) * 0.85f;
                SB.Draw(starShape, drawPos - Projectile.SafeDir() * i * 6f, null, lerpColor * 0.85f, Projectile.rotation + PiOver2, starShape.ToOrigin(), Projectile.scale * scale, 0, 0);
                SB.Draw(starShape, drawPos - Projectile.SafeDir() * i * 3f, null, Color.White, Projectile.rotation + PiOver2, starShape.ToOrigin(), Projectile.scale * scale * 0.535f, 0, 0);
            }


            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawBeam(Color color, float height, Asset<Texture2D> value)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -1);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.4f);
            shader.Parameters["uFadeinLength"].SetValue(0.08f);
            shader.CurrentTechnique.Passes[0].Apply();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), height * 0.025f * Projectile.scale), 0, 0);
        }


    }
}
