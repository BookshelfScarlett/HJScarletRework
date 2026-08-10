using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{

    public static class SolarRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.DarkOrange, Color.Lerp(Color.White, Color.DarkOrange, 0.65f), Color.White, 1);

        }
        public static void DrawItemNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = RarityDrawHelper.GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f);
                RaritySmoke rarityShinyOrb = new RaritySmoke(position, velocity, RandLerpColor(Color.DarkOrange, Color.OrangeRed), lifetime, RandRotTwoPi, 1, scale * 0.28f, true, true);
                particleList.Add(rarityShinyOrb);
            }
        }

        public static void DrawFlavorNameRarity(DrawableTooltipLine drawableTooltipLine)
        {

        }
        public static void DrawFlavorNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = RarityDrawHelper.GetParticlePosition(tooltipLine);
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, RandLerpColor(Color.DarkViolet, Color.Purple).ToAddColor(), lifetime, scale);
                position = RarityDrawHelper.GetParticlePosition(tooltipLine);
                velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, RandLerpColor(Color.DarkViolet, Color.Purple).ToAddColor(), lifetime, scale);
                particleList.Add(rarityShinyOrb);
                particleList.Add(rarityShinyOrb2);
            }
        }

    }
}
