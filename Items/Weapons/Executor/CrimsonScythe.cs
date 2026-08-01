using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class CrimsonScythe : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 40;
        public static int DefensePerAdd = 2;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.damage = 956;
            Item.useTime = Item.useAnimation = 30;
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.HJScarlet().CanDrawIcon = false;
            Item.HJScarlet().CanDrawGhost = true;
            Item.shootSpeed = 10;
            Item.shoot = ProjectileType<CrimsonScytheHeldProj>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot) && !player.HasProj<CrimsonScytheSkillProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            bool exe = player.GetExecutionSrike();
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((CrimsonScytheHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((CrimsonScytheHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return true;
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (HJScarletConfigClient.Instance.TraditionalExecutionTooltipShowcase)
                return;
            //记录起始点坐标。
            //通常情况下，物品不可能没有名字，而物品名称通常都在第一行，所以可以用这个来记录第一行的坐标
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            var settingList = new List<TextboxSettings>();
            string detailText = this.GetLocalizationKey("ExecutionStrike").ToLangValue();
            int requirements = Math.Max(0, ExecutionProgress - Main.LocalPlayer.HJScarlet().bonusExecutionReduce);
            int curRequirement = Main.LocalPlayer.HJScarlet().ExecutionListStored.TryGetValue(Type, out int value) ? value : 0;
            string numberText = Mod.GetLocalizationKey("ExecutorDamageClass.ExecutionProgressRevampedMode").ToLangValue().ToFormatValue(curRequirement, requirements);
            detailText += "\n" + "\n" + numberText;
            //一堆设置，巴拉巴拉。
            TextboxSettings sets = new TextboxSettings()
            {
                TitleText = Mod.GetLocalizationKey("ExecutorDamageClass.ExecutionDescriptionName").ToLangValue(),
                TitleTextColor = Color.Lerp(Color.Crimson, Color.WhiteSmoke, 1f) with { A = 255 },
                TitleEdgeColor = Color.DarkRed,
                HasTitle = true,
                BackgroundColor = Color.Lerp(Color.WhiteSmoke, Color.Black, .9f) * .60f,
                BackgroundEdgeColor = Color.Lerp(Color.White, Color.Red, 0.0f) * .78f,
                MainText = detailText,
                TextColor = Color.White,
                TextEdgeColor = Color.Black,
                TitleTextSize = 1.15f
            };
            settingList.Add(sets);
            //最后传值。
            detailText = this.GetLocalizationKey("ExtraMechanic").ToLangValue();
            sets = new TextboxSettings()
            {
                TitleText = Mod.GetLocalizationKey("ExecutorDamageClass.CompanionWeapon").ToLangValue(),
                TitleTextColor = Color.Lerp(Color.Crimson, Color.WhiteSmoke, 1f) with { A = 255 },
                TitleEdgeColor = Color.DarkRed,
                HasTitle = true,
                BackgroundColor = Color.Lerp(Color.WhiteSmoke, Color.Black, .9f) * .60f,
                BackgroundEdgeColor = Color.Lerp(Color.White, Color.Red, 0.0f) * .78f,
                MainText = detailText,
                TextColor = Color.White,
                TextEdgeColor = Color.Black,
                TitleTextSize = 1.15f
            };
            settingList.Add(sets);
            TextboxMethods.DrawMultipleTextboxes(line, CacheTooltipList, settingList, 30);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DeathSickle).
                AddCondition(HJScarletCraftingConditions.IsDownSlimeGodAndInEclipse).
                AddTile(FinalAnvilTile).
                DisableDecraft().
                Register();
        }
    }
}
