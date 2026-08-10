using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class GrassPoison : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
        }
    }
}
