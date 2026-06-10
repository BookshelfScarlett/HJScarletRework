using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class TerraFlamethrowerTank : HJScarletProj
    {
        public enum State
        {
            Idle,
            Attack
        }
        public AnimationStruct Helper = new(3);
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public float TargetRotation = 0;
        public float BeginTargetRotation = 0;
        public override ClassCategory Category => ClassCategory.Ranged;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 0;
            Projectile.noEnchantmentVisuals = true;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 1;

        }
        public override bool ShouldUpdatePosition()
        {
            return base.ShouldUpdatePosition();
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 30;
            TargetRotation = BeginTargetRotation;

        }
        public override void ProjAI()
        {
            UpdateAttackAI();
        }

        public void UpdateAttackAI()
        {
            switch(AttackState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
            }
        }
        public void DoIdle()
        {
            if (Helper.IsDone[0])
            {
                AttackState = State.Attack;
                return;
            }
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            float progress = EaseOutBack(Helper.GetAniProgress(0));
            //结尾角度
            float endAngle = 135f;
            //起始角度
            float beginAngle = -150;
            //更新转交
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, progress);
            //投射到矩阵上
            Matrix transform = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1, 1, 1);
            //转化为targetPos，即目标指向。其具备了模长
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, transform) * 1.1f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Helper.UpdateAniState(0);

            //处理玩家处死情况 
            Projectile.Center = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation); ;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

            //处理玩家本身的状态
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);


        }
        public void DoAttack()
        {
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = AttackState != State.Idle;
            if (canHit)
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;
            SB.Draw(tex, pos, null, Color.White, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
