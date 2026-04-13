using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class SimpleHandAxe : ExecutorWeaponClass
    {
        public override int ExecutionTime => 20;
        public override int ExecutionProj => ProjectileType<SimpleHandAxeExecution>();
        public override void ExSD()
        {
            Item.width = 40;
            Item.height = 40;
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.shootSpeed = 16;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ProjectileType<SimpleHandAxeProj>();
            //音效在射弹初始化时进行
            Item.UseSound = null;
            Item.damage = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
