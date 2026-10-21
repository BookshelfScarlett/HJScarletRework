using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class RocketCharmBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(RocketCharm.ExtraAP);
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            ScarletBuffIDSets.IsStatueBuff[Type] = true;

        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetArmorPenetration<ExecutorDamageClass>() += player.HJScarlet().blackKeyReduceDefense;
        }
    }
}
