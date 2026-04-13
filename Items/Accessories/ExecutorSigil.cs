using Terraria.ID;
using Terraria;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorSigil : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().tacticalExecution = true;
        }
        public override void AddRecipes()
        {
            if (HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient<ExecutorEmblem>().
                    AddIngredient(ItemID.SoulofLight, 5).
                    AddIngredient(ItemID.SoulofNight, 5).
                    AddTile(TileID.Anvils).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient<ExecutorEmblem>().
                    AddIngredient(ItemID.SoulofLight, 5).
                    AddIngredient(ItemID.SoulofNight, 5).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }

        }
    }
}
