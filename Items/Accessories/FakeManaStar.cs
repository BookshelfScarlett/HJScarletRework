using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class FakeManaStar : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 30;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaFlower = true;
            player.HJScarlet().fakeManaStar = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StoneBlock, 20).
                AddRecipeGroup(RecipeGroupID.IronBar, 5).
                AddIngredient(ItemID.Sapphire).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
