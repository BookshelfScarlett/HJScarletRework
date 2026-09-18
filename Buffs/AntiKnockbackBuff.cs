using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class AntiKnockbackBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.noKnockback = true;
        }
    }
}
