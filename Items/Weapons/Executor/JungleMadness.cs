using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class JungleMadness : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 15;
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 24;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<JungleMadnessProj>();
            Item.useTime = Item.useAnimation = 30;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RichMahoganyHammer).
                AddIngredient(ItemID.JungleSpores, 15).
                AddIngredient(ItemID.Stinger, 8).
                AddIngredient(ItemID.Vine, 4).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
