using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs
{
    public class RegenOrbsPink : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory UseDamage => ClassCategory.Typeless;
        private enum Styles
        {
            Slowdown,
            Return
        }
        private Styles AttackType
        {
            get => (Styles)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(40, 2);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.damage = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            switch (AttackType)
            {
                case Styles.Slowdown:
                    DoSlowdown();
                    break;
                case Styles.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoReturn()
        {
            Projectile.HomingTarget(Owner.MountedCenter, 9999, 20f, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                return;
        }
        public void DoSlowdown()
        {
            Projectile.velocity *= 0.95f;
            if (Projectile.velocity.Length() <= 0.5f)
            {
                Projectile.velocity *= 0;
                Projectile.timeLeft = 300;
                Projectile.netUpdate = true;
                AttackType = Styles.Return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
