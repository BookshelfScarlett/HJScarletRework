using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Core.NetCode
{
    public static class HJNetInput
    {
        public static bool ActionJustPressed => !Main.dedServ && HJScarletKeybinds.GeneralActionKeybind is { } action && action.JustPressed;
        public static bool ActionJustReleased => !Main.dedServ && HJScarletKeybinds.GeneralActionKeybind is { } action && action.JustReleased;
        public static bool ActionCurrent => !Main.dedServ && HJScarletKeybinds.GeneralActionKeybind is { } action && action.Current;
        public static bool SkillJustPressed => !Main.dedServ && HJScarletKeybinds.GeneralSkillKeybind is { } skill && skill.JustPressed;
        public static bool ParryCurrent => !Main.dedServ && HJScarletKeybinds.ParryActionKeybind is { } parry && parry.Current;
        public static bool LocalMouseLeft => Main.netMode != NetmodeID.Server && Main.mouseLeft;
        public static bool LocalMouseRight => Main.netMode != NetmodeID.Server && Main.mouseRight;
        public static Vector2 LocalAimWorld => Main.netMode == NetmodeID.Server ? Vector2.Zero : Main.MouseWorld;
        public static Vector2 AimWorldOf(this Player player)
        {
            if (player == null)
                return Vector2.Zero;
            return player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server ? Main.MouseWorld : player.HJScarlet().SyncedMouseWorld;
        }
        public static bool MouseLeftOf(this Player player)
        {
            if (player == null)
                return false;
            return player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server ? Main.mouseLeft : player.HJScarlet().MouseLeft;
        }
        public static bool MouseRightOf(this Player player)
        {
            if (player == null)
                return false;
            return player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server ? Main.mouseRight : player.HJScarlet().MouseRight;
        }
    }
}
