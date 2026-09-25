using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class MoonfireHeldProj : HJScarletRangedWeaponoutClass
    {
        public override int OriginalItemID => ItemType<Moonfire>();
        public override string Texture => GetInstance<Moonfire>().Texture;
        public override Vector2 HoldoutOffset => new Vector2(16, -2.5f);
        public override float HoldoutDrawScale => .85f;
        public override Color HoldoutEdgeColor => Color.Green;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override float RecoilPower => 15f;
        public override bool IsUsing => ((Owner.channel) && !Owner.noItems && !Owner.CCed) || Projectile.HJScarlet().ExecutionStrike;
        public int NextBulletTileCollidingTime = 0;
        /// <summary>
        /// 复写这个是因为月火的特殊处决攻击，需要让月火在玩家按下处决键（互动键）时立刻发射一枚子弹
        /// <br>而这个钩子会在执行完毕后立刻执行攻击的判定，即上方的<see cref="IsUsing"/></br>
        /// </summary>
        protected override void UpdateRecoil()
        {
            //保留原本的自动管理方案
            base.UpdateRecoil();
            MoonfireExecutionCheck();
        }
        protected override void PostAttack()
        {
            if (!Projectile.HJScarlet().ExecutionStrike)
            {
                RecoilTimer = AttackSpeed;
                Timer = 0;
            }
            else
            {
                RecoilTimer = AttackSpeed + Projectile.MaxUpdates * 10f;
                Timer = -Projectile.MaxUpdates * 10f;
            }
        }
        protected override void UpdateGlobalReset()
        {
            //计时器的重置
            if (RecoilTimer > 0)
            {
                RecoilTimer--;
                if (RecoilTimer == 0)
                    Projectile.HJScarlet().ExecutionStrike = false;
            }
        }
        /// <summary>
        /// 我也不知道我这攻击写了个啥
        /// </summary>
        protected override void OnAttack()
        {
            Vector2 offset = new Vector2(20, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<MoonfireBullet>();
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                type = ProjectileType<MoonfireBulletExecution>();
            }
            pos -= new Vector2(5, 0).RotatedBy(Projectile.rotation);
            Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), pos, dir * 18f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                proj.penetrate = NextBulletTileCollidingTime;
                ScreenShakeSystem.AddScreenShakes(pos, 30, 30, -Projectile.SafeDirByRot().ToRotation(), 0, true, easingFunc: EaseOutExpo);
            }
            else
            {
                ScreenShakeSystem.AddScreenShakes(pos, 6, 30, -Projectile.SafeDirByRot().ToRotation(), 0, true, easingFunc: EaseOutExpo);
                ScarletSound(HJScarletSounds.Wingman, Projectile.Center, 1, 0, 0, 0.1f);
                proj.HJScarlet().HasExecutionMechanic = true;
            }

            pos = Projectile.Center + offset;
            //震屏，粒子特效
            Vector2 particleOffset = new Vector2(0, 0 * Projectile.direction).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 30; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 11.6f);
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.GlowSquare(pos2, vel, RandLerpColor(Color.LimeGreen, Color.Lime), timeLeft, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 0.71f, 0, Main.rand.NextFloat(-.08f, 0.09f), 1f);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(3) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5), .1f, 9.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.LightntingGlow(pos2, vel, RandLerpColor(Color.Green, Color.LimeGreen), timeLeft, 1, scale);
            }
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.HighResolutionThunder(pos.ToRandCirclePos(3) - particleOffset, Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5), .1f, .2f), RandLerpColor(Color.Green, Color.LimeGreen), 45, 1, Projectile.SafeDirByRot().ToRotation(), 0.12f, 1);
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
            int curExecuteCount = Owner.GetExecuteProgress();
            if (curExecuteCount == 0)
                return;
            if (Owner.HJScarlet().tacticalExecutionInputCache > 0)
            {
                //归一化比率
                int maxProgressForMoonfire = HJScarletList.ExecuteRequests[OriginalItemID];
                float ratios = (float)curExecuteCount / maxProgressForMoonfire;
                //将其转化为Lerp值，用于下一发子弹的反弹次数强化
                NextBulletTileCollidingTime = (int)Lerp(1, Moonfire.MaxPenetrateTimeExecution, ratios);
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
