using HJScarletRework.Globals.Configs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace HJScarletRework.Core.ScreenEffect
{
    public class ScreenDarknessInfo(float darkStrength, int inTime, int holdTime, int outTime, Func<float, float> fadeInFunc = null, Func<float, float> fadeOutFunc = null)
    {
        /// <summary>
        /// 屏幕暗化强度
        /// </summary>
        public float DarkStrength = darkStrength;
        /// <summary>
        /// 屏幕暗化渐入时间
        /// </summary>
        public int InTime = inTime;
        /// <summary>
        /// 屏幕暗化淡出时间
        /// </summary>
        public int OutTime = outTime;
        /// <summary>
        /// 屏幕暗化持续时间
        /// </summary>
        public int HoldTime = holdTime;
        /// <summary>
        /// 屏幕暗化总时间
        /// </summary>
        public float TotalDarkTime = inTime + outTime + holdTime;
        public float DarkTimer = 0;
        /// <summary>
        /// 屏幕暗化的渐入曲线
        /// <br>默认<see cref="EaseOutCubic(float)"/></br>
        /// </summary>
        public Func<float, float> FadeInFunc = fadeInFunc ?? EaseOutCubic;
        /// <summary>
        /// 屏幕暗化的淡出曲线
        /// <br>默认<see cref="EaseOutCubic(float)"/></br>
        /// </summary>
        public Func<float, float> FadeOutFunc = fadeOutFunc ?? EaseOutCubic;
        public bool IsDone => DarkTimer >= TotalDarkTime;
        public float Update()
        {
            DarkTimer++;
            if (DarkTimer <= InTime)
            {
                float t = (float)DarkTimer / InTime;
                return FadeInFunc(t);
            }
            else if (DarkTimer <= InTime + HoldTime)
            {
                return 1f;
            }
            else if (DarkTimer <= TotalDarkTime)
            {
                float t = (float)(DarkTimer - InTime - HoldTime) / OutTime;
                return (1f - FadeOutFunc(t));
            }
            return 0f;
        }
    }
    public class ScreenDarknessSystem : ModSystem
    {
        public static readonly List<ScreenDarknessInfo> ActiveDarkness = [];
        public static void DrawScreenDarkness(On_Main.orig_DrawBackground orig, Main self)
        {
            orig(self);
            if (ActiveDarkness.Count < 1)
                return;
            if (Main.dedServ)
                return;
            float darkRatio = 0f;
            for (int i = ActiveDarkness.Count - 1; i >= 0; i--)
            {
                ScreenDarknessInfo info = ActiveDarkness[i];
                float ratio = info.Update() * info.DarkStrength;
                //多个暗化效果取当前的最大值
                if (ratio > darkRatio)
                    darkRatio = ratio;
                if (info.IsDone)
                    ActiveDarkness.RemoveAt(i);
            }
            //最后画出来
            Vector2 pos = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
            Rectangle rec = Utils.CenteredRectangle(pos, new Vector2(Main.screenWidth, Main.screenHeight));
            Texture2D tex = TextureAssets.MagicPixel.Value;
            darkRatio *= HJScarletConfigClient.Instance.ScreenDarkStrength;
            Main.spriteBatch.Draw(tex, pos, rec, Color.Lerp(Color.DarkGray, Color.Black, 0.95f) with { A = 250 } * darkRatio, 0, tex.Size() / 2, new Vector2(Main.screenWidth, Main.screenHeight / 2), 0, 0);
        }
        public static void AddScreenDarkness(float maxStrength, int inTime, int holdTime, int outTime, Func<float, float> easeIn = null, Func<float, float> easeOut = null)
        {
            ActiveDarkness.Add(new ScreenDarknessInfo(maxStrength, inTime, holdTime, outTime, easeIn, easeOut));
        }
        public static void AddScreenDarkness(float maxStrength, int holdTime, Func<float, float> easeIn = null, Func<float, float> easeOut = null)
        {
            int inTime = (int)(holdTime * .1f);
            int outTime = (int)(holdTime * .9f);
            ActiveDarkness.Add(new ScreenDarknessInfo(maxStrength, inTime, holdTime, outTime, easeIn, easeOut));
        }

    }
}
