using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;

namespace HJScarletRework.Projs.Executor
{
    internal class FleshtumorHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Fleshtumor>();
        public override string Texture => GetInstance<Fleshtumor>().Texture;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
