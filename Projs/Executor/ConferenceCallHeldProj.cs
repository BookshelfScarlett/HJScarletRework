using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
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
            base.OnFirstFrame();
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

        
        public void HandleAttack()
        {
            Timer++;
            if (Timer < AttackSpeed)
                return;
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
            float pullBackpower = 12;
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
