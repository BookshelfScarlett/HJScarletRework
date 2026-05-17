using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class FrostHammer : ExecutorWeaponClass
    {
        public override int ExecutionProj => ProjectileType<FrostHammerExecution>();
        public override int ExecutionTime => 30;
        public override void ExSD()
        {
            Item.damage = 42;
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { MaxInstances = 0, Pitch = -0.5f };
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpItemShoot<FrostHammerProj>(18);
            Item.SetUpItemUseTime(ItemUseStyleID.Swing, 30);
        }
    }
}
