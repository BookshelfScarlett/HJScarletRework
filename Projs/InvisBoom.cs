using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Terraria;

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
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = 60;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool? CanHitNPC(NPC target)
        {
            NPC tar = Projectile.HJScarlet().CurStoredTarget;
            if (tar.IsLegal() && tar.Equals(target))
                return false;
            return null;
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
