using ContinentOfJourney;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReaperLegs : HJScarletArmor
    {
        public float MoveSpeed = .25f;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeed.ToPercent());
        public int Defense = 45;
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
            player.moveSpeed += MoveSpeed;
            player.aggro += 500;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MaidPants2).
                AddCondition(HJScarletCraftingConditions.IsDownSlimeGodAndInEclipse).
                DisableDecraft().
                Register();
        }

    }
}
