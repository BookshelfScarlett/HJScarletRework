using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using HJScarletRework.Items.Materials;

namespace HJScarletRework.Items.Accessories
{
    public class SpaceHorror : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((CycleMadness.CritsAdd * 2) + "%", (CycleMadness.CritsPerSecond * 2), (CycleMadness.MaxCrits * 2) + "%");
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.accessory = true;
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().cycleMadnessLevel = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CycleMadness>().
                AddIngredient<UniversalCube>(5).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

    }
}
