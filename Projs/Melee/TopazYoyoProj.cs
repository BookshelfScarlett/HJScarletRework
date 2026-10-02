using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public abstract class GemYoyoProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        protected virtual float YoyoTopSpeed => 10f;
        protected virtual float YoyoLength => HJScarletMethods.TilePixel(13.4375f);
        protected virtual float YoyoLifeTime => 300;
        protected virtual int YoyoMaxUpdates => 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = YoyoLifeTime;
            ProjectileID.Sets.YoyosMaximumRange[Type] = YoyoLength;
            ProjectileID.Sets.YoyosTopSpeed[Type] = YoyoTopSpeed / YoyoMaxUpdates;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = YoyoMaxUpdates;
        }
    }
    public class TopazYoyoProj : GemYoyoProj
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
            Projectile.tileCollide = false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle rec = Utils.CenteredRectangle(Projectile.Center, new Vector2(64));
            return rec.Intersects(targetHitbox);
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Yellow);
            base.ProjAI();
            ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(8), RandVelTwoPi(1), RandLerpColor(Color.Yellow, Color.Gold), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .13f, 1);
            if (Projectile.velocity.LengthSquared() > 2.5f * 2.5f)
            {
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 8f, RandLerpColor(Color.Yellow, Color.Gold), 40, 1, Projectile.velocity.ToRotation(), 0.16f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ECSParticle.LightntingGlow(target.Center, Projectile.rotation.ToRotationVector2() * .01f, Color.Gold, 40, 1, 0.6f);
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(3), RandLerpColor(Color.Yellow, Color.Gold), 45, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);

            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
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
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.DarkGoldenrod, Color.Gold, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .59f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.DarkGoldenrod, Color.Gold, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(5, 5), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.Gold.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
    }
}
