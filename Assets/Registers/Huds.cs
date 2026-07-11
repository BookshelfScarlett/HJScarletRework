using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public partial class HJScarletTexture : ModSystem
    {
        private string Path_Huds => $"{TexPath}/Huds/";
        public static Asset<Texture2D> Hud_ExecutorCounter { get; set; }
        public static Asset<Texture2D> Hud_DarkOverlayer { get; set; }
        public void LoadHud()
        {
            Hud_ExecutorCounter = Request<Texture2D>($"{Path_Huds}{nameof(Hud_ExecutorCounter)}");
            Hud_DarkOverlayer = Request<Texture2D>($"{Path_Huds}{nameof(Hud_DarkOverlayer)}");
        }
        public static void UnloadHud()
        {
            Hud_ExecutorCounter = null;
            Hud_DarkOverlayer = null;
        }
    }
}
