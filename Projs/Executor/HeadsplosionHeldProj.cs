using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using HJScarletRework.Items.Weapons.Requirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class HeadsplosionHeldProj :ExecutorHeldProj
    {
        public override string Texture => GetInstance<Headsplosion>().Texture;
        public override int OriginalItemID => ItemType<Headsplosion>();
        public ref float Timer => ref Projectile.ai[0];
        public ref float RecoilTimer => ref Projectile.localAI[0];
        public float RecoilPower = 30;
        public override void ExSD()
        {
            Projectile.SetDefaultsHeldProj(2);
        }
        public override void OnFirstFrame()
        {
            Timer = (int)(AttackSpeed * .9f);
        }
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override void ProjAI()
        {
            UpdateHeldProjState();
            UpdatePlayerState();
        }

        public void UpdateHeldProjState()
        {
            if (Owner.HeldItem.type != OriginalItemID || Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }

        public void UpdatePlayerState()
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
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = new(20 * Owner.direction, 0);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.BetterRotatedBy(drawRot);
            float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
            float scale = Projectile.scale;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 3f * EaseInCubic(progress), null, Color.Red.ToAddColor(), drawRot, rotPoint, scale, se, 0);
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, scale, se, 0);
            return false;
        }
    }
}
