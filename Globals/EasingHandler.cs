using System;

namespace HJScarletRework.Globals
{
    public static class EasingHandler
    {
        public static float EaseInOutQuad(float t)
        {
            if (!(t < 0.5f))
            {
                return 1f - (-2f * t + 2f) * (-2f * t + 2f) / 2f;
            }

            return 2f * t * t;
        }

        public static float EaseOutExpo(float t)
        {
            if (t != 1f)
            {
                return 1f - MathF.Pow(2f, -10f * t);
            }

            return 1f;
        }

        public static float EaseInOutExpo(float t)
        {
            if (!(t < 0.5f))
            {
                return 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;
            }

            return 2f * t * t;
        }

        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        public static float EaseOutCubic(float t)
        {
            return (float)(1.0 - Math.Pow(1f - t, 3.0));
        }

        public static float EaseOutBack(float t)
        {
            if (t == 1f)
            {
                return 1f;
            }

            return (float)(1.0 + 2.7015800476074219 * Math.Pow(t - 1f, 3.0) + 1.7015800476074219 * Math.Pow(t - 1f, 2.0));
        }

        public static float EaseInBack(float t)
        {
            if (t == 1f)
            {
                return 1f;
            }

            return 2.70158f * t * t * t - 1.70158f * t * t;
        }

        public static float EaseInOutSin(float t)
        {
            return (float)Math.Sin(MathF.PI * t);
        }
    }
}
