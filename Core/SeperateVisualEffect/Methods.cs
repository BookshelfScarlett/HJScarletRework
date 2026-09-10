using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Core.SeperateVisualEffect
{
    public static class SeperateVisualMethods
    {
        /// <summary>
        /// 摧毁当前的分离视效
        /// </summary>
        public static void Destroy(this SeperateVisualInstance instance)
        {
            instance.Time = instance.LifeTime;
            instance.Active = false;
            instance.Behavior.OnDestroy();
        }
        public static SeperateVisualInstance NewVisual(int Type, Vector2 position, Vector2 velocity, Color drawColor, float rotation = 0, float scale = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            for (int i = 0; i < SeperateVisualManager.MaxVisual; i++)
            {
                SeperateVisualInstance vfx = SeperateVisualManager.Visuals[i];
                if (vfx != null && !vfx.Active)
                {
                    vfx.Reset();
                    SeperateVisualBehavior vfxBeh = SeperateVisualManager.VisualBehaviors[Type];
                    vfx.Behavior = vfxBeh.CloneForSpawner();
                    vfx.Behavior.Instance = SeperateVisualManager.Visuals[i];

                    vfx.Position = position;
                    vfx.Velocity = velocity;
                    vfx.Time = 0;
                    vfx.ExtraUpdates = 0;
                    vfx.Scale = scale;
                    vfx.ScaleVector2 = Vector2.One * scale;
                    vfx.Opacity = 1f;
                    vfx.DrawColor = drawColor;
                    vfx.Rotation = rotation;
                    vfx.OldPos.Clear();
                    vfx.OldRot.Clear();
                    vfx.Active = true;
                    vfx.aifloats[0] = ai0;
                    vfx.aifloats[1] = ai1;
                    vfx.aifloats[2] = ai2;

                    SeperateVisualManager.AnyActiveVisual = true;

                    vfx.Behavior.OnSpawn();
                    return vfx;
                }
            }
            return null;
        }
    }
}
