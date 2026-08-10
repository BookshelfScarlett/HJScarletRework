using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Rarity.RarityShinyMethod
{
    public static class SakuraRarity
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.HotPink, Color.Violet.ToAddColor(), Color.HotPink);
        }
        public static void DrawItemNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.10f * 0.5f, 0.10f) * 0.8f;
                int lifetime = 80;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.3f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(1.5f, 2.15f);
                SakuraPetals sakuraPetals = new SakuraPetals(position, velocity, RandLerpColor(Color.LightPink, Color.HotPink).ToAddColor(), lifetime, RandRotTwoPi, 1f, scale, 0.1f);
                particleList.Add(sakuraPetals);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }

    }
}
