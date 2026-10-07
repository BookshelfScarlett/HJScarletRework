using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Database.Localization;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;
namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void ReplaceAllTooltip(this List<TooltipLine> tooltips, string replacedTextPath, Color? textColor = null)
        {
            tooltips.RemoveAll((line) => line.Mod == "Terraria" && line.Name != "Tooltip0" && line.Name.StartsWith("Tooltip"));
            TooltipLine getTooltip = tooltips.FirstOrDefault((x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string formateText = replacedTextPath.ToLangValue();
            Color overrideColor = textColor ?? Color.White;
            if (getTooltip is not null)
            {
                getTooltip.Text = formateText;
                getTooltip.OverrideColor = overrideColor;
            }
        }
        /// <summary>
        /// 干翻所有Tooltip，并借助本地化完全重写一次，重载染色，附带键入值
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="replacedTextPath"></param>
        /// <param name="args"></param>
        public static void ReplaceAllTooltip(this List<TooltipLine> tooltips, string replacedTextPath, Color? textColor = null, params object[] args)
        {
            tooltips.RemoveAll((line) => line.Mod == "Terraria" && line.Name != "Tooltip0" && line.Name.StartsWith("Tooltip"));
            TooltipLine getTooltip = tooltips.FirstOrDefault((x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string formateText = replacedTextPath.ToLangValue().ToFormatValue(args);
            Color overrideColor = textColor ?? Color.White;
            if (getTooltip is not null)
            {
                getTooltip.Text = formateText;
                getTooltip.OverrideColor = textColor;
            }
        }
        public static void CreateTooltip(this List<TooltipLine> tooltips, string textPath, Color? color = null, Mod mod = null, string LineName = "HJScarlet", int index = -1)
        {
            string text = textPath.ToLangValue();
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }
        public static void CreateTooltip(this List<TooltipLine> tooltips, string textPath, Color? color = null, Mod mod = null, string LineName = "HJScarlet", int index = -1, params object[] args)
        {
            string text = textPath.ToLangValue().ToFormatValue(args);
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }
        public static void CreateTooltipDirect(this List<TooltipLine> tooltips, string textPath, Color? color = null, Mod mod = null, string LineName = "HJScarlet", int index = -1)
        {
            string text = textPath;
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }

        public static void CreateTooltipDirect(this List<TooltipLine> tooltips, string textValue, Color? color = null, Mod mod = null, string LineName = "HJScarlet", int index = -1, params object[] args)
        {
            string text = textValue.ToFormatValue(args);
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }
        public static void CreateHoldShiftRightClickTooltip(this List<TooltipLine> tooltips)
        {
            tooltips.CreateTooltip(ScarletTextSets.GeneralText_HoldShiftRightClick, ScarletTextSets.GeneralText_BuffShowColor, HJScarletRework.Instance, "HoldShiftAndRightClickTooltipLine");
        }
        public static int FindLineIndex(this List<TooltipLine> tooltips, string lineName, string lineMod = "Terraria") => tooltips.FindIndex(t => t.Name == lineName && t.Mod == lineMod);
        public static int FindLineIndexLast(this List<TooltipLine> tooltips, string lineName, string lineMod = "Terraria") => tooltips.FindLastIndex(t => t.Name.Contains(lineName) && t.Mod == lineMod);
        public static void AddSwapTooltipValueBossCondition(this List<TooltipLine> tooltips, int downedNPCID, Color? color = null)
        {
            string listPath = "Mods.HJScarletRework.Database.DownedConditionList";
            string downedConditionPath = ScarletTextSets.GenericText.SwitchCondition;
            string bossValue = string.Empty;
            if (HJScarletList.DownedBossConditionList.TryGetValue(downedNPCID, out string keyValue))
                bossValue = keyValue;
            Color color1 = color ?? Color.Lerp(Color.LawnGreen, Color.LightGreen, 0.5f);
            string downedValue = (listPath + bossValue).ToLangValue();
            string listValue = downedConditionPath.ToFormatValue(downedValue);
            tooltips.CreateTooltipDirect(listValue, color1, HJScarletRework.Instance, "SwapConditionName");

        }
        public static string ToLangValue(this string textPath) => Language.GetTextValue(textPath);

        public static string ToFormatValue(this string baseTextValue, params object[] args)
        {
            try
            {
                return string.Format(baseTextValue, args);
            }
            catch
            {
                return baseTextValue + "格式化出错";
            }
        }
        public static bool PressingAlt(this KeyboardState keyboardState) => keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
    }
}
