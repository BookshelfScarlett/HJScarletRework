using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.IDSets
{
    [ReinitializeDuringResizeArrays]
    public static class ScarletItemIDSets
    {
        /// <summary>
        /// 如果为<see langword="true"/>，该武器将会强制执行自动处决
        /// <br>只作用于<see cref="Executor"/>代行者伤害</br>
        /// <br>默认为所有<see cref="Executor.ExecutorWeaponType.Assistance"/>辅助道具类武器，在<see cref="Executor.ExecutorWeaponClass.SetStaticDefaults"/>内自动添加</br>
        /// </summary>
        public static bool[] ForceToAutomaticExecute = ItemID.Sets.Factory.CreateBoolSet();
        /// <summary>
        /// 如果为<see langword="true"/>，该武器将会强制执行手动处决
        /// <br>只作用于<see cref="Executor"/>代行者伤害</br>
        /// <br>默认为所有<see cref="Executor.ExecutorWeaponType.Caster"/>魔术载体武器，在<see cref="Executor.ExecutorWeaponClass.SetStaticDefaults"/>内自动添加</br>
        /// </summary>
        public static bool[] ForceToTacticalExecute= ItemID.Sets.Factory.CreateBoolSet();
        /// <summary>
        /// 如果为<see langword="true"/>，该武器将会禁用处决条的显示，无论设置开关
        /// <br>只作用于<see cref="Executor"/>代行者伤害</br>
        /// </summary>

        public static bool[] NoGeneralExecutionProgressDraw = ItemID.Sets.Factory.CreateBoolSet();
    }
}
