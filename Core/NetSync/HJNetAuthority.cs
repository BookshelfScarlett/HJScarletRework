using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetSync
{
    public enum HJNetRole
    {
        Owner,
        Remote,
        Server,
    }

    public static class HJNetAuthority
    {
        public static bool IsOwnerSide(this Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            return player != null && player.whoAmI == Main.myPlayer;
        }

        public static bool IsOwnerSide(this Projectile proj) => proj.owner.IsOwnerSideSlot();

        public static bool IsRemoteMirror(this Projectile proj) => !proj.IsOwnerSide();

        public static bool IsServerSide() => Main.netMode == NetmodeID.Server;

        public static bool IsSingleSide() => Main.netMode == NetmodeID.SinglePlayer;

        public static bool IsOwnerSideSlot(this int slot)
        {
            if (slot < 0 || slot >= Main.maxPlayers)
                return false;
            return slot == Main.myPlayer;
        }

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

        public static HJNetRole RoleOf(this Projectile proj)
        {
            if (Main.netMode == NetmodeID.Server)
                return HJNetRole.Server;
            return proj.IsOwnerSide() ? HJNetRole.Owner : HJNetRole.Remote;
        }
    }
}
