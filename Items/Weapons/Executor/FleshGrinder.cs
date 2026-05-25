using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class FleshGrinder : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 0.8f;
        public override int ExecutionProgress => 20;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 33;
            Item.knockBack = 5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<FleshGrinderProj>();
            Item.UseSound = SoundID.Item1;
            Item.useTime = Item.useAnimation = 25;
            Item.SetUpRarityPrice(ItemRarityID.Green);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShadewoodHammer).
                AddIngredient(ItemID.CrimtaneBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
