using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Vanity.Misc
{
    public class TendouKeiItem : AccVanityItem
    {
        public override bool HasFlavorTooltip => false;
        public override VanityData VanityData => new VanityData(Color.HotPink, Color.Lerp(Color.WhiteSmoke, Color.HotPink, 0.165f), Color.Lerp(Color.HotPink, Color.DeepPink, 0.3f));
        public override Color ParticleColor1 => Color.Lerp(Color.LightPink, Color.HotPink, 0.3f);
        public override Color ParticleColor2 => Color.White;
        public override void ExLoad()
        {
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Back", EquipType.Back, this);
        }
        public override void ExSSD()
        {
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override string VanityName => "TendouKei";

    }
}
