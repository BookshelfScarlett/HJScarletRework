using Terraria.ModLoader;

namespace HJScarletRework.Globals.Keybinds
{
    public partial class HJScarletKeybinds : ModSystem
    {
        /// <summary>
        /// 模组互动键，绝大部分交互内容都会经过这个键位
        /// </summary>
        public static ModKeybind GeneralActionKeybind { get; private set; }
        /// <summary>
        /// 模组技能键，对于部分盔甲/饰品类技能专用
        /// </summary>
        public static ModKeybind GeneralSkillKeybind { get; private set; }
        /// <summary>
        /// 圆盾格挡，核心战斗机制
        /// </summary>
        public static ModKeybind ParryActionKeybind { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            GeneralActionKeybind = KeybindLoader.RegisterKeybind(Mod, "GeneralActionKeybind", "V");
            GeneralSkillKeybind = KeybindLoader.RegisterKeybind(Mod, "GenerialSkillKeybind", "T");
            ParryActionKeybind = KeybindLoader.RegisterKeybind(Mod, "ParryActionKeybind", "F");
        }

        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            GeneralActionKeybind = null;
            GeneralSkillKeybind = null;
            ParryActionKeybind = null;
        }
    }
}
