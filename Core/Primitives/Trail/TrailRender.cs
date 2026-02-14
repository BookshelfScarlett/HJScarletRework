using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Core.Primitives.Trail
{
    public class TrailRender
    {

        public static void RenderTrail(TrailDrawDate[] drawDate, DrawSetting drawSetting)
        {
            DrawTrail(drawDate, drawSetting);
        }

        public static void DrawTrail(TrailDrawDate[] DrawDate, DrawSetting drawSetting)
        {
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();

            for (int i = 0; i < DrawDate.Length; i++)
            {
                float progress = (float)i / DrawDate.Length;
                // 绘制位置
                Vector2 DrawPos = DrawDate[i].PosDate - (drawSetting.usePosTransformation ? Main.screenPosition : Vector2.Zero);

                if (drawSetting.usePixelTransformation)
                    DrawPos = DrawPos / 2;

                // 每个片的高度与旋转
                Vector2 PrimitivesHeight = DrawDate[i].PrimitivesOffset;
                float PrimitivesHeightRot = DrawDate[i].PrimitivesHeightRot;
                Color DrawColor = DrawDate[i].DrawColor;

                Vertexlist.Add(new ScarletVertex(DrawPos - PrimitivesHeight.RotatedBy(PrimitivesHeightRot), DrawColor, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new ScarletVertex(DrawPos + PrimitivesHeight.RotatedBy(PrimitivesHeightRot), DrawColor, new Vector3(progress, 1, 0)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = drawSetting.texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
