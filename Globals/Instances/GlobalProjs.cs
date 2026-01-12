using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalProjs : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int GlobalTargetIndex = -1;
        public bool FirstFrame = false;
        public bool FocusStrike = false;
        public bool UseFocusStrikeMechanic = false;
        public bool IsHitOnEnablFocusMechanicProj = false;
        public float[] ExtraAI = new float[10];
        public override void AI(Projectile projectile)
        {
            if (!FirstFrame)
                FirstFrame = true;
        }
    }
}
