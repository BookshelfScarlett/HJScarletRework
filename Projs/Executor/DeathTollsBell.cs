using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DeathTollsBell : HJScarletProj
    {
        public override string Texture => GetInstance<DeathTollsProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            Attack,
            Hanging,
            ReStrike,
            Return
        }
        public AnimationStruct Helper = new(3);
        public ref float Timer => ref Projectile.ai[0];
        public State BellState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public NPC CurTarget = null;
        public bool VelocityIsWorking = true;
        public float Osci = 0f;
        public bool IsBell = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.noEnchantmentVisuals = true;
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
            switch (BellState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Hanging:
                    DoHanging();
                    break;
                case State.ReStrike:
                    DoStrike();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }


        public override bool ShouldUpdatePosition() => VelocityIsWorking;
        public bool GeneralUpdate(float velMult=0.95f, int frames = 10)
        {
            Timer++;
            Projectile.rotation = Projectile.SpeedAffectRotation();
            Projectile.velocity *= velMult;
            return Projectile.MeetMaxUpdatesFrame(Timer, frames);
        }
        public void DoShoot()
        {
            Timer++;
            Projectile.rotation = Projectile.SpeedAffectRotation();
            Projectile.velocity *= 0.96f;
            if (!Projectile.MeetMaxUpdatesFrame(Timer, 10f))
                return;
            if (GetTarget(out NPC target))
            {
                CurTarget = target;
                UpdateNextState(State.Attack);
            }
            else
            {
                UpdateNextState(State.Return);
            }
        }
        public bool GetTarget(out NPC target)
        {
            target = null;
            //如果同时攻击了超过了需要的单位，处死他 
            //创建一个链表，搜索附近可能的单位
            float searchDist = 300;
            foreach (NPC needTar in Main.ActiveNPCs)
            {
                bool legalTarget = needTar != target && needTar.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, needTar.Center, 1, 1);
                float distPerTar = Vector2.Distance(needTar.Center, Main.MouseWorld);
                if (legalTarget && distPerTar < searchDist)
                {
                    searchDist = distPerTar;
                    target = needTar;
                    //把可用单位甩进去，因为我们需要最后使用一个最靠近的单位
                }
            }
            //确保链表正确
            return target != null;
        }
        public void UpdateNextState(State id)
        {
            Projectile.netUpdate = true;
            Timer = 0;
            BellState = id;
        }
        public void DoAttack()
        {
            Projectile.rotation += 0.2f;
            if (IsLegalTarget)
                Projectile.HomingTarget(CurTarget.Center, -1, 18f, 20f);
            else
                UpdateNextState(State.Return);
        }

        public void DoHanging()
        {
            Osci += ToRadians(5f);
            if(IsBell)
            {
                UpdateNextState(State.Return);
                return;
            }
            if (IsLegalTarget)
            {
                UpdateIdleAI();
                UpdateHangingAttack();
            }
            else
            {
                UpdateNextState(State.Return);
            }
        }

        public void UpdateHangingAttack()
        {
            Timer++;
            if (Timer < 40)
                return;

        }

        public void UpdateIdleAI()
        {
            //锤子应当朝向的位置
            float anchorPosX = CurTarget.Center.X;
            float anchorPosY = CurTarget.Center.Y - (60f * MathF.Sin(Osci) / 9f + 300f);
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = (Projectile.Center - CurTarget.Center).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
        }

        public void DoStrike()
        {
        }
        public void DoReturn()
        {
            Projectile.rotation += 0.2f;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                Projectile.Kill();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White);
            return false; 
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(BellState == State.Attack)
            {
                Timer += 1;
                if (Timer > 8)
                {
                    //立马做掉速度
                    Projectile.velocity *= 0f;
                    VelocityIsWorking = false;
                    Projectile.extraUpdates = 0;
                    //然后我们再更新下一个状态
                    UpdateNextState(State.Hanging);
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (IsLegalTarget && CurTarget.Equals(target))
                return null;
            return false;
        }
        public bool IsLegalTarget
        {
            get
            {
                return CurTarget != null && CurTarget.CanBeChasedBy();
            }
        }
    }
}
