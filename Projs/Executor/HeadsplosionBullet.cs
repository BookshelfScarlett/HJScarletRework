using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class HeadsplosionBullet : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 8;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            float glowScale = .45f;
            ECSParticle.LightntingGlow(Projectile.Center, Projectile.SafeDir(), Color.Goldenrod, 10, 1, glowScale);
            ECSParticle.LightntingGlow(Projectile.Center, Projectile.SafeDir(), Color.White, 10, 1, glowScale * .50f);
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8f, RandLerpColor(Color.DarkGoldenrod, Color.White), 40, 1, 0.74f, .2f);
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 pos = Projectile.Center;
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 11.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int time = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, RandVelTwoPi(0, 16), RandLerpColor(Color.Goldenrod, Color.White), time, 1, scale);
            }
            for (int i = 0; i < 26; i++)
            {
                ECSParticle.HRShinyOrb(pos, RandVelTwoPi(0, 16), RandLerpColor(Color.Goldenrod, Color.White), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .16f, .4f);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(3);
                Vector2 vel = RandVelTwoPi(0.1f, 16f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * .61f;
                int time = Main.rand.Next(30, 45);
                ECSParticle.Stain(pos2, vel, RandLerpColor(Color.Goldenrod, Color.LightGoldenrodYellow), 90, 1, vel.ToRotation(), scale);
                ECSParticle.Stain(pos2, vel, Color.White, 90, 1, vel.ToRotation(), scale * .65f);
            }
            float crossGlowScale = .4f;
            ECSParticle.CrossGlow(pos, Color.Gold, 40, 1, crossGlowScale, .2f);
            ECSParticle.CrossGlow(pos, Color.LightGoldenrodYellow, 40, 1, crossGlowScale * .98f, .2f);
            ECSParticle.CrossGlow(pos, Color.White, 40, 1, crossGlowScale * .95f, .2f);
            ScarletSound(HJScarletSounds.Misc_Boom, Projectile.Center, variantType: 4);
            for (int i = 0; i < 2; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(14f, 17f), ProjectileType<HeadsplosionBombBullet>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            }
            Projectile.Resize(160, 160);
            Projectile.Damage();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
