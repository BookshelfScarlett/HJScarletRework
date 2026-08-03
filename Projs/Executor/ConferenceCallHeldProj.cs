using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ConferenceCallHeldProj : ExecutorHeldProj
    {
        public override string Texture => GetInstance<ConferenceCall>().Texture;
        public override int OriginalItemID => ItemType<ConferenceCall>();
        public ref float Timer => ref Projectile.ai[0];
        public ref float RecoilTimer => ref Projectile.localAI[0];
        public bool SetExecution => Owner.HJScarlet().conferenceCallBuffTime > 0;
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
            UpdatePlayerState();
            UpdateWeaponAttack();
            UpdateMiscLerp();
        }
        public void UpdateWeaponAttack()
        {
            if (IsUsing)
                HandleAttack();
            else
                HandleReset();
        }

        public override void OnExecution()
        {
            Owner.HJScarlet().conferenceCallBuffTime = GetSeconds(5);
            ScarletSound(HJScarletSounds.GrabCharge,Projectile.Center);
        }
        public void HandleAttack()
        {
            Timer++;
            if (Timer < AttackSpeed)
                return;
            Vector2 offset = new Vector2(20, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.Center + offset;
            Vector2 dir = Projectile.SafeDirByRot();
            int type = ProjectileType<ConferenceCallBullet>();
            HandleExecution();
            pos -= new Vector2(20, 0).RotatedBy(Projectile.rotation);
            for (int i = 0; i < (ConferenceCall.BulletsPerShot); i++)
            {
                Vector2 randomVelocity = dir.RotatedByRandom(ToRadians(12.5f)) * Main.rand.NextFloat(0.88f, 1.12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, randomVelocity * 8f, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = SetExecution;

            }
            ScarletSound(HJScarletSounds.ASMD_IceBlockSplit, Projectile.Center, 0.20f, 0, .34f, 0.1f);
            pos = Projectile.Center + offset;
            //震屏，粒子特效
            Vector2 particleOffset = new Vector2(10, 0 * Projectile.direction).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8) - particleOffset;
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(25), .1f, 11.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), timeLeft, 1, scale);
            }
            for (int i = 0; i < 16; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.4f, 21.4f), RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), Main.rand.Next(45, 65), RandRotTwoPi, 1, 0.33f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }
            Projectile.HJScarlet().ExecutionStrike = false;
            Timer = 0;
            RecoilTimer = AttackSpeed;
        }
        public void HandleReset()
        {
            if (Timer < AttackSpeed)
                Timer++;
        }

        public void UpdatePlayerState()
        {
            if (Owner.IsHolding(OriginalItemID) && !Owner.dead)
                Projectile.timeLeft = 2;
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
            float pullBackpower = 6;
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
        public void UpdateMiscLerp()
        {
            //计时器的重置
            if (RecoilTimer > 0)
                RecoilTimer--;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = new(10 * Owner.direction, -5);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.BetterRotatedBy(drawRot);
            float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
            float scale = Projectile.scale * .65f;
            Color c = SetExecution ? Color.Red : Color.WhiteSmoke;
            float lerp = SetExecution ? 2f : 2f * EaseInCubic(progress);
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * lerp, null, c.ToAddColor(), drawRot, rotPoint, scale, se, 0);
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, scale, se, 0);
            return false;
        }
    }
}
