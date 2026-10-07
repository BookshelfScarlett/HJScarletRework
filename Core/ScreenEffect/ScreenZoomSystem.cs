using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace HJScarletRework.Core.ScreenEffect
{
    /// <summary>
    /// Credit from Luminance
    /// </summary>
    public class ScreenZoomSystem : ModSystem
    {
        public static Vector2 ZoomPosition { get; set; }
        /// <summary>
        /// 位置的移动幅度，或者说归一化比率
        /// <br>值在0~1Clamp</br>
        /// </summary>
        public static float CameraMoveLerp { get; set; }
        /// <summary>
        /// 缩放的幅度
        /// <br>接收的是-1->1的值，如果传入<0，则表示扩大</br>
        /// </summary>
        public static float ZoomValue { get; set; }
        public static Texture2D MaskTexture => HJScarletTexture.Texture_ScreenMask.Value;
        /// <summary>
        /// 将镜头移动到指定位置，并使用lerp来限制其移动幅度
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="lerp"></param>
        public static void MoveCameraTo(Vector2 destination, float lerp)
        {
            ZoomPosition = destination;
            CameraMoveLerp = lerp;
        }
        /// <summary>
        /// 输入一个比率缩放。
        /// </summary>
        /// <param name="zoom"></param>
        public static void ZoomIn(float zoom)
        {
            if (zoom < 0f)
                zoom = 0;
            ZoomValue = zoom;
        }
        public static void ZoomOut(float zoom)
        {
            if (zoom < 0)
                zoom = 0;
            ZoomValue = 1f / (zoom + 1f) - 1;
        }
        public static bool UseTexture = false;
        public override void ModifyScreenPosition()
        {
            if (Main.LocalPlayer.dead && !Main.gamePaused)
            {
                return;
            }
            if (CameraMoveLerp > 0f)
            {
                Vector2 targetScreenPos = ZoomPosition - HJScarletMethods.GetScreenSize / 2f;
                Main.screenPosition = Vector2.Lerp(Main.screenPosition, targetScreenPos, CameraMoveLerp);
            }
        }
        public override void PreUpdateEntities()
        {
            if (Main.LocalPlayer.dead && !Main.gamePaused)
            {
                ZoomValue = Lerp(ZoomValue, 0f, .13f);
                CameraMoveLerp = 0f;
                return;
            }
            if (!Main.gamePaused)
            {
                CameraMoveLerp = Clamp(CameraMoveLerp, 0f, 1f);
                ZoomValue = Lerp(ZoomValue, 0, 0.09f);
            }
            base.PreUpdateEntities();
        }
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            Transform.Zoom *= 1f + ZoomValue;
        }
        public static void DrawCameraMask(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (!UseTexture)
                return;
                        float v = (Math.Abs(ZoomValue - 1f));
            float cV = Utils.GetLerpValue(1f, 0f, v, true);
            if (cV <= .02f)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //用纹理原始宽度计算缩放，使绘制宽度恰好等于 desiredWidth
            Main.spriteBatch.Draw(MaskTexture, Main.LocalPlayer.Center - Main.screenPosition, null, Color.White*(cV), 0, MaskTexture.Size() / 2f, new Vector2(1)*Lerp(.3f,1f,v), 0, 0);
            Main.spriteBatch.Draw(MaskTexture, Main.LocalPlayer.Center - Main.screenPosition, null, Color.White*(cV), 0, MaskTexture.Size() / 2f, new Vector2(1)*Lerp(.3f,1f,v), 0, 0);
            Main.spriteBatch.End();
        }
    }
}
