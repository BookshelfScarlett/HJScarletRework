using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public struct ColorCombination
    {
        public Color ParticleColorBegin;
        public Color ParticleColorEnd;
        public Color GlowColor;
        public Color EdgeColor;
        public Color MainColor;
    }
    public class RareItemRarity : ModRarity
    {
        public enum RareType
        {
            Copper,
            White,
            Gold,
            Donator,
            Developer,
            Support
        }
        public override Color RarityColor => Color.Red;
        public static List<RaritySparkle> RaritySparkles = [];
        public static List<RaritySparkle> FlavorSparkles = [];
        public static void DrawItemName(DrawableTooltipLine line, RareType type)
        {
            float glowMult = 1f;
            ColorCombination colorCombination;
            switch (type)
            {
                case RareType.White:
                    colorCombination.MainColor = Color.Black;
                    colorCombination.GlowColor = Color.Ivory;
                    colorCombination.EdgeColor = Color.Ivory;
                    colorCombination.ParticleColorBegin = Color.Black;
                    colorCombination.ParticleColorEnd = Color.White;
                    break;
                case RareType.Gold:
                    colorCombination.MainColor = new Color(255, 237, 139);
                    colorCombination.GlowColor = new Color(255, 236, 101);
                    colorCombination.EdgeColor = Color.Black;
                    colorCombination.ParticleColorBegin = Color.LightGoldenrodYellow;
                    colorCombination.ParticleColorEnd = Color.Gold;
                    break;
                case RareType.Donator:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.LightPink;
                    colorCombination.EdgeColor = Color.Violet;
                    colorCombination.ParticleColorBegin = Color.Pink;
                    colorCombination.ParticleColorEnd = Color.DeepPink;
                    break;

                case RareType.Developer:
                    colorCombination.MainColor = Color.DarkViolet;
                    colorCombination.GlowColor = Color.Violet;
                    colorCombination.EdgeColor = Color.Black;
                    colorCombination.ParticleColorBegin = Color.Violet;
                    colorCombination.ParticleColorEnd = Color.Purple;
                    break;

                case RareType.Copper:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.Brown;
                    colorCombination.EdgeColor = new Color(139, 69, 19);
                    colorCombination.ParticleColorBegin = Color.Brown;
                    colorCombination.ParticleColorEnd = Color.RosyBrown;
                    break;
                case RareType.Support:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.GreenYellow;
                    colorCombination.EdgeColor = Color.DarkOliveGreen;
                    colorCombination.ParticleColorBegin = Color.Green;
                    colorCombination.ParticleColorEnd = Color.Green;


                    break;

                default:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.White;
                    colorCombination.EdgeColor = Color.White;
                    colorCombination.ParticleColorBegin = Color.White;
                    colorCombination.ParticleColorEnd = Color.White;
                    break;
            }


            if (type != RareType.Copper)
                PostDrawRarity(ref RaritySparkles, line, colorCombination.ParticleColorBegin, colorCombination.ParticleColorEnd);
            RarityDrawHelper.DrawCustomTooltipLine(line, colorCombination.GlowColor, colorCombination.EdgeColor, colorCombination.MainColor, glowMult);
        }
        public static void DrawFlavorTooltipName(DrawableTooltipLine line, RareType type)
        {
            float glowMult = 1;
            ColorCombination colorCombination;
            switch (type)
            {
                case RareType.White:
                    colorCombination.MainColor = Color.Black;
                    colorCombination.GlowColor = Color.Ivory;
                    colorCombination.EdgeColor = Color.Ivory;
                    colorCombination.ParticleColorBegin = Color.Black;
                    colorCombination.ParticleColorEnd = Color.White;
                    break;
                case RareType.Gold:
                    colorCombination.MainColor = new Color(255, 237, 139);
                    colorCombination.GlowColor = new Color(255, 236, 101);
                    colorCombination.EdgeColor = Color.Black;
                    colorCombination.ParticleColorBegin = Color.LightGoldenrodYellow;
                    colorCombination.ParticleColorEnd = Color.Gold;
                    break;
                case RareType.Donator:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.LightPink;
                    colorCombination.EdgeColor = Color.Violet;
                    colorCombination.ParticleColorBegin = Color.Pink;
                    colorCombination.ParticleColorEnd = Color.DeepPink;
                    break;

                case RareType.Copper:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.Brown;
                    colorCombination.EdgeColor = new Color(139, 69, 19);
                    colorCombination.ParticleColorBegin = Color.Brown;
                    colorCombination.ParticleColorEnd = Color.RosyBrown;
                    break;
                case RareType.Developer:
                    colorCombination.MainColor = Color.Black;
                    colorCombination.GlowColor = Color.Crimson;
                    colorCombination.EdgeColor = Color.Crimson;
                    colorCombination.ParticleColorBegin = Color.Red;
                    colorCombination.ParticleColorEnd = Color.Black;
                    break;
                case RareType.Support:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.GreenYellow;
                    colorCombination.EdgeColor = Color.DarkOliveGreen;
                    colorCombination.ParticleColorBegin = Color.Green;
                    colorCombination.ParticleColorEnd = Color.Green;
                    break;

                default:
                    colorCombination.MainColor = Color.White;
                    colorCombination.GlowColor = Color.White;
                    colorCombination.EdgeColor = Color.White;
                    colorCombination.ParticleColorBegin = Color.White;
                    colorCombination.ParticleColorEnd = Color.White;
                    break;
            }

            if (type != RareType.Copper)
            {
                //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
                if (Main.rand.NextBool(10))
                {
                    float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                    int lifetime = 160;
                    Vector2 position = GetParticlePosition(line);
                    Vector2 velocity = -Vector2.UnitX * Main.rand.NextFloat(-0.25f, 0.55f) * (1);
                    RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(colorCombination.ParticleColorBegin, colorCombination.ParticleColorEnd), lifetime, scale);
                    FlavorSparkles.Add(rarityShinyOrb);
                }
                //最后更新他。
                RarityDrawHelper.UpdateTooltipParticles(line, ref FlavorSparkles);
            }
            RarityDrawHelper.DrawCustomTooltipLine(line, colorCombination.GlowColor, colorCombination.EdgeColor, colorCombination.MainColor, glowMult);
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
