using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class ArcBow : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public int HealAmt = 25;
        public override void ExSD()
        {
            Item.width = Item.height = 32;
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.defense = 2;
            Item.accessory = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealAmt);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyHeal = 25;
            player.HJScarlet().blackKeyDefenseBuff = .25f;
            player.longInvince = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient(ItemID.HallowedBar, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
