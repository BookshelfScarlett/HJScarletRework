using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{
    public static class ScarletRedRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.Crimson, Color.Lerp(Color.Crimson, Color.Red, 0.9f), Color.Black, 1f);
        }
        public static void DrawFlavorNameRarity(DrawableTooltipLine drawableTooltipLine)
        {
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.Crimson, Color.Lerp(Color.Crimson, Color.Red, 0.9f), Color.Black, 0);
        }
        public static void DrawItemNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(8))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, RandLerpColor(Color.Crimson, Color.Red).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb);
            }
        }
    }
}
