using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        public static Tex2DWithPath Trail_ManaStreak { get; set; }
        public static Tex2DWithPath Trail_ManaStreakTiny{ get; set; }
        public static Tex2DWithPath Trail_RvSlash { get; set; }
        public static Tex2DWithPath Trail_VShapeWithTail { get; set; }
        public static Tex2DWithPath Trail_ParaLine { get; set; }
        public static Tex2DWithPath Trail_TerraRayFlow { get; set; }
        public static Tex2DWithPath Trail_SquareLine { get; set; }
        public static Tex2DWithPath Trail_FadedStreak{ get; set; }
        public static Tex2DWithPath Trail_MegaBeam { get; set; }
        public static Tex2DWithPath Trail_ManaMegaBeam{ get; set; }
        public static Tex2DWithPath Trail_Lightning0    { get; set; }
        public static Tex2DWithPath Trail_Lightning1    { get; set; }
        public static Tex2DWithPath Trail_Lightning2    { get; set; }
        public static Tex2DWithPath Trail_Lightning3    { get; set; }
        public static Tex2DWithPath Trail_Lightning4    { get; set; }
        public void LoadTrail()
        {
            Trail_ManaStreak = new Tex2DWithPath($"{Path_General}{nameof(Trail_ManaStreak)}");
            Trail_ManaStreakTiny = new Tex2DWithPath($"{Path_General}{nameof(Trail_ManaStreakTiny)}");
            Trail_ParaLine = new Tex2DWithPath($"{Path_General}{nameof(Trail_ParaLine)}");
            Trail_VShapeWithTail = new Tex2DWithPath($"{Path_General}{nameof(Trail_VShapeWithTail)}");
            Trail_RvSlash = new Tex2DWithPath($"{Path_General}{nameof(Trail_RvSlash)}");
            Trail_TerraRayFlow = new Tex2DWithPath($"{Path_General}{nameof(Trail_TerraRayFlow)}");
            Trail_SquareLine = new Tex2DWithPath($"{Path_General}{nameof(Trail_SquareLine)}");
            Trail_ManaMegaBeam = new Tex2DWithPath($"{Path_General}{nameof(Trail_ManaMegaBeam)}");
            Trail_MegaBeam = new Tex2DWithPath($"{Path_General}{nameof(Trail_MegaBeam)}");
            Trail_FadedStreak = new Tex2DWithPath($"{Path_General}{nameof(Trail_FadedStreak)}");
            Trail_Lightning0 = new Tex2DWithPath($"{Path_General}{nameof(Trail_Lightning0)}");
            Trail_Lightning1 = new Tex2DWithPath($"{Path_General}{nameof(Trail_Lightning1)}");
            Trail_Lightning2 = new Tex2DWithPath($"{Path_General}{nameof(Trail_Lightning2)}");
            Trail_Lightning3 = new Tex2DWithPath($"{Path_General}{nameof(Trail_Lightning3)}");
            Trail_Lightning4 = new Tex2DWithPath($"{Path_General}{nameof(Trail_Lightning4)}");
        }
        public void UnloadTrail()
        {
            Trail_ManaStreak = null;
            Trail_ParaLine = null;
            Trail_RvSlash = null;
            Trail_VShapeWithTail = null;
            Trail_TerraRayFlow = null;
            Trail_SquareLine = null;
            Trail_ManaStreakTiny= null;
            Trail_FadedStreak= null;
            Trail_MegaBeam= null;
            Trail_ManaMegaBeam = null;
            Trail_Lightning0 = null;
            Trail_Lightning1 = null;
            Trail_Lightning2 = null;
            Trail_Lightning3 = null;
            Trail_Lightning4 = null;
        }
    }
}
