using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class AxeofPerun : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = 42;
            Item.height = 34;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance<ExecutorDamageClass>() += 5;
            player.HJScarlet().critDamageExecutor += 0.10f;


        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 15).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
