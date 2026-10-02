using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class AmethystYoyoProj : GemYoyoProj
    {
        public ref float BoosterTime => ref Projectile.ai[2];
        protected override float YoyoLifeTime => 5;
        protected override int YoyoMaxUpdates => 1;
        protected override float YoyoTopSpeed => 16;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle rec = Utils.CenteredRectangle(Projectile.Center, new Vector2(16 * 8));
            return rec.Intersects(targetHitbox);
        }
        public override void ProjAI()
        {
            base.ProjAI();
            ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(60), RandVelTwoPi(1), RandLerpColor(Color.Violet, Color.DarkViolet), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .13f, 1);
            ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(60), RandVelTwoPi(1), RandLerpColor(Color.Violet, Color.DarkViolet), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .13f, 1);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ECSParticle.LightntingGlow(target.Center, Projectile.rotation.ToRotationVector2() * .01f, Color.Violet, 40, 1, 0.6f);
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(3), RandLerpColor(Color.Violet, Color.DarkViolet), 45, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);

            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
            SapphireYoyoProj.DrawSmear(Projectile, Color.Violet, Color.Purple, 1);
            SapphireYoyoProj.DrawSmear(Projectile, Color.Violet, Color.Purple, 1.5f, Pi);
            SapphireYoyoProj.DrawSmear(Projectile, Color.Violet, Color.Purple, 2f, PiOver2);
            SapphireYoyoProj.DrawSmear(Projectile, Color.Violet, Color.Purple, 2.5f, -PiOver2);
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.DarkViolet, Color.Purple, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .59f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Violet, Color.Purple, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(5, 5), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.Violet.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
    }
}
