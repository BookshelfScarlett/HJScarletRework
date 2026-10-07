using HJScarletRework.Core.NetCode;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    // 输入同步的发送端：把本机准星与鼠标左右键发给服务器，再由 Core/NetCode 里的处理器落到所有端。
    // 别人端的这些值靠 HJNetInput.AimWorldOf() / MouseLeftOf() / MouseRightOf() 读，不要直接读 Main.MouseWorld。
    public partial class HJScarletPlayer : ModPlayer
    {
        // 上一帧的值，用来做到“只在变化时发包”，不要每帧广播。
        internal Vector2 oldSyncedMouseWorld;
        // 本机的准星世界坐标（别人端的那一份由 ReadSyncMouseWorld 写进来）。
        public Vector2 SyncedMouseWorld;

        internal bool OldMouseLeft;
        public bool MouseLeft;

        internal bool OldMouseRight;
        public bool MouseRight;

        // 武器技能键的同步标记，目前只有 ReadWeaponSkill 会写它，发送端还没人调。
        public bool JustPressedWeaponSKill;
        // 在 PostUpdate 里被调，那里对所有玩家都会跑，所以先挡掉不是本机的那些。
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
