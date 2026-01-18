using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class BambooSong : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Magic;
        public override void ExSD()
        {
            Item.height = Item.width = 50;
            Item.mana = 20;
            Item.useAnimation = Item.useTime= 20;
            Item.rare = ItemRarityID.Purple;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 12f;
            Item.noMelee = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BambooBlock, 20).
                AddIngredient(ItemID.JungleGrassSeeds, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
