using HJScarletRework.Assets.Registers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void EndShaderAreaPixel()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);
        }
        public static void EnterShaderAreaPixel(BlendState blendState)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);
        }
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
        public static void EnterShaderArea(this SpriteBatch SB, SpriteSortMode mode, BlendState blendState)
        {
            SB.End();
            SB.Begin(mode, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void EnterShaderArea(this SpriteBatch SB, BlendState blendState)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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
        public static void ApplyAlphaCut(Vector4 UVOffset, Vector2 UV, Vector2 UVMult, Color color)
        {
            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(UVOffset.X);
            effect2.Parameters["uFadeinRigtLength"].SetValue(UVOffset.Y);
            effect2.Parameters["uFadeinTopLength"].SetValue(UVOffset.Z);
            effect2.Parameters["uFadeinBottomLength"].SetValue(UVOffset.W);
            effect2.Parameters["UVOffset"].SetValue(UV);
            effect2.Parameters["UVMult"].SetValue(UVMult);
            effect2.Parameters["OverlayColor"].SetValue(color.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();
        }
        public static void ApplyAlphaCut(Vector4 UVOffset, Vector2 UV, Vector2 UVMult)
        {
            Effect effect2 = HJScarletShader.AlphaFade;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(UVOffset.X);
            effect2.Parameters["uFadeinRigtLength"].SetValue(UVOffset.Y);
            effect2.Parameters["uFadeinTopLength"].SetValue(UVOffset.Z);
            effect2.Parameters["uFadeinBottomLength"].SetValue(UVOffset.W);
            effect2.Parameters["UVOffset"].SetValue(UV);
            effect2.Parameters["UVMult"].SetValue(UVMult);
            effect2.CurrentTechnique.Passes[0].Apply();
        }
        /// <summary>
        /// 记得重置默认批次
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="edgeColor"></param>
        /// <param name="progress"></param>
        /// <param name="edgeWidth"></param>
        public static void ApplyMeltShader(this Texture2D tex, Color edgeColor, float progress, float edgeWidth = .01f)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = tex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = HJScarletTexture.Noise_Misc.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Effect shader = HJScarletShader.EdgeMeltsShader;
            shader.Parameters["progress"].SetValue(progress);
            shader.Parameters["InPutTextureSize"].SetValue(tex.Size());
            shader.Parameters["EdgeColor"].SetValue(edgeColor.ToVector4());
            shader.Parameters["EdgeWidth"].SetValue(edgeWidth);
            shader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
