using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class MonocleHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Monocle>();
        public override string Texture => GetInstance<Monocle>().Texture;
        public ref float Timer => ref Projectile.ai[0];
        public ref float RecoilTimer => ref Projectile.localAI[0];
        public float RecoilPower = 32f;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(2);
        }
        public override void OnFirstFrame()
        {
            Timer = (int)(AttackSpeed * .9f);
        }
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override void ProjAI()
        {
            if (HandleDeadOrAlive())
            {
                Projectile.Kill();
                return;
            }
            HandleOwnerState();
            HandleAttack();
            UpdateTimer();
            Projectile.HJScarlet().ExecutionStrike = false;
        }
        public void HandleAttack()
        {
            if (IsUsing)
            {
                DoAttack();
            }
            else
            {
                DoReset();
            }
        }

        public void DoAttack()
        {
            Timer++;
            Owner.itemAnimation = Owner.itemTime = 2;
            int attackSpeed = AttackSpeed;
            if (Timer < attackSpeed)
                return;
            if (Projectile.IsMe())
            {
                HandleShoot();
            }
            Timer = 0;
            RecoilTimer = attackSpeed;

        }

        public void HandleShoot()
        {
            Vector2 offset = new Vector2(90, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<MonocleBullet>();
            HandleExecution();
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                type = ProjectileType<MonocleBulletExecution>();
            }
            pos -= new Vector2(80, 0).RotatedBy(Projectile.rotation);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, dir * 18f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
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
            ScreenShakeSystem.AddScreenShakes(pos, 32 + Projectile.HJScarlet().ExecutionStrike.ToInt() * 12, 60, -Projectile.SafeDirByRot().ToRotation(), 0, true, easingFunc: EaseOutExpo);
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
            Projectile.HJScarlet().ExecutionStrike = false;
        }
        public void DoReset()
        {
            if (Timer < AttackSpeed)
                Timer++;
        }

        public bool HandleDeadOrAlive()
        {
            if (Owner.HeldItem.type != OriginalItemID)
            {
                return true;
            }
            Projectile.timeLeft = 2;
            return false;
        }
        public void HandleOwnerState()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation);
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;

            //处理后坐力动画
            float progress = Utils.GetLerpValue(AttackSpeed, 0, RecoilTimer, true);
            float pullBack;
            float pullBackpower = RecoilPower;
            float rot = (Projectile.Center - Main.MouseWorld).ToRotation() * Owner.gravDir;
            float proDivide = .13f;
            if (progress > proDivide)
            {
                float pro = (1 - progress) / (1 - proDivide);
                pullBack = Lerp(0, pullBackpower, (EaseOutBack(pro)));
                //Projectile.rotation += rot.ToRotationVector2().RotatedBy((pro) * .1f * -Projectile.spriteDirection).ToRotation();
            }
            else
            {
                float pro = (progress) / proDivide;
                pullBack = Lerp(0, pullBackpower, (EaseOutCubic(pro)));
                //Projectile.rotation += rot.ToRotationVector2().RotatedBy(pro * .1f * -Projectile.spriteDirection).ToRotation();
            }
            Projectile.Center += rot.ToRotationVector2() * pullBack;
        }
        public void UpdateTimer()
        {
            //计时器的重置
            if (RecoilTimer > 0)
                RecoilTimer--;

        }


        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = new(40 * Owner.direction, 0);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.BetterRotatedBy(drawRot);
            float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
            float scale = Projectile.scale * 0.65f;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 3f * EaseInCubic(progress), null, Color.Violet.ToAddColor(), drawRot, rotPoint, scale, se, 0);
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, scale, se, 0);
            return false;
        }
    }
}
