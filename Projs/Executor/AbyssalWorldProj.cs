using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class AbyssalWorldProj : ExecutorHeldProj
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int OriginalItemID => ItemType<AbyssalWorld>();
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
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
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
