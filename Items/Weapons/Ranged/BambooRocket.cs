using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class BambooRocket : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override void ExSD()
        {
            Item.width = Item.height = 30;
            Item.damage = 26;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item51;
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.useTime = Item.useAnimation = 30;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileType<BambooStick>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;

        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BambooBlock, 50).
                AddIngredient(ItemID.JungleSpores, 5).
                AddTile(TileID.Sawmill).
                Register();
        }
    }
}
