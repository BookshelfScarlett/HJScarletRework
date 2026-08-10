using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class EndlessWarMinion : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override string Texture => GetInstance<EndlessWarProj>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public enum State
        {
            /// <summary>
            /// 状态0：入场
            /// </summary>
            JustSpawn,
            /// <summary>
            /// 状态1（初始）：待机
            /// </summary>
            Idle,
            /// <summary>
            /// 状态2：攻击
            /// </summary>
            Attack,
            /// <summary>
            /// 状态3：重击时间
            /// </summary>
            Strike,
        }
        public AnimationStruct Helper = new AnimationStruct(6);
        /// <summary>
        /// 大锤的内置Timer，大多用于一些攻击的内在逻辑
        /// <br>对于锤子的动画逻辑，使用<see cref="Helper"/>进行处理</br>
        /// </summary>
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        /// <summary>
        /// 缓动值
        /// </summary>
        public float Oscillation = 0;
        /// <summary>
        /// 是否启用薄荷台风
        /// <br>薄荷台风的展开需要一个自己的前摇</br>
        /// <br><see langword="刮飞你们！"/></br>
        /// </summary>
        public bool ApplyMintTyphoon = false;
        public int AttackExtraUpdates = 4;
        public NPC CurTarget = null;
        public bool VelocityFunc = false;
        /// <summary>
        /// 挂载状态下的总共攻击次数，使用timeLeft很难精确控制其存货时长
        /// </summary>
        public int MountedAttackCounter = 0;
        public int MaxAttackTime = 360;
        public enum EndlessWarMinionDeadType
        {
            AnormalyDead,
            SwitchToStirkeMode,
            AutoDead,
            TacticalDead
        }
        public EndlessWarMinionDeadType DeadFunc = EndlessWarMinionDeadType.AutoDead;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
        }

        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.JustSpawn:
                    DoJustSpawn();
                    break;
                case State.Idle:
                    DoIdle();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Strike:
                    DoStrike();
                    break;
            }
        }
        /// <summary>
        /// 薄荷台风会锁定在敌人上，并持续不断地在周围制造大范围的闪电力场
        /// <br>这里会给一个独立的屏幕暗化效果</br>
        /// </summary>
        public void DoMintTyphoon()
        {

        }
        public void ResetHelperProgress()
        {
            for (int i = 0; i < 3; i++)
            {
                Helper.IsDone[i] = false;
                Helper.Progress[i] = 0;
            }

        }
        public void DoStrike()
        {
            if (!CurTarget.IsLegal())
            {
                DeadFunc = EndlessWarMinionDeadType.AnormalyDead;
                Projectile.Kill();
                return;
            }
            if (!Helper.IsDone[0])
            {
                StrikeBeginAnimation();
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                StrikeSecondAnimation();
                Helper.UpdateAniState(1);
            }
            else if (!Helper.IsDone[2])
            {
                StrikeThirdAnimation();
                Helper.UpdateAniState(2);
            }
            else
            {
                AttackState = State.Attack;
                Projectile.netUpdate = true;
                ResetHelperProgress();
            }
        }

        public void StrikeThirdAnimation()
        {
        }

        public void StrikeBeginAnimation()
        {
        }
        public void StrikeSecondAnimation()
        {
        }



        private void DoAttack()
        {
            if (Projectile.GetTargetSafe(out NPC target, true, 1600f, canPassWall: true))
            {
                Projectile.HomingTarget(target.Center, -1f, 16f, 5f);
                CurTarget = target;
                Timer++;
                if (Timer > Projectile.MaxUpdates * 120)
                {
                    AttackState = State.Strike;
                    Projectile.netUpdate = true;
                    Timer = 0;
                }
            }
            else
            {
                AttackState = State.Idle;
            }

        }
        public void DoIdle()
        {
            IdleHandler();
            if (Timer < 60)
                return;
            if (Projectile.GetTargetSafe(out NPC target, true, searchDistance: 1600f, canPassWall: true))
            {
                //需注意的是，有敌对单位的情况下直接切换攻击模式就行了，会在Attack里全程搜索CurTarget的结果
                AttackState = State.Attack;
                CurTarget = target;
                VelocityFunc = true;
                Projectile.extraUpdates = AttackExtraUpdates;
                Timer = 0;
            }
        }
        /// <summary>
        /// 刚生成
        /// <br>雷霆之锤将会从天而降</br>
        /// </summary>
        public void DoJustSpawn()
        {
            float xValue = Owner.MountedCenter.X + Owner.direction * 100f;
            float yValue = Owner.MountedCenter.Y;
            Vector2 pos = new Vector2(xValue, yValue);
            float maxTime = 20;
            float ratios = Utils.GetLerpValue(0, maxTime, Timer, true);
            Timer++;
            Projectile.Center = Vector2.Lerp(Projectile.Center, pos, ratios);
            if (ratios < 1)
                return;
            Timer = 0;
            AttackState = State.Idle;
            Projectile.netUpdate = true;
        }
        public void IdleHandler()
        {
            Projectile.velocity *= .01f;
            Oscillation += ToRadians(2.5f);
            float anchorPosX = Owner.MountedCenter.X;
            float anchorPosY = Owner.MountedCenter.Y - (60f * MathF.Sin(Oscillation) / 9f) - 100;
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, .2f);
            float angleTo = ToRadians(-105f);
            if (Owner.direction < 0)
                angleTo = ToRadians(-65f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.rotation.AngleLerp(angleTo, .2f);
        }
        public void HomingTargetHandler()
        {

        }
        public override void OnKill(int timeLeft)
        {
            switch (DeadFunc)
            {
                case EndlessWarMinionDeadType.AnormalyDead:
                    break;
                case EndlessWarMinionDeadType.AutoDead:
                    break;
                case EndlessWarMinionDeadType.SwitchToStirkeMode:
                    break;
                case EndlessWarMinionDeadType.TacticalDead:
                    break;
            }
        }
        public void CreateThunder()
        {

        }
        public override bool? CanHitNPC(NPC target)
        {
            //三种状态。
            if (AttackState != State.Idle)
                return false;
            if (AttackState != State.Strike)
                return null;
            if (CurTarget.IsLegal() && target.Equals(CurTarget))
                return null;
            return false;
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool ShouldUpdatePosition()
        {
            return VelocityFunc;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Attack)
            {
                MountedAttackCounter += 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects flipSprite);
            Vector2 rotationPoint = texture.Size() / 2f;
            SB.FastDraw(texture, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}
