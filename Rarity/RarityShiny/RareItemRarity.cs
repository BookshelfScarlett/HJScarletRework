using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public class RareItemRarity : ModRarity
    {
        public enum RareType
        {
            Copper,
            White,
            Gold,
            Donator
        }
        public override Color RarityColor => Color.Red;
        public static List<RaritySparkle> RaritySparkles = [];
        public static List<RaritySparkle> FlavorSparkles = [];
        public static void DrawItemName(DrawableTooltipLine line, RareType type)
        {
            Color particleColor1 = Color.White;
            Color particleColor2 = Color.White;
            Color glowColor = Color.White;
            Color edgeColor = Color.White;
            Color mainColor = Color.White;
            float glowMult = 1f;
            switch (type)
            {
                case RareType.White:
                    particleColor1 = Color.White;
                    particleColor2 = Color.Black;
                    glowColor = Color.Ivory;
                    mainColor = Color.Black;
                    edgeColor = Color.Ivory;
                    break;
                case RareType.Copper:
                    particleColor1 = Color.Brown;
                    particleColor2 = Color.RosyBrown;
                    glowColor = Color.Brown;
                    mainColor = Color.White;
                    edgeColor = new(139, 69, 19);
                    break;
                case RareType.Gold:
                    particleColor1 = Color.LightGoldenrodYellow;
                    particleColor2 = Color.Gold;
                    glowColor = new(255, 236, 101);
                    mainColor = new(255, 237, 139);
                    edgeColor = Color.Black;
                    break;
                case RareType.Donator:
                    particleColor1 = Color.Pink;
                    particleColor2 = Color.DeepPink;
                    glowColor = Color.LightPink;
                    mainColor = Color.White;
                    edgeColor = Color.Violet;
                    break;
            }

            if (type != RareType.Copper)
                PostDrawRarity(ref RaritySparkles, line, particleColor1, particleColor2);
            RarityDrawHelper.DrawCustomTooltipLine(line, glowColor, edgeColor, mainColor, glowMult);
        }
        public static void DrawFlavorTooltipName(DrawableTooltipLine line, RareType type)
        {
            Color particleColor1 = Color.White;
            Color particleColor2 = Color.White;
            Color glowColor = Color.White;
            Color edgeColor = Color.White;
            Color mainColor = Color.White;
            float glowMult = 1f;
            switch (type)
            {
                case RareType.White:
                    particleColor1 = Color.White;
                    particleColor2 = Color.Black;
                    glowColor = Color.Ivory;
                    mainColor = Color.Black;
                    edgeColor = Color.Ivory;
                    break;
                case RareType.Copper:
                    particleColor1 = Color.Brown;
                    particleColor2 = Color.RosyBrown;
                    glowColor = Color.Brown;
                    mainColor = Color.White;
                    edgeColor = new(139, 69, 19);
                    break;
                case RareType.Gold:
                    particleColor1 = Color.LightGoldenrodYellow;
                    particleColor2 = Color.Gold;
                    glowColor = new(255, 236, 101);
                    mainColor = new(255, 237, 139);
                    edgeColor = Color.Black;
                    break;
                case RareType.Donator:
                    particleColor1 = Color.Pink;
                    particleColor2 = Color.LightPink;
                    glowColor = Color.LightPink;
                    mainColor = Color.White;
                    edgeColor = Color.Violet;
                    break;
            }

            if (type != RareType.Copper)
            {
                //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
                Vector2 textSize = line.Font.MeasureString(line.Text);
                if (Main.rand.NextBool(10))
                {
                    float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                    int lifetime = 160;
                    Vector2 position = GetParticlePosition(line);
                    Vector2 velocity = -Vector2.UnitX * Main.rand.NextFloat(-0.25f, 0.55f) * (1);
                    RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(particleColor2, particleColor1), lifetime, scale);
                    FlavorSparkles.Add(rarityShinyOrb);
                }
                //最后更新他。
                RarityDrawHelper.UpdateTooltipParticles(line, ref FlavorSparkles);
            }
            RarityDrawHelper.DrawCustomTooltipLine(line, glowColor, edgeColor, mainColor, glowMult);
        }
        public static void DrawMisc(DrawableTooltipLine line, RareType type)
        {
        }

        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine, Color c, Color c2, bool slowdown = false)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f) * (1 + slowdown.ToInt() * -0.75f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(c, c2), lifetime, scale);
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
