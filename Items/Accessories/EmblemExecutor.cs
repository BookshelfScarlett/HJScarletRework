using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{

    [LegacyName("ExecutorEmblem")]
    public class EmblemExecutor : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 38;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyClassEmblem).
                DisableDecraft().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
