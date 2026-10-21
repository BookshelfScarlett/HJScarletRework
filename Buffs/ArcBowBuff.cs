using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class ArcBowBuff : ModBuff
    {
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (Main.LocalPlayer.HJScarlet().arcBowLevel > 1)
            {
                tip = tip.Replace("25", "50");
            }
        }
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            ScarletBuffIDSets.IsStatueBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}
