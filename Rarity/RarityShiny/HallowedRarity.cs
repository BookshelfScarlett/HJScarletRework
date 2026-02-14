using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public class HallowedRarity : ModRarity
    {
        public static Color RareColor = Color.Gold;
        public static List<RaritySparkle> RaritySparkles = [];
        public override Color RarityColor => RareColor;
        public static void DrawRarity(DrawableTooltipLine drawableTooltipLine)
        {
            PostDrawRarity(ref RaritySparkles, drawableTooltipLine);
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.Yellow, Color.Lerp(Color.DarkGoldenrod, Color.Yellow, 0.1f), Color.White, 0.45f);
        }
        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(8))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityCrossStar crossStar = new RarityCrossStar(position, velocity, RandLerpColor(Color.Gold, Color.Yellow).ToAddColor(), lifetime, velocity.ToRotation(), 1f, scale * 0.55f);
                particleList.Add(crossStar);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
    }
}
