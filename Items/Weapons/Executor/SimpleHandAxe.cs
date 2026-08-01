using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class SimpleHandAxe : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 15;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.shootSpeed = 44;
            Item.useTime = Item.useAnimation = 16;
            Item.shoot = ProjectileType<SimpleHandAxeProj>();
            Item.HJScarlet().ExecutionProj = ProjectileType<SimpleHandAxeExecution>();
            //音效在射弹初始化时进行
            Item.UseSound = null;
            Item.damage = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 5).
                AddIngredient(ItemID.Diamond, 2).
                AddIngredient(ItemID.FallenStar, 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
