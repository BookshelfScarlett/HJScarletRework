using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class MiniatureBayonet : HJScarletItemClass
    {
        public int Crit = 10;
        public float CritDamage = .15f;

        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Crit + "%", CritDamage.ToPercent());
        public override void ExSD()
        {
            Item.width = Item.height = 33;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyExecutorCriticalChanceAdd = Crit;
            player.HJScarlet().critDamageExecutor += 0.15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AxeofPerun>().
                AddIngredient(ItemID.SpectreBar, 10).
                AddIngredient(ItemID.SoulofMight, 10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }

    }

}
