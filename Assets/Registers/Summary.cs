using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        public static Tex2DWithPath ScarletGhost { get; set; }
        public static Tex2DWithPath InvisAsset { get; private set; }
        private string TexPath => "HJScarletRework/Assets/Texture";
        private string Path_Particle => $"{TexPath}/Particles/";
        private string Path_General => $"{TexPath}/General/";
        private string Path_Metaball => $"{TexPath}/Metaball/";
        private static string InvisAssetPath => "HJScarletRework/Assets/Texture/InvisibleProj";


        public override void Load()
        {
            InvisAsset = new Tex2DWithPath(InvisAssetPath);
            ScarletGhost = new Tex2DWithPath($"{TexPath}/{nameof(ScarletGhost)}");
            LoadParticle();
            LoadTrail();
            LoadTexture();
            LoadMisc();
            LoadHud();
        }
        public override void Unload()
        {
            InvisAsset = null;
            ScarletGhost = null;
            UnLoadParticle();
            UnloadTrail();
            UnloadTexture();
            UnloadMisc();
            UnloadHud();
        }
    }
}
