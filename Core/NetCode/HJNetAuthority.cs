using Terraria;
using Terraria.ID;

namespace HJScarletRework.Core.NetCode
{
    public static class HJNetAuthority
    {
        public static bool IsOwnerSide(this Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            return player != null && player.whoAmI == Main.myPlayer;
        }
        public static bool IsOwnerSide(this Projectile proj)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            return proj.owner == Main.myPlayer;
        }
        public static bool IsRemoteMirror(this Projectile proj) => !proj.IsOwnerSide();
        public static Player OwnerOrNull(this Projectile proj)
        {
            int slot = proj.owner;
            if (slot < 0 || slot >= Main.maxPlayers)
                return null;
            Player owner = Main.player[slot];
            return owner != null && owner.active ? owner : null;
        }
        public static bool OwnerAlive(this Projectile proj)
        {
            Player owner = proj.OwnerOrNull();
            return owner != null && !owner.dead;
        }
        public static bool CanSpawnChild(this Projectile proj) => proj.IsOwnerSide() && proj.OwnerAlive();
        public static bool IsServerSide() => Main.netMode == NetmodeID.Server;
        public static bool IsSingleSide() => Main.netMode == NetmodeID.SinglePlayer;
    }
}
