using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static string CalamityMod => "CalamityMod";
        public static string HomewardJourney => "ContinentOfJourney";
        public static bool HasFuckingCalamity
        {
            get
            {
                return ModLoader.HasMod(CalamityMod);
            }
        }
    }
}
