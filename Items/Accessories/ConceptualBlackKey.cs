using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
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
        public static int ExtraAP = 60;
        public static float DamageReduce = .99f;
        public int HealAmit = 24;
        public static int BuffTime = 5;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.width = Item.height = 32;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.accessory = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            DamageAdd.ToPercent(), CritDamage.ToPercent(), AP, BuffTime, HealAmit);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //黑键系列的常驻伤害加成，这样写用于防止上下位叠加
            player.HJScarlet().blackKeyExecutorDamageAdd = DamageAdd;
            player.HJScarlet().blackKeyExecutorCriticalChanceAdd = Crit;
            player.HJScarlet().critDamageExecutor += CritDamage;
            //黑键系列的治疗加成，防止上下位叠加
            player.HJScarlet().blackKeyHeal = HealAmit;
            //继承火箭挂饰的穿甲
            player.GetArmorPenetration<ExecutorDamageClass>() += AP;
            player.HJScarlet().blackKeyReduceDefense = ExtraAP;
            //继承拱形弓弩的防御
            player.HJScarlet().blackKeyDefenseBuff = DamageReduce;
            //标记为黑键
            player.HJScarlet().blackKeyDoT = true;
            player.longInvince = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
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
