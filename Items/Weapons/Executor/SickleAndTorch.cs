using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class SickleAndTorch : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 6;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.damage = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 30;
        }
        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Sickle).
                AddIngredient(ItemID.TikiTorch).
                AddRecipeGroup(RecipeGroupID.IronBar, 4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
