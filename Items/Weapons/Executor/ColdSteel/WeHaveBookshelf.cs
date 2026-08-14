using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    /// <summary>
    /// 是的，我把书架做成了武器
    /// </summary>
    public class WeHaveBookshelf : ExecutorWeaponClass
    {
        public override string Texture => GetVanillaAssetPath(Globals.Enums.VanillaAsset.Item, ItemID.Bookcase);
        public override int ExecutionProgress => 20;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.damage = 120;
            Item.knockBack = 5f;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<WeHaveBookshelfHeldProj>();
            Item.shootSpeed = 16f;
        }

        public override bool CanShoot(Player player)
        {
            return !player.HasProj<WeHaveBookshelfHeldProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.GetExecutionSrike();
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((WeHaveBookshelfHeldProj)proj.ModProjectile).TargetRotation = dir.ToRotation();
            ((WeHaveBookshelfHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Book, 3).
                AddRecipeGroup(RecipeGroupID.Wood, 6).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
