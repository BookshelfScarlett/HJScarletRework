using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class AetherfireSmasher : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 40;
        public override float ExecutionStrikeDamageMult => 1f;
        public override void ExSSD()
        {
            HJScarletList.DisasterRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = HJScarletSounds.Blunt_Swing with { MaxInstances = 1, Pitch = -0.4f, PitchVariance = 0.2f, Volume = 0.5f };
            Item.shoot = ProjectileType<AetherfireSmasherProj>();
            Item.knockBack = 6f;
            Item.DamageType = ExecutorDamageClass.Instance;
            Item.width = Item.height = 66;
            Item.damage = 71;
            //这里的ut有意为之
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.shootSpeed = 18f;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
        }
        //实际合成材料可随意，我个人推荐为花后
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
