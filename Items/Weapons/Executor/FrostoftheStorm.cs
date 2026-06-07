using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class FrostoftheStorm : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 12;
        public override void ExSSD()
        {
            HJScarletList.FrostRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.damage = 1600;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true, true);
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = null;
            Item.knockBack = 2f;
            Item.shoot = ProjectileType<FrostoftheStormHeldProj>();
            Item.shootSpeed = 16;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot) && !player.HasProj<FrostoftheStormExecution>() && !player.HasProj<FrostoftheStormChargeProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            ((FrostoftheStormHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((FrostoftheStormHeldProj)proj.ModProjectile).Flip = true;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Frostbrand).
                AddIngredient<Frostgrief>().
                AddIngredient<FinalBar>(5).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
