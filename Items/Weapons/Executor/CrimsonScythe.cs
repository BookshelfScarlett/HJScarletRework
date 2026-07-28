using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
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
        public override int ExecutionProgress => 80;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.damage = 456;
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
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.CheckExecution(Type);
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
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }
}
