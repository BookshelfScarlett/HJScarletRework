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
    public class BlackKeyExecutionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            ScarletBuffIDSets.IsStatueBuff[Type] = true;

        }
        public override LocalizedText Description => base.Description.WithFormatArgs(ConceptualBlackKey.ExtraAP, ConceptualBlackKey.DamageReduce.ToPercent());
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            //base.ModifyBuffText(ref buffName, ref tip, ref rare);
            rare = ItemRarityID.Cyan;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetArmorPenetration<ExecutorDamageClass>() += player.HJScarlet().blackKeyReduceDefense;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
