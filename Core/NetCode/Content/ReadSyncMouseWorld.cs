using HJScarletRework.Globals.Methods;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
    // 同步某个玩家的准星世界坐标。
    // 落地和转发分开写：服务器也会执行落地那一段，所以服务端的 AimWorldOf() 同样有值
    // （旧的 NetCode.cs 在服务器只转发不落地的毛病就在这里）。
    public class ReadSyncMouseWorld : BaseHJHandlePack
    {
        public override void Read(BinaryReader reader, int whoAmI)
        {
            // 读取顺序与 HJNetUtils.SyncedMouseWorld 的写入顺序一致：玩家编号 + 坐标。
            byte playerIndex = reader.ReadByte();
            Vector2 mouseWorld = reader.ReadVector2();
            // 写回那个玩家的实例字段，三端都会进这一行。
            if (playerIndex < Main.maxPlayers && Main.player[playerIndex].active)
                Main.player[playerIndex].HJScarlet().SyncedMouseWorld = mouseWorld;
            // 服务器收到后原样广播给其他客户端，自己排除在外。
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = HJScarletRework.Instance.GetPacket();
                packet.Write(Type);
                packet.Write(playerIndex);
                packet.WriteVector2(mouseWorld);
                packet.Send(-1, whoAmI);
            }
        }
    }
}
