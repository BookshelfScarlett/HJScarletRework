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
    public class MonocleHeldProj : HJScarletRangedWeaponoutClass
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override int OriginalItemID => ItemType<Monocle>();
        public override float RecoilPower => 32f;
        public override float HoldoutDrawScale => .65f;
        public override Vector2 HoldoutOffset => new(40, 0);
        public override Color HoldoutEdgeColor => Color.Violet;
        public override int ProjExtraUpdates => 2;
        protected override void PreAttack()
        {
            if (!Owner.GetExecutionSrike())
                Projectile.HJScarlet().ExecutionStrike = false;
            if (Owner.GetExecutionSrike() && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Owner.RemoveExecutionProgress(OriginalItemID);
            }
        }
        protected override void OnAttack()
        {
            Vector2 offset = new Vector2(90, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<MonocleBullet>();
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                type = ProjectileType<MonocleBulletExecution>();
            }
            pos -= new Vector2(80, 0).RotatedBy(Projectile.rotation);
            Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), pos, dir * 18f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            proj.HJScarlet().HasExecutionMechanic = true;
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                ScarletSound(HJScarletSounds.ASMD_ExecutionFire, Projectile.Center, 0.30f, 0, .24f, 0.1f);
                ScreenDarknessSystem.AddScreenDarkness(0.75f, 20);
            }
            else
                ScarletSound(HJScarletSounds.ASMD_Fire, Projectile.Center, 0.20f, 0, .34f, 0.1f);

            pos = Projectile.Center + offset;
            //震屏，粒子特效
            ScreenShakeSystem.AddScreenShakes(pos, 12 + Projectile.HJScarlet().ExecutionStrike.ToInt() * 12, 60, -Projectile.SafeDirByRot().ToRotation(), 0, true, easingFunc: EaseOutExpo);
            Vector2 particleOffset = new Vector2(10, 0 * Projectile.direction).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 11.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.Violet, Color.Purple), timeLeft, 1, scale);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(3) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(0), .1f, 19.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.LightntingGlow(pos2, vel, RandLerpColor(Color.Purple, Color.Violet), timeLeft, 1, scale);
            }
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.HighResolutionThunder(pos.ToRandCirclePos(3) - particleOffset, Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5), .1f, .2f), RandLerpColor(Color.Violet, Color.Purple), 45, 1, Projectile.SafeDirByRot().ToRotation(), 0.12f, 1);
            }
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 24; i++)
                {
                    bool alt = Main.rand.NextBool();
                    BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                    ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(15), 0.4f, 21.4f), RandLerpColor(Color.Violet, Color.White), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.33f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
                }
            }
        }
    }
}
