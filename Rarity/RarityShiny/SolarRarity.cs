using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public class SolarRarity : ModRarity
    {
        public static List<RaritySparkle> RaritySparkles = [];
        public static List<RaritySparkle> FlavorSparkles = [];
        public override Color RarityColor => Color.Orange;

        public static void DrawItemName(DrawableTooltipLine line)
        {
            PostDrawRarity(ref RaritySparkles, line);
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.DarkOrange, Color.Lerp(Color.White, Color.DarkOrange, 0.65f), Color.White, 1);

        }
        public static void DrawFlavorTooltip(DrawableTooltipLine line)
        {

        }
        private static void PostDrawFlavorParticle(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = GetParticlePosition(tooltipLine);
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, RandLerpColor(Color.DarkViolet, Color.Purple).ToAddColor(), lifetime, scale);
                position = GetParticlePosition(tooltipLine);
                velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, RandLerpColor(Color.DarkViolet, Color.Purple).ToAddColor(), lifetime, scale);
                particleList.Add(rarityShinyOrb);
                particleList.Add(rarityShinyOrb2);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f);
                RaritySmoke rarityShinyOrb = new RaritySmoke(position, velocity, RandLerpColor(Color.DarkOrange, Color.OrangeRed), lifetime, RandRotTwoPi, 1, scale * 0.28f, true, true);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
        public static Vector2 GetParticlePosition(DrawableTooltipLine line)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            return Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
        }
    }
}
