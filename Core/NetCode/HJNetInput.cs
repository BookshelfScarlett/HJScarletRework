using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Core.NetCode
{
    // 读输入的统一入口，解决两个联机问题：
    // 1）专用服务器上 ModKeybind 从未初始化，直接读 .Current/.JustPressed 会抛 KeyNotFoundException，
    //    所以这里全部先挡 Main.dedServ，并用 is { } 防止键位实例为 null；
    // 2）想看“另一个玩家”的鼠标/准星不能读 Main.MouseWorld（那是本机自己的视角），
    //    必须用下面三个 Of 方法，它们取的是 NetPacket 同步过来的值。
    // 注意：Local* 系列只代表本机输入，在服务器上永远为 false / Vector2.Zero。
    public static class HJNetInput
    {
        // 通用动作键（处决/收刀之类）的三种状态。
        public static bool ActionJustPressed => !Main.dedServ && HJScarletKeybinds.GeneralActionKeybind is { } action && action.JustPressed;
        public static bool ActionJustReleased => !Main.dedServ && HJScarletKeybinds.GeneralActionKeybind is { } action && action.JustReleased;
        public static bool ActionCurrent => !Main.dedServ && HJScarletKeybinds.GeneralActionKeybind is { } action && action.Current;
        public static bool SkillJustPressed => !Main.dedServ && HJScarletKeybinds.GeneralSkillKeybind is { } skill && skill.JustPressed;
        public static bool ParryCurrent => !Main.dedServ && HJScarletKeybinds.ParryActionKeybind is { } parry && parry.Current;
        // 本机鼠标左/右键。武器逻辑里要“谁按了鼠标”请用它而不是 Main.mouseLeft，
        // 后者在服务器上仍然会是默认值，容易把别人的射弹往错误方向生成。
        public static bool LocalMouseLeft => Main.netMode != NetmodeID.Server && Main.mouseLeft;
        public static bool LocalMouseRight => Main.netMode != NetmodeID.Server && Main.mouseRight;
        // 本机准星的世界坐标（只在主人端有意义）。
        public static Vector2 LocalAimWorld => Main.netMode == NetmodeID.Server ? Vector2.Zero : Main.MouseWorld;
        // 取“这个玩家想打哪里”：自己就用本机准星，别人就用同步值。
        // 队友端挂载射弹里的后坐线、拖尾朝向这类应该用它，而不是 Main.MouseWorld。
        // 服务器端读到的是发包者传过来的最新值（处理器在服务器分支也会落地一次）。
        public static Vector2 AimWorldOf(this Player player)
        {
            if (player == null)
                return Vector2.Zero;
            return player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server ? Main.MouseWorld : player.HJScarlet().SyncedMouseWorld;
        }
        // 取“这个玩家左键是否按着”，同样是自己读本机、别人读同步值。
        public static bool MouseLeftOf(this Player player)
        {
            if (player == null)
                return false;
            return player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server ? Main.mouseLeft : player.HJScarlet().MouseLeft;
        }
        // 取“这个玩家右键是否按着”。
        public static bool MouseRightOf(this Player player)
        {
            if (player == null)
                return false;
            return player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server ? Main.mouseRight : player.HJScarlet().MouseRight;
        }
    }
}
