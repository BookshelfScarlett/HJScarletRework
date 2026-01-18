using Terraria.ID;
using HJScarletRework.Globals.Enums;
using Terraria;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Items.Accessories
{
    public class PreciousTarget : HJScarletItems
    {
        public override ItemCategory ItemCate => ItemCategory.Accessories;
        public override AssetCategory GetAssetCategory => AssetCategory.Equip;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
