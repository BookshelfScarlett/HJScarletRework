using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorAscension : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);

        }
        public float MeleeStat = 0.10f;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().tacticalExecution = true;
            player.HJScarlet().executorAscension = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExecutorSigil>().
                AddIngredient<EssenceofTime>(5).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddTile<FinalAnvil>().
                Register();

        }
    }
}
