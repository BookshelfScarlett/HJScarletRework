using HJScarletRework.Assets.Registers;
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
    public class DreamingLight : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1f;
        public override int ExecutionProgress => 15;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ForeverNight);
        }

        public override void ExSD()
        {
            Item.damage = 60;
            Item.knockBack = 8f;
            Item.shootSpeed = 20f;
            Item.useTime = Item.useAnimation = 40;
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.shoot = ProjectileType<DreamingLightProj>();
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { MaxInstances = 0, Pitch = -0.5f };
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //初始化。
            int projID = type;
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj<DreamingLightMinion>())
                return;
            if (!player.GetExecutionSrike())
                return;
            int damage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ProjectileType<DreamingLightMinion>(), damage, Item.knockBack, player.whoAmI);
            player.RemoveExecutionProgress();
            player.HJScarlet().tacticalExecutionInputCache = 0;
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            int flavorTooltipIndex2 = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new(Mod, "FlavorTooltipsName", "「" + value + "」")
            {
                OverrideColor = Color.Lerp(Color.MediumPurple, Color.LightPink, 0.3f)
            };
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex2 + 1, flavorTooltips);

        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DreamlessNight>().
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddTile(TileID.Beds).
                Register();
        }
    }
}
