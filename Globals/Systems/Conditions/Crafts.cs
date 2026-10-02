using ContinentOfJourney;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Globals.Systems.Conditions
{
    public static class HJScarletCraftingConditions
    {
        public static string ConditionString => "Mods.HJScarletRework.Conditions.Crafting";
        public static Condition FirstTimeGaiaStriker = new Condition("Mods.HJScarletRework.Weapons.Executor.GaiaStriker.Conditions.FirstTime",
            () => (!Main.LocalPlayer.HJScarlet().firstTimeCraftGaia && !Main.dedServ));
        public static Condition AnyAfterCrafting = new Condition("Mods.HJScarletRework.Weapons.Executor.GaiaStriker.Conditions.SecondTime",
            () => (Main.LocalPlayer.HJScarlet().firstTimeCraftGaia || Main.dedServ));
        public static Condition IsDownSlimeGodAndInEclipse = new Condition($"{ConditionString}.{nameof(IsDownSlimeGodAndInEclipse)}",
            () => DownedBossSystem.downedSunGod && Main.eclipse);

        public static Condition HasFuckingCalamityMod = new Condition($"{ConditionString}.{nameof(HasFuckingCalamityMod)}", () => HJScarletRework.CrossMod_Calamity is not null);
        public static Condition NoFuckingCalamityMod = new Condition($"{ConditionString}.{nameof(NoFuckingCalamityMod)}", () => HJScarletRework.CrossMod_Calamity is null);
        public static Condition InMultiplayer = new Condition($"{ConditionString}.{nameof(InMultiplayer)}", () => Main.dedServ);
        public static Condition InSingleplayer = new Condition($"{ConditionString}.{nameof(InSingleplayer)}", () => !Main.dedServ);
    }

}
