using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class CrimsonCharm : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static int OverSatuTime = 45;
        public static int MininumHeal = 1;
        public static float MinusRatios = .25f;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinusRatios.ToPercent(), OverSatuTime, MininumHeal);
        public override void ExSD()
        {
            Item.defense = 5;
            Item.accessory = true;
            Item.HJScarlet().drawBuffIcon = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 4;
            player.crimsonRegen = true;
            player.PotionDelayModifier *= 0f;
            player.HJScarlet().crimsonCharm = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrimsonRune>().
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
