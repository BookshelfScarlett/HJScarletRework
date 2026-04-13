using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        public static Tex2DWithPath Particle_ShinyOrb { get; set; }
        public static Tex2DWithPath Particle_HRShinyOrb { get; set; }
        public static Tex2DWithPath Particle_HRShinyOrbMedium { get; set; }
        public static Tex2DWithPath Particle_HRShinyOrbSmall { get; set; }
        public static Tex2DWithPath Particle_HRStar { get; set; }
        public static Tex2DWithPath Particle_HRStarWhite { get; set; }
        public static Tex2DWithPath Particle_Fire { get; set; }
        public static Tex2DWithPath Particle_Leafs { get; set; }
        public static Tex2DWithPath Particle_CrossGlow { get; set; }
        public static Tex2DWithPath Particle_Petal { get; set; }
        public static Tex2DWithPath Particle_OpticalLineGlow { get; set; }
        public static Tex2DWithPath Particle_FusableBall { get; set; }
        public static Tex2DWithPath Particle_FireShiny { get; set; }
        public static Tex2DWithPath Particle_Smoke { get; set; }
        public static Tex2DWithPath Particle_ShinyOrbHard { get; set; }
        public static Tex2DWithPath Particle_NoahButterfly { get; set; }
        public static Tex2DWithPath Particle_KiraStar { get; set; }
        public static Tex2DWithPath Particle_KiraStarGlow { get; set; }
        public static Tex2DWithPath Particle_MusicSymbol { get; set; }
        public static Tex2DWithPath Particle_Ring { get; set; }
        public static Tex2DWithPath Particle_RingHard { get; set; }
        public static Tex2DWithPath Particle_ScytheBlood { get; set; }
        public static Tex2DWithPath Particle_SmokeAlt { get; set; }
        public static Tex2DWithPath Particle_BoomSparkle { get; set; }
        public static Tex2DWithPath Particle_SharpTearClean { get; set; }
        public static Tex2DWithPath Particle_Lightning0 { get; set; }
        public static Tex2DWithPath Particle_Lightning1 { get; set; }
        public static Tex2DWithPath Particle_Lightning2 { get; set; }
        public static Tex2DWithPath Particle_HeartFullFill { get; set; }
        public static Tex2DWithPath Particle_HeartHalfFill { get; set; }
        public static Tex2DWithPath Particle_HeartNoFill { get; set; }
        public static Texture2D Particle_SharpTear => TextureAssets.Extra[ExtrasID.SharpTears].Value;


        public void LoadParticle()
        {
            Particle_ShinyOrb = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_ShinyOrb)}");
            Particle_HRShinyOrb = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRShinyOrb)}");
            Particle_HRShinyOrbMedium = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRShinyOrbMedium)}");
            Particle_HRShinyOrbSmall = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRShinyOrbSmall)}");
            Particle_HRStar = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRStar)}");
            Particle_HRStarWhite = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRStarWhite)}");
            Particle_Fire = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Fire)}");
            Particle_Leafs = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Leafs)}");
            Particle_CrossGlow = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_CrossGlow)}");
            Particle_Petal = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Petal)}");
            Particle_OpticalLineGlow = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_OpticalLineGlow)}");
            Particle_FusableBall = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_FusableBall)}");
            Particle_FireShiny = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_FireShiny)}");
            Particle_Smoke = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Smoke)}");
            Particle_ShinyOrbHard = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_ShinyOrbHard)}");
            Particle_KiraStar = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_KiraStar)}");
            Particle_KiraStarGlow = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_KiraStarGlow)}");
            Particle_NoahButterfly = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_NoahButterfly)}");
            Particle_Ring = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Ring)}");
            Particle_RingHard = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_RingHard)}");
            Particle_MusicSymbol = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_MusicSymbol)}");
            Particle_ScytheBlood = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_ScytheBlood)}");
            Particle_SmokeAlt = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_SmokeAlt)}");
            Particle_BoomSparkle = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_BoomSparkle)}");
            Particle_SharpTearClean = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_SharpTearClean)}");
            Particle_Lightning0 = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Lightning0)}");
            Particle_Lightning1 = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Lightning1)}");
            Particle_Lightning2 = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Lightning2)}");
            Particle_HeartFullFill = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HeartFullFill)}");
            Particle_HeartHalfFill = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HeartHalfFill)}");
            Particle_HeartNoFill = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HeartNoFill)}");

        }
        public void UnLoadParticle()
        {
            Particle_ShinyOrb = null;
            Particle_HRShinyOrb = null;
            Particle_HRShinyOrbMedium = null;
            Particle_HRShinyOrbSmall = null;
            Particle_HRStar = null;
            Particle_HRStarWhite = null;
            Particle_Fire = null;
            Particle_Leafs = null;
            Particle_CrossGlow = null;
            Particle_Petal = null;
            Particle_OpticalLineGlow = null;
            Particle_FusableBall = null;
            Particle_FireShiny = null;
            Particle_Smoke = null;
            Particle_ShinyOrbHard = null;
            Particle_KiraStar = null;
            Particle_KiraStarGlow = null;
            Particle_NoahButterfly = null;
            Particle_Ring = null;
            Particle_RingHard = null;
            Particle_MusicSymbol = null;
            Particle_ScytheBlood = null;
            Particle_SmokeAlt = null;
            Particle_BoomSparkle = null;
            Particle_SharpTearClean = null;
            Particle_Lightning0 = null;
            Particle_Lightning1 = null;
            Particle_Lightning2 = null;
            Particle_HeartHalfFill = null;
            Particle_HeartFullFill = null;
            Particle_HeartNoFill = null;
        }
    }
}
