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
