using ContinentOfJourney;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Armor.Diver;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{
    [AutoloadEquip(EquipType.Body)]
    public class ReaperBody : HJScarletArmor
    {
        public float CritDamage = 0.15f;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent());
        public int Defense = 50;
        public override void ExSD()
        {
            Item.defense = Defense;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ReaperHead.ModifyTooltipsAdd(tooltips, Mod);
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            ReaperHead.ModifyTooltipLine(line);
        }

        public override void UpdateEquip(Player player)
        {
            if (!DownedBossSystem.downedSunGod)
            {
                player.statDefense -= Defense;
                return;
            }
            player.HJScarlet().critDamageExecutor += CritDamage;
            player.aggro += 500;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MaidShirt2).
                AddCondition(HJScarletCraftingConditions.IsDownSlimeGodAndInEclipse).
                DisableDecraft().
                Register();
        }
    }
}
