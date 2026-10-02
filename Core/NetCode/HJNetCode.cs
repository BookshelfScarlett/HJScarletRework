using System.Collections.Generic;
using System.IO;

namespace HJScarletRework.Core.NetCode
{
    // 包总表与总入口。HJScarletRework.HandlePacket 只需把数据包流交给 HandleHJPacket，
    // 由它按包头分发给对应的 BaseHJHandlePack 子类。
    public static class HJNetCode
    {
        // 下标就是包号，由 BaseHJHandlePack.Register() 按加载顺序填进来。
        public static List<BaseHJHandlePack> Handler = [];
        // 发送时第一个字段用它取包号：pack.Write(HJNetCode.PackHandleType<ReadXxx>());
        // 取不到实例就返回 0，宁可让对端判成越界抛错，也不要静默发给错误的处理器。
        public static int PackHandleType<T>() where T : BaseHJHandlePack => GetInstance<T>()?.Type ?? 0;
        // 本模组自定义数据包唯一的进入点，服务器与客户端都会走到这里。
        public static void HandleHJPacket(BinaryReader reader, int whoAmI)
        {
            // 包头固定是 4 字节 int，不是 1 字节枚举，处理器多于 256 个也不会溢出。
            int index = reader.ReadInt32();
            // 越界意味着两端构建不一致（有人增删了处理器却没整包更新），
            // 此时直接抛出让日志可见，继续读只会把后续字段错位解析。
            if (index < 0 || index >= Handler.Count)
                throw new IOException($"HJScarletRework : Received invalid packet index {index} from player {whoAmI}");
            Handler[index].Read(reader, whoAmI);
        }
    }
}
