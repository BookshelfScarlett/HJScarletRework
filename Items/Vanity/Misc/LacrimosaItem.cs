using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Misc
{
    public class LacrimosaItem : AccVanityItem
    {
        public override VanityData VanityData => new VanityData(Color.Red, Color.Lerp(Color.WhiteSmoke, Color.Crimson, 0.165f), Color.Lerp(Color.Red, Color.WhiteSmoke, 0.3f));
        public override Color ParticleColor1 => Color.Lerp(Color.Red, Color.Crimson, 0.3f);
        public override Color ParticleColor2 => Color.DarkRed;
        public override string VanityName => "Lacrimosa";
        public override bool ExDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria")
            {
                //if (line.Name == "Tooltip0" || line.Name == "Tooltip1" || line.Name == "Tooltip2" || line.Name == "Tooltip3" || line.Name == "Tooltip4" || line.Name == "Tooltip5")
                //{
                //    VanityEffectClass.DrawMisc(line, VanityData, ParticleColor1, ParticleColor2);
                //    return true;
                //}
            }
            return false;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Umbrella).
                AddIngredient(ItemID.UlyssesButterfly).
                DisableDecraft().
                AddTile(TileID.Loom).
                Register();
        }

    }
}
