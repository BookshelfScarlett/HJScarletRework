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
