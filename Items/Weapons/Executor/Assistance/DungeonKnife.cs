using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Assistance
{
    public class DungeonKnife: ExecutorWeaponClass
    {
        public override int ExecutionProgress => 20;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Assistance;
        public override void ExSSD()
        {
            base.ExSSD();
        }
        public override void ExSD()
        {
            Item.damage = 54;
            Item.knockBack = 4;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 25;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<DungeonKnifeProj>();
            Item.HJScarlet().ExecutionProj = ProjectileType<DungeonKnifeMark>();
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.SetUpNoUseGraphicItem();
            Item.HJScarlet().ForceAutomaticExecution = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            GhostKnife.QuickSpawnMark(Type, Item.HJScarlet().ExecutionProj, player, source, position);
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
