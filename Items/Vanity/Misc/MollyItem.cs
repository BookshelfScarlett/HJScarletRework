using HJScarletRework.Globals.Methods;

namespace HJScarletRework.Items.Vanity.Misc
{
    public class MollyItem : AccVanityItem
    {
        public override VanityData VanityData => new VanityData(
            Color.LightPink,
            Color.Violet,
            Color.White);
        public override bool HasFlavorTooltip => false;
        public override Color ParticleColor1 => Color.HotPink;
        public override Color ParticleColor2 => Color.LightPink;
        public override string VanityName => "Molly";
        public override void ExSD()
        {
            Item.HJScarlet().ItemBelongTo = Globals.Database.Enums.EnumItemOwner.Donator;
            Item.HJScarlet().OwnerName = "初镜巡弱杂骸音";
        }
    }
}
