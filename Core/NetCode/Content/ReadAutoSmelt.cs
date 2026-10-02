using HJScarletRework.Globals.Instances;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
    // 自动烧矿数据包（GlobalTiles 里那个挖矿饰品的效果）。
    // 它和鼠标同步包不一样：没有玩家编号字段，载荷就是四个 ushort（格 x、格 y、概率、目标矿块类型）。
    // 流程：发起端只发包不结算（见 GlobalTiles.PacketOres）→ 服务器落地并向其他客户端广播。
    public class ReadAutoSmelt : BaseHJHandlePack
    {
        public override void Read(BinaryReader reader, int whoAmI)
        {
            ushort x = reader.ReadUInt16();
            ushort y = reader.ReadUInt16();
            ushort chance = reader.ReadUInt16();
            ushort targetType = reader.ReadUInt16();
            // 服务器：原样广播给除发起者以外的所有客户端（他已经自己发过一次了）。
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = HJScarletRework.Instance.GetPacket();
                packet.Write(Type);
                packet.Write(x);
                packet.Write(y);
                packet.Write(chance);
                packet.Write(targetType);
                packet.Send(-1, whoAmI);
            }
            // 三端都会走到这里：服务器结算矿块，收到广播的客户端也各自再跑一次（含自己的随机与 CurChance 保底）。
            // 这是保留原来的写法。如果发现别人端会多出铟或音效重了，把这一行包进 if (IsServerSide()) 就行。
            GetInstance<HJScarletGlobalTiles>().SmeltOres(x, y, chance, targetType);
        }
    }
}
