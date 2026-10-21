using Terraria;

namespace HJScarletRework.Buffs.ParryShields
{
    public class SquireParryBuff : ParryShieldBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
        }
    }
}
