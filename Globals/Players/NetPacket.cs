using HJScarletRework.Core.NetCode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        internal Vector2 oldSyncedMouseWorld;
        public Vector2 SyncedMouseWorld;

        internal bool OldMouseLeft;
        public bool MouseLeft;

        internal bool OldMouseRight;
        public bool MouseRight;

        public bool JustPressedWeaponSKill;
        public void UpdateNetPacket()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;
            SyncedMouseWorld = Main.MouseWorld;
            MouseLeft = Main.mouseLeft;
            MouseRight = Main.mouseRight;
            if (SyncedMouseWorld != oldSyncedMouseWorld)
                Player.SyncedMouseWorld(Main.MouseWorld);
            if (MouseLeft != OldMouseLeft)
                Player.SyncedMouseLeft(Main.mouseLeft);
            if (MouseRight != OldMouseRight)
                Player.SyncedMouseRight(Main.mouseRight);
            OldMouseLeft = Main.mouseLeft;
            OldMouseRight = Main.mouseRight;
            oldSyncedMouseWorld = Main.MouseWorld;
        }

    }
}
