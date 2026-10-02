using HJScarletRework.Core.NetCode.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode
{
    public static partial class HJNetUtils
    {
        public static void SyncedMouseWorld(this Player player, Vector2 mouseWorld)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            ModPacket packet = HJScarletRework.Instance.GetPacket();
            packet.Write(HJNetCode.PackHandleType<ReadSyncMouseWorld>());
            packet.Write((byte)player.whoAmI);
            packet.WriteVector2(mouseWorld);
            packet.Send();
        }
        public static void SyncedMouseLeft(this Player player, bool mouseLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            ModPacket packet = HJScarletRework.Instance.GetPacket();
            packet.Write(HJNetCode.PackHandleType<ReadSyncMouseLeft>());
            packet.Write((byte)player.whoAmI);
            packet.Write(mouseLeft);
            packet.Send();
        }
        public static void SyncedMouseRight(this Player player, bool mouseRight)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            ModPacket packet = HJScarletRework.Instance.GetPacket();
            packet.Write(HJNetCode.PackHandleType<ReadSyncMouseRight>());
            packet.Write((byte)player.whoAmI);
            packet.Write(mouseRight);
            packet.Send();
        }
        public static void SyncedWeaponSkill(this Player player, bool weaponSkill)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            ModPacket packet = HJScarletRework.Instance.GetPacket();
            packet.Write(HJNetCode.PackHandleType<ReadWeaponSkill>());
            packet.Write((byte)player.whoAmI);
            packet.Write(weaponSkill);
            packet.Send();
        }
    }
}
