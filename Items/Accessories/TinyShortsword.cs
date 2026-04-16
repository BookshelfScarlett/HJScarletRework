using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class TinyShortsword : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 33;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            
            player.GetArmorPenetration<ExecutorDamageClass>() += 30;
            player.HJScarlet().blackKeyReduceDefense = 20;
            player.HJScarlet().blackKeyDoT = true;
            player.longInvince = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AxeofPerun>().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient(ItemID.SpectreBar,10).
                AddIngredient(ItemID.SoulofMight, 10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }

    }

}
