using System.IO;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode
{
    // 联机数据包的抽象基类：一个包 = 一个继承它的小类，没有注册表也没有枚举。
    // tModLoader 在内容加载阶段会为每个子类各建一个实例并调用 Register()，
    // 所以新写的处理器只要放进 Core/NetCode/Content 里就自动生效，不用在任何地方登记。
    public abstract class BaseHJHandlePack : ModType
    {
        // 本包的编号，等于它在 HJNetCode.Handler 里的下标，收发双方都拿它当包头。
        // 编号由加载顺序决定：增删处理器类会让排在后面的编号整体平移，
        // 所以动过这里以后客户端与服务器必须用同一份构建，不能新旧版本混连。
        public int Type;
        protected override void Register()
        {
            Type = HJNetCode.Handler.Count;
            if (!HJNetCode.Handler.Contains(this))
                HJNetCode.Handler.Add(this);
        }
        // 收到本包时被调用。reader 的读取顺序必须与发送端 Write 的顺序逐项对应，
        // 少读或多读一个字段都不会报错，只会把后面的数据错位读成垃圾。
        // whoAmI 是发包者的玩家编号：服务器转发时用 Send(-1, whoAmI) 把他排除，避免回给自己。
        public virtual void Read(BinaryReader reader, int whoAmI)
        {

        }
    }
}
