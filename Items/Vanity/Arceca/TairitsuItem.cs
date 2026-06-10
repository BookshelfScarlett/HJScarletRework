using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Arceca
{
    public class TairitsuItem : AccVanityItem
    {
        public override VanityData VanityData => new VanityData(Color.RoyalBlue, Color.Lerp(Color.White, Color.DeepSkyBlue, 0.65f), Color.Black);
        public override Color ParticleColor1 => Color.DeepSkyBlue;
        public override Color ParticleColor2 => Color.Black;
        public override string VanityName => "Tairitsu";
        public override void ExLoad()
        {
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Hair", EquipType.Back, this);
        }
        public override bool ExDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria")
            {
                if (line.Name == "Tooltip0" || line.Name == "Tooltip1" || line.Name == "Tooltip2" || line.Name == "Tooltip3" || line.Name == "Tooltip4" || line.Name == "Tooltip5")
                {
                    VanityEffectClass.DrawMisc(line, VanityData, ParticleColor1, ParticleColor2);
                    return true;
                }
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
