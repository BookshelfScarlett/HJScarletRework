using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class TheBleachingBuff : ModBuff
    {
        public static float HitDamageMult = .5f;
        public static float HitDamageMultEnemy= .05f;
        public static int BadLifeRegenEnemy = 1000;
        public static int BadLifeRegen = 100;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
        }
        public override void Update(Player player, ref int buffIndex)
        {

            base.Update(player, ref buffIndex);
        }
    }
}
