using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class SelfPortraitoftheSunflower : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static float DotDamageMult = 3f;
        public static float ExtraDamageMult = .25f;
        public static int MaxDoTCounts = 12;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DotDamageMult.ToPercent(), ExtraDamageMult.ToPercent(), MaxDoTCounts);
        public override void SetStaticDefaults()
        {
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Sunflower, 1).
                AddIngredient(ItemID.LunarBar, 15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
