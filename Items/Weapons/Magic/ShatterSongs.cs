using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class ShatterSongs : HJScarletWeapon
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ClassCategory Category => ClassCategory.Magic;
        public override void ExSD()
        {
            Item.height = Item.width = 50;
            Item.mana = 20;
            Item.useAnimation = Item.useTime= 20;
            Item.rare = ItemRarityID.Purple;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.shootSpeed = 12f;
            Item.noMelee = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CarbonGuitar).
                AddIngredient<DisasterEssence>(10).
                AddIngredient(ItemID.LunarBar, 10).
                AddTile<FinalAnvil>().
                Register();
        }
    }
}
