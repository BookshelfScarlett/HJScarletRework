using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;

namespace HJScarletRework.Projs
{
    public class InvisBoom : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(60);
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
