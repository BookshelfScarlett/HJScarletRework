using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class EndlessWarMinion : HJScarletProj
    {
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
            /// <summary>
            /// 状态4：薄荷台风
            /// </summary>
            MintTyphoon
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
        public int AttackExtraUpdates = 4;
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
            switch(AttackState)
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
                case State.MintTyphoon:
                    DoMintTyphoon();
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

        public void DoStrike()
        {
        }

        private void DoAttack()
        {

        }

        public void DoIdle()
        {
            IdleHandler();
            //刚生成的时候
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
            if(ratios < 1)
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
            base.OnKill(timeLeft);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool ShouldUpdatePosition()
        {
            return base.ShouldUpdatePosition();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
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
