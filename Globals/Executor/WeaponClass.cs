using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool isPressingLeftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.ExecutionProgress").ToLangValue().ToFormatValue(Math.Max(0, ExecutionProgress - Main.LocalPlayer.HJScarlet().bonusExecutionReduce));
            if (isPressingLeftAlt)
                value = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.ExecutionDescriptionName").ToLangValue();
            Color overrideColor = isPressingLeftAlt ? Color.Lerp(Color.Red, Color.White, 0.4f) : Color.GreenYellow;
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "ExecutionTooltipName", value)
            {
                OverrideColor = overrideColor
            };
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex, flavorTooltips);
            if (isPressingLeftAlt)
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey("ExecutionStrike"));
            string value2 = Mod.GetLocalizationKey($"DamageClasses.ExecutorDamageClass.WeaponType.{WeaponCategory}").ToLangValue();
            int executionTooltipNameLine = tooltips.FindIndex(line => line.Name == "ExecutionTooltipName" && line.Mod == "HJScarletRework");
            TooltipLine cate = new(Mod, "ExecutorWeaponTypeName", value2)
            {
                OverrideColor =Color.LightGoldenrodYellow, 
            };
            tooltips.Insert(executionTooltipNameLine, cate);
            ExModifyTooltips(tooltips);
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public virtual void ExSD() { }
    }
}
