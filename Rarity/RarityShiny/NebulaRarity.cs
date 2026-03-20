using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public class NebulaRarity : ModRarity
    {
        public static Color RareColor = Color.HotPink;
        public static List<RaritySparkle> RaritySparkles = [];
        public static List<RaritySparkle> FlavorSparkles = [];
        public override Color RarityColor => RareColor;
        public static void DrawRarityReverse(DrawableTooltipLine drawableTooltipLine)
        {
            PostDrawRarity(ref RaritySparkles, drawableTooltipLine);
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.Pink, Color.Lerp(Color.MediumPurple, Color.HotPink, 0.2f), Color.White, 1);
        }
        public static void DrawFlavorReverse(DrawableTooltipLine drawableTooltipLine)
        {
            PostDrawFlavorParticle(ref FlavorSparkles, drawableTooltipLine);
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.Pink, Color.Lerp(Color.MediumPurple, Color.HotPink, 0.2f), Color.White, 0);
        }
        public static void DrawMisc(DrawableTooltipLine drawableTooltipLine)
        {
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.Pink, Color.Lerp(Color.MediumPurple, Color.HotPink, 0.2f), Color.White, 0);
        }

        private static void PostDrawFlavorParticle(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)(textSize.X), (int)(textSize.Y * .78f)));
                Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.25f, 0.35f) * 1.1f;
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, RandLerpColor(Color.Violet, Color.White).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)(textSize.X), (int)(textSize.Y * 0.78f)));
                velocity = Vector2.UnitX * Main.rand.NextFloat(0.25f, 0.35f);
                scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                RarityShinyOrb rarityShinyOrb3 = new RarityShinyOrb(position, velocity, RandLerpColor(Color.DarkViolet, Color.White).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb4 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb3);
                particleList.Add(rarityShinyOrb4);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);

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
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, RandLerpColor(Color.Violet, Color.White).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
    }
}
