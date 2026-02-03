using HJScarletRework.Assets.Registers;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class PanicBuff : ModBuff
    {
        public override string Texture => HJScarletBuffIcon.Buff_RewardsofWarrior.Path;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
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
