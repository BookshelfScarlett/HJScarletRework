using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ASMDHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<ASMD>();
        public override string Texture => GetInstance<ASMD>().Texture;
        public override int MinAttackRates => 5;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = new(20 * Owner.direction, 0);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.BetterRotatedBy(drawRot);
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, Projectile.scale, se, 0);
            return false;
        }
    }
}
