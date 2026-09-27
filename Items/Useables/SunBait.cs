using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class SunWorm : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.SunGod);
        }
        public override void ExSD()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.bait = 100;
            Item.consumable = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.EnchantedNightcrawler, 5).
                AddIngredient<SolarFlareScoria>().
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
