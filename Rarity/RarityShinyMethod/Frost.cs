using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{
    public static class FrostRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            //最后更新他。
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.RoyalBlue, Color.Lerp(Color.White, Color.RoyalBlue, 0.65f), Color.White, 1);
        }
        public static void DrawItemNameParticle(DrawableTooltipLine line, ref List<RaritySparkle> raritySparklesList)
        {
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = RarityDrawHelper.GetParticlePosition(line);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f);
                RaritySnowCloud rarityShinyOrb = new(position, velocity, RandLerpColor(Color.RoyalBlue, Color.WhiteSmoke), lifetime, RandRotTwoPi, 1, scale * 0.128f, true, true);
                raritySparklesList.Add(rarityShinyOrb);
            }
        }
        public static void DrawFlavorTooltip(DrawableTooltipLine line, ref List<RaritySparkle> flavorSparklesList)
        {
        }
    }
}
