using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

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
