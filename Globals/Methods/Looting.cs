using Terraria;
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
        public static void ApplyForTheWorthyMasterLoot(ref NPCLoot loot, int itemID, int FTWDropRate, int noneFTWDropRate, int min = 1, int max = 1)
        {
            LeadingConditionRule norExepert = new LeadingConditionRule(new Conditions.ForTheWorthyIsUp());
            norExepert.OnFailedConditions(ItemDropRule.Common(itemID, noneFTWDropRate, min, max));
            norExepert.OnSuccess(ItemDropRule.Common(itemID, FTWDropRate, min, max));
            loot.Add(norExepert);
        }

        public static void ApplyMasterLoot(ref ItemLoot loot, int itemID, int dropRate, int min = 1, int max = 1)
        {
            LeadingConditionRule norExepert = new LeadingConditionRule(new Conditions.IsMasterMode());
            norExepert.OnSuccess(ItemDropRule.Common(itemID, dropRate, min, max));
            loot.Add(norExepert);
        }
        public static NPCShop ToCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem
        {
            return ToCustomValue(shop, ItemType<T>(), customValue, conditions);
        }
        public static NPCShop ToCustomValue<T>(this NPCShop shop, int plat = 0, int gold = 0, int silver = 10, int copper = 0, params Condition[] conditions) where T : ModItem
        {
            return ToCustomValue(shop, ItemType<T>(), plat, gold, silver, copper, conditions);
        }
        public static NPCShop ToCustomValue(this NPCShop shop, int itemType, int plat = 0, int gold = 0, int silver = 10, int copper = 0, params Condition[] conditions)
        {
            return ToCustomValue(shop, itemType, Item.buyPrice(plat, gold, silver, copper), conditions);
        }
        public static NPCShop ToCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions)
        {
            var item = new Item(itemType)
            {
                shopCustomPrice = customValue
            };
            return shop.Add(item, conditions);
        }

    }

}
