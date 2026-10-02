using System.Collections.Generic;
using System.IO;

namespace HJScarletRework.Core.NetCode
{
    public static class HJNetCode
    {
        public static List<BaseHJHandlePack> Handler = [];
        public static int PackHandleType<T>() where T : BaseHJHandlePack => GetInstance<T>()?.Type ?? 0;
        public static void HandleHJPacket(BinaryReader reader, int whoAmI)
        {
            int index = reader.ReadInt32();
            if (index < 0 || index >= Handler.Count)
                throw new IOException($"HJScarletRework : Received invalid packet index {index} from player {whoAmI}");
            Handler[index].Read(reader, whoAmI);
        }
    }
}
