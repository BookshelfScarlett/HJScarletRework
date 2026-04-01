using HJScarletRework.Globals.Executor;
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
    public class CrimsonScythe : ExecutorWeaponClass
    {
        public override int ExecutionTime => 30;
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
            Item.shoot = ProjectileType<CrimsonScytheProj>();
            Item.shootSpeed = 10;
            Item.knockBack = 5;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2+1, flavorTooltips);

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && (line.Name == "ItemName"))
            {
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            if(line.Mod == "Terraria" && (line.Name == "CritChance"))
            {
                DisasterRarity.DrawMisc(line);
                return false;
            }

            if(line.Mod == "Terraria" && (line.Name == "Damage"))
            {
                DisasterRarity.DrawMisc(line);
                return false;
            }
            if(line.Mod == Mod.Name && (line.Name == "FlavorTooltipsName"))
            {
                DisasterRarity.DrawMisc(line);
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
            ((CrimsonScytheProj)proj.ModProjectile).SwingType = (CrimsonScytheProj.SwingState)CurSwingTime;
            CurSwingTime++;
            if (CurSwingTime > 2)
                CurSwingTime = 0;
            return false;
        }
    }
}
