using Terraria.ID;
using HJScarletRework.Globals.Enums;
using Terraria;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using Terraria.ModLoader;
using HJScarletRework.Items.Materials;

namespace HJScarletRework.Items.Accessories
{
    public class PreciousTarget : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 45);
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
