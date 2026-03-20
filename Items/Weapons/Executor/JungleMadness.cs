using HJScarletRework.Executor;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class JungleMadness: ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 0.5f;
        public override int ExecutionTime => 5;
        public override int ExecutionProj => base.ExecutionProj;
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
            Item.shoot = ProjectileType<JungleMadnessProj>();
            Item.useTime = Item.useAnimation = 30;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RichMahoganyHammer).
                AddIngredient(ItemID.JungleSpores, 10).
                AddIngredient(ItemID.Stinger, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
