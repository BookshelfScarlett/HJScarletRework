using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class LavaFlow : ExecutorWeaponClass
    {
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override int ExecutionProj => ProjectileType<LavaFlowExecution>();
        public override int ExecutionProgress => 8;
        public override void ExSD()
        {
            Item.damage = 27;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item45 with { MaxInstances = 0 };
            Item.shootSpeed = 13f;
            Item.shoot = ProjectileType<LavaFlowProj>();
            Item.rare = ItemRarityID.Orange;
            Item.HJScarlet().ExecutionProj = ProjectileType<LavaFlowExecution>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 18).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
