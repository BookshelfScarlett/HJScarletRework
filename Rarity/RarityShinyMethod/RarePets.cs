using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{
    public static class RarePetsRarity
    {
        public static void DrawItemName(DrawableTooltipLine tooltipLine)
        {
            RarityDrawHelper.DrawCustomTooltipLine(tooltipLine, Color.SkyBlue, Color.Black, Color.DeepSkyBlue, 1.2f);
        }

    }
}
