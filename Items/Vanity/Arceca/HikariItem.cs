using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Arceca
{
    public class HikariItem : AccVanityItem
    {
        public override string VanityName => "Hikari";
        public override VanityData VanityData => new VanityData(
            Color.Lerp(Color.White, Color.DarkRed, 0.35f),
            Color.Lerp(Color.White, Color.IndianRed, 0.95f),
            Color.White);
        public override Color ParticleColor1 => Color.IndianRed;
        public override Color ParticleColor2 => Color.White;

        public override bool ExDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            //if (line.Mod == "Terraria")
            //{
            //    if (line.Name == "Tooltip0" || line.Name == "Tooltip1" || line.Name == "Tooltip2" || line.Name == "Tooltip3" || line.Name == "Tooltip4" || line.Name == "Tooltip5" || line.Name == "Tooltip6")
            //    {
            //        VanityEffectClass.DrawMisc(line, VanityData, ParticleColor1, ParticleColor2);
            //        return true;
            //    }
            //}
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleRose).
                AddIngredient(ItemID.JuliaButterfly).
                DisableDecraft().
                AddTile(TileID.Loom).
                Register();
        }
    }
}

