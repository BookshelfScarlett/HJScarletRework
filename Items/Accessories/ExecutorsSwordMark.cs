using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorsSwordMark : HJScarletItemClass
    {
        public static float CritDamage = .10f;
        public static int ExecutionProgressRegen = 2;
        public static int CasterExecutionProgressRegen = 6;
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);

        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExecutionProgressRegen, CasterExecutionProgressRegen, CritDamage.ToPercent());
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().executorSwordMarkLevel = 2;
        }
        public override void AddRecipes()
        {
            if (HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient<ExecutorsSwordMarkSmall>().
                    AddIngredient(ItemID.SoulofLight, 5).
                    AddIngredient(ItemID.SoulofNight, 5).
                    AddTile(TileID.Anvils).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient<ExecutorsSwordMarkSmall>().
                    AddIngredient(ItemID.SoulofLight, 5).
                    AddIngredient(ItemID.SoulofNight, 5).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
        }
    }
}
