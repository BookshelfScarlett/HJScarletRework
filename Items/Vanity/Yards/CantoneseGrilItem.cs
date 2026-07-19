using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Yards
{
    public class CantoneseGirlItem : AccVanityItem
    {
        public override string VanityName => "CantoneseGirl";
        public override VanityData VanityData => new VanityData(
            Color.Lerp(Color.White, Color.Green, 0.35f),
            Color.Lerp(Color.White, Color.DarkGreen, 0.95f),
            Color.White);
        public override Color ParticleColor1 => Color.Green;
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
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldSword, 1).
                AddIngredient(ItemID.Silk, 15).
                DisableDecraft().
                AddTile(TileID.Loom).
                Register();
        }
    }
}
