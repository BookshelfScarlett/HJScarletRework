using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.MetaballSystem
{
    public class MetaBallManager : ModSystem
    {
        public static List<BaseMetaBall> MetaBallCollection = [];

        #region 加载卸载
        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.CheckMonoliths += PrepareRenderTarget;
            // On_Main.DrawDust += DrawRenderTarget;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                // 卸载资源
                foreach (BaseMetaBall baseMetaBall in MetaBallCollection)
                {
                    baseMetaBall.AlphaTexture?.Dispose();
                    baseMetaBall.AlphaTexture = null;
                }
            });
            MetaBallCollection.Clear();

            On_Main.CheckMonoliths -= PrepareRenderTarget;
            // On_Main.DrawDust -= DrawRenderTarget;
        }
        #endregion

        #region 更新每一个元球
        public override void PostUpdateDusts()
        {
            foreach (BaseMetaBall baseMetaBall in MetaBallCollection)
            {
                if (!baseMetaBall.Active())
                    continue;

                baseMetaBall.Update();
            }
        }
        #endregion

        #region 进行离屏渲染
        public static void PrepareRenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            if (Main.dedServ)
            {
                orig();
                return;
            }

            if (Main.gameMenu)
            {
                orig();
                return;
            }

            orig();

            foreach (BaseMetaBall baseMetaBall in MetaBallCollection)
            {
                if (!baseMetaBall.Active())
                    continue;

                HJScarletMethods.SwapToTarget(baseMetaBall.AlphaTexture);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

                baseMetaBall.PrepareRenderTarget();

                Main.spriteBatch.End();

                Main.graphics.GraphicsDevice.SetRenderTargets(null);
            }
        }
        #endregion

        #region 最终输出渲染
        public static void DrawRenderTarget(On_Main.orig_DrawDust orig, Main self)
        {
            if (Main.dedServ)
            {
                orig(self);
                return;
            }

            orig(self);

            foreach (BaseMetaBall baseMetaBall in MetaBallCollection)
            {
                if (!baseMetaBall.Active())
                    continue;

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
              
                if (baseMetaBall.PreDrawRT2D())
                {
                    baseMetaBall.PrepareShader();
                    Main.spriteBatch.Draw(baseMetaBall.AlphaTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }

                Main.spriteBatch.End();
            }
        }
        #endregion
    }
}