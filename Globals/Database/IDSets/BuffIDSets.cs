using HJScarletRework.Buffs;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Database.IDSets
{
    [ReinitializeDuringResizeArrays]
    public static class ScarletBuffIDSets
    {
        /// <summary>
        /// 如果为<see langword="true"/>，该效果标记为<see langword="状态类Buff"/>
        /// <br>标记为<see langword="状态类"/>时，会自动不被模组（空想归途）视作debuff</br>
        /// </summary>
        public static bool[] IsStatueBuff = BuffID.Sets.Factory.CreateBoolSet(BuffType<JellyfishGroupBuff>(), BuffType<SaintChurchBuff>());

    }
}
