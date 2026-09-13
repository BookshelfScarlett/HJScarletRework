using HJScarletRework.Globals.Methods;
using System;
using Terraria;

namespace HJScarletRework.Globals.Classes
{
    /// <summary>
    /// 远程武器的手持射弹基类
    /// <br>这个基类专门用于实现类似电话会议<see langword="ConferenceCall"/>的手持显示效果，已经自动管理了绝大部分从攻击到绘制的内容</br>
    /// <br>一般情况下和大部分武显模组本身冲突。</br>
    /// </summary>
    public abstract class HJScarletRangedWeaponoutClass : HJScarletProj
    {
        /// <summary>
        /// 该射弹的原始归属物品的id
        /// <br>用于处理处死</br>
        /// </summary>
        public virtual int OriginalItemID => -1;
        public override string Texture => $"HJScarletRework/Assets/Texture/Projs/" + GetType().Name;
        /// <summary>
        /// 最低攻击频率，用于<see cref="AttackSpeed"/>
        /// </summary>
        public virtual int MinAttackRate => 5;
        /// <summary>
        /// 攻击速度，使用<see cref="Player.HeldItem"/>作为基础
        /// <br>这里的管理方案会自动将<see cref="Projectile.MaxUpdates"/>纳入计算</br>
        /// </summary>
        public virtual int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, MinAttackRate * Projectile.MaxUpdates);
        /// <summary>
        /// 额外更新，这个额外更新默认为<see langword="1"/>，即提供1额外更新
        /// <br>一般情况下会用于手持射弹本身的粒子特效</br>
        /// </summary>
        public virtual int ProjExtraUpdates => 1;
        /// <summary>
        /// 该远程武器的计时器，用于和<see cref="AttackSpeed"/>一起实际控制武器的攻击频率
        /// </summary>
        public ref float Timer => ref Projectile.ai[0];
        public ref float RecoilTimer => ref Projectile.ai[1];
        /// <summary>
        /// 后坐力动画的力度
        /// <br>如果选择复写<see cref="HandleRecoilStatement"/>则不会有任何作用</br>
        /// </summary>
        public virtual float RecoilPower => 10;
        /// <summary>
        /// 执行后坐力动画时，拉回武器的时刻
        /// <br>这是一个归一化比率，默认值为<see langword="0.13f"/>，即在13%进程时开始拉回</br>
        /// <br>自动管理，如果你完全复写了<see cref="HandleRecoilStatement"/>，则不会生效</br>
        /// </summary>
        public virtual float RecoilWeaponPullbackRatios => .13f;
        /// <summary>
        /// 手持武器的绘制偏移
        /// <br>不需要考虑玩家朝向（如果不选择复写<see cref="PreDraw(ref Color)"/>），因为会自动管理</br>
        /// </summary>
        public virtual Vector2 HoldoutOffset => Vector2.Zero;
        /// <summary>
        /// 手持射弹的大小
        /// </summary>
        public virtual float HoldoutDrawScale => 1f;
        /// <summary>
        /// 手持武器的描边颜色
        /// <br>这个描边颜色只在执行后坐力动画的时候出现</br>
        /// </summary>
        public virtual Color HoldoutEdgeColor => Color.White;
        /// <summary>
        /// 手持武器是否允许绘制描边
        /// </summary>
        public virtual bool HoldoutEdgeEnable => true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = SetDamageClass;
            Projectile.SetUpHeldProj(ProjExtraUpdates);
            ExSD();
        }
        public override void OnFirstFrame()
        {
            Timer = (int)(AttackSpeed * .9f);
        }
        public virtual bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override void ProjAI()
        {
            //手持物品不对，玩家状态不对，处死射弹
            if (Owner.IsHolding(OriginalItemID) && !Owner.CCed && !Owner.dead)
                Projectile.timeLeft = 2;

            //处理玩家手持该武器时的状态
            HandlePlayerHeldStatement();
            //后坐力动画
            HandleRecoilStatement();
            
            if (IsUsing)
            {
                Timer++;
                Owner.itemAnimation = Owner.itemTime = 2;
                HandleWeaponUsingReset();
                if (Timer >= AttackSpeed && Projectile.IsMe())
                {
                    PreHandleWeaponAttackStatement();
                    HandleWeaponAttackStatement();
                    HandleWeaponAttackReset();
                }
            }
            else
            {
                HandleWeaponIdleReset();
            }
            HandleGlobalIdleReset();
        }
        protected virtual void HandleWeaponUsingReset()
        {

        }
        protected virtual void PreHandleWeaponAttackStatement()
        {

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        /// <summary>
        /// 全局状态重置
        /// <br>无论条件，永远在<see cref="Projectile.AI()"/>内执行</br>
        /// </summary>

        protected virtual void HandleGlobalIdleReset()
        {
            if (RecoilTimer > 0)
                RecoilTimer--;
        }

        /// <summary>
        /// 在停止攻击时的状态重置
        /// <br>仅在<see cref="IsUsing"/>为<see langword="false"/>时执行</br>
        /// </summary>
        protected virtual void HandleWeaponIdleReset()
        {
            //我做的不是灾厄，在没有攻击的时候计时器也会叠到attackspeed这的
            if (Timer < AttackSpeed)
                Timer++;
        }
        /// <summary>
        /// 在执行攻击之后的状态重置
        /// <br>在<see cref="HandleWeaponAttackStatement"/>后立刻执行</br>
        /// </summary>
        protected virtual void HandleWeaponAttackReset()
        {
            Timer = 0;
            RecoilTimer = AttackSpeed;
        }
        /// <summary>
        /// 武器的实际攻击效果
        /// <br>复写这个就可以实际进行攻击，继续怎么操作就看你了</br>
        /// <br>只支持左键</br>
        /// </summary>
        protected virtual void HandleWeaponAttackStatement()
        {

        }
        /// <summary>
        /// 玩家手持状态的控制
        /// </summary>
        protected virtual void HandlePlayerHeldStatement()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation);
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;

        }
        /// <summary>
        /// 武器后坐力动画的进程控制
        /// </summary>

        protected virtual void HandleRecoilStatement()
        {
            float progress = Utils.GetLerpValue(AttackSpeed, 0, RecoilTimer, true);
            float pullback;
            float rot = (Projectile.Center - Owner.LocalMouseWorld()).ToRotation() * Owner.gravDir;
            if (progress > RecoilWeaponPullbackRatios)
            {
                float pro = (1 - progress) / (1 - RecoilWeaponPullbackRatios);
                pullback = Lerp(0, RecoilPower, EaseOutBack(pro));
            }
            else
            {
                float pro = progress / RecoilWeaponPullbackRatios;
                pullback = Lerp(0, RecoilPower, EaseOutCubic(pro));
            }
            Projectile.Center += rot.ToRotationVector2() * pullback;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = HoldoutOffset * new Vector2(Owner.direction, 1);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.BetterRotatedBy(drawRot);
            
            float scale = Projectile.scale * HoldoutDrawScale;
            if (HoldoutEdgeEnable)
            {
                float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
                float edgeProgress = EaseInCubic(progress);
                for (int i = 0; i < 8; i++)
                {
                    if (edgeProgress <= .02f)
                        break;
                    DrawWeaponEdge(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 2.5f * edgeProgress, drawRot, rotPoint, scale, se);
                }
            }
            SB.FastDraw(tex, drawPos, Color.White, drawRot, rotPoint, scale, se);
            return false;
        }

        protected virtual void DrawWeaponEdge(Texture2D tex, Vector2 drawPos, float drawRot, Vector2 rotPoint, float scale, SpriteEffects se)
        {
            SB.FastDraw(tex, drawPos, HoldoutEdgeColor.ToAddColor(), drawRot, rotPoint, scale, se);
        }
    }
}
