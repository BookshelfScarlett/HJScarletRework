using Terraria.ID;
using Terraria;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;

namespace HJScarletRework.Items.Accessories
{
    public class PreciousTarget : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().PreciousTargetAcc = true;
            player.HJScarlet().PreciousCritsMin = 10;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DestroyerEmblem).
                AddIngredient(ItemID.FragmentVortex, 15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
