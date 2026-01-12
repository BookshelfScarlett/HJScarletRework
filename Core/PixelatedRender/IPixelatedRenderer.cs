using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace HJScarletRework.Core.PixelatedRender
{
    /// <summary>
    /// 只支持BeforePlayers与BeforeDusts图层
    /// </summary>
    public interface IPixelatedRenderer
    {
        HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        void RenderPixelated(SpriteBatch spriteBatch);
    }
}
