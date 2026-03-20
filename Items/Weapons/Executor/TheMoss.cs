using HJScarletRework.Globals.Executor;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class TheMoss: ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 0.5f;
        public override int ExecutionTime => 15;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<TheMossProj>();
            Item.useTime = Item.useAnimation = 30;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EbonwoodHammer).
                AddIngredient(ItemID.DemoniteBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
