using HJScarletRework.Globals.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public abstract class ExecutorWhipBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            HJScarletList.ExecutorWhip.Add(Type);
        }
    }

    public class StarofHopeBuff : ExecutorWhipBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
        }
    }
}
