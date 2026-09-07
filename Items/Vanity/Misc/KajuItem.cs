namespace HJScarletRework.Items.Vanity.Misc
{
    public class KajuItem : AccVanityItem
    {
        public override VanityData VanityData => new VanityData(Color.Gold, Color.Lerp(Color.DarkGoldenrod, Color.White, 0f), Color.Lerp(Color.White, Color.WhiteSmoke, 0.3f));
        public override bool HasFlavorTooltip => false;
        public override Color ParticleColor1 => Color.Lerp(Color.Gold, Color.White, 0.3f);
        public override Color ParticleColor2 => Color.Gold;
        public override string VanityName => "Kaju";
    }
}
