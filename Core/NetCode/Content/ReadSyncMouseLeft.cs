using HJScarletRework.Globals.Methods;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
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
