using HJScarletRework.Globals.Enums;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.List
{
    public partial class HJScarletList : ModSystem
    {
        /// <summary>
        /// 主要用于存储并引用特殊稀有度绘制
        /// </summary>
        public static Dictionary<int, ShinyRarityType> ShinyRarityItemDictionary = [];
        public void LoadRarity()
        {
        }
        public void UnloadRarity()
        {

        }
    }
}
