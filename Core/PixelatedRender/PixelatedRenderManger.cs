using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.PixelatedRender
{
    public class PixelatedRenderManger : ModSystem
    {
        public static bool BeginDrawProj = false;
        public static RenderTarget2D BeforePlayerTarget;
        public static RenderTarget2D BeforeDustTarget;
        public static List<IPixelatedRenderer> BeforePlayers = [];
        public static bool BeginDrawBeforePlayers = false;
        public static List<IPixelatedRenderer> BeforeDusts = [];
        public static bool BeginDrawBeforeDusts = false;
        // public static Matrix PixelRenderMatrix;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                BeforePlayerTarget = HJScarletMethods.NewRT2D();
                BeforeDustTarget = HJScarletMethods.NewRT2D();
            });
            On_Main.CheckMonoliths += PrepareRenderTarget;
        }
        public override void Unload()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                BeforePlayerTarget?.Dispose();
                BeforePlayerTarget = null;
                BeforeDustTarget?.Dispose();
                BeforeDustTarget = null;
            });
            On_Main.CheckMonoliths -= PrepareRenderTarget;
        }
        public static void PrepareRenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            if (Main.dedServ || Main.gameMenu)
            {
                orig();
                return;
            }
            orig();
            // 收集所有接口的信息
            BeforePlayers.Clear();
            BeforeDusts.Clear();
            // 创建像素化矩阵，暂时用不到
            // float pixelScale = 2f;
            // Matrix shrinkMatrix = Matrix.CreateScale(1f / pixelScale, 1f / pixelScale, 1f);
            if (BeginDrawProj)
            {
                // 检查所有弹幕，如果弹幕继承了接口，那就会把这个添加到对应图层表单中
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.ModProjectile != null && projectile.ModProjectile is IPixelatedRenderer pRPlayer)
                    {
                        if (pRPlayer.LayerToRenderTo.HasFlag(HJScarletDrawLayer.BeforePlayer))
                            BeforePlayers.Add(pRPlayer);
                        if (pRPlayer.LayerToRenderTo.HasFlag(HJScarletDrawLayer.BeforeDusts))
                            BeforeDusts.Add(pRPlayer);
                    }
                }
                // 收集到绘制到玩家图层前的才绘制
                if (BeforePlayers.Count != 0)
                {
                    DrawToRenderTarget(BeforePlayerTarget, BeforePlayers);
                    BeginDrawBeforePlayers = true;// 打一个可以绘制出来玩家层的标记
                }
                // 收集到绘制到粒子图层前的才绘制
                if (BeforeDusts.Count != 0)
                {
                    DrawToRenderTarget(BeforeDustTarget, BeforeDusts);
                    BeginDrawBeforeDusts = true;// 打一个可以绘制出来粒子层的标记
                }
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
                BeginDrawProj = false;
            }
        }
        public static void DrawToRenderTarget(RenderTarget2D renderTarget, List<IPixelatedRenderer> pixelPrimitives)
        {
            renderTarget.SwapToTarget();
            if (pixelPrimitives.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
                foreach (var pixelPrimitiveDrawer in pixelPrimitives)
                    pixelPrimitiveDrawer.RenderPixelated(Main.spriteBatch);
                Main.spriteBatch.End();
            }
        }
        public static void DrawTarget_BeforePlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            // 只有当前面标记启用时才会尝试画出
            if (BeginDrawBeforePlayers)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = HJScarletShader.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(HJScarletMethods.ScreenSize() / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforePlayerTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforePlayers = false;
            }
            orig(self);
        }
        public static void DrawTarget_BeforeDust(On_Main.orig_DrawDust orig, Main self)
        {            // 只有当前面标记启用时才会尝试画出
            if (BeginDrawBeforeDusts)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = HJScarletShader.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(HJScarletMethods.ScreenSize() / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforeDustTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforeDusts = false;
            }
            orig(self);
        }
    }
}
