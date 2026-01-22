global using static Terraria.ModLoader.ModContent;
global using static Microsoft.Xna.Framework.MathHelper;
global using static HJScarletRework.Globals.Handlers.EasingHandler;
global using static HJScarletRework.Globals.Handlers.RandHandler;
using Terraria.ModLoader;
using HJScarletRework.Globals.Methods;

namespace HJScarletRework
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class HJScarletRework : Mod
	{
		public static HJScarletRework Instance;
        public static Mod CrossMod_Calamity;
        public static Mod CrossMod_HomewardJourney;
        public static Mod CrossMod_UCA;
        public override void Load()
        {
            Instance = this;
            ModLoader.TryGetMod(HJScarletMethods.CalamityMod, out CrossMod_Calamity);
            ModLoader.TryGetMod(HJScarletMethods.HomewardJourney, out CrossMod_HomewardJourney);
            ModLoader.TryGetMod(HJScarletMethods.HomewardJourney, out CrossMod_UCA);
        }
        public override void Unload()
        {
            Instance = null;
            CrossMod_Calamity = null;
            CrossMod_HomewardJourney = null;
            CrossMod_Calamity = null;
        }
    }
}
