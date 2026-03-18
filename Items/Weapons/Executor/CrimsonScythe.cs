using HJScarletRework.Executor;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class CrimsonScythe : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public int FocusStrikeTime = 30;
        public int CurSwingTime = 0;
        public override void ExSD()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.width = 88;
            Item.height = 82;
            Item.damage = 456;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.rare = RarityType<DisasterRarity>();
            Item.shoot = ProjectileType<CrimsonScytheHeldProj>();
            Item.shootSpeed = 10;
            Item.knockBack = 5;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "Damage" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value2 = Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.FocusTime").ToLangValue().ToFormatValue(FocusStrikeTime);
            //实例化toolti并注册名字
            TooltipLine flavorTooltips2 = new TooltipLine(Mod, "FocusTooltipName", value2);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips2);
            //通过本地化路径搜索需要的特殊文本
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);

            if(Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey("FocusStrike"));

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && (line.Name == "ItemName"))
            {
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            if(line.Mod == "Terraria" && (line.Name == "Damage" ))
            {
                DisasterRarity.DrawMisc(line);
                return false;

            }
            if (line.Name == "FlavorTooltipsName" && line.Mod == Mod.Name)
            {
                DisasterRarity.DrawFlavorRarity2(line);
                return false;
            }
            if (line.Name == "FocusTooltipName" && line.Mod == Mod.Name)
            {
                DisasterRarity.DrawFlavorRarity2(line);
                return false;
            }

            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            ((CrimsonScytheHeldProj)proj.ModProjectile).SwingType = (CrimsonScytheHeldProj.SwingState)CurSwingTime;
            CurSwingTime++;
            if (CurSwingTime > 2)
                CurSwingTime = 0;
            return false;
        }
    }
}
