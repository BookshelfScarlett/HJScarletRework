using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HJScarletRework.Executor
{
    public abstract class ExecutorWeaponClass : ModItem, ILocalizedModType
    {
        public override string Texture => $"HJScarletRework/Assets/Texture/Items/Weapons/{GetType().Name}";
        public virtual int FocusProj => -1;
        public new string LocalizationCategory => $"Weapons.Executor";
        public virtual int FocusStrikeTime => 10;
        public virtual float FocusStrikeDamageMultipler => 1.0f;
        public override void SetDefaults()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool useFocus = player.HJScarlet().FocusStrikeTime >= FocusStrikeTime;
            int projID = FocusProj == -1 ? type : FocusProj;
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
            proj.HJScarlet().UseFocusStrikeMechanic = true;
            if (useFocus)
            {
                proj.damage = (int)(proj.damage * FocusStrikeDamageMultipler);
                proj.HJScarlet().FocusStrike = true;
                player.HJScarlet().FocusStrikeTime = 0;
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.FocusTime").ToLangValue().ToFormatValue(FocusStrikeTime);
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "FocusTooltipName", value)
            {
                OverrideColor = Color.LightGreen
            };
            //植入Tooltip
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey("FocusStrike"));
            else
                tooltips.Insert(flavorTooltipIndex, flavorTooltips);
            ExModifyTooltips(tooltips);
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public virtual void ExSD() { }
    }
    }
