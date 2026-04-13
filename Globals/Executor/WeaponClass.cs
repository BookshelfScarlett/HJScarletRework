using HJScarletRework.Globals.Methods;
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
        public virtual int ExecutionProj => -1;
        public new string LocalizationCategory => $"Weapons.Executor";
        public override bool RangedPrefix() => false;
        public override bool AllowPrefix(int pre) => true;
        public virtual int ExecutionTime => 10;
        public virtual float ExecutionStrikeDamageMult => 1.0f;
        public override void SetDefaults()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Item.GetGlobalItem<ExecutorGlobalItem>().FocusDamageMult != 0 && damage > 0)
                damage = (int)(damage * (1 + Item.GetGlobalItem<ExecutorGlobalItem>().FocusDamageMult)); 
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //初始化。
            if (!player.HJScarlet().StopExecutionInit)
                player.HJScarlet().ExecutionListStored.TryAdd(Type, 0);
            //bool useExecution = CheckExecutionAvailable(player, Type, ExecutionTime);
            bool useExecution = player.CheckExecution(Type, ExecutionTime);
            player.HJScarlet().StopExecutionInit = useExecution;

            int projID = ExecutionProj != -1  && useExecution ? ExecutionProj : type;

            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);

            proj.HJScarlet().HasExecutionMechanic = true;
            if (useExecution)
            {
                float addMult = 1f;
                proj.damage = (int)((proj.damage * ExecutionStrikeDamageMult) * addMult);
                proj.HJScarlet().ExecutionStrike = true;
                player.HJScarlet().StopExecutionInit = false;
                RemoveSlot(player,Type);
            }
            return false;
        }
        public static bool CheckExecutionAvailable(Player player,int curItemType, int itemExecutionTime)
        {
            if(player.HJScarlet().ExecutionListStored.TryGetValue(curItemType, out int value))
            {
                return value >= itemExecutionTime;
            }
            return false;
        }
        public static void RemoveSlot(Player player, int curItemType)
        {
            player.HJScarlet().ExecutionListStored.Remove(curItemType);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool isPressingLeftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.ExecutionTime").ToLangValue().ToFormatValue(Math.Max(0,ExecutionTime - Main.LocalPlayer.HJScarlet().bonusExecutionReduce));
            if (isPressingLeftAlt)
                value = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.ExecutionDescriptionName").ToLangValue();
            Color overrideColor = isPressingLeftAlt ? Color.Lerp(Color.Red,Color.White,0.4f) : Color.GreenYellow;
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "ExecutionTooltipName", value)
            {
                OverrideColor = overrideColor
            };
            //植入Tooltip
                tooltips.Insert(flavorTooltipIndex, flavorTooltips);
            if (isPressingLeftAlt)
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey("ExecutionStrike"));
            ExModifyTooltips(tooltips);
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public virtual void ExSD() { }
    }
    }
