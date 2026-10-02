using HJScarletRework.Globals.Methods;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
    // 同步某个玩家鼠标右键的按住状态，供队友端读 MouseRightOf()（例如处决/格挡类武器看主人有没有按住右键）。
    public class ReadSyncMouseRight : BaseHJHandlePack
    {
        public override void Read(BinaryReader reader, int whoAmI)
        {
            byte playerIndex = reader.ReadByte();
            bool mouseRight = reader.ReadBoolean();
            if (playerIndex < Main.maxPlayers && Main.player[playerIndex].active)
                Main.player[playerIndex].HJScarlet().MouseRight = mouseRight;
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = HJScarletRework.Instance.GetPacket();
                packet.Write(Type);
                packet.Write(playerIndex);
                packet.Write(mouseRight);
                packet.Send(-1, whoAmI);
            }
        }
    }
}
