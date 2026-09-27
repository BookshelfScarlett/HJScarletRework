using HJScarletRework.Projs.Melee;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TopazYoyo : GemYoyoWeapon
    {
        public override void ExSD()
        {
            base.ExSD();
            Item.damage = 20;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<TopazYoyoProj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodYoyo).
                AddIngredient(ItemID.Topaz, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
