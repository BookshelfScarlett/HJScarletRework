using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public class RarePets : ModRarity
    {
        public override Color RarityColor => Color.SkyBlue;
        internal static List<RaritySparkle> RarePetsParticle = [];
        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            RarityDrawHelper.DrawCustomTooltipLine(tooltipLine, Color.SkyBlue, Color.Black, Color.DeepSkyBlue, 1.2f);
        }
    }
}
