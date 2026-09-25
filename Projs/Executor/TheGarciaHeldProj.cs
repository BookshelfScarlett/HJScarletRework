using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheGarciaHeldProj : HJScarletRangedWeaponoutClass
    {
        public override string Texture => GetInstance<TheGarcia>().Texture;
        public override float RecoilPower => 35;
        public override int OriginalItemID => ItemType<TheGarcia>();
        public override float HoldoutDrawScale => base.HoldoutDrawScale;
        public override Color HoldoutEdgeColor => Color.WhiteSmoke;
        public override Vector2 HoldoutOffset => new(25, -5);
        protected override void PreAttack()
        {
            if (!Owner.GetExecutionSrike())
            {
                Projectile.HJScarlet().ExecutionStrike = false;
                Owner.AddExecutionTimeDirectly(OriginalItemID);
            }
            if (Owner.GetExecutionSrike() && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                ScarletSound(HJScarletSounds.Shotgun_EvaAuto, Projectile.Center);
                Owner.RemoveExecutionProgress(OriginalItemID);
            }
        }
        protected override void OnAttack()
        {
            ScarletSound(HJScarletSounds.Shotgun_Mastiff, Projectile.Center);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 16, 16, Projectile.rotation, ToRadians(2));

            Vector2 particleOffset = ((HoldoutOffset + new Vector2(0, -10)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 pos = Projectile.Center + particleOffset + dir * 35;
            float randRot = Projectile.HJScarlet().ExecutionStrike ? ToRadians(4.5f) : ToRadians(16.5f);
            for (int i = 0; i < 5; i++)
            {
                Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, randomVelocity * 16f, ProjectileType<TheGarciaBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.HJScarlet().HasExecutionMechanic = true;
                if (Projectile.HJScarlet().ExecutionStrike)
                {
                    proj.CritChance += 10;
                    proj.penetrate += 1;
                }
            }
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos2 = pos.ToRandCirclePos(8);
                    Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(10), .1f, 18.6f);
                    float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.28f;
                    int timeLeft = Main.rand.Next(30, 45);
                    ECSParticle.ShinyCrossStarSmall(pos2, vel, RandLerpColor(Color.LightGoldenrodYellow, Color.Gold), 45, 1, scale, Main.rand.NextFloat(-.1f, .1f));
                }
                for (int i = 0; i < 26; i++)
                {
                    bool alt = Main.rand.NextBool();
                    BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                    ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.1f, 21.4f), RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.33f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
                }
            }
            else
            {
                for (int i = 0; i < 28; i++)
                {
                    Vector2 pos2 = pos.ToRandCirclePos(8);
                    Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(20), .1f, 10.6f);
                    float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.28f;
                    int timeLeft = Main.rand.Next(30, 45);
                    ECSParticle.ShinyCrossStarSmall(pos2, vel, RandLerpColor(Color.LightGoldenrodYellow, Color.Gold), timeLeft, 1, scale, Main.rand.NextFloat(-.1f, .1f));
                }
                for (int i = 0; i < 20; i++)
                {
                    bool alt = Main.rand.NextBool();
                    BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                    ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(20), 0.1f, 12.4f), RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.33f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
                }

            }
        }
    }
}
