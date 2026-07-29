using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Core.Lightning
{
    public partial class LightningBuilder
    {
        public static List<TrailDrawDate> trailDrawData = [];
        private static List<Vector2> _drawPosBuffer = [];
        private static List<float> _drawWidthBuffer = [];
        public static void DrawLightning(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (HasAnyLightning)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                DrawSetting setting = new(HJScarletTexture.Texture_WhiteCubeBig.Value, SamplerState.PointWrap);
                for (int i = 0; i < HJLightnings.Length; i++)
                {
                    if (HJLightnings[i].Active)
                    {
                        HJLightning lightning = HJLightnings[i];
                        Draw(lightning.CachedTrails, lightning.Color * lightning.Opacity, 1f, setting);
                    }
                }
                Main.spriteBatch.End();
            }
        }
        public static void Draw(List<List<TrailDrawDate>> OldPos, Color color, float widthMult, DrawSetting setting)
        {
            foreach (List<TrailDrawDate> list in OldPos)
            {
                trailDrawData.Clear();
                foreach (TrailDrawDate data in list)
                {
                    trailDrawData.Add(new TrailDrawDate(data.PosDate, color, data.PrimitivesOffset * widthMult, data.PrimitivesHeightRot));
                }
                TrailRender.RenderTrail(trailDrawData.ToArray(), setting);
            }
        }
    }
}
