using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class RefluxHeldProj : HJScarletRangedWeaponoutClass
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override float HoldoutDrawScale => 1;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Color HoldoutEdgeColor => Color.LimeGreen;
        public override Vector2 HoldoutOffset => new Vector2(20f,0);
        public override float RecoilPower => 15;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        public override int OriginalItemID => ItemType<Reflux>();
        public override string Texture => GetInstance<Reflux>().Texture;
        public override bool IsUsing => base.IsUsing || Projectile.HJScarlet().ExecutionStrike;
        protected override void UpdateRecoil()
        {
            base.UpdateRecoil();
        }
        protected override void PreAttack()
        {
            base.PreAttack();
            ScarletSound(HJScarletSounds.Shotgun_EvaAuto, Projectile.Center);
        }
        protected override void OnAttack()
        {
            Vector2 offset = new Vector2(20, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<RefluxBullet>();
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                type = ProjectileType<MoonfireBulletExecution>();
            }
            pos -= new Vector2(5, 0).RotatedBy(Projectile.rotation);
                        float randRot = Projectile.HJScarlet().ExecutionStrike ? ToRadians(4.5f) : ToRadians(17.5f);
            for (int i = 0; i < 5; i++)
            {
                Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, randomVelocity * 11f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            }
            if (Projectile.HJScarlet().ExecutionStrike)
            {
            }
            else
            {
            }
                ScreenShakeSystem.AddScreenShakes(pos, 15, 15, -Projectile.SafeDirByRot().ToRotation(), 0, true, easingFunc: EaseOutExpo);

            pos = Projectile.Center + offset;
            //震屏，粒子特效
            Vector2 particleOffset = new Vector2(-55, 0 * Projectile.direction).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(5) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(20), .1f, 11.6f);
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.SmokeParticle(pos2, vel, RandLerpColor(Color.LimeGreen, Color.DarkGreen), timeLeft, RandRotTwoPi, .55f, Main.rand.NextFloat(.9f, 1.1f) * .30f,blendstate:BlendState.AlphaBlend);
                //ECSParticle.GlowSquare(pos2, vel, RandLerpColor(Color.LimeGreen, Color.Lime), timeLeft, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 0.71f, 0, Main.rand.NextFloat(-.08f, 0.09f), 1f);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(3) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(20), .1f, 9.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.LimeGreen, Color.DarkGreen), 45, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            }
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 24; i++)
                {
                    bool alt = Main.rand.NextBool();
                    BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                    ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(15), 0.4f, 21.4f), RandLerpColor(Color.Green, Color.LimeGreen), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.33f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
                }
            }
        }
    }
}
