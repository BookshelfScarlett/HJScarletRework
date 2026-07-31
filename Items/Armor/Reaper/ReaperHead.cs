using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{

    [AutoloadEquip(EquipType.Head)]
    public class ReaperHead : HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public float DamageAdd = 0.25f;
        public int CritAdd = 25;

        public override int[] ArmorSlots => [Type, ItemType<ReaperBody>(), ItemType<ReaperLegs>()];
        public override bool SetUpArmorSet => true;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAdd.ToPercent(), CritAdd + "%");
        public override void ExSD()
        {
            Item.defense = 50;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.HJScarlet().maidReaperArmor = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += DamageAdd;
            player.GetCritChance<ExecutorDamageClass>() += CritAdd;
            player.aggro += 500;
        }
    }
}
