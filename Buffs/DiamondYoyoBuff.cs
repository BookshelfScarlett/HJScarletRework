using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class DiamondYoyoBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.GemDiamond);
            d.noGravity = true;
            base.Update(npc, ref buffIndex);
        }
    }
}
