using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.IDSets
{
    [ReinitializeDuringResizeArrays]
    public static class ScarletItemIDSets
    {
        /// <summary>
        /// 如果为<see langword="true"/>，该武器标记为<see langword="巨人杀手"/>
        /// <br>标记为<see langword="巨人杀手"/>时，会有提示性的Tooltip文本</br>
        /// </summary>
        public static bool[] GiantKiller = ItemID.Sets.Factory.CreateBoolSet(ItemType<StormSaber>());
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
        public static bool[] ForceToTacticalExecute = ItemID.Sets.Factory.CreateBoolSet();
        /// <summary>
        /// 如果为<see langword="true"/>，该武器的处决模式将会完全不由模组内处决系统进行接管，需要手动处理
        /// <br>同时，他也不受<see cref="Players.HJScarletPlayer.tacticalExecutionManual"/>手动处决的切换影响</br>
        /// <br>只作用于<see langword="代行者伤害"/></br>
        /// </summary>
        public static bool[] ForceToCustomExecute = ItemID.Sets.Factory.CreateBoolSet();

        /// <summary>
        /// 如果为<see langword="true"/>，该武器将会禁用处决条的显示，无论设置开关
        /// <br>只作用于<see cref="Executor"/>代行者伤害</br>
        /// </summary>

        public static bool[] NoGeneralExecutionProgressDraw = ItemID.Sets.Factory.CreateBoolSet();
        /// <summary>
        /// 如果为<see langword="true"/>，该武器将会在击败圣子后获得10倍伤害增幅，用于作为毕业武器
        /// <br>只作用于<see cref="Executor"/>代行者伤害</br>
        /// </summary>

        public static bool[] GrantsBoosterAfterSon = ItemID.Sets.Factory.CreateBoolSet();
        /// <summary>
        /// 如果为<see langword="true"/>，该武器将会与其他武器一样，分享玩家类内同一个Timer作为增强计时器
        /// <br>使用这个的情况下，Timer的减少只会在玩家手持对应武器时发生</br>
        /// <br>只作用于<see cref="Executor"/>代行者伤害</br>
        /// </summary>

        public static bool[] SharedSameBuffTimer = ItemID.Sets.Factory.CreateBoolSet();
    }
}
