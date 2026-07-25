using ContinentOfJourney.NPCs.Boss_SlimeGod;
using ContinentOfJourney.NPCs.Boss_TheLifebringer;
using ContinentOfJourney.NPCs.Boss_TheMaterealizer;
using ContinentOfJourney.NPCs.Boss_TheSon;
using ContinentOfJourney.NPCs.Boss_WorldsEndEverlastingFallingWhale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.IDSets
{
    [ReinitializeDuringResizeArrays]
    public static partial class ScarletNPCIDSets
    {
        /// <summary>
        /// 如果为<see langword="true"/>，则该NPC会被视为对泰拉瑞亚的威胁。主要用于被视作外神类的单位。
        /// <br>默认集合里包括了月球入侵事件的所有敌人，与月球领主本身</br>
        /// </summary>
        public static bool[] ThreatToTerraria = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.MoonLordCore, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye, NPCID.MoonLordLeechBlob,
            NPCID.LunarTowerNebula, NPCID.LunarTowerSolar, NPCID.LunarTowerStardust, NPCID.LunarTowerVortex,
            NPCID.NebulaBeast, NPCID.NebulaBrain, NPCID.NebulaHeadcrab, NPCID.NebulaSoldier,
            NPCID.VortexHornet, NPCID.VortexHornetQueen, NPCID.VortexLarva, NPCID.VortexRifleman, NPCID.VortexSoldier,
            NPCID.SolarCorite, NPCID.SolarCrawltipedeBody, NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeTail, NPCID.SolarDrakomire, NPCID.SolarSroller, NPCID.SolarDrakomireRider, NPCID.SolarFlare, NPCID.SolarGoop, NPCID.SolarSolenian, NPCID.SolarSpearman,
            NPCID.StardustCellBig,NPCID.StardustCellSmall,NPCID.StardustJellyfishSmall,NPCID.StardustJellyfishBig,NPCID.StardustSoldier,NPCID.StardustSpiderBig,NPCID.StardustSpiderSmall,NPCID.StardustWormBody,NPCID.StardustWormHead,NPCID.StardustWormTail);
        /// <summary>
        /// 如果为<see langword="true"/>，则该NPC会被视为具备神性。主要用于神明类的单位。
        /// <br>默认集合里，包括了旅人归途中所有的至尊与其下的门徒们</br>
        /// </summary>
        public static bool[] DivineNPC = NPCID.Sets.Factory.CreateBoolSet(
            NPCType<TheMaterealizer>(), NPCType<TheMaterealizer_Minion>(),
            NPCType<TheLifebringerHead>(),NPCType<TheLifebringer_Minion>(),
            NPCType<SlimeGod>(),
            NPCType<WorldsEndEverlastingFallingWhale>(),
            NPCType<TheSon>());
        /// <summary>
        /// 如果为<see langword="true"/>，则该NPC会被视为巨人。主要用于极其巨大的单位。
        /// <br>默认集合里，包括了滴答钟塔、全景监狱、世界之树与他们的FTW世界变体、和永落鲸本身</br>
        /// </summary>
        public static bool[] Giant = NPCID.Sets.Factory.CreateBoolSet(NPCType<WorldsEndEverlastingFallingWhale>());

    }
}
