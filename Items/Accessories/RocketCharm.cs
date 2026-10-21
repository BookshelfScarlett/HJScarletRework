using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class RocketCharm : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public int AP = 30;
        public static int ExtraAP = 20;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.accessory = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AP, ConceptualBlackKey.BuffTime);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration<ExecutorDamageClass>() += AP;
            player.HJScarlet().rocketCharmLevel = 1;
            player.HJScarlet().blackKeyReduceDefense = ExtraAP;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeepBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
