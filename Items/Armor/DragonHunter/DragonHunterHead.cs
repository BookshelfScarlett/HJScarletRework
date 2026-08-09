using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.RedDragonKnight;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonHunter
{
    [AutoloadEquip(EquipType.Head)]
    public class DragonHunterHead :HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemType<DragonHunterBody>(),ItemType<DragonHunterLegs>()];
        public override bool SetUpArmorSet => true;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.defense = 30;
        }
        public float Damage = .20f;
        public float AttackSpeed = .15f;
        public static int RangedCrit = 40;
        public int MagicMana = 50;
        public float MagicManaCost = .20f;
        public int SummonerMinionSlot = 1;
        public int SummonerSentrySlot = 1;
        public static float FixedDamage = .20f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage; 
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += AttackSpeed;
            player.GetCritChance<RangedDamageClass>() += RangedCrit;
            player.statManaMax2 += MagicMana;
            player.manaCost -= MagicManaCost;
            player.maxMinions = SummonerMinionSlot;
            player.maxTurrets = SummonerSentrySlot;
            player.HJScarlet().dragonHunter = true;
            string value = setBonusPath.ToLangValue().ToFormatValue(AttackSpeed.ToPercent(), RangedCrit + "%", MagicMana, MagicManaCost.ToPercent(), SummonerMinionSlot, FixedDamage.ToPercent());
            player.setBonus += "\n" + value;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RedDragonKnightHead>().
                AddIngredient<SunlightGel>(4).
                AddIngredient<EssenceofTime>(4).
                AddIngredient<EssenceofLife>(4).
                AddIngredient<EssenceofMatter>(4).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
