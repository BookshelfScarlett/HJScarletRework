using Microsoft.Xna.Framework;
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
        public static void CreateTooltip(this List<TooltipLine> tooltips, string textPath, Color? color = null, Mod mod = null, string LineName = "HJScarlet")
        {
            string text = textPath.ToLangValue();
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        public static void CreateTooltip(this List<TooltipLine> tooltips, string textPath, Color? color = null, Mod mod = null, string LineName = "HJScarlet", params object[] args)
        {
            string text = textPath.ToLangValue().ToFormatValue(args);
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需直接传入需要的文本内容而不是对应的本地化路径，重载颜色代码
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textValue">文本内容</param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltipDirect(this List<TooltipLine> tooltips, string textValue, Color? color = null, Mod mod = null, string LineName = "HJScarlet")
        {
            string text = textValue.ToLangValue();
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需直接传入需要的文本内容而不是对应的本地化路径，需直接传入需要的文本内容而不是对应的本地化路径，重载传参方法，颜色代码
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textValue">文本内容</param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltipDirect(this List<TooltipLine> tooltips, string textValue, Color? color = null, Mod mod = null, string LineName = "HJScarlet", params object[] args)
        {
            string text = textValue.ToFormatValue(args);
            Mod tooltipMod = mod ?? HJScarletRework.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = overrideColor
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
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
    }
}
