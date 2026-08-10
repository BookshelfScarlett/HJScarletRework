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
            Item.width = Item.height = 86;
            Item.damage = 198;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<BinaryStarsProj>();
            Item.knockBack = 12f;
        }
        /// <summary>
        /// 双子星不再以微光作为前置。
        /// 现在双子星正常10个锭与下位的两个锤子
        /// 火山锤目前是个占位符，后续应该是要变成泰拉物品的
        /// </summary>
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