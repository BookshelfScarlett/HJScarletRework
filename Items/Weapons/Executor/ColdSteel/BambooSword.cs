using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class BambooSword : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 8;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public override void ExSSD()
        {
            ScarletItemIDSets.ForceToTacticalExecute[Type] = true;
        }
        public bool Flip = false;
        public int ReuseDelay = 0;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.damage = 34;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<BambooSwordHeldProj>();
            Item.knockBack = 4;
            Item.HJScarlet().ExecutionProj = ProjectileType<BambooSwordSpin>();
            Item.shootSpeed = 14;
        }
        public override bool CanShoot(Player player)
        {
            return ReuseDelay == 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.GetExecutionSrike();
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((BambooSwordHeldProj)proj.ModProjectile).Flip = Flip;
            Flip = !Flip;
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (ReuseDelay > 0)
            {
                player.KillCertainProj(Item.shoot);
                ReuseDelay--;
            }
            if (player.GetExecutionSrike())
            {
                ReuseDelay = 50;
                int damage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, player.Center.GetNormalVector2(Main.MouseWorld) * 40f, Item.HJScarlet().ExecutionProj, damage, 1f, player.whoAmI);
                }
                player.RemoveExecutionProgress(Type);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BambooBlock, 30).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
