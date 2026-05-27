using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Vanity
{
    public class HikariItem : AccVanityItem
    {
        public override string VanityName => "Hikari";
        public override VanityData VanityData => new VanityData(
            Color.Lerp(Color.White,Color.DarkRed,0.35f),
            Color.Lerp(Color.White, Color.IndianRed, 0.95f),
            Color.White);
        public override Color ParticleColor1 => Color.IndianRed;
        public override Color ParticleColor2 => Color.White;

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

