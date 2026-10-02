using System.IO;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode
{
    public abstract class BaseHJHandlePack : ModType
    {
        public int Type;
        protected override void Register()
        {
            Type = HJNetCode.Handler.Count;
            if (!HJNetCode.Handler.Contains(this))
                HJNetCode.Handler.Add(this);
        }
        public virtual void Read(BinaryReader reader, int whoAmI)
        {

        }
    }
}
