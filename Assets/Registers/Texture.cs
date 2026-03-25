using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        public static Tex2DWithPath Texture_BloomRing { get; set; }
        public static Tex2DWithPath Texture_BloomShockwave { get; set; }
        public static Tex2DWithPath Texture_SoftCircleEdge { get; set; }
        public static Tex2DWithPath Texture_WhiteCube { get; set; }
        public static Tex2DWithPath Texture_WhiteCubeBig { get; set; }
        public static Tex2DWithPath Texture_WhiteCircle { get; set; }
        public static Tex2DWithPath Texture_Spirite { get; set; }
        public static Tex2DWithPath Texture_EnergySword { get; set; }
        public static Tex2DWithPath Texture_Swirl { get; set; }
        public static Tex2DWithPath Texture_Swirl1 { get; set; }
        public static Tex2DWithPath Texture_Swirl2 { get; set; }
        public static Tex2DWithPath Texture_Swirl3 { get; set; }
        public static Tex2DWithPath Texture_Swirl4 { get; set; }
        public static Tex2DWithPath Texture_Swirl5 { get; set; }
        public static Tex2DWithPath Texture_RarityGlow { get; set; }
        public static Tex2DWithPath Texture_StandardGradient { get; set; }
        public void LoadTexture()
        {
            Texture_BloomRing = new Tex2DWithPath($"{Path_General}{nameof(Texture_BloomRing)}");
            Texture_BloomShockwave = new Tex2DWithPath($"{Path_General}{nameof(Texture_BloomShockwave)}");
            Texture_SoftCircleEdge = new Tex2DWithPath($"{Path_General}{nameof(Texture_SoftCircleEdge)}");
            Texture_WhiteCube = new Tex2DWithPath($"{Path_General}{nameof(Texture_WhiteCube)}");
            Texture_WhiteCubeBig = new Tex2DWithPath($"{Path_General}{nameof(Texture_WhiteCubeBig)}");
            Texture_WhiteCircle = new Tex2DWithPath($"{Path_General}{nameof(Texture_WhiteCircle)}");
            Texture_Spirite = new Tex2DWithPath($"{Path_General}{nameof(Texture_Spirite)}");
            Texture_EnergySword = new Tex2DWithPath($"{Path_General}{nameof(Texture_EnergySword)}");
            Texture_Swirl = new Tex2DWithPath($"{Path_General}{nameof(Texture_Swirl)}");
            Texture_Swirl1 = new Tex2DWithPath($"{Path_General}{nameof(Texture_Swirl1)}");
            Texture_Swirl2 = new Tex2DWithPath($"{Path_General}{nameof(Texture_Swirl2)}");
            Texture_Swirl3 = new Tex2DWithPath($"{Path_General}{nameof(Texture_Swirl3)}");
            Texture_Swirl4 = new Tex2DWithPath($"{Path_General}{nameof(Texture_Swirl4)}");
            Texture_Swirl5 = new Tex2DWithPath($"{Path_General}{nameof(Texture_Swirl5)}");
            Texture_RarityGlow = new Tex2DWithPath($"{Path_General}{nameof(Texture_RarityGlow)}");
            Texture_StandardGradient = new Tex2DWithPath($"{Path_General}{nameof(Texture_StandardGradient)}");

        }
        public void UnloadTexture()
        {
            Texture_BloomRing = null;
            Texture_BloomShockwave = null;
            Texture_SoftCircleEdge = null;
            Texture_WhiteCube = null;
            Texture_WhiteCubeBig = null;
            Texture_WhiteCircle = null;
            Texture_Spirite = null;
            Texture_EnergySword = null;
            Texture_Swirl = null;
            Texture_Swirl1 = null;
            Texture_Swirl2 = null;
            Texture_Swirl3 = null;
            Texture_Swirl4 = null;
            Texture_Swirl5 = null;
            Texture_RarityGlow = null;
            Texture_StandardGradient = null;
        }
    }
}
