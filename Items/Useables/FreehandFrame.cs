using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class FreehandFrame : HJScarletItemClass
    {
        public override string AssetPath =>AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateGolden);
        }
        public override void ExSD()
        {
            Item.maxStack = 9999;
            Item.SetUpRarityPrice(ItemRarityID.Green);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 5).
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
