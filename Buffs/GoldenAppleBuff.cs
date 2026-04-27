using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class GoldenAppleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
