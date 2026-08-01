using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{
    [AutoloadEquip(EquipType.Body)]
    public class ReaperBody : HJScarletArmor
    {
        public float CritDamage = 0.15f;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent());
        public override void ExSD()
        {
            Item.defense = 60;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateEquip(Player player)
        {
            player.HJScarlet().critDamageExecutor += CritDamage;
            player.aggro += 500;
        }
    }
}
