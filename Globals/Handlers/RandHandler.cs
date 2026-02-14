using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace HJScarletRework.Globals.Handlers
{
    public static class RandHandler
    {
        public static float RandRotTwoPi
        {
            get
            {
                return Main.rand.NextFloat(TwoPi);
            }
        }
        public static Vector2 RandDirTwoPi
        {
            get
            {
                return RandRotTwoPi.ToRotationVector2();
            }
        }
        public static Vector2 RandVelTwoPi(float minValue = 0f, float maxValue = 1f) => RandDirTwoPi * Main.rand.NextFloat(minValue, maxValue);
        public static Vector2 RandVelTwoPi(float maxValue = 1f) => RandDirTwoPi * Main.rand.NextFloat(maxValue);
        public static float RandZeroToOne => Main.rand.NextFloat(0f, 1f);
        public static Vector2 ToRandCirclePosEdge(this Vector2 pos, float valueX = 2f, float? valueY = null)
        {
            float edgeY = valueY ?? valueX;
            return pos + Main.rand.NextVector2CircularEdge(valueX, edgeY);
        }
        public static Vector2 ToRandCirclePos(this Vector2 pos, float valueX = 2f, float? valueY = null)
        {
            float edgeY = valueY ?? valueX;
            return pos + Main.rand.NextVector2Circular(valueX, edgeY);
        }
        public static int GetSeconds(int seconds) => seconds * 60;
        public static Color RandLerpColor(Color beginColor, Color endColor) => Color.Lerp(beginColor, endColor, RandZeroToOne);
        public static string GetVanillaAssetPath(VanillaAsset vanillaAsset, int id)=> $"Terraria/Images/{vanillaAsset}_{id}";
        public static Asset<Texture2D> GetVanillaAsset(VanillaAsset vanillaAsset, int id)=> Request<Texture2D>($"Terraria/Images/{vanillaAsset}_{id}");
    }
}
