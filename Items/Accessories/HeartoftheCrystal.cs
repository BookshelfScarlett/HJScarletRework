using HJScarletRework.Globals.Enums;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheCrystal: HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
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
