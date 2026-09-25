using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class SpellBreakerSmall : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static float DamageMult = .05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageMult.ToPercent());
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().spellBreakerLevel = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FlaskofFire).
                AddIngredient(ItemID.FlaskofPoison).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
