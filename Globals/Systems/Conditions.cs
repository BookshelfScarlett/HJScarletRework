using ContinentOfJourney;
using HJScarletRework.Globals.Methods;
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
    public static class HJScarletCraftingConditions
    {
        public static string ConditionString => "Mods.HJScarletRework.Conditions.Crafting";
        public static Condition FirstTimeGaiaStriker = new Condition("Mods.HJScarletRework.Weapons.Executor.GaiaStriker.Conditions.FirstTime",
            () => (!Main.LocalPlayer.HJScarlet().firstTimeCraftGaia && !Main.dedServ));
        public static Condition AnyAfterCrafting = new Condition("Mods.HJScarletRework.Weapons.Executor.GaiaStriker.Conditions.SecondTime",
            () => (Main.LocalPlayer.HJScarlet().firstTimeCraftGaia || Main.dedServ));
        public static Condition IsDownSlimeGodAndInEclipse = new Condition($"{ConditionString}.{nameof(IsDownSlimeGodAndInEclipse)}",
            () => DownedBossSystem.downedSunGod && Main.eclipse);
    }
}
