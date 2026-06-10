using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class BitingClaw : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().bitingClaw = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(RecipeGroupID.IronBar, 5).
                AddIngredient(ItemID.Seashell, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
