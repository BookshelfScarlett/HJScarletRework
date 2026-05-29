using ContinentOfJourney.NPCs.Boss_TheLifebringer;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public class FloretProtectorHelmetAlter : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FloretProtectorHelmet;
        public static int Defense = 40;
        public override string SetupName => "FloretProtector";
        public override ArmorType Category => ArmorType.Helmet;
        public override bool SetUpArmorSet => true;
        public override int DownedConditionID => NPCType<TheLifebringerHead>();
        public override int[] ArmorSlots => [ItemID.FloretProtectorHelmet,ItemID.FloretProtectorChestplate,ItemID.FloretProtectorLegs];
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.4f;
            player.GetCritChance<ExecutorDamageClass>() += 40f;
        }
        public override void SetStaticDefaults()
        {
            HJScarletList.ConvertedItemRarityDrawDictionary.Add(ApplyArmor, LivingRarity.DrawRarity);
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Red;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (!set.Equals(ArmorSetName))
                return;
            player.statLifeMax2 += 100;
            player.HJScarlet().floretProtectorExecutor = true;
            string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.SetBonus");
            string armorCategory2 = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.BuffDetail");
            string value;
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                value = armorCategory2;
            else
                value = armorCategory;
            player.setBonus += "\n" + value.ToLangValue();
        }
        public override void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
        {
        }

    }
    public class FlorectProtectorChestplateAlter : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FloretProtectorChestplate;
        public static int Defense = 60;
        public override int DownedConditionID => NPCType<TheLifebringerHead>();
        public override ArmorType Category => ArmorType.Chestplate;
        public override string SetupName => "FloretProtector";
        public override void SetStaticDefaults()
        {
            HJScarletList.ConvertedItemRarityDrawDictionary.Add(ApplyArmor, LivingRarity.DrawRarity);
        }

        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.10f;
            player.noKnockback = true;
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Red;
        }
        public override void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
        {
            tooltips.AddSwapTooltipValueBossCondition(NPCType<TheLifebringerHead>());
        }

    }
    public class FlorectProtectorLegsAlter : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FloretProtectorLegs;
        public static int Defense = 30;
        public override int DownedConditionID => NPCType<TheLifebringerHead>();
        public override string SetupName => "FloretProtector";
        public override ArmorType Category => ArmorType.Legs;
                public override void SetStaticDefaults()
        {
            HJScarletList.ConvertedItemRarityDrawDictionary.Add(ApplyArmor, LivingRarity.DrawRarity);
        }

        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.05f;
            player.GetCritChance<ExecutorDamageClass>() += 5;
            player.moveSpeed += 0.20f;
        }
        public override void ExSD(Item item)
        {
            item.vanity = false;
            item.defense = Defense;
            item.rare = ItemRarityID.Red;
        }
        public override void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
        {
            tooltips.AddSwapTooltipValueBossCondition(NPCType<TheLifebringerHead>());
        }
    }
}
