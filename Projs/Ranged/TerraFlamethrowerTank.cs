using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

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
        public bool UseVelocity = false;
        public override bool ShouldUpdatePosition()
        {
            return UseVelocity;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 15 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 4 * Projectile.MaxUpdates;
            Helper.MaxProgress[2] = 10 * Projectile.MaxUpdates;
            TargetRotation = BeginTargetRotation;

        }
        public override void ProjAI()
        {
            UpdateAttackAI();
        }

        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
            }


        }
        public float IdleAngleFinal = -135f;
        public float IdleAngleBegin = 90f;
        public float AttackAAngleFinal = -90f;
        public void DoIdle()
        {
            Projectile.timeLeft = GetSeconds(10);
            if (!Helper.IsDone[0])
            {
                UpdateAniBegin();
                if (Helper.IsDone[0])
                {
                    SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit);
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(30 * Projectile.scale, 15 * Projectile.scale)));
                        Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2);
                        float speed = Main.rand.NextFloat(6f, 10f);
                        ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-speed, speed), RandLerpColor(Color.White, Color.LawnGreen), 40, RandRotTwoPi, .75f, 0.331f, true, BlendState.Additive);
                        ECSParticle.ShinyCrossStarECS(pos, dir * Main.rand.NextFloat(-speed, speed) * .70f, RandLerpColor(Color.White, Color.LawnGreen), 40, RandRotTwoPi, .75f, 0.10f, BlendState.Additive);
                    }
                }
            }
            else if (!Helper.IsDone[1])
            {
                UpdateAniMid();
            }
            else
            {
                UseVelocity = true;
                AttackState = State.Attack;
                Projectile.extraUpdates = 1;
                Owner.itemTime = Owner.itemAnimation = 2;
                Projectile.velocity = Projectile.Center.GetNormalVector2(Main.MouseWorld) * 26f;
            }


        }

        private void UpdateAniMid()
        {
            float progress = EaseOutBack(Helper.GetAniProgress(1));
            if (progress > .90f)
            {
                Helper.Progress[1] = 8 * Projectile.MaxUpdates;
                Helper.IsDone[1] = true;
                return;
            }
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            //更新转交
            float rot = Helper.UpdateAngle(IdleAngleFinal, AttackAAngleFinal, Owner.direction, progress);
            //投射到矩阵上
            Matrix transform = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1, 1, 1);
            //转化为targetPos，即目标指向。其具备了模长
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, transform) * 1.1f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Helper.UpdateAniState(1);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            if (Owner.dead)
                Projectile.Kill();
            //处理玩家处死情况 
            Projectile.Center = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation) + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * Owner.direction) * progress * 30f;

            //处理玩家本身的状态
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        private void UpdateAniBegin()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            float progress = EaseOutBack(Helper.GetAniProgress(0));
            //更新转交
            float rot = Helper.UpdateAngle(IdleAngleBegin, IdleAngleFinal, Owner.direction, progress);
            //投射到矩阵上
            Matrix transform = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1, 1, 1);
            //转化为targetPos，即目标指向。其具备了模长
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, transform) * 1.1f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Helper.UpdateAniState(0);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            if (Owner.dead)
                Projectile.Kill();
            //处理玩家处死情况 
            Projectile.Center = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation);

            //处理玩家本身的状态
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);


        }

        public void DoAttack()
        {
            if (Helper.IsDone[2])
            {
            }
            else
            {
                Owner.itemTime = Owner.itemAnimation = 2;
                Helper.UpdateAniState(2);
            }
            Projectile.rotation += .2f;
            AttackParticle();
        }

        public void AttackParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(30 * Projectile.scale, 15 * Projectile.scale)));
                Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2);
                float speed = Main.rand.NextFloat(6f, 10f);
                ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-speed, speed), RandLerpColor(Color.White, Color.LawnGreen), 40, RandRotTwoPi, .60f, 0.331f, true, BlendState.Additive);
            }

            ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(15), Projectile.velocity / 6f, RandLerpColor(Color.LimeGreen, Color.GreenYellow), 40, 1, .8f, .15f);
            if (Main.rand.NextBool(3))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePosEdge(15), Projectile.velocity / 6f, RandLerpColor(Color.LimeGreen, Color.GreenYellow), 40, RandRotTwoPi, .8f, .4f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = AttackState != State.Idle;
            if (canHit)
                return null;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 safeDir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 4; j++)
                    ECSParticle.StarShape(Projectile.Center.ToRandCirclePos(3f), safeDir.RotatedBy(PiOver2 * j + PiOver4) * Main.rand.NextFloat(.1f, 19f), RandLerpColor(Color.GreenYellow, Color.WhiteSmoke), 30, 1f, 1.091f);
            }
            for (int i = 0; i < 80; i++)
            {
                if (i % 2 == 0)
                    ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(3f), RandVelTwoPi(1f, 24f), RandLerpColor(Color.GreenYellow, Color.DarkGreen), 30, 1f, Main.rand.NextFloat(.7f, 1.3f) * .1f, 0.45f);
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(2f, 21f), RandLerpColor(Color.LightGreen, Color.DarkGreen), 45, 1f, Main.rand.NextFloat(.75f, 1.3f) * 1.1f, 0.13f);
            }
            for (int i = 0; i < 70; i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(7f, 28), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, RandRotTwoPi, .61f, 0.87f * Main.rand.NextFloat(0.8f, 1.1f), false).Spawn();
                new SmokeParticle(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(4f, 28f), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 20, RandRotTwoPi, 1f, 1.09f * Main.rand.NextFloat(0.8f, 1.1f), true).SpawnToPriorityNonPreMult();
            }
            float squareSplitScale = .80f;
            new ShinySquareSplit(Projectile.Center, Vector2.Zero, Color.Green, 20, squareSplitScale, rot: Projectile.rotation + PiOver4, fadeIn: true).Spawn();
            new ShinySquareSplit(Projectile.Center, Vector2.Zero, Color.White, 20, squareSplitScale, rot: Projectile.rotation + PiOver4 + Pi, opacity: 0.85f, fadeIn: true).Spawn();
            float starScale = 1.60f * .4f;
            new KiraStar(Projectile.Center, Vector2.Zero, RandLerpColor(Color.Green, Color.ForestGreen), 20, safeDir.ToRotation() + PiOver4, 0.78f, starScale * .95F, 0, true, useAlt: true).Spawn();
            new KiraStar(Projectile.Center, Vector2.Zero, Color.White, 20, safeDir.ToRotation() + PiOver4, 0.78f, starScale * 0.90f, 0, true, useAlt: true).Spawn();
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 8f, 10, Projectile.rotation);

            SoundEngine.PlaySound(HJScarletSounds.Frostwave_Boom with { Pitch = -.4f }, Projectile.Center);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ExpandHitboxBy(292);
            Projectile.Damage();
            target.AddBuff(BuffType<TerraFlamethrowerBuff>(), GetSeconds(5));

        }
        public override bool PreDraw(ref Color lightColor)
        {

            Projectile.DrawGlowEdge(Color.Lerp(Color.Transparent, Color.White, Helper.GetAniProgress(0)), posMove: Main.rand.NextFloat(1.3f, 2.4f));
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
