using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using rail;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics;

namespace HJScarletRework.Projs.Magic
{
    public class BrimstoneHeartFireball : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public float MaxTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 3;
            Projectile.width = Projectile.height = 32;
            Projectile.ignoreWater = true;
            Projectile.scale = 0;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            MaxTime = Main.rand.Next(30, 60);
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.scale = Lerp(Projectile.scale, 1.01f, .1f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(2))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePosEdge(10), Projectile.velocity / 6f, Color.DarkRed*1.2f, Main.rand.Next(20, 45), RandRotTwoPi, 1, 0.25f, true, BlendState.Additive);
            if (Main.rand.NextBool(2))
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePosEdge(10), Projectile.velocity / 6f, RandLerpColor(Color.DarkRed, Color.Crimson), Main.rand.Next(20, 45), 1, RandRotTwoPi, 0.4f, 1,blendstate:BlendState.Additive);
            if (Main.rand.NextBool())
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16), 1.2f, RandLerpColor(Color.Red, Color.DarkRed), 45, 1, .1f, RandRotTwoPi, .40f);
            }
            float maxtime = Projectile.MaxUpdates * MaxTime;
            Timer++;
            if (Timer > maxtime)
                Timer = maxtime;
            if (Projectile.GetTargetSafe(out NPC target, true, searchDistance: 300, false))
            {
                Projectile.HomingTarget(target.Center, -1, 13f, Lerp(20f, 1.5f, Timer / maxtime),90);
            }
            if (Projectile.velocity.LengthSquared() < 13f * 13f)
                Projectile.velocity *= 1.1f;
            else
                Projectile.velocity *= .9f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<BrimstoneHeartBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public BlendState BlendState => BlendState.AlphaBlend;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch) { }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Texture2D fireball = HJScarletTexture.Texture_FireBall.Value;
            Texture2D fireballPixel = HJScarletTexture.Texture_FireBallPixel.Value;
            Texture2D glow = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int length = Projectile.oldPos.Length - 16;
            SB.EnterShaderArea();
            SB.FastDraw(glow, drawPos, Color.Crimson, 0, glow.Size() / 2f, Projectile.scale * .5f, 0);
            SB.EndShaderArea();
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .05f, (progress));
                float yMult = Lerp(1.2f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.Crimson, Color.Lerp(Color.White,Color.DarkRed,.63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .9f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.Crimson, Color.Lerp(Color.DarkRed, Color.Red, .63f), EaseInOutQuad(progress));
                SB.FastDraw(fireballPixel, oldPos, pixelColor.ToAddColor(100) * opac * 1.15f, oldRot, fireballPixel.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(fireball, oldPos + Main.rand.NextVector2Circular(10, 10) * (1 - progress), c.ToAddColor() * opac *1.5f*progress, oldRot, fireball.Size() / 2f, scale, 0);
                
            }
            SB.FastDraw(fireball, drawPos + Main.rand.NextVector2Circular(10, 10), Color.DarkRed.ToAddColor() * 1.5f, Projectile.rotation + PiOver2, fireball.Size() / 2f, Projectile.scale * new Vector2(1f, 1.2f), 0);
            SB.FastDraw(fireball, drawPos, Color.White.ToAddColor(0) * .855f, Projectile.rotation + PiOver2, fireball.Size() / 2f, Projectile.scale * .95f * new Vector2(1f, 1.2f), 0);

            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(offsetHeight*Main.GlobalTimeWrappedHourly*-.1f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            shader.Parameters["uFadeoutLength"].SetValue(.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.31f);
            shader.CurrentTechnique.Passes[0].Apply();

            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = Projectile.oldPos.Length;
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j].Equals(Vector2.Zero))
                    continue;
                float rot = Projectile.oldRot[j];
                trailDrawDates.Add(new(Projectile.oldPos[j] + Projectile.Size / 2, drawColor, new Vector2(0, 15 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
