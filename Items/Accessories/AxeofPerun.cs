using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class AxeofPerun : HJScarletItemClass
    {
        public int Crit = 5;
        public float CritDamage = .10f;

        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Crit + "%", CritDamage.ToPercent());
        public override void ExSD()
        {
            Item.width = 42;
            Item.height = 34;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyExecutorCriticalChanceAdd = Crit;
            player.HJScarlet().critDamageExecutor += CritDamage;


        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 15).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilBar, 10).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
