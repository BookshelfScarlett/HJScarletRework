
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Core.ScreenEffect
{
    public class ScreenLerpSystem(Vector2 beginPos, Vector2 lerpPos, int lerpFrames, int lerpTime)
    {
        public Vector2 BeginPos = beginPos;
        public Vector2 LerpPos = lerpPos;
        public int LerpFrames = lerpFrames;
        public int LerpTime = lerpTime;
        public int CurLerpTime = 0;
        public void Update()
        {
            float lerpValue = Lerp(1f, 0f, EaseOutCubic(CurLerpTime / (float)LerpTime));
            Main.screenPosition = Vector2.Lerp(BeginPos, LerpPos, lerpValue);
            LerpTime++;
        }
    }
}
