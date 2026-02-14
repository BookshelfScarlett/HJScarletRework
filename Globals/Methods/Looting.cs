using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void AddLoot(this ItemLoot itemLoot, int itemID, int dropRateInt = 1, int minQuantity =1 , int maxQuantity = 1) => AddLootSimple(itemLoot, itemID, dropRateInt, minQuantity, maxQuantity);
        public static void AddLoot<T>(this ItemLoot itemLoot, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1) where T : ModItem => AddLootSimple(itemLoot, ItemType<T>(), dropRateInt, minQuantity, maxQuantity);
        public static IItemDropRule AddLootSimple(this ILoot loot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity));
        }
    }
}
