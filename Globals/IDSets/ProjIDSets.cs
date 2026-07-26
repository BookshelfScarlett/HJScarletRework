using ContinentOfJourney.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.IDSets
{
    [ReinitializeDuringResizeArrays]
    public static class ScarletProjIDSets
    {

        /// <summary>
        /// 如果为<see langword="true"/>，则该射弹会被视为具备神性。主要用于神明类的单位。
        /// <br>由于泰拉瑞亚的特殊性质，如果需要在攻击上做操作，也需要管理对应的射弹</br>
        /// <br>默认集合里，包括了旅人归途中所有的至尊与其下的门徒们的射弹</br>
        /// </summary>

        public static bool[] DivingProjectile = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileType<Materealizer_1>(), ProjectileType<Materealizer_2>(), ProjectileType<Materealizer_3>(),
        #region 化物天匠大师射弹
            ProjectileType<Materealizer_Master_1>(), ProjectileType<Materealizer_Master_2>(), ProjectileType<Materealizer_Master_3>(),
            ProjectileType<Materealizer_Master_4>(), ProjectileType<Materealizer_Master_5>(), ProjectileType<Materealizer_Master_6>(),
            ProjectileType<Materealizer_Master_7>(), ProjectileType<Materealizer_Master_8>(), ProjectileType<Materealizer_Master_9>(),
            ProjectileType<Materealizer_Master_10>(), ProjectileType<Materealizer_Master_11>(),
            ProjectileType<Materealizer_Master_12>(), ProjectileType<Materealizer_Master_13>(),
            ProjectileType<Materealizer_Master_14>(), ProjectileType<Materealizer_Master_15>(),
            ProjectileType<Materealizer_Master_16>(), ProjectileType<Materealizer_Master_17>(),
            ProjectileType<Materealizer_Master_18>(), ProjectileType<Materealizer_Master_19>(),
            ProjectileType<Materealizer_Master_20>(), ProjectileType<Materealizer_Master_21>(),
        #endregion
            ProjectileType<Lifebringer_1>(), ProjectileType<Lifebringer_2>(), ProjectileType<Lifebringer_3>(),
            ProjectileType<Lifebringer_4>(), ProjectileType<Lifebringer_5>(), ProjectileType<Lifebringer_6>(),
        #region 万籁携生大师射弹
            ProjectileType<Lifebringer_Master_1>(), ProjectileType<Lifebringer_Master_2>(), ProjectileType<Lifebringer_Master_4>(),
            ProjectileType<Lifebringer_Master_5>(), ProjectileType<Lifebringer_Master_6>(), ProjectileType<Lifebringer_Master_7>(),
            ProjectileType<Lifebringer_Master_8>(), ProjectileType<Lifebringer_Master_9>(),/*ProjectileType<Lifebringer_Master_3>(),*/
            ProjectileType<Lifebringer_Master_10>(), ProjectileType<Lifebringer_Master_11>(),
            ProjectileType<Lifebringer_Master_12>(), ProjectileType<Lifebringer_Master_13>(),
            ProjectileType<Lifebringer_Master_14>(), ProjectileType<Lifebringer_Master_15>(),
            ProjectileType<Lifebringer_Master_16>(), ProjectileType<Lifebringer_Master_17>(),
            ProjectileType<Lifebringer_Master_18>(), ProjectileType<Lifebringer_Master_19>(),
            ProjectileType<Lifebringer_Master_20>(), ProjectileType<Lifebringer_Master_21>(),
            ProjectileType<Lifebringer_Master_22>(), ProjectileType<Lifebringer_Master_23>(),
            ProjectileType<Lifebringer_Master_24>(), ProjectileType<Lifebringer_Master_25>(),
            ProjectileType<Lifebringer_Master_26>(), ProjectileType<Lifebringer_Master_27>(),
        #endregion
            ProjectileType<Overwatcher_1>(), ProjectileType<Overwatcher_2>(), ProjectileType<Overwatcher_3>(),
            ProjectileType<Overwatcher_4>(), ProjectileType<Overwatcher_5>(), ProjectileType<Overwatcher_6>(),
            ProjectileType<Overwatcher_7>(), ProjectileType<Overwatcher_AlternativeUniverse>(),
        #region 极视钟表大师射弹
            ProjectileType<Overwatcher_Master_1>(), ProjectileType<Overwatcher_Master_2>(), ProjectileType<Overwatcher_Master_3>(),
            ProjectileType<Overwatcher_Master_4>(), ProjectileType<Overwatcher_Master_5>(), ProjectileType<Overwatcher_Master_6>(),
            ProjectileType<Overwatcher_Master_7>(), ProjectileType<Overwatcher_Master_8>(), ProjectileType<Overwatcher_Master_9>(),
            ProjectileType<Overwatcher_Master_10>(), ProjectileType<Overwatcher_Master_11>(),
            ProjectileType<Overwatcher_Master_12>(), ProjectileType<Overwatcher_Master_13>(),
            ProjectileType<Overwatcher_Master_14>(), ProjectileType<Overwatcher_Master_15>(),
            ProjectileType<Overwatcher_Master_16>(), ProjectileType<Overwatcher_Master_17>(),
            ProjectileType<Overwatcher_Master_18>(), ProjectileType<Overwatcher_Master_19>(),
            ProjectileType<Overwatcher_Master_20>(), ProjectileType<Overwatcher_Master_21>(),
            ProjectileType<Overwatcher_Master_22>(), ProjectileType<Overwatcher_Master_23>(),
            ProjectileType<Overwatcher_Master_24>(), ProjectileType<Overwatcher_Master_25>(),
            ProjectileType<Overwatcher_Master_26>(), ProjectileType<Overwatcher_Master_27>(),
            ProjectileType<Overwatcher_Master_28>(), ProjectileType<Overwatcher_Master_29>(),
            ProjectileType<Overwatcher_Master_30>(), ProjectileType<Overwatcher_Master_31>(),
            ProjectileType<Overwatcher_Master_32>(), ProjectileType<Overwatcher_Master_33>(),
        #endregion
            ProjectileType<SlimeGod_Expert_1>(), ProjectileType<SlimeGod_Expert_2>(),
            ProjectileType<SlimeGod_Expert_3>(), ProjectileType<SlimeGod_Expert_4>(),
ProjectileType<SlimeGodLightOrb>(), ProjectileType<SlimeGodLightOrb_2>(), ProjectileType<SlimeGodLightOrb_3>(),
        #region 史莱姆之神大师射弹 
            ProjectileType<SlimeGod_Master_1>(), ProjectileType<SlimeGod_Master_2>(), ProjectileType<SlimeGod_Master_3>(),
            ProjectileType<SlimeGod_Master_4>(), ProjectileType<SlimeGod_Master_5>(), ProjectileType<SlimeGod_Master_6>(),
            ProjectileType<SlimeGod_Master_7>(), ProjectileType<SlimeGod_Master_8>(), ProjectileType<SlimeGod_Master_9>(),
            ProjectileType<SlimeGod_Master_10>(), ProjectileType<SlimeGod_Master_11>(),
            ProjectileType<SlimeGod_Master_12>(), ProjectileType<SlimeGod_Master_13>(),
            ProjectileType<SlimeGod_Master_14>(), ProjectileType<SlimeGod_Master_15>(),
            ProjectileType<SlimeGod_Master_16>(), ProjectileType<SlimeGod_Master_17>(),
            ProjectileType<SlimeGod_Master_18>(), ProjectileType<SlimeGod_Master_19>(),
            ProjectileType<SlimeGod_Master_20>(), ProjectileType<SlimeGod_Master_21>(),
            ProjectileType<SlimeGod_Master_22>(), ProjectileType<SlimeGod_Master_23>(),
            ProjectileType<SlimeGod_Master_24>(), ProjectileType<SlimeGod_Master_25>(),
            ProjectileType<SlimeGod_Master_26>(), ProjectileType<SlimeGod_Master_27>(),
            ProjectileType<SlimeGod_Master_28>(), ProjectileType<SlimeGod_Master_29>(),
            ProjectileType<SlimeGod_Master_30>(), ProjectileType<SlimeGod_Master_31>()
        #endregion
            );
    }
}