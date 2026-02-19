using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer :ModPlayer
    {
        public override void PostUpdate()
        {
            UpdateNetPacket();
            SwitchWeaponSystem();
        }

        public override void PostUpdateRunSpeeds()
        {
            if (NoSlowFall > 0)
            {
                Player.slowFall = false;
                Player.maxFallSpeed = 10000;
                Player.GoingDownWithGrapple = true;
            }
        }
    }
}
