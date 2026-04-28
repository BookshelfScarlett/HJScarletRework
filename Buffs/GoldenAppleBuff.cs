using HJScarletRework.Globals.Methods;
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
            player.HJScarlet().goldenAppleDamageAbsorb = 20;
            player.lifeRegen += 2;
            player.statDefense += 8;

        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
