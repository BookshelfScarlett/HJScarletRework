using ContinentOfJourney;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Armor.Diver;
using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{

    [AutoloadEquip(EquipType.Head)]
    public class ReaperHead : HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public float DamageAdd = 0.25f;
        public int CritAdd = 25;
        public static int MaidReaperMaxHealCooldown = 10;
        public override int[] ArmorSlots => [Type, ItemType<ReaperBody>(), ItemType<ReaperLegs>()];
        public override bool SetUpArmorSet => true;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAdd.ToPercent(), CritAdd + "%");
        public int Defense = 50;
        public override void ExSD()
        {
            Item.defense = Defense;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            if (!DownedBossSystem.downedSunGod)
            {
                player.statDefense -= Defense;
                return;
            }
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.HJScarlet().maidReaperArmor = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ModifyTooltipsAdd(tooltips, Mod);
        }
        public static void ModifyTooltipsAdd(List<TooltipLine> tooltips, Mod mod)
        {
            if (DownedBossSystem.downedSunGod)
                return;
            tooltips.ReplaceAllTooltip("");
            string mechanic = mod.GetLocalizationKey("Weapons.Executor.CrimsonScythe.ConditionMechanic");
            tooltips.CreateTooltip(mechanic, Color.White, null, "TlipocaScytheCondition");
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            ModifyTooltipLine(line);
        }
        public static void ModifyTooltipLine(DrawableTooltipLine line)
        {
            if (line.Name == "TlipocaScytheCondition" && line.Mod == "HJScarletRework")
            {
                RarityDrawHelper.DrawCustomTooltipLine(line, Color.Gold, Color.Black);
            }
        }
        public override void UpdateEquip(Player player)
        {
            if (!DownedBossSystem.downedSunGod)
            {
                player.statDefense -= Defense;
                return;
            }

            player.GetDamage<ExecutorDamageClass>() += DamageAdd;
            player.GetCritChance<ExecutorDamageClass>() += CritAdd;
            player.aggro += 500;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleRose).
                AddCondition(HJScarletCraftingConditions.IsDownSlimeGodAndInEclipse).
                DisableDecraft().
                Register();
        }
    }
}
