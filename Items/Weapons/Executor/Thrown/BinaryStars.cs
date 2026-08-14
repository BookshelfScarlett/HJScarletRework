using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Thrown
{
    public class BinaryStars : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionProgress => 32;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Nebula);
        }
        public override void ExSD()
        {
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.damage = 380;
            Item.useTime = Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<BinaryStarsProj>();
            Item.knockBack = 12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AetherfireSmasher>().
                AddIngredient<DeathTolls>().
                AddIngredient<CrownofSilveryLight>(15).
                AddTile<ContinentOfJourney.Tiles.FinalAnvil>().
                Register();
        }
    }
}