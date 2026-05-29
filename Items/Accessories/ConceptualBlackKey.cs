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
    public class ConceptualBlackKey : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public float DamageAdd = .15f;
        public int Crit = 15;
        public float CritDamage = .30f;
        public int AP = 60;
        public int HealAmit = 50;
        public override void ExSD()
        {
            Item.width = Item.height = 32;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.accessory = true;
            Item.defense = 10;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAdd.ToPercent(), CritDamage.ToPercent(), AP, HealAmit);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration<ExecutorDamageClass>() += AP;
            player.HJScarlet().blackKeyExecutorDamageAdd = DamageAdd;
            player.HJScarlet().blackKeyExecutorCriticalChanceAdd = (int)(DamageAdd * 100f);
            player.HJScarlet().critDamageExecutor += CritDamage;
            player.HJScarlet().blackKeyHeal = HealAmit;
            player.HJScarlet().blackKeyDoT = true;
            player.HJScarlet().blackKeyReduceDefense = 60;
            player.HJScarlet().blackKeyDefenseBuff = 0.99f;
            player.longInvince = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MiniatureBayonet>().
                AddIngredient<MachineGunTalisman>().
                AddIngredient<RocketCharm>().
                AddIngredient<LivingBar>(10).
                AddIngredient<CubistBar>(10).
                AddIngredient<EternalBar>(10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
