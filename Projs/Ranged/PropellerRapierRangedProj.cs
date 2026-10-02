using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class PropellerRapierRangedProj : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override string Texture => GetInstance<PropellerRapierRanged>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = 2;
            Projectile.SetupImmnuity(10 * Projectile.maxPenetrate);
            Projectile.penetrate = 16;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3))
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 4f, Color.White, 50, .75f, Projectile.velocity.ToRotation(), Main.rand.NextFloat(.9f, 1.1f) * .164f, 1, squashScale: new Vector2(1.4f, 0.96f), 1.01f, 0f);
            if (Main.rand.NextBool(3))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 4f, RandLerpColor(Color.LightSkyBlue, Color.White), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D tex, out Vector2 drawPos, out Vector2 ori);
            float rot = Projectile.rotation + PiOver4;
            DrawProjTrail(tex);
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos.ToRandCirclePos(1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), rot, ori, Projectile.scale, 0);
            SB.FastDraw(tex, drawPos, Color.White, rot, ori, Projectile.scale, 0);
            return false;
        }
        public void DrawProjTrail(Texture2D tex)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver4;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.LightSkyBlue, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .59f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Lime, Color.LightSkyBlue, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, tex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(tex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
        }

    }
}
