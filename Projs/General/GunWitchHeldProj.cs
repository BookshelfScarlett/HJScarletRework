using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class GunWitchHeldProj : HJScarletRangedWeaponoutClass
    {
        //无所谓，会直接复写
        public override int OriginalItemID => -1;
        public override string Texture => GetVanillaAssetPath(Globals.Database.Enums.VanillaAsset.Item,ItemID.QuadBarrelShotgun);
        public override float HoldoutDrawScale => base.HoldoutDrawScale;
        public override Vector2 HoldoutOffset => base.HoldoutOffset;
        public override Color HoldoutEdgeColor => base.HoldoutEdgeColor;
        public override float RecoilPower => base.RecoilPower;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void UpdatePlayerState()
        {
            if (Owner.HeldItem.CountsAsClass<ExecutorDamageClass>() && !Owner.noItems && !Owner.CCed)
                Projectile.timeLeft = 2;
        }
        protected override void PreAttack()
        {
            base.PreAttack();
        }
        protected override void OnAttack()
        {
            base.OnAttack();
        }
        protected override void UpdateHeldProjectile()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
        }
    }
}
