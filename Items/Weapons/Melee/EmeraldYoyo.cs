using HJScarletRework.Projs.Melee;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EmeraldYoyo : GemYoyoWeapon
    {
        public override void ExSD()
        {
            base.ExSD();
            Item.damage = 12;
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<EmeraldYoyoProj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodYoyo).
                AddIngredient(ItemID.Emerald, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }

}
