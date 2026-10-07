using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Misc
{
    public class TendouArisuItem : AccVanityItem
    {
        public override bool HasFlavorTooltip => false;
        public override VanityData VanityData => new VanityData(
                    Color.SkyBlue,
                    Color.Lerp(Color.DeepSkyBlue, Color.RoyalBlue, 0.86f),
                    Color.Lerp(Color.White, Color.DeepPink, 0f));
        public override Color ParticleColor1 => Color.Lerp(Color.RoyalBlue, Color.Blue, 0.3f);
        public override Color ParticleColor2 => Color.White;

        public override void ExLoad()
        {
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Back", EquipType.Back, this);
        }
        public override string VanityName => "TendouArisu";
    }
}
