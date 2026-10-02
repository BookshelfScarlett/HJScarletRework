using HJScarletRework.Core.NetCode.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode
{
    // 发送端工具，和 Content 里的处理器一一对应。
    // 统一的包格式：包头 int（包号）+ byte（玩家编号）+ 各自的载荷，写入顺序必须和 Read 里的读取顺序一致。
    // 这些都是“把本机输入告诉别人”，所以只在多人客户端发；服务器上没本地输入可发，单人模式也没必要发。
    // 调用方应该只在变化时发（见 Globals/Players/NetPacket.cs），不要每帧都发。
    public static partial class HJNetUtils
    {
        // 同步准星世界坐标，队友端 player.AimWorldOf() 和 ToMouseVector2() 读的就是它。
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
        // 同步鼠标左键的按住状态。
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
        // 同步鼠标右键的按住状态。
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
        // 同步“武器技能键按下”的那一帧。现在只有处理器、没有发送方，
        // 需要在联机里让别人看见你的技能起手时，自己在按键那帧调一次 player.SyncedWeaponSkill(true)。
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
