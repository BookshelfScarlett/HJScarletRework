using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class LoveRing : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 30;
            Item.accessory = true;
            Item.defense = 5;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
                player.HJScarlet().loveRing = player.HJScarlet().genderChangeTimer < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LargeDiamond).
                AddRecipeGroup(HJScarletRecipeGroup.AnyLifeCrystal, 12).
                AddIngredient(ItemID.LovePotion, 5).
                AddIngredient(ItemID.GoldCoin, 69).
                AddIngredient(ItemID.SoulofLight, 5).
                AddTile(TileID.Beds).
                Register();
        }
    }
}
