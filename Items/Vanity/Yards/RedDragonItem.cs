using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Yards
{
    public class RedDragonItem : AccVanityItem
    {
        public override string VanityName => "RedDragon";
        public override VanityData VanityData => new VanityData(
            Color.Lerp(Color.White, Color.Red, 0.35f),
            Color.Black,
            Color.Lerp(Color.Crimson, Color.Red, 0.95f));
        public override Color ParticleColor1 => Color.Red;
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
