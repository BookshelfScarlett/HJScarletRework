using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletHealProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public virtual int TrailLength => 20;
        public virtual int ExtraUpdates => 1;
        public virtual int HealAmt => 10;
        public bool IsIntersect = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(TrailLength);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.SetupImmnuity(-1);
            Projectile.extraUpdates = ExtraUpdates;
        }
        public override bool? CanDamage() => false;
        /// <summary>
        /// 默认的射弹AI。
        /// </summary>
        /// <returns></returns>
        public void DefaultHealProjAI()
        {
            Projectile.HomingTarget(Owner.Center, -1, 20, 12);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                if (!IsIntersect)
                {
                    Owner.HealDirectly(HealAmt);
                }
                IsIntersect = true;
            }
        }
    }
}
