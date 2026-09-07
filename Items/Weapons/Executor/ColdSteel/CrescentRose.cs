using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class CrescentRose : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 40;
        public static int DefensePerAdd = 2;
        public static int MaxSoulStone = 20;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
            ScarletItemIDSets.GrantsBoosterAfterSon[Type] = true;
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 1413;
            Item.useTime = Item.useAnimation = 30;
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "FlavorTooltipsName", value)
            {
                OverrideColor = Color.Lerp(Color.MediumPurple, Color.LightPink, 0.3f)
            };
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            //if (line.Name == "FlavorTooltipsName" && line.Mod == Mod.Name && HJScarletConfigClient.Instance.SpecialRarity)
            //{
            //    ScarletRedRarity.DrawFlavorNameRarity(line);
            //    return false;
            //}

            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrimsonScythe>().
                AddIngredient<UniversalCube>(15).
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
