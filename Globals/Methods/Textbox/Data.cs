using Microsoft.Xna.Framework;

namespace HJScarletRework.Globals.Methods.Textbox
{
    public struct FloatingTooltipDrawSetting(Color backgroundColor, Color textColor, Color textEdgeColor)
    {
        public Color BackgroundColor = backgroundColor;
        public Color TextColor = textColor;
        public Color TextEdgeColor = textEdgeColor;
    }
    public struct TextboxSettings(Color backgroundColor, Color textColor, Color textEdgeColor, string mainText, Color? backgroundEdgeColor = null, bool hasTitle = false, string titleText = null, Color? titleEdgeColor = null, Color? titleTextColor = null, float titleTextSize = 1.0f, float multboxSpacing = 0f)
    {
        public Color BackgroundColor = backgroundColor;
        public Color? BackgroundEdgeColor = backgroundEdgeColor;
        public Color TextColor = textColor;
        public Color TextEdgeColor = textEdgeColor;
        public string MainText = mainText;
        public bool HasTitle = hasTitle;
        public string TitleText = titleText;
        public Color TitleEdgeColor = titleEdgeColor ?? Color.Transparent;
        public Color TitleTextColor = titleTextColor ?? Color.Transparent;
        public float TitleTextSize = titleTextSize;
        public float MultboxSpacing = multboxSpacing;
    }
}
