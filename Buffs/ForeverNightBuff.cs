using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class ForeverNightBuff : ModBuff
    {
        public static int EnemyDoT = 100;
        public static int OwnerDoT = 8;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.HJScarlet().foreverNightBuff = true;
        }
    }
}
