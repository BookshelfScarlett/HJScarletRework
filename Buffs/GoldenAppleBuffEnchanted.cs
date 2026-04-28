using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class GoldenAppleBuffEnchanted : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[BuffType<GoldenAppleBuff>()] = true;
            player.HJScarlet().goldenAppleDamageAbsorb = 50;
            player.lavaImmune = true;
            player.lifeRegen += 4;
            player.statDefense += 10;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
