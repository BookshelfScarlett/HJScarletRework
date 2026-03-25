using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
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
        public virtual int ExecutionTime => 10;
        public virtual float ExecutionStrikeDamageMult => 1.0f;
        public override void SetDefaults()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool useFocus = player.HJScarlet().ExecutionTime >= ExecutionTime;
            int projID = ExecutionProj != -1  && useFocus ? ExecutionProj : type;
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            if (useFocus)
            {
                proj.damage = (int)(proj.damage * ExecutionStrikeDamageMult);
                proj.HJScarlet().ExecutionStrike = true;
                player.HJScarlet().ExecutionTime = 0;
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool isPressingLeftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.ExecutionTime").ToLangValue().ToFormatValue(ExecutionTime);
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
