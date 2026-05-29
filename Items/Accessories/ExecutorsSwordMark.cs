using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorsSwordMark : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().tacticalExecution = true;
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
