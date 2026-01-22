using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void BeginDefault(this SpriteBatch SB) =>
          SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        public static void EnterShaderArea()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void EnterShaderArea(this SpriteBatch SB)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void EndShaderArea(this SpriteBatch SB)
        {
            SB.End();
            SB.BeginDefault();
        }
        public static RasterizerState ShowTriangleShapeForVertex(this SpriteBatch SB)
        {
            RasterizerState ori = Main.graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.FillMode = FillMode.WireFrame;
            Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
            return ori;
        }
        public static void EndShaderArea()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        public static RenderTarget2D NewRT2D(float Mult = 1f)
        {
            return new RenderTarget2D(Main.graphics.GraphicsDevice, (int)(Main.screenWidth * Mult), (int)(Main.screenHeight * Mult));
        }
        /// <summary>
        /// 将当前渲染目标设置为提供的渲染目标。
        /// </summary>
        /// <param name="rt">要交换到的渲染目标</param>
        public static bool SwapToTarget(this RenderTarget2D rt)
        {
            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Main.gameMenu || Main.dedServ || spriteBatch is null || rt is null || gD is null)
                return false;

            gD.SetRenderTarget(rt);
            gD.Clear(Color.Transparent);
            return true;
        }
        public static void ResetRT2D(this RenderTarget2D rt)
        {
            Vector2 size = rt.Size();
            Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (size != ScreenSize)
            {
                Main.QueueMainThreadAction(() =>
                {
                    rt = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
            }
        }
        public static Vector2 GetScreenSize
        {
            get
            {
                return new Vector2(Main.screenWidth, Main.screenHeight);
            }
        }
        public static Color ToAddColor(this Color color, byte alphaValue = 0) => color with { A = alphaValue };
    }
}
