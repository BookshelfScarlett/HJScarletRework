using HJScarletRework.Globals.Instances;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
    public class ReadAutoSmelt : BaseHJHandlePack
    {
        public override void Read(BinaryReader reader, int whoAmI)
        {
            ushort x = reader.ReadUInt16();
            ushort y = reader.ReadUInt16();
            ushort chance = reader.ReadUInt16();
            ushort targetType = reader.ReadUInt16();
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
            GetInstance<HJScarletGlobalTiles>().SmeltOres(x, y, chance, targetType);
        }
    }
}
