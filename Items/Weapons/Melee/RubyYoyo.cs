using HJScarletRework.Projs.Melee;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class RubyYoyo : GemYoyoWeapon
    {
        public override void ExSD()
        {
            base.ExSD();
            Item.damage = 30;
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<RubyYoyoProj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodYoyo).
                AddIngredient(ItemID.Ruby, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }

}
