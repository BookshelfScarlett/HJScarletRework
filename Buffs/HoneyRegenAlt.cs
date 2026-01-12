using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class HoneyRegenAlt : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 1;
        }
    }
}
