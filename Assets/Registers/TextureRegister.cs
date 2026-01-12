using HJScarletRework.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Security;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public class HJScarletTexture : ModSystem
    {
        public static Asset<Texture2D> Particle_ShinyOrb {  get; set; }
        public static Asset<Texture2D> Particle_HRShinyOrb {  get; set; }
        public static Asset<Texture2D> Particle_HRShinyOrbMedium {  get; set; }
        public static Asset<Texture2D> Particle_HRShinyOrbSmall {  get; set; }
        public static Asset<Texture2D> Particle_HRStar { get; set; }
        public static Asset<Texture2D> Particle_Fire { get; set; }
        public static Asset<Texture2D> Particle_Leafs { get; set; }
        public static Asset<Texture2D> Particle_CrossGlow { get; set; }
        public static Tex2DWithPath Particle_Petal { get; set; }
        public static Tex2DWithPath Particle_OpticalLineGlow { get; set; }

        public static Asset<Texture2D> Texture_BloomRing { get; set; }
        public static Asset<Texture2D> Texture_BloomShockwave { get; set; }
        public static Asset<Texture2D> Texture_SoftCircleEdge { get; set; }
        public static Tex2DWithPath Texture_WhiteCube { get; set; }
        public static Tex2DWithPath Texture_WhiteCubeBig { get; set; }
        public static Tex2DWithPath Texture_WhiteCircle { get; set; }

        public static Asset<Texture2D> Specific_DialectBall { get; set; }
        public static Tex2DWithPath Specific_DialectCube { get; set; }
        public static Tex2DWithPath Specific_AimLabBox { get; set; }
        public static Asset<Texture2D> Specific_SecondArrow { get; set; }
        public static Asset<Texture2D> Specific_Clock { get; set; }

        public static Asset<Texture2D> Trail_ManaStreak { get; set; }
        public static Asset<Texture2D> Trail_RvSlash { get; set; }
        public static Asset<Texture2D> Trail_VShapeWithTail { get; set; }
        public static Asset<Texture2D> Trail_ParaLine { get; set; }
        public static Tex2DWithPath Trail_TerraRayFlow { get; set; }
        public static Tex2DWithPath Trail_SquareLine { get; set; }

        public static Asset<Texture2D> Noise_Misc { get; set; }
        public static Asset<Texture2D> Noise_Misc2 { get; set; }

        public static Tex2DWithPath Metaball_ShadowNebula { get; set; }

        public static Asset<Texture2D> ScarletGhost { get;set; }
        public static Tex2DWithPath InvisAsset { get; private set; }
        private string TexPath => "HJScarletRework/Assets/Texture";
        private string Path_Particle => $"{TexPath}/Particles/";
        private string Path_General => $"{TexPath}/General/";
        private string Path_Metaball => $"{TexPath}/Metaball/";
        private static string InvisAssetPath => "HJScarletRework/Assets/Texture/InvisibleProj";
        public static string Specific_SecondArrowPath => "HJScarletRework/Assets/Texture/Particles/Specific_SecondArrow";
        public override void Load()
        {
            InvisAsset = new Tex2DWithPath(InvisAssetPath);
            ScarletGhost = Request<Texture2D>($"{TexPath}/{nameof(ScarletGhost)}");

            Particle_ShinyOrb = Request<Texture2D>($"{Path_Particle}{nameof(Particle_ShinyOrb)}");
            Particle_HRShinyOrb = Request<Texture2D>($"{Path_Particle}{nameof(Particle_HRShinyOrb)}");
            Particle_HRShinyOrbMedium = Request<Texture2D>($"{Path_Particle}{nameof(Particle_HRShinyOrbMedium)}");
            Particle_HRShinyOrbSmall = Request<Texture2D>($"{Path_Particle}{nameof(Particle_HRShinyOrbSmall)}");
            Particle_HRStar = Request<Texture2D>($"{Path_Particle}{nameof(Particle_HRStar)}");
            Particle_Fire = Request<Texture2D>($"{Path_Particle}{nameof(Particle_Fire)}");
            Particle_Leafs = Request<Texture2D>($"{Path_Particle}{nameof(Particle_Leafs)}");
            Particle_CrossGlow = Request<Texture2D>($"{Path_Particle}{nameof(Particle_CrossGlow)}");
            Particle_Petal = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_Petal)}");
            Particle_OpticalLineGlow = new Tex2DWithPath($"{Path_Particle}{nameof(Particle_OpticalLineGlow)}");

            Specific_DialectBall = Request<Texture2D>($"{Path_Particle}{nameof(Specific_DialectBall)}");
            Specific_DialectCube = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_DialectCube)}");
            Specific_AimLabBox = new Tex2DWithPath($"{Path_Particle}{nameof(Specific_AimLabBox)}");
            Specific_SecondArrow = Request<Texture2D>($"{Path_Particle}{nameof(Specific_SecondArrow)}");
            Specific_Clock = Request<Texture2D>($"{Path_Particle}{nameof(Specific_Clock)}");
            
            Texture_BloomRing = Request<Texture2D>($"{Path_General}{nameof(Texture_BloomRing)}");
            Texture_BloomShockwave = Request<Texture2D>($"{Path_General}{nameof(Texture_BloomShockwave)}");
            Texture_SoftCircleEdge = Request<Texture2D>($"{Path_General}{nameof(Texture_BloomRing)}");
            Texture_WhiteCube = new Tex2DWithPath($"{Path_General}{nameof(Texture_WhiteCube)}");
            Texture_WhiteCubeBig = new Tex2DWithPath($"{Path_General}{nameof(Texture_WhiteCubeBig)}");
            Texture_WhiteCircle = new Tex2DWithPath($"{Path_General}{nameof(Texture_WhiteCircle)}");


            Trail_ManaStreak = Request<Texture2D>($"{Path_General}{nameof(Trail_ManaStreak)}");
            Trail_ParaLine = Request<Texture2D>($"{Path_General}{nameof(Trail_ParaLine)}");
            Trail_VShapeWithTail = Request<Texture2D>($"{Path_General}{nameof(Trail_VShapeWithTail)}");
            Trail_RvSlash = Request<Texture2D>($"{Path_General}{nameof(Trail_RvSlash)}");
            Trail_TerraRayFlow = new Tex2DWithPath($"{Path_General}{nameof(Trail_TerraRayFlow)}");
            Trail_SquareLine = new Tex2DWithPath($"{Path_General}{nameof(Trail_SquareLine)}");

            Metaball_ShadowNebula = new Tex2DWithPath($"{Path_Metaball}{nameof(Metaball_ShadowNebula)}");

            Noise_Misc = Request<Texture2D>($"{Path_General}{nameof(Noise_Misc)}");
            Noise_Misc2 = Request<Texture2D>($"{Path_General}{nameof(Noise_Misc2)}");

        }
        public override void Unload()
        {
            InvisAsset = null;
            ScarletGhost = null;

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

            Specific_DialectCube = null;
            Specific_DialectBall = null;
            Specific_AimLabBox = null;
            Specific_Clock = null;

            Texture_BloomRing = null;
            Texture_BloomShockwave = null;
            Texture_SoftCircleEdge = null;
            Texture_WhiteCube = null;
            Texture_WhiteCubeBig = null;
            Texture_WhiteCircle = null;

            Trail_ManaStreak = null;
            Trail_ParaLine = null;
            Trail_RvSlash = null;
            Trail_VShapeWithTail = null;
            Trail_TerraRayFlow = null;
            Trail_SquareLine = null;

            Metaball_ShadowNebula = null;

            Noise_Misc = null;
            Noise_Misc2 = null;
        }
    }
}
