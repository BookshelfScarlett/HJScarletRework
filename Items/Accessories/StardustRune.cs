using HJScarletRework.Globals.Enums;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class StardustRune : HJScarletItems
    {
        public int MinionSlots = 2;
        public override ItemCategory ItemCate => ItemCategory.Accessories;
        public override AssetCategory GetAssetCategory => AssetCategory.Equip;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs("10%", MinionSlots);
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
            player.maxMinions += MinionSlots;
            player.dashType = 1;
            player.blackBelt = true;
        }
    }
}
