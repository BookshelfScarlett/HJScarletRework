using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Thrown
{
    public class AetherfireSmasher : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 40;
        public override float ExecutionStrikeDamageMult => 1f;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Solar);
        }
        public override void ExSD()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = HJScarletSounds.Blunt_Swing with { MaxInstances = 1, Pitch = -0.4f, PitchVariance = 0.2f, Volume = 0.5f };
            Item.shoot = ProjectileType<AetherfireSmasherProj>();
            Item.knockBack = 6f;
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.damage = 71;
            Item.useTime = Item.useAnimation = 12;
            Item.shootSpeed = 18f;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.SetUpNoUseGraphicItem(false);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PaladinsHammer).
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
