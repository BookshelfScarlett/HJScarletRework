using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<Sundowner>().Texture;
        public override ClassCategory Category => ClassCategory.Ranged;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.SetupImmnuity(-1);
            Projectile.width = Projectile.height = 40;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.Center = Owner.Center;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
