using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Item.HJScarlet().drawBuffIconAndDetail = true;
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
