using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class SpellBreakerAdvanced : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static float Damage = .10f;
        public static int Crit = 5;
        public static float DamageMult = .1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%", DamageMult.ToPercent());
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<MeleeDamageClass>() += Damage;
            player.GetCritChance<MeleeDamageClass>() += Crit;
            player.HJScarlet().spellBreakerLevel = 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpellBreaker>().
                AddIngredient<DivineFireFlask>().
                AddIngredient<PlagueFlask>().
                AddIngredient<SteelFlask>().
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
