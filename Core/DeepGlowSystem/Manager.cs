using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.DeepGlowSystem
{
    public partial class DeepGlow : ModSystem
    {
        public static Queue<Action> GlowRequests_AfterDust = new Queue<Action>();
        public static Queue<Action> GlowRequests_AfterProjectiles = new Queue<Action>();
        public static Queue<Action> GlowRequests_BeforeTile = new Queue<Action>();
        public static void Hook_BeforeTile(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            RenderLayerGlow(ref GlowRequests_BeforeTile);
            orig(self);
        }
        public static void Hook_AfterProjectile(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);
            RenderLayerGlow(ref GlowRequests_AfterProjectiles);
        }

        public static void Hook_AfterDust(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            RenderLayerGlow(ref GlowRequests_AfterDust);
        }
        public static void RenderLayerGlow(ref Queue<Action> action)
        {
            if (action.Count == 0)
                return;
            if (_downTargets == null || HighlightTarget== null || GlowEffect == null || Iterations < 2)
            {
                action.Clear();
                return;
            }
            // 设置着色器参数
            GlowEffect.Parameters["uThreshold"].SetValue(Threshold);
            GlowEffect.Parameters["uIntensity"].SetValue(Intensity);
            GlowEffect.Parameters["uBlurRadius"].SetValue(BlurRadius);
            GlowEffect.Parameters["uSoftKnee"].SetValue(SoftKnee);
            // 保存当前原版的RT
            var originalTargets = Main.instance.GraphicsDevice.GetRenderTargets();
            // 提取高亮并绘制
            HighlightTarget.SwapToTarget();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            while (action.Count > 0)
            {
                Action drawAction = action.Dequeue();
                drawAction.Invoke();
            }
            Main.spriteBatch.End();
            DownSampler();
            UpSampler();
            Main.instance.GraphicsDevice.SetRenderTargets(originalTargets);
            RenderTarget2D finalGlowTexture = _upTargets[Iterations - 2];
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            Main.spriteBatch.Draw(finalGlowTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), DrawColor);
            Main.spriteBatch.End();
        }
    }
}
