using ContinentOfJourney;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Globals.Systems
{
    public class ScarletWearingFullCowboy : IItemDropRuleCondition
    {
        private static LocalizedText Description;
        public ScarletWearingFullCowboy()
        {
            Description ??= Language.GetOrRegister("Mods.HJScarletRework.Conditions.Drop.FullCowboy");
        }
        public bool CanDrop(DropAttemptInfo info)
        {
            Player player = info.player;
            return (player.armor[0].type == ItemID.CowboyHat && player.armor[1].type == ItemID.CowboyJacket && player.armor[2].type == ItemID.CowboyPants);
        }
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
    public class FirstTimeKillingTheSon : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => !DownedBossSystem.downedSon;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
    public class PostSupremeCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;
        public PostSupremeCondition()
        {
            Description ??= Language.GetOrRegister("Mods.HJScarletRework.Conditions.Drop.PostSupremeCondition");
        }
        public bool CanDrop(DropAttemptInfo info)
        {
            return DownedBossSystem.downedLifeGod && DownedBossSystem.downedMatterGod && DownedBossSystem.downedTimeGod;
        }
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
}
