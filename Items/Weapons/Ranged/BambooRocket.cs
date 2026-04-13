using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class BambooRocket : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override void ExSD()
        {
            Item.width = Item.height = 30;
            Item.damage = 15;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.useTime = Item.useAnimation = 30;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileType<BambooStick>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;

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
