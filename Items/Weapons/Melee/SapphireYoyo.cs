using HJScarletRework.Projs.Melee;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SapphireYoyo : GemYoyoWeapon
    {
        public override void ExSD()
        {
            base.ExSD();
            Item.damage = 13;
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<SapphireYoyoProj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodYoyo).
                AddIngredient(ItemID.Sapphire, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
