using Terraria.Localization;

namespace HJScarletRework.Globals.Database.Localization
{
    public static partial class ScarletTextSets
    {
        public static string DatabasePrefix = "Mods.HJScarletRework.Database";
        public static string GeneralText_BuffShow => $"{DatabasePrefix}.GenericText.BuffDetail";
        public static Color GeneralText_BuffShowColor => Color.LightGray;
        public static string GeneralText_HoldShiftRightClick => $"{DatabasePrefix}.GenericText.HoldShiftAndRightClick";
        public class CustomDeath
        {
            public static string Prefix => "Database.CustomDeathReason";
            public static string SuicidePath => $"{Prefix}.Suicide";
        }
        public class GenericText
        {
            public static string Prefix => $"{DatabasePrefix}.GenericText.";
            public static string SwitchTooltip => Language.GetTextValue(Prefix + "SwitchWeapon.Tooltip");
            public static string SwitchVisual => Language.GetTextValue(Prefix + "SwitchWeapon.Visual");
            public static string SwitchAllFix => Language.GetTextValue(Prefix + "SwitchWeapon.AllFix");
            public static string SwitchCondition => Language.GetTextValue(Prefix + "SwitchWeapon.Condition");
            public static string ModNamePrefix => Language.GetTextValue(Prefix + "LostbeltJourneyText");
            public static string ApplyDoT => Language.GetTextValue(Prefix + "ApplyDoT");
            public static string CombineModName(string actualText) => ModNamePrefix + actualText;
        }
    }
}
