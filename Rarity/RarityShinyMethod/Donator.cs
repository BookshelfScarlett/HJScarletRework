using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{
    public static class DonatorRarity
    {
        public static void DrawItemName(DrawableTooltipLine line, ref List<RaritySparkle> raritySparklesList)
        {
            PostDrawRarity(ref raritySparklesList, line, Color.Pink, Color.LightPink, false);
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.LightPink, Color.Violet, Color.White, 1);
        }
        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine, Color c, Color c2, bool slowdown = false)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = RarityDrawHelper.GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f) * (1 + slowdown.ToInt() * -0.75f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(c, c2), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
        }
    }
}
