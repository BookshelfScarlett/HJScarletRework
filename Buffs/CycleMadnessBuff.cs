using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class CycleMadnessBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
