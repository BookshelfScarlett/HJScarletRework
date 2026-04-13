using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class BlackKey : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 32;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.accessory = true;
            Item.defense = 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.15f;
            player.GetArmorPenetration<ExecutorDamageClass>() += 90;
            player.HJScarlet().critDamageExecutor += 0.15f;
            player.HJScarlet().blackKeyHeal = 50;
            player.HJScarlet().blackKeyDoT = true;
            player.HJScarlet().blackKeyReduceDefense = 60;
            player.HJScarlet().blackKeyDefenseBuff = 0.99f;
            player.longInvince = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TinyShortsword>().
                AddIngredient<ExecutorGun>().
                AddIngredient<ExecutorLauncher>().
                AddIngredient<LivingBar>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
