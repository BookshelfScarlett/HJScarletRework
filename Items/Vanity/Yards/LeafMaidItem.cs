using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Yards
{
    public class LeafMaidItem : AccVanityItem
    {
        public override string VanityName => "LeafMaid";
        public override VanityData VanityData => new VanityData(
            Color.Lerp(Color.White, Color.SkyBlue, 0.35f),
            Color.Lerp(Color.White, Color.White, 0.95f),
            Color.Lerp(Color.RoyalBlue,Color.SkyBlue,0.65f));
        public override Color ParticleColor1 => Color.SkyBlue;
        public override Color ParticleColor2 => Color.White;
        public override bool HasFlavorTooltip => false;
        public override bool ExDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return false;
        }
        public override void ExSD()
        {
            Item.HJScarlet().CanDrawIcon = false;
            Item.HJScarlet().CanDrawGhost = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 15).
                DisableDecraft().
                AddTile(TileID.Loom).
                Register();
        }
    }
}
