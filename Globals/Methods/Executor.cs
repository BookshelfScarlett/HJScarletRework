using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static bool AddFocusHitNoFocusProj<T>(this Projectile proj) where T : ModProjectile
        {
            Player owner = Main.player[proj.owner];
            return proj.HJScarlet().AddFocusHit && !owner.HasProj<T>();
        }
    }
}
