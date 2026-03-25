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
        public static Tex2DWithPath Trail_ManaStreak { get; set; }
        public static Tex2DWithPath Trail_RvSlash { get; set; }
        public static Tex2DWithPath Trail_VShapeWithTail { get; set; }
        public static Tex2DWithPath Trail_ParaLine { get; set; }
        public static Tex2DWithPath Trail_TerraRayFlow { get; set; }
        public static Tex2DWithPath Trail_SquareLine { get; set; }
        public void LoadTrail()
        {
            Trail_ManaStreak = new Tex2DWithPath($"{Path_General}{nameof(Trail_ManaStreak)}");
            Trail_ParaLine = new Tex2DWithPath($"{Path_General}{nameof(Trail_ParaLine)}");
            Trail_VShapeWithTail = new Tex2DWithPath($"{Path_General}{nameof(Trail_VShapeWithTail)}");
            Trail_RvSlash = new Tex2DWithPath($"{Path_General}{nameof(Trail_RvSlash)}");
            Trail_TerraRayFlow = new Tex2DWithPath($"{Path_General}{nameof(Trail_TerraRayFlow)}");
            Trail_SquareLine = new Tex2DWithPath($"{Path_General}{nameof(Trail_SquareLine)}");
        }
        public void UnloadTrail()
        {
            Trail_ManaStreak = null;
            Trail_ParaLine = null;
            Trail_RvSlash = null;
            Trail_VShapeWithTail = null;
            Trail_TerraRayFlow = null;
            Trail_SquareLine = null;
        }
    }
}
