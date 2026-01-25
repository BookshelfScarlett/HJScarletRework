using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace HJScarletRework.Rarity
{
    public class DisasterRarity : ModRarity
    {
        public static Color RareColor = Color.Crimson;
        public override Color RarityColor => RareColor;
        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            string textValue = tooltipLine.Text;
            Vector2 textSize = tooltipLine.Font.MeasureString(textValue);
            Vector2 textCenter = textSize *0.5f;
            // The position to draw the text.
            Vector2 textPosition = new(tooltipLine.X, tooltipLine.Y);
            // Get the position to draw the glow behind the text.
            Vector2 glowPosition = new(tooltipLine.X + textCenter.X, tooltipLine.Y + textCenter.Y / 1.5f);
            // Get the scale of the glow texture based off of the text size.
            Vector2 glowScale = new Vector2(textSize.X * 0.135f, 0.6f) * 1.2f;
            // Draw the glow texture.
            Main.spriteBatch.Draw(HJScarletTexture.Texture_RarityGlow.Value, glowPosition, null, RareColor.ToAddColor() * 0.85f, 0f, HJScarletTexture.Texture_RarityGlow.Origin, glowScale, SpriteEffects.None, 0f);

            // Get an offset to the afterimageOffset based on a sine wave.
            float sine = (float)((1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f)) / 2);
            float sineOffset = Lerp(0.5f, 1f, sine);

            // Draw text backglow effects.
            for (int i = 0; i < 12; i++)
            {
                Vector2 afterimageOffset = (TwoPi * i / 12f).ToRotationVector2() * (2f * sineOffset);
                // Draw the text. Rotate the position based on i.
                ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, textValue, (textPosition + afterimageOffset).RotatedBy(TwoPi * (i / 12)), Color.Black * 0.9f, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
            }

            // Draw the main inner text.
            Color mainTextColor = Color.Lerp(Color.Crimson, Color.Red, 0.9f);
            ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, textValue, textPosition, mainTextColor, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
        }
    }
}
