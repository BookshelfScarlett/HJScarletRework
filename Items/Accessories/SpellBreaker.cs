using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class SpellBreaker : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static float Damage = .05f;
        public static float DamageMult = .05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(),DamageMult.ToPercent());
        public override void ExSD()
        {
            Item.accessory = true;
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<MeleeDamageClass>() += Damage;
            player.HJScarlet().spellBreakerLevel = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpellBreakerSmall>().
                AddIngredient(ItemID.FlaskofCursedFlames).
                AddIngredient(ItemID.FlaskofVenom).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
