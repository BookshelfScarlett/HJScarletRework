using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class AbsoluteZeroBuff : ModBuff
    {
        public static int BadLifeRegen = 35;
        public static int BadLifeRegenEnemy = 500;
        public static float BadMoveSpeed = .5f;
        public static float BadMoveSpeedEnemy = .7f;
        public static float BadDamageMult = .1f;
        public static int BadDefense = 20;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.HJScarlet().absoluteZeroBuff = true;
        }
    }
}
