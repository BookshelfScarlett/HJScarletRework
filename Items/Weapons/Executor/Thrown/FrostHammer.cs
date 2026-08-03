using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Thrown
{
    public class FrostHammer : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 30;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSSD()
        {
            Type.ShimmerEach(ItemID.Amarok);
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Frost);
        }
        public override void ExSD()
        {
            Item.damage = 42;
            Item.UseSound = HJScarletSounds.HymnFireball_Release with { MaxInstances = 1, Pitch = -0.20f, PitchVariance = 0.15f, Volume = 0.8f };
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpItemShoot<FrostHammerProj>(18);
            Item.SetUpItemUseTime(ItemUseStyleID.Swing, 30);
        }
    }
}
