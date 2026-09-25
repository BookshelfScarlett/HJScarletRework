using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.RenderTargetsManager;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Database;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace HJScarletRework.Core.DeepGlowSystem
{
    public partial class DeepGlow : ModSystem
    {
        public static int OtherLayerSave = -1;
        //降采样/升采样的目标 huancun
        public static Effect GlowEffect => HJScarletShader.DeepGlow;
        public static RenderTarget2D HighlightTarget;
        public static RenderTarget2D[] _downTargets;
        public static RenderTarget2D[] _upTargets;
        //迭代次数，会用于控制光晕的广度
        public static int Iterations = 5;
        public static float Threshold = .8f;
        public static float Intensity = 2f;
        public static float BlurRadius = 1f;
        public static Color DrawColor = Color.White;
        public static float SoftKnee = .25f;
        public static Queue<Action> GlowRequest = new Queue<Action>();
        public static void SubmitCustomGlow(Action drawAction, ScarletDrawLayer layer = ScarletDrawLayer.EndCapture)
        {
            if (Main.dedServ || drawAction is null || HJScarletConfigClient.Instance.PerformanceMode)
                return;
            if (layer == ScarletDrawLayer.AfterProjectiles)
                GlowRequests_AfterProjectiles.Enqueue(drawAction);
            else if (layer == ScarletDrawLayer.AfterDusts)
                GlowRequests_AfterDust.Enqueue(drawAction);
            else
                GlowRequest.Enqueue(drawAction);
        }
        public override void Load()
        {
            if (Main.dedServ)
                return;
            BuildRenderTargets();
        }
        public override void Unload()
        {
            Main.QueueMainThreadAction(() =>
            {
                HighlightTarget.Dispose();
                for (int i = 0; i < _downTargets.Length; i++)
                {
                    _downTargets[i].Dispose();
                }
                for (int i = 0; i < _upTargets.Length; i++)
                {
                    _upTargets[i].Dispose();
                }
            });
            // On_FilterManager.EndCapture -= DrawDeepGlow;
            GlowRequest.Clear();
        }
        public static void BuildRenderTargets()
        {
            RenderTarget2DManager.RequestScreenSizeRT2D(out OtherLayerSave);
            Main.QueueMainThreadAction(() =>
            {
                float width = Main.screenWidth;
                float height = Main.screenHeight;
                _downTargets = new RenderTarget2D[Iterations];
                HighlightTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)width, (int)height);
                //开始迭代
                for (int i = 0; i < Iterations; i++)
                {
                    //每一次迭代都会缩小一半的分辨率
                    width = Math.Max(1, width / 2f);
                    height = Math.Max(1, height / 2f);
                    //我们进行的是平滑模糊，使用SurfaceFormat.Color即可。
                    _downTargets[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)width, (int)height, false, SurfaceFormat.Color, DepthFormat.None);
                }
            });
            Main.QueueMainThreadAction(() =>
            {
                float width = Main.screenWidth;
                float height = Main.screenHeight;
                _upTargets = new RenderTarget2D[Iterations - 1];
                //这边vibe了一次
                //需要进行反向遍历，并且要确保迭代次数少一个元素，这里是很重要的，因为uptargets本就比downtargets少一个元素了
                for (int i = Iterations - 2; i >= 0; i--)
                {
                    width = Math.Max(1, width / 2f);
                    height = Math.Max(1, height / 2f);
                    //我们进行的是平滑模糊，使用SurfaceFormat.Color即可。
                    _upTargets[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)width, (int)height, false, SurfaceFormat.Color, DepthFormat.None);
                }
            });

        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.dedServ)
                return;
            if (RenderTarget2DManager.OldScreenSize != ScarletDatabase.ScreenSize)
                BuildRenderTargets();
        }

        public static void DrawDeepGlow(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (Main.dedServ || HJScarletConfigClient.Instance.PerformanceMode)
            {
                orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
                return;
            }
            if (GlowRequest.Count == 0 || _downTargets is null || HighlightTarget is null || GlowEffect is null || Iterations < 2)
            {
                GlowRequest.Clear();
                orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
                return;
            }
            //着色器参数
            //阈值
            GlowEffect.Parameters["uThreshold"].SetValue(Threshold);
            //泛光强度
            GlowEffect.Parameters["uIntensity"].SetValue(Intensity);
            //模糊半径
            GlowEffect.Parameters["uBlurRadius"].SetValue(BlurRadius);
            //软膝参数
            GlowEffect.Parameters["uSoftKnee"].SetValue(SoftKnee);
            //记录原始画面到screenTargetSwarp
            SaveScreenTarget(screenTarget1, screenTarget2);
            //绘制所有自定义光源至高亮图
            DrawCustomGlowsToHighlight();
            //降采样
            DownSampler();
            //开采样，混合，从最小的图开始往上叠层
            UpSampler();
            //最终结果，画回目标
            FinalDraw(screenTarget1, screenTarget2);
            Main.instance.GraphicsDevice.SetRenderTarget(null);
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
        public static void SaveScreenTarget(RenderTarget2D screenTarget, RenderTarget2D screenTargetSwap)
        {
            screenTargetSwap.SwapToTarget();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            Main.spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            Main.spriteBatch.End();
        }
        public static void DrawCustomGlowsToHighlight()
        {
            if (GlowRequest.Count == 0)
                return;
            HighlightTarget.SwapToTarget();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            while (GlowRequest.Count > 0)
            {
                Action drawAction = GlowRequest.Dequeue();
                drawAction.Invoke();
            }
            Main.spriteBatch.End();
        }
        public static void DownSampler()
        {
            for (int i = 0; i < Iterations; i++)
            {
                _downTargets[i].SwapToTarget();
                // 更新像素尺寸且降采样的同时模糊一次，得到更加平滑的效果，防止后续升采样时出现块状失真
                if (i == 0)
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    // 以半分辨率为单位的像素尺寸
                    GlowEffect.Parameters["uTexelSize"].SetValue(new Vector2(1f / _downTargets[i].Width, 1f / _downTargets[i].Height));
                    GlowEffect.CurrentTechnique.Passes["Downsample"].Apply();
                    Main.spriteBatch.Draw(HighlightTarget, new Rectangle(0, 0, _downTargets[i].Width, _downTargets[i].Height), Color.White);
                    Main.spriteBatch.End();
                }
                else
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    GlowEffect.Parameters["uTexelSize"].SetValue(new Vector2(1f / _downTargets[i].Width, 1f / _downTargets[i].Height));
                    GlowEffect.CurrentTechnique.Passes["Downsample"].Apply();
                    Main.spriteBatch.Draw(_downTargets[i - 1], new Rectangle(0, 0, _downTargets[i].Width, _downTargets[i].Height), Color.White);
                    Main.spriteBatch.End();
                }
            }
        }
        public static void UpSampler()
        {
            // 从最小的图开始往上叠，开始为最小的降采样图
            RenderTarget2D currentSource = _downTargets[Iterations - 1];
            RenderTarget2D currentSource2;
            Effect effect = GlowEffect;
            for (int i = 0; i < Iterations - 1; i++)
            {
                currentSource2 = _downTargets[Iterations - 2 - i];
                if (i == 0)
                {
                    _upTargets[i].SwapToTarget();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    effect.Parameters["uTexelSize"].SetValue(new Vector2(1f / currentSource.Width, 1f / currentSource.Height));
                    effect.CurrentTechnique.Passes["Upsample"].Apply();
                    Main.spriteBatch.Draw(currentSource, new Rectangle(0, 0, _upTargets[i].Width, _upTargets[i].Height), Color.White);
                    effect.Parameters["uTexelSize"].SetValue(new Vector2(1f / _upTargets[i].Width, 1f / _upTargets[i].Height));
                    effect.CurrentTechnique.Passes["Upsample"].Apply();
                    Main.spriteBatch.Draw(currentSource2, new Rectangle(0, 0, _upTargets[i].Width, _upTargets[i].Height), Color.White);
                    Main.spriteBatch.End();
                }
                else
                {
                    _upTargets[i].SwapToTarget();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    effect.Parameters["uTexelSize"].SetValue(new Vector2(1f / currentSource.Width, 1f / currentSource.Height));
                    effect.CurrentTechnique.Passes["Upsample"].Apply();
                    Main.spriteBatch.Draw(currentSource, new Rectangle(0, 0, _upTargets[i].Width, _upTargets[i].Height), Color.White);
                    effect.Parameters["uTexelSize"].SetValue(new Vector2(1f / _upTargets[i].Width, 1f / _upTargets[i].Height));
                    effect.CurrentTechnique.Passes["Upsample"].Apply();
                    Main.spriteBatch.Draw(currentSource2, new Rectangle(0, 0, _upTargets[i].Width, _upTargets[i].Height), Color.White);
                    Main.spriteBatch.End();
                }
                currentSource = _upTargets[i];
            }
        }
        public static void FinalDraw(RenderTarget2D screenTarget, RenderTarget2D screenTargetSwap)
        {
            screenTarget.SwapToTarget();
            RenderTarget2D target = _upTargets[Iterations - 2];
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            Main.spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);
            Main.spriteBatch.Draw(target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), DrawColor);
            Main.spriteBatch.End();
        }
    }
}
