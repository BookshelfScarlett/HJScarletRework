using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletShinyRarity : ModRarity
    {
        public abstract void DrawItemName(DrawableTooltipLine line);
        public abstract void DrawFlavorTooltip(DrawableTooltipLine line);
        public static Vector2 GetParticlePosition(DrawableTooltipLine line)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            return Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
        }
    }
}
