using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Executor
{
    public abstract class ExecutorWeaponClass : ModItem, ILocalizedModType
    {
        public override string Texture => $"HJScarletRework/Assets/Texture/Items/Weapons/{GetType().Name}";
        /// <summary>
        /// 该武器处决攻击时的射弹
        /// 实际上只有一个标记，而且用于默认Shoot的直接作用
        /// 连我自己大部分情况下都不会直接用这个字段
        /// </summary>
        public virtual int ExecutionProj => -1;
        public new string LocalizationCategory => $"Weapons.Executor";
        /// <summary>
        /// 代行者的武器类型
        /// 划分为五种：投掷品、不脱手冷兵器、热武器、魔术载体和仆从
        /// </summary>
        public virtual WeaponCategory WeaponCategory => WeaponCategory.Misc;
        public override bool WeaponPrefix() => true;
        public override bool RangedPrefix() => false;
        public virtual int ExecutionProgress => 10;
        public virtual float ExecutionStrikeDamageMult => 1.0f;
        public override void SetStaticDefaults()
        {
            ExSSD();
            HJScarletList.ExecutorWeaponDictionary.Add(Type, ExecutionProgress);
            HJScarletList.ExecutorWeaponTypeDictionary.Add(Type, WeaponCategory);

        }

        public virtual void ExSSD()
        {
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            if (Item.GetGlobalItem<ExecutorGlobalItem>().ExecutionDamageMult != 1 && damage > 0 && player.CheckExecution(Type))
            {
                damage = (int)(damage * ExecutionStrikeDamageMult * (Item.GetGlobalItem<ExecutorGlobalItem>().ExecutionDamageMult));
            }
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //初始化。
            bool useExecution = player.CheckExecution(Type);
            int projID = Item.HJScarlet().ExecutionProj != -1 && useExecution ? Item.HJScarlet().ExecutionProj : type;
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            player.HJScarlet().tacticalExecutionInputCache = 0;
            if (useExecution)
            {
                proj.HJScarlet().ExecutionStrike = true;
                player.RemoveExecutionProgress(Type);
            }
            return false;
        }
        public int FirstLineY = -1;
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool traditionalMode = HJScarletConfigClient.Instance.TraditionalExecutionTooltipShowcase;
            bool isPressingLeftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
            int requirements = Math.Max(0, ExecutionProgress - Main.LocalPlayer.HJScarlet().bonusExecutionReduce);
            string progressText = Mod.GetLocalizationKey("ExecutorDamageClass.ExecutionProgress").ToLangValue().ToFormatValue(requirements);
            string executionText = (traditionalMode && isPressingLeftAlt) ? Mod.GetLocalizationKey("ExecutorDamaegeClass.ExecutionDescriptionName").ToLangValue() : progressText;
            Color executionColor = (traditionalMode && isPressingLeftAlt) ? Color.Lerp(Color.Red, Color.White, .4f) : Color.GreenYellow;

            int executionProgressIndex = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            if (traditionalMode)
            {
                var executionLine = new TooltipLine(Mod, "ExecutionTooltipName", executionText)
                {
                    OverrideColor = executionColor
                };
                tooltips.Insert(executionProgressIndex, executionLine);
                if (traditionalMode && isPressingLeftAlt)
                    tooltips.ReplaceAllTooltip(this.GetLocalizationKey("ExecutionStrike"));
            }

            string categoryText = Mod.GetLocalizationKey($"ExecutorDamageClass.WeaponType.{WeaponCategory}").ToLangValue();
            int executionLineIndex = tooltips.FindIndex(line => line.Name == "ExecutionTooltipName" && line.Mod == "HJScarletRework");
            if (!traditionalMode)
                executionLineIndex = executionProgressIndex - 1;
            var categoryLine = new TooltipLine(Mod, "ExecutorWeaponTypeName", categoryText)
            {
                OverrideColor = Color.LightGoldenrodYellow
            };
            tooltips.Insert(executionLineIndex + 1, categoryLine);
            CacheTooltipList = tooltips;
            ExModifyTooltips(tooltips);
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {

            if (HJScarletConfigClient.Instance.TraditionalExecutionTooltipShowcase)
                return base.PreDrawTooltipLine(line, ref yOffset);
            else
            {
                //记录起始点坐标。
                //通常情况下，物品不可能没有名字，而物品名称通常都在第一行，所以可以用这个来记录第一行的坐标
                if (line.IsItemName())
                {
                    TextboxManager.FirstLineY = line.Y;
                }
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
                    BackgroundEdgeColor = Color.Lerp(Color.Black, Color.Red, 0.40f) * .98f,
                    MainText = detailText,
                    TextColor = Color.White,
                    TextEdgeColor = Color.Black,
                    TitleTextSize = 1.15f
                };
                //最后传值。
                TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
                return true;
            }
        }
        public virtual void ExSD() { }
    }
}
