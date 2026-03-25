using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DreamlessNightArrow : HJScarletProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;

        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public float CurSpeed = 0f;
        public float Ratios = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 1;
            Projectile.scale = 0;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 100;
        }
        public override void ProjAI()
        {
            if (Projectile.timeLeft > 80)
                Projectile.scale = Lerp(Projectile.scale, 0.8f, 0.04f);
            if (Projectile.timeLeft < 20)
                Projectile.scale = Lerp(Projectile.scale, 0f, 0.04f);

            Projectile.rotation = Projectile.SpeedAffectRotation();
            Projectile.velocity *= 0.92f;
            if (CurSpeed == 0f)
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(ToRadians(-8f), ToRadians(8f)));
            float clampValue = Clamp(Projectile.velocity.Length(), 0.2f, 1);
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3))
                return;
            if (Main.rand.NextFloat() < clampValue)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(3f), DustID.WitherLightning);
                d.noGravity = true;
                d.velocity = Projectile.velocity / 2;
                d.scale = 0.45f;
                d.color = Color.Purple;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i) * Projectile.scale;
                float colorAlpha = GetAlphaFade(1 - i);
                SB.Draw(star, Projectile.Center - Main.screenPosition, null, Color.Violet * colorAlpha, Projectile.rotation, star.Size() / 2, starScale, SpriteEffects.None, 0);
                SB.Draw(star, Projectile.Center - Main.screenPosition, null, Color.Violet * colorAlpha, Projectile.rotation + PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
            }
            HJScarletMethods.EndShaderAreaPixel();
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(1.2f, 0.8f);
            Vector2 beginScale = new(0.2f, 0.01f);
            return Vector2.Lerp(beginScale, starScale, t);
        }
    }
}
