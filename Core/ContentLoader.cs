using HJScarletRework.Globals.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace HJScarletRework.Core
{
    public static class ScarletContent
    {
        public static int DashType<T>() where T : PlayerDashClass=> GetInstance<T>()?.Type ?? 0;
    }
}
