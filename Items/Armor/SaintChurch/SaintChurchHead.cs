using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.SaintChurch
{
    [AutoloadEquip(EquipType.Head)]
    public class SaintChurchHead : HJScarletArmor
    {
        public override bool SetUpArmorSet => true;
        public override int[] ArmorSlots => [Type, ItemType<SaintChurchBody>(), ItemType<SaintChurchLegs>()];
        public float Damage = .08f;
        public int Crit = 10;
        public float CritDamage = .15f;

        public static float RespawnLifePercentFirst = .20f;
        public static float RespawnLifePercent = .10f;
        public static float DamageBonus = .05f;
        public static int CritBonus = 5;
        public static int Aggro = 1000;
        public static int RespawnChance = 1;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.FateGolden);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%", CritDamage.ToPercent());
        public override void ExSD()
        {
            Item.defense = 12;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += '\n'+ setBonusPath.ToLangValue().ToFormatValue(Aggro, RespawnLifePercentFirst.ToPercent(), RespawnLifePercent.ToPercent(), DamageBonus.ToPercent(), CritBonus + "%");
            player.HJScarlet().saintChurch = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeepBar>(6).
                AddIngredient(ItemID.Silk, 6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
