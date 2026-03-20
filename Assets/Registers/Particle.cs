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
        public static Tex2DWithPath Particle_MusicSymbol { get; set; }
        public static Tex2DWithPath Particle_Ring { get; set; }
        public static Tex2DWithPath Particle_RingHard { get; set; }
        public static Tex2DWithPath Particle_ScytheBlood { get; set; }
        public static Tex2DWithPath Particle_SmokeAlt  { get; set; }
        public static Tex2DWithPath Particle_BoomSparkle  { get; set; }
        

        public void LoadParticle()
        {
            Particle_ShinyOrb = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_ShinyOrb)}");
            Particle_HRShinyOrb = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRShinyOrb)}");
            Particle_HRShinyOrbMedium = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRShinyOrbMedium)}");
            Particle_HRShinyOrbSmall = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRShinyOrbSmall)}");
            Particle_HRStar = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_HRStar)}");
            Particle_Fire = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Fire)}");
            Particle_Leafs = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Leafs)}");
            Particle_CrossGlow = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_CrossGlow)}");
            Particle_Petal = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Petal)}");
            Particle_OpticalLineGlow = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_OpticalLineGlow)}");
            Particle_FusableBall = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_FusableBall)}");
            Particle_FireShiny = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_FireShiny)}");
            Particle_Smoke = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Smoke)}");
            Particle_ShinyOrbHard = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_ShinyOrbHard)}");
            Particle_KiraStar = new Tex2DWithPath($"{Path_Particle}{nameof( Particle_KiraStar)}");
            Particle_NoahButterfly = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_NoahButterfly)}");
            Particle_Ring = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Ring)}");
            Particle_RingHard = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_RingHard)}");
            Particle_MusicSymbol = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_MusicSymbol)}");
            Particle_ScytheBlood = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_ScytheBlood)}");
            Particle_SmokeAlt = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_SmokeAlt)}");
            Particle_BoomSparkle= new Tex2DWithPath($"{Path_Particle}{nameof(Particle_BoomSparkle)}");

        }
        public void UnLoadParticle()
        {
            Particle_ShinyOrb = null;
            Particle_HRShinyOrb = null;
            Particle_HRShinyOrbMedium = null;
            Particle_HRShinyOrbSmall = null;
            Particle_HRStar = null;
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
            Particle_NoahButterfly = null;
            Particle_Ring = null;
            Particle_RingHard = null;
            Particle_MusicSymbol = null;
            Particle_ScytheBlood = null;
            Particle_SmokeAlt = null;
            Particle_BoomSparkle = null;
        }
    }
}
