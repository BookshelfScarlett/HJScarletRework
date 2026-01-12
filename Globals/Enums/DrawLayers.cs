using System;

namespace HJScarletRework.Globals.Enums
{
    [Flags]
    public enum HJScarletDrawLayer
    {
        BeforeTiles,
        BeforeNPCs,
        BeforeProjectiles,
        BeforePlayer,
        BeforeDusts,
        AfterDusts,
    }
}
