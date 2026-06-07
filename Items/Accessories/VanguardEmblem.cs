using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class VanguardEmblem : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.accessory = true;
            Item.defense = 1;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().vanguardEmblem = true;
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            if (!HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient(ItemID.CobaltShield).
                    AddIngredient<ExecutorEmblem>().
                    AddIngredient(ItemID.HallowedBar, 10).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient(ItemID.CobaltShield).
                    AddIngredient<ExecutorEmblem>().
                    AddRecipeGroup(HJScarletRecipeGroup.AnyMechBossSoul).
                    AddTile(TileID.Anvils).
                    Register();
            }

        }
    }
}
