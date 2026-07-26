using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ASMDHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<ASMD>();
        public override string Texture => GetInstance<ASMD>().Texture;
        public override int MinAttackRates => 5;
        public ref float Timer => ref Projectile.ai[0];
        public ref float RecoilTimer => ref Projectile.ai[1];
        public float RecoilPower = 0;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
        }
        public override void OnFirstFrame()
        {
            //首次生成的时候Timer也就是开火的计时器会自动设置为少8帧数
            //最主要的原因是防止切武器卡手
            Timer = (AttackSpeed - 8);
            RecoilPower = 19.5f;
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

        public void DoReset()
        {
            if (Timer < AttackSpeed)
                Timer++;
        }
        public void DoAttack()
        {
            Owner.itemAnimation = Owner.itemTime = 2;
            Timer++;
            int attackSpeed = AttackSpeed;
            if (Owner.HJScarlet().ASMDBuffTime > 0)
                attackSpeed /= 3;
            if (Timer < attackSpeed)
                return;
            HandleExecution();
            Vector2 offset = new Vector2(90, -10 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<ASMDBullet>();
            if(Projectile.HJScarlet().ExecutionStrike)
            {
                type = ProjectileType<ASMDExecutionBullet>();
                Owner.HJScarlet().ASMDBuffTime = GetSeconds(3);
            }

            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, dir * 18f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            proj.HJScarlet().HasExecutionMechanic = true;
            if (Projectile.HJScarlet().ExecutionStrike)
                ScarletSound(HJScarletSounds.ASMD_ExecutionFire, Projectile.Center, 0.20f, 0, -.4f, 0.1f);
            else
                ScarletSound(HJScarletSounds.ASMD_Fire, Projectile.Center, 0.20f, 0, -.4f, 0.1f);
            
            //震屏，粒子特效
            ScreenShakeSystem.AddScreenShakes(pos, 32 + Projectile.HJScarlet().ExecutionStrike.ToInt() * 12, 60, -Projectile.rotation, 0, true, easingFunc: EaseOutExpo);
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(15f), 1).ToSafeNormalize();
                ECSParticle.HighResolutionThunder(pos, vel * .1f, RandLerpColor(Color.DodgerBlue, Color.White), 40, 1, vel.ToRotation() + PiOver2, Projectile.scale * .63f, 0);
            }
            Vector2 particleOffset = new Vector2(10, 0 * Projectile.direction).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 11.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.CornflowerBlue, Color.White), timeLeft, 1, scale);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(0), .1f, 19.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.LightntingGlow(pos2, vel, RandLerpColor(Color.CornflowerBlue, Color.White), timeLeft, 1, scale);
            }
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 24; i++)
                {
                    bool alt = Main.rand.NextBool();
                    BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                    ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(15), 0.4f, 21.4f), RandLerpColor(Color.CornflowerBlue, Color.White), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.33f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 vel = dir.ToRandVelocity(ToRadians(15f), 1).ToSafeNormalize();
                    ECSParticle.HighResolutionThunder(pos + vel.ToSafeNormalize() * 35, vel * .1f, RandLerpColor(Color.DodgerBlue, Color.White), 40, 1, vel.ToRotation(), Projectile.scale * .263f, 1);
                }

            }

            RecoilTimer = attackSpeed;
            Timer = 0;
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
            float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
            float pullBack;
            float pullBackpower = RecoilPower;
            float rot = (Projectile.Center - Main.MouseWorld).ToRotation() * Owner.gravDir;
            if (progress >= 0.5f)
            {
                float pro = (progress - 0.5f) / .5f;
                pullBack = Lerp(pullBackpower, 0, (EaseInCubic(pro)));
            }
            else
            {
                float pro = (progress) / .5f;
                pullBack = Lerp(0, pullBackpower, (EaseOutCubic(pro)));
            }
            Projectile.Center += Main.rand.NextVector2Circular(1.3f * progress, 1.3f * progress) + rot.ToRotationVector2() * pullBack;
        }
        public void UpdateTimer()
        {

            //计时器的重置
            if (RecoilTimer > 0)
                RecoilTimer--;
    
            if (Owner.HJScarlet().ASMDBuffTime > 0 && Projectile.FinalUpdate())
            {
                Owner.HJScarlet().ASMDBuffTime--;
                if (Owner.HJScarlet().ASMDBuffTime == 0)
                    Timer = (AttackSpeed / 3 * 2);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = new(20 * Owner.direction, 0);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.BetterRotatedBy(drawRot);
            float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 2f * EaseInCubic(progress), null, Color.WhiteSmoke.ToAddColor(), drawRot, rotPoint, Projectile.scale, se, 0);
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, Projectile.scale, se, 0);
            return false;
        }
    }
}
