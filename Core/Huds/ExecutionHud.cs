using HJScarletRework.Assets.Registers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.Huds
{
    public class ExecutorBackground
    {
        public Vector2 Center;
        public Vector2 Orig;
        public Texture2D Texture = HJScarletTexture.Hud_ExecutorCounter.Value;
    }
}
