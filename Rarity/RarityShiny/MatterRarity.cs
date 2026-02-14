using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShiny
{
    public class MatterRarity : ModRarity
    {
        public static List<RaritySparkle> RaritySparkles = [];
        public override Color RarityColor => Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, 0.9f);
        public static void DrawRarity(DrawableTooltipLine drawableTooltipLine)
        {
            PostDrawRarity(ref RaritySparkles, drawableTooltipLine);
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.DeepSkyBlue, Color.SkyBlue.ToAddColor(), Color.DeepSkyBlue, 1.1f);
        }
        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb);
                position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                position.Y += textSize.Y * 0.4f;
                velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityCube rarityCube = new RarityCube(position, velocity, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue).ToAddColor(), lifetime, scale);
                particleList.Add(rarityCube);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
    }
}
