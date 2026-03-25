using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        public static Tex2DWithPath Specific_DialectBall { get; set; }
        public static Tex2DWithPath Specific_DialectCube { get; set; }
        public static Tex2DWithPath Specific_AimLabBox { get; set; }
        public static Tex2DWithPath Specific_Clock { get; set; }
        public static Tex2DWithPath Specific_RocketTrail { get; set; }

        public static Tex2DWithPath Noise_Misc { get; set; }
        public static Tex2DWithPath Noise_Misc2 { get; set; }

        public static Tex2DWithPath ColorMap_Aqua {  get; set; }

        public static Tex2DWithPath Metaball_ShadowNebula { get; set; }
        public static Tex2DWithPath Metaball_FlickerWater { get; set; }

        public void LoadMisc()
        {
            Specific_DialectBall = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_DialectBall)}");
            Specific_DialectCube = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_DialectCube)}");
            Specific_AimLabBox = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_AimLabBox)}");
            Specific_Clock = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_Clock)}");
            Specific_RocketTrail = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_RocketTrail)}");

            Metaball_ShadowNebula = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_ShadowNebula)}");
            Metaball_FlickerWater = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_FlickerWater)}");

            Noise_Misc = new Tex2DWithPath($"{Path_General}{nameof(Noise_Misc)}");
            Noise_Misc2 = new Tex2DWithPath($"{Path_General}{nameof(Noise_Misc2)}");

            ColorMap_Aqua = new Tex2DWithPath($"{Path_General}{nameof(ColorMap_Aqua)}");

        }
        public void UnloadMisc()
        {
            Specific_DialectCube = null;
            Specific_DialectBall = null;
            Specific_AimLabBox = null;
            Specific_Clock = null;
            Specific_RocketTrail = null;

            Metaball_ShadowNebula = null;
            Metaball_FlickerWater = null;

            Noise_Misc = null;
            Noise_Misc2 = null;

            ColorMap_Aqua = null;

        }
    }
}
