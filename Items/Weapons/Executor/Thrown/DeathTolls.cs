using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.Thrown
{
    public class DeathTolls : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionProgress => 25;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ForeverNight);
        }
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<DeathTollsProj>();
            Item.useTime = Item.useAnimation = 21;
            Item.knockBack = 8f;
            Item.damage = 162;
            //这里的UseTime是有意改的很慢的
            Item.shootSpeed = 19f;
            //这里不会给音效，因为要考虑一些射弹的联动
            //实际音效会在射弹初始化的时候提供
            Item.UseSound = null;

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //初始化。
            int projID = type;
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            if (player.GetExecutionSrike() && player.HasProj<DeathTollsMinion>())
                proj.HJScarlet().ExecutionStrike = true;
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj<DeathTollsMinion>())
                return;
            if (!player.GetExecutionSrike())
                return;
            int damage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ProjectileType<DeathTollsMinion>(), damage, Item.knockBack, player.whoAmI);
            player.RemoveExecutionProgress();
            player.HJScarlet().tacticalExecutionInputCache = 0;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "FlavorTooltipsName", "「" + value + "」")
            {
                OverrideColor = Color.Lerp(Color.MediumPurple, Color.LightPink, 0.3f)
            };

            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex + 1, flavorTooltips);
        }
    }
}
