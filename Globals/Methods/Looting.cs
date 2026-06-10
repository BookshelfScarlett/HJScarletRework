using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void AddLoot(this ItemLoot itemLoot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1) => AddLootSimple(itemLoot, itemID, dropRateInt, minQuantity, maxQuantity);
        public static void AddLoot<T>(this ItemLoot itemLoot, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1) where T : ModItem => AddLootSimple(itemLoot, ItemType<T>(), dropRateInt, minQuantity, maxQuantity);
        public static IItemDropRule AddLootSimple(this ILoot loot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity));
        }
        public static void AddCommon(this ItemLoot item, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            item.Add(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity));
        }
        public static void ApplyNoBossBagLoot(ref NPCLoot loot, int itemID, int dropRate, int min = 1, int max = 1)
        {
            LeadingConditionRule norExepert = new LeadingConditionRule(new Conditions.NotExpert());
            norExepert.OnSuccess(ItemDropRule.Common(itemID, dropRate, min, max));
            loot.Add(norExepert);
        }
        public static void ApplyMasterLoot(ref NPCLoot loot, int itemID, int dropRate, int min = 1, int max = 1)
        {
            LeadingConditionRule norExepert = new LeadingConditionRule(new Conditions.IsMasterMode());
            norExepert.OnSuccess(ItemDropRule.Common(itemID, dropRate, min, max));
            loot.Add(norExepert);
        }
        public static void ApplyMasterLoot(ref ItemLoot loot, int itemID, int dropRate, int min = 1, int max = 1)
        {
            LeadingConditionRule norExepert = new LeadingConditionRule(new Conditions.IsMasterMode());
            norExepert.OnSuccess(ItemDropRule.Common(itemID, dropRate, min, max));
            loot.Add(norExepert);
        }
    }

}
