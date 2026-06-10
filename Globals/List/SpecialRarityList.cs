using HJScarletRework.Rarity.RarityShiny;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.List
{
    public partial class HJScarletList : ModSystem
    {
        public static HashSet<int> FrostRarityHashSet = new HashSet<int>();
        public static HashSet<int> DisasterRarityHashSet = new HashSet<int>();
        public static HashSet<int> NebulaRarityHashSet = new HashSet<int>();
        public static HashSet<int> NightRarityHashSet = new HashSet<int>();
        public static HashSet<int> HallowedRarityHashSet = new HashSet<int>();
        public static HashSet<int> ScarletRarityHashSet = new HashSet<int>();
        public static HashSet<int> SunlightRarityHashSet = new HashSet<int>();
        public static Dictionary<int, Action<DrawableTooltipLine>> MiscRarityDrawDictionary = new();
        public static Dictionary<int, Action<DrawableTooltipLine>> ConvertedItemRarityDrawDictionary = new();
        public static Dictionary<int, RareItemRarity.RareType> RareItemRarityDrawDictionary = [];
        public void LoadRarity()
        {
        }
        public void UnloadRarity()
        {
            FrostRarityHashSet.Clear();
            DisasterRarityHashSet.Clear();
            NebulaRarityHashSet.Clear();
            NightRarityHashSet.Clear();
            HallowedRarityHashSet.Clear();
            ScarletRarityHashSet.Clear();
            SunlightRarityHashSet.Clear();
            FrostRarityHashSet = null;
            DisasterRarityHashSet = null;
            NebulaRarityHashSet = null;
            NightRarityHashSet = null;
            HallowedRarityHashSet = null;
            ScarletRarityHashSet = null;
            SunlightRarityHashSet = null;

        }
    }
}
