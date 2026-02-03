using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheMountain : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.rare = ItemRarityID.Purple;
            Item.defense = 5;
            Item.accessory = true;
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
