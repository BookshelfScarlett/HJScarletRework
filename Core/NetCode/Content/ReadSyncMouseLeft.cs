using HJScarletRework.Globals.Methods;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
    // 同步某个玩家鼠标左键的按住状态，供队友端的挂载射弹读 MouseLeftOf()。
    // 整体结构与 ReadSyncMouseWorld 相同：先落地，服务器再广播（排除发起者）。
    public class ReadSyncMouseLeft : BaseHJHandlePack
    {
        public override void Read(BinaryReader reader, int whoAmI)
        {
            byte playerIndex = reader.ReadByte();
            bool mouseLeft = reader.ReadBoolean();
            if (playerIndex < Main.maxPlayers && Main.player[playerIndex].active)
                Main.player[playerIndex].HJScarlet().MouseLeft = mouseLeft;
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = HJScarletRework.Instance.GetPacket();
                packet.Write(Type);
                packet.Write(playerIndex);
                packet.Write(mouseLeft);
                packet.Send(-1, whoAmI);
            }
        }
    }
}
