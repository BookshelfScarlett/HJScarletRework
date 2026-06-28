using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class DungeonBreaker : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 0.5f;
        public override int ExecutionProgress => 8;
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.shootSpeed = 19f;
            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[1] with { Pitch = -0.4f, PitchVariance = 0.1f };
            Item.shoot = ProjectileType<DungeonBreakerProj>();
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.SetUpNoUseGraphicItem();
            Item.HJScarlet().ExecutionProj = ProjectileType<DungeonBreakerProj>();
        }
    }
}
