using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        private string Path_Fonts => "HJScarletRework/Assets/Fonts/";
        public static Tex2DWithPath Specific_DialectBall { get; set; }
        public static Tex2DWithPath Specific_DialectCube { get; set; }
        public static Tex2DWithPath Specific_AimLabBox { get; set; }
        public static Tex2DWithPath Specific_Clock { get; set; }
        public static Tex2DWithPath Specific_RocketTrail { get; set; }

        public static Tex2DWithPath Noise_Misc { get; set; }
        public static Tex2DWithPath Noise_Misc2 { get; set; }
        public static Tex2DWithPath Noise_Aura { get; set; }
        public static Tex2DWithPath Noise_EmptyAura { get; set; }
        public static Tex2DWithPath Noise_HeavyAura { get; set; }
        public static Tex2DWithPath Noise_Smoke { get; set; }
        public static Tex2DWithPath Noise_WaterFlow { get; set; }
        public static Tex2DWithPath Noise_BlackGalaxy1 { get; set; }
        public static Tex2DWithPath Noise_BlackGalaxy2 { get; set; }


        public static Tex2DWithPath ColorMap_Aqua { get; set; }

        public static Tex2DWithPath Metaball_ShadowNebula { get; set; }
        public static Tex2DWithPath Metaball_FlickerWater { get; set; }
        public static Tex2DWithPath Metaball_GreenWater { get; set; }
        public static Tex2DWithPath Metaball_ShinyStardust { get; set; }
        public static Tex2DWithPath Metaball_Bloody { get; set; }
        public static Asset<DynamicSpriteFont> Font_Eras_Bold { get; set; }
        public static Asset<DynamicSpriteFont> Font_MGR { get; set; }

        public void LoadMisc()
        {
            Specific_DialectBall = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_DialectBall)}");
            Specific_DialectCube = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_DialectCube)}");
            Specific_AimLabBox = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_AimLabBox)}");
            Specific_Clock = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_Clock)}");
            Specific_RocketTrail = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_RocketTrail)}");

            Metaball_ShadowNebula = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_ShadowNebula)}");
            Metaball_FlickerWater = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_FlickerWater)}");
            Metaball_GreenWater = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_GreenWater)}");
            Metaball_ShinyStardust = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_ShinyStardust)}");
            Metaball_Bloody = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_Bloody)}");

            Noise_Misc = new Tex2DWithPath($"{Path_General}{nameof(Noise_Misc)}");
            Noise_Misc2 = new Tex2DWithPath($"{Path_General}{nameof(Noise_Misc2)}");
            Noise_Aura = new Tex2DWithPath($"{Path_General}{nameof(Noise_Aura)}");
            Noise_EmptyAura = new Tex2DWithPath($"{Path_General}{nameof(Noise_EmptyAura)}");
            Noise_HeavyAura = new Tex2DWithPath($"{Path_General}{nameof(Noise_HeavyAura)}");
            Noise_Smoke = new Tex2DWithPath($"{Path_General}{nameof(Noise_Smoke)}");
            Noise_WaterFlow = new Tex2DWithPath($"{Path_General}{nameof(Noise_WaterFlow)}");
            Noise_BlackGalaxy1 = new Tex2DWithPath($"{Path_General}{nameof(Noise_BlackGalaxy1)}");
            Noise_BlackGalaxy2 = new Tex2DWithPath($"{Path_General}{nameof(Noise_BlackGalaxy2)}");

            ColorMap_Aqua = new Tex2DWithPath($"{Path_General}{nameof(ColorMap_Aqua)}");

            Font_Eras_Bold = Request<DynamicSpriteFont>($"{Path_Fonts}{nameof(Font_Eras_Bold)}");
            Font_MGR = Request<DynamicSpriteFont>($"{Path_Fonts}{nameof(Font_MGR)}");
        }
        public static void UnloadMisc()
        {
            Specific_DialectCube = null;
            Specific_DialectBall = null;
            Specific_AimLabBox = null;
            Specific_Clock = null;
            Specific_RocketTrail = null;

            Metaball_ShadowNebula = null;
            Metaball_FlickerWater = null;
            Metaball_GreenWater = null;
            Metaball_ShinyStardust = null;
            Metaball_Bloody = null;

            Noise_Misc = null;
            Noise_Misc2 = null;
            Noise_Aura = null;
            Noise_EmptyAura = null;
            Noise_Smoke = null;
            Noise_HeavyAura = null;
            Noise_WaterFlow = null;
            Noise_BlackGalaxy2 = null;
            Noise_BlackGalaxy1 = null;

            ColorMap_Aqua = null;

            Font_Eras_Bold = null;
            Font_MGR = null;
        }
    }
}
