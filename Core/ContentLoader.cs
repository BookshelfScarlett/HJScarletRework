using HJScarletRework.Globals.Systems;

namespace HJScarletRework.Core
{
    public static class ScarletContent
    {
        public static int DashType<T>() where T : PlayerDashClass => GetInstance<T>()?.Type ?? 0;
    }
}
