using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class CobaltYoyoSlash : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void ExSD()
        {
            Projectile.MaxUpdates = 3;
            Projectile.SetupImmnuity(10 * Projectile.MaxUpdates);
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            if (Projectile.HJScarlet().CurStoredTarget.IsLegal())
            {
                Projectile.timeLeft = 2;
                Projectile.Center = Projectile.HJScarlet().CurStoredTarget.Center;
            }
            else
                Projectile.Kill();
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.HJScarlet().CurStoredTarget.IsLegal() && target.Equals(Projectile.HJScarlet().CurStoredTarget))
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 35; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1f, 12f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 25, RandRotTwoPi, .75f, Projectile.scale * Main.rand.NextFloat(.9f, 1.15f) * .24f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 35; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1f, 4.5f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 25, 1, Projectile.scale * Main.rand.NextFloat(.90f, 1.05f) * .75f, .2f);
            }
            ScarletSound(SoundID.DD2_GoblinBomb, Projectile.Center, volume: .75f, pitch: .6f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 0.62f;
            SB.EnterShaderArea();
            SB.FastDraw(tex, pos, Color.RoyalBlue, 0, tex.Size() / 2f, scale, 0);
            SB.FastDraw(tex, pos, Color.LightSkyBlue, 0, tex.Size() / 2f, scale * .95f, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
