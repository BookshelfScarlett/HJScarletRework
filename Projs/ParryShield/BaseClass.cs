using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.ParryShield
{
    public abstract class BaseParryShield : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public AnimationStruct Helper = new AnimationStruct(2);
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float Width = 1f;
        public float StopTiming = 0;
        /// <summary>
        /// 这个格挡的动画时间，以帧
        /// <br>会自动计算<see cref="Projectile.MaxUpdates"/></br>
        /// </summary>
        protected virtual int AnimationTime => 25;
        /// <summary>
        /// 格挡的最大更新
        /// <br>默认为5</br>
        /// </summary>
        protected virtual int MaxUpdates => 5;
        /// <summary>
        /// 格挡的最大水平距离
        /// <br>不过，这里的实现方式可能和实际看起来不太一样，如果想的话可以阅读基类代码</br>
        /// </summary>
        protected virtual float ParryRange => 120f;
        /// <summary>
        /// 存残影的点位数量
        /// </summary>
        protected virtual int TrailCounts => 10;
        /// <summary>
        /// 命中冻结帧，对射弹。默认为5帧*<see cref="Projectile.MaxUpdates"/>
        /// </summary>
        protected virtual float HitStopFrame => 5 * Projectile.MaxUpdates;
        /// <summary>
        /// 格挡盾的大小
        /// </summary>
        protected virtual float ParryShieldScale => 1.3f;
        /// <summary>
        /// 这个字段有点绕：它表示格挡盾达到 <see cref="ParryRange"/> 所允许的最大水平距离时，
        /// 格挡动画的归一化进度应该所在的阈值。
        /// <br>也就是说，动画进度到达这个值时，归一化进度就被视为 1f（最大值）。</br>
        /// <br>它和自动管理方案 <see cref="UpdateAnimation"/> 与<see cref="ApplyParryShieldHorizonalRangeScale(float)"/> 是绑在一起的，想搞明白的话建议直接去读那部分的默认实现。</br>
        /// <br>换句话说，如果你把 <see cref="PreUpdateAnimation"/> 重写成返回 <see langword="false"/>，
        /// 那包括这个字段在内的一大堆动画状态，你都得自己手动管了。</br>
        /// </summary>
        protected virtual float ParryProgressScaleThreshold => .5f;
        /// <summary>
        /// 格挡成功时向上的挑飞力度，默认为15
        /// </summary>
        protected virtual float ParryPower => 15f;
        /// <summary>
        /// 格挡动画的角度范围，以度，而非弧度
        /// <br>X：起始角度相对于格挡生成时鼠标角度的偏移量。</br>
        /// <br>Y：结束角度相对于格挡生成时鼠标角度的偏移量。</br>
        /// </summary>
        protected virtual Vector2 ParryAngleRange => new Vector2(-185f, 185f);
        public Vector2 _parryRange => new Vector2(ParryRange * ApplyParryShieldHorizonalRangeScale(), 0).RotatedBy(Projectile.rotation);
        private int AttackSpeed => AnimationTime * Projectile.MaxUpdates;
        public List<Vector2> OldParryShieldPos = new List<Vector2>();
        public List<float> OldParryShieldRot = new List<float>();
        protected float PrevParryProgress = 0f;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 16;
            ExSD();
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = AttackSpeed;
            TargetRotation = Owner.ToMouseVector2().ToRotation();
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                OnFirstFrame();

            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
            UpdateAnimation();
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        #region 动画进程控制
        /// <summary>
        /// 在动画更新之前调用。
        /// <br>返回 <see langword="true"/> 表示继续执行默认的动画更新逻辑；返回 <see langword="false"/> 则跳过默认更新。</br>
        /// <br>默认实现返回 <see langword="true"/>。</br>
        /// </summary>
        protected virtual bool PreUpdateAnimation() => true;
        public void UpdateAnimation()
        {
            if (!PreUpdateAnimation())
                return;
            if (OldParryShieldPos.Count > TrailCounts)
            {
                OldParryShieldPos.RemoveAt(0);
                OldParryShieldRot.RemoveAt(0);
            }
            //控制卡肉效果
            if (StopTiming > 0)
            {
                //卡肉时，射弹的位置会抖动一会
                Projectile.position += Main.rand.NextVector2Circular(5, 5);
                StopTiming--;
                return;
            }
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                float easedProgress = ApplyingParryingProgress();
                float beginAngle = ParryAngleRange.X * Flip.ToDirectionInt();
                float endAngle = ParryAngleRange.Y * Flip.ToDirectionInt();
                float rot = Helper.UpdateAngle(beginAngle, endAngle, Projectile.direction, easedProgress);
                Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
                Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * ParryShieldScale;
                Projectile.scale = tarPos.Length();
                Projectile.rotation = tarPos.ToRotation() + TargetRotation;
                if (easedProgress < .01f)
                    TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
                else
                {
                    OldParryShieldPos.Add(Owner.MountedCenter + new Vector2(0f, Owner.gfxOffY) - Projectile.Size / 2f + _parryRange);
                    OldParryShieldRot.Add(Projectile.rotation);
                    OnActuallyGoingParry(easedProgress, tarPos, _parryRange);
                }
                PrevParryProgress = easedProgress;
            }
            else
                Projectile.Kill();
        }
        public float GetParryRotation(float beginAngle, float endAngle, float easedProgress)
        {
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * ParryShieldScale;
            return tarPos.ToRotation() + TargetRotation;
        }
        /// <summary>
        /// 我们实际真的开始进行了格挡
        /// <br>复写这个用于手动开始存储你需要的点位信息，或者生成一些粒子，甚至是弹幕，都可以</br>
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="tarPos"></param>
        /// <param name="offset"></param>
        protected virtual void OnActuallyGoingParry(float progress, Vector2 tarPos, Vector2 offset)
        {

        }
        /// <summary>
        /// 获取格挡时动画进程的归一化比率
        /// <br>如果你不想使用默认管理方案，可以复写这个</br>
        /// <br>记住格挡的动画使用了<see cref="Helper"/>这一全局变量去管理</br>
        /// </summary>
        /// <returns></returns>
        protected virtual float ApplyingParryingProgress() =>
            (EaseOutCubic(Helper.GetAniProgress(0)));
        protected virtual float ApplyParryShieldHorizonalRangeScale()
        {
            float lerp;
            float pro = ApplyingParryingProgress();
            if (pro <= ParryProgressScaleThreshold)
            {
                lerp = Lerp(0f, 1f, (Utils.GetLerpValue(0, ParryProgressScaleThreshold, pro, true)));
            }
            else
                lerp = Lerp(1f, .10f, (Utils.GetLerpValue(ParryProgressScaleThreshold, 1f, pro, true)));
            return lerp;
        }
        #endregion
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = ApplyingParryingProgress();
            if (easedProgress < .01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * ParryRange * ApplyParryShieldHorizonalRangeScale();
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 34f, ref _);
            return c;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
            {
                StopTiming = HitStopFrame;
                Vector2 offset = _parryRange;
                Vector2 dir = ((Projectile.Center + offset).GetNormalVector2(target.Center)).ToSafeNormalize(Vector2.UnitX);
                Vector2 finalDir = Owner.Center.GetNormalVector2(target.Center) - dir * ParryPower;
                target.PunchTarget(dir, ParryPower);
                target.HJScarlet().parryNoPassingWall = target.noTileCollide;
                target.HJScarlet().parryTime = GetSeconds(5);
                new ParryParticle(target.Center, 1, 40).Spawn();
                ScreenShakeSystem.AddScreenShakes(target.Center, 30, 30, RandRotTwoPi, RandRotTwoPi);
                ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, pitch: .24f, pitchVariance: .1f, variantType: 2);
                PostOnHitNPC(target, hit, damageDone, finalDir.ToSafeNormalize());

            }
        }
        protected virtual void PostOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone, Vector2 parryDirection)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }
    }
}
