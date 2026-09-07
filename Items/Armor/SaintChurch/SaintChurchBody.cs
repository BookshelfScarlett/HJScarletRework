using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.SaintChurch
{
    [AutoloadEquip(EquipType.Body)]
    public class SaintChurchBody : HJScarletArmor
    {
        public float CritDamage = 0.10f;
        public int Crit = 5;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateGolden);
        }
        public override void ExSD()
        {
            Item.defense = 12;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent(), Crit + "%");
        public override void UpdateEquip(Player player)
        {
            player.HJScarlet().critDamageExecutor += CritDamage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeepBar>(10).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
