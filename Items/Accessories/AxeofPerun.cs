using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances;
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
            player.GetArmorPenetration<ExecutorDamageClass>() += 10;
            player.HJScarlet().blackKeyDoT = true;
            player.HJScarlet().blackKeyReduceDefense = 5;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 15).
                AddIngredient(ItemID.SharkToothNecklace).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
