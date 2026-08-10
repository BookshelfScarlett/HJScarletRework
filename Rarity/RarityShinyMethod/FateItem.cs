using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{
    public static class FateWhiteRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            //最后更新他。
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.Ivory, Color.Black, Color.Ivory, 1);
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
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f) * (1 * -0.75f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(Color.Black, Color.White), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
        }
    }
    public static class FateCopperRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            //最后更新他。
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.Brown, new Color(139, 69, 19), Color.White, 1);
        }

    }
    public static class FateGoldenRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            //最后更新他。
            RarityDrawHelper.DrawCustomTooltipLine(line, new Color(255, 236, 191), Color.Black, new Color(255, 237, 139), 1);
        }
        public static void DrawItemNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = RarityDrawHelper.GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f) * (1 * -0.75f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
        }
    }
}
