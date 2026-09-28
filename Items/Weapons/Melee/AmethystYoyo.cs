using HJScarletRework.Projs.Melee;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class AmethystYoyo : GemYoyoWeapon
    {
        public override void ExSD()
        {
            base.ExSD();
            Item.damage = 11;
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<AmethystYoyoProj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodYoyo).
                AddIngredient(ItemID.Amethyst, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }

}
