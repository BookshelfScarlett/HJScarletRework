using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class CrimsonCharmBuff: ModBuff
    {
        public override void SetStaticDefaults()
        {
            Terraria.Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }

}
