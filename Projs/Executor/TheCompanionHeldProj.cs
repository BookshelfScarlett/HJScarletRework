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
    public class TheCompanionHeldProj : HJScarletRangedWeaponoutClass
    {
        public override int OriginalItemID => ItemType<TheCompanion>();
        public override string Texture => GetInstance<TheCompanion>().Texture;
        public override Vector2 HoldoutOffset => new Vector2(19, -2.5f);
        public override float HoldoutDrawScale => .85f;
        public override Color HoldoutEdgeColor => Color.White;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override float RecoilPower => 5f;
        public override bool IsUsing => ((Owner.channel) && !Owner.noItems && !Owner.CCed);
        public int NextBulletTileCollidingTime = 0;
        /// <summary>
        /// 复写这个是因为月火的特殊处决攻击，需要让月火在玩家按下处决键（互动键）时立刻发射一枚子弹
        /// <br>而这个钩子会在执行完毕后立刻执行攻击的判定，即上方的<see cref="IsUsing"/></br>
        /// </summary>
        protected override void UpdateRecoil()
        {
            //保留原本的自动管理方案
            base.UpdateRecoil();
        }
        protected override void PostAttack()
        {
            RecoilTimer = AttackSpeed;
            Timer = 0;
        }
        protected override void UpdateGlobalReset()
        {
            //计时器的重置
            if (RecoilTimer > 0)
            {
                RecoilTimer--;
            }
            Projectile.HJScarlet().ExecutionStrike = false;
        }
        /// <summary>
        /// 我也不知道我这攻击写了个啥
        /// </summary>
        protected override void OnAttack()
        {
            MoonfireExecutionCheck();
            Vector2 offset = new Vector2(20, -10 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<TheCompanionBullet>();
            pos -= new Vector2(5, 0).RotatedBy(Projectile.rotation);
            Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), pos, dir * 18f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                proj.HJScarlet().ExecutionStrike = true;
                proj.extraUpdates += 1;
                ScarletSound(HJScarletSounds.Misc_PistolClear, Projectile.Center, .13f);
                ScreenShakeSystem.AddScreenShakes(pos, 15, 15, -Projectile.SafeDirByRot().ToRotation(), RandRotTwoPi, true, easingFunc: EaseOutExpo);
            }
            else
            {
                ScarletSound(HJScarletSounds.Misc_Pistol, Projectile.Center, 0.13f, 0, .2f, 0.1f);
                //ScarletSound(HJScarletSounds.Misc_Pistol, Projectile.Center, .75f, 1, -.2f, 0.1f);
                //ScreenShakeSystem.AddScreenShakes(pos, 5, 5, -Projectile.SafeDirByRot().ToRotation(), 0, true, easingFunc: EaseOutExpo);
                proj.HJScarlet().HasExecutionMechanic = true;
            }

            pos = Projectile.Center + offset;
            //震屏，粒子特效
            Vector2 particleOffset = new Vector2(0, 0 * Projectile.direction).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(10), .1f, 10.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.28f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarSmall(pos2, vel, RandLerpColor(Color.LightGoldenrodYellow, Color.Gold), timeLeft, 1, scale, Main.rand.NextFloat(-.1f, .1f));
            }
            for (int i = 0; i < 10; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.1f, 12.4f), RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.21f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }

        }
        protected override void UpdateHeldProjectile()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation, 1);
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
        }
        /// <summary>
        /// 月火的处决模式
        /// </summary>
        public void MoonfireExecutionCheck()
        {
            if (!Owner.IsHolding(OriginalItemID))
                return;
            if (Owner.GetExecutionSrike() && !Projectile.HJScarlet().ExecutionStrike)
            {
                //归一化比率
                //将武器标记为发起处决模式
                Projectile.HJScarlet().ExecutionStrike = true;
                //移除处决进程
                Owner.RemoveExecutionProgress();
                Owner.HJScarlet().tacticalExecutionInputCache = 0;
                //处决会强行发射这枚子弹
                Timer = AttackSpeed;
            }
        }

    }

}
