using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorsSwordMarkSmall : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Orange);

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().tacticalExecution = true;
            player.HJScarlet().executorSwordMarkLevel = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldSword).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilBar, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
