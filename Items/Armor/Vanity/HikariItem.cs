using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Vanity
{
    public class HikariItem : AccVanityItem
    {
        public override string VanityName => "Hikari";
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                HikariEffect.DrawItemName(line);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleRose).
                AddIngredient(ItemID.JuliaButterfly).
                AddTile(TileID.Loom).
                Register();
        }
    }
    public class HikariPlayer : MenuVanityPlayer
    {
        public override int VanityItemType => ItemType<HikariItem>();
    }
    public class HikariEffect : ModRarity
    {
        public static List<RaritySparkle> RaritySparkles = [];
        public static void DrawItemName(DrawableTooltipLine line)
        {
            PostDrawRarity(ref RaritySparkles, line);
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.IndianRed, Color.Lerp(Color.White, Color.DarkRed, 0.65f), Color.White, 1);
        }

        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(Color.IndianRed, Color.White), lifetime,scale);
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

