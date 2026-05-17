using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;

namespace HJScarletRework.Projs.Executor
{
    public class SpectreStaffHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<SpectreStaff>();
        public override string Texture => HJScarletItemProj.SpectreStaff.Path;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
