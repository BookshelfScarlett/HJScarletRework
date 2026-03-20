using HJScarletRework.Executor;
using HJScarletRework.Globals.Instances;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class PerfectNight: ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 0.5f;
        public override int ExecutionTime => 20;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useTime = Item.useAnimation = 30;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilHammer).
                AddIngredient<JungleMadness>().
                AddIngredient<DungeonBreaker>().
                AddIngredient<BlazingStriker>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
