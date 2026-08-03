using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Misc
{
    public class ArcticGuanDao : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 9;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Frost);
        }
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.HJScarlet().ForceTacticalExecution = true;
            Item.damage = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 25;
            Item.shoot = ProjectileType<ArcticGuanDaoHeldProj>();
            Item.shootSpeed = 7;
            Item.UseSound = null;
            Item.knockBack = 6.7f;
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            bool exe = player.GetExecutionSrike();
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((ArcticGuanDaoHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((ArcticGuanDaoHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.IceBlock, 5).
                AddIngredient(ItemID.Shiverthorn, 1).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
