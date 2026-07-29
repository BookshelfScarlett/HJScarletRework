using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void PreUpdate()
        {
            if (infiniteFlightTime)
                Player.wingTime = Player.wingTimeMax;
        }
    }
}
