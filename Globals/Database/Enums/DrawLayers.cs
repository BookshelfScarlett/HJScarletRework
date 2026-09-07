using System;

namespace HJScarletRework.Globals.Database.Enums
{
    [Flags]
    public enum ScarletDrawLayer
    {
        BeforeTiles,
        BeforeNPCs,
        BeforeProjectiles,
        BeforePlayer,
        BeforeDusts,
        AfterDusts,
        AfterProjectiles,
        EndCapture
    }
}
