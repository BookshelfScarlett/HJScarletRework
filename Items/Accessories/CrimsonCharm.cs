using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class CrimsonCharm : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static int MinusHeal = 75;
        public static int OverSatuTime = 15;
        public static int MininumHeal = 1;
        public static float MaxHealPower = 2.0f;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinusHeal,OverSatuTime,MininumHeal,MaxHealPower.ToPercent());
        public override void ExSD()
        {
            Item.defense = 4;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.PotionDelayModifier *= 0f;
            player.HJScarlet().crimsonCharm = true;
            player.HJScarlet().healingPotionMult += MaxHealPower;
            player.crimsonRegen = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrimsonRune>().
                AddIngredient<EssenceofLife>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
