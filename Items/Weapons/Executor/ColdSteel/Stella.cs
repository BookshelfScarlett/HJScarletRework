using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class Stella : ExecutorWeaponClass
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override int ExecutionProgress => 50;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Solar);
        }
        public override void ExSD()
        {
            Item.damage = 1426;
            Item.useTime = Item.useAnimation = 36;
            Item.knockBack = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.shootSpeed = 9f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenFury).
                AddIngredient<DisasterBar>(5).
                AddIngredient<UniversalCube>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
