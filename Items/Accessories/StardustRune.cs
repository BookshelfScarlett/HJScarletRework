using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class StardustRune : HJScarletItemClass
    {
        public int MinionSlots = 2;
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().stardustRune = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient(ItemID.FragmentStardust, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
