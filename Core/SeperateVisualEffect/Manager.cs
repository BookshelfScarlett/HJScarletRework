using HJScarletRework.Globals.Database.Enums;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.SeperateVisualEffect
{
    /// <summary>
    /// 将视觉效果与部分实例分离出来的管理方案
    /// <br>对于一些极为复杂的特效，如果不想绑定在如射弹上导致射弹生成或消失时带来的违和感，就可以用上这个了</br>
    /// </summary>
    public class SeperateVisualManager:ModSystem
    {
        public const int MaxVisual = 500;
        public static bool AnyActiveVisual;
        public static List<SeperateVisualBehavior> VisualBehaviors = [];
        public static SeperateVisualInstance[] Visuals = new SeperateVisualInstance[MaxVisual];
        public override void Load()
        {
            for (int i = 0; i < MaxVisual; i++)
                Visuals[i] = new SeperateVisualInstance { WhoAmI = i, Active = false };
        }
        public override void Unload()
        {
            for (int i = 0; i < MaxVisual; i++)
                Visuals[i] = null;
        }
        public override void PostUpdateDusts()
        {
            if (!AnyActiveVisual)
                return;
            AnyActiveVisual = false;
            UpdateVisuals(Visuals);
        }
        public static void SeperateVisual_PreProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            if (!AnyActiveVisual)
            {
                orig(self);
                return;
            }
            DrawVisual(Visuals, BlendState.AlphaBlend, ScarletDrawLayer.BeforeProjectiles);
            DrawVisual(Visuals, BlendState.NonPremultiplied, ScarletDrawLayer.BeforeProjectiles);
            DrawVisual(Visuals, BlendState.Additive, ScarletDrawLayer.BeforeProjectiles);
            orig(self);

        }
        public static void SeperateVisual_PostDust(On_Main.orig_DrawDust orig, Main self)
        {
if (!AnyActiveVisual)
            {
                orig(self);
                return;
            }
            DrawVisual(Visuals, BlendState.AlphaBlend, ScarletDrawLayer.AfterDusts);
            DrawVisual(Visuals, BlendState.NonPremultiplied, ScarletDrawLayer.AfterDusts);
            DrawVisual(Visuals, BlendState.Additive, ScarletDrawLayer.AfterDusts);

            orig(self);
        }
        public static void UpdateVisuals(SeperateVisualInstance[] updatelist)
        {
            for (int i = 0; i < updatelist.Length; i++)
            {
                SeperateVisualInstance vfx = updatelist[i];
                if (!vfx.Active)
                    continue;
                AnyActiveVisual = true;
                SingleUpdate();
                if (vfx.ExtraUpdates!= 0)
                {
                    for (int a = 0; a < vfx.ExtraUpdates; a++)
                    {
                        if (vfx.ExtraUpdates== 0 || vfx.Time > vfx.LifeTime)
                            break;
                        SingleUpdate();
                    }
                }
                void SingleUpdate()
                {
                    vfx.Behavior.Update();
                    if (vfx.Behavior.ShouldUpdatePosition())
                        vfx.Position += vfx.Velocity;
                    vfx.Time++;
                }
                if (vfx.Time >= vfx.LifeTime)
                {
                    vfx.Behavior.OnDestroy();
                    vfx.Active = false;
                }
            }
        }
        public static void DrawVisual(SeperateVisualInstance[] updatelist, BlendState state, ScarletDrawLayer layer)
        {
            if (AnyActiveVisual)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, state, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < updatelist.Length; i++)
                {
                    SeperateVisualInstance vfx = updatelist[i];
                    if (vfx == null || !vfx.Active)
                        continue;
                    if (vfx.Behavior.BlendState == state && vfx.Behavior.DrawLayer == layer)
                    {
                        vfx.Behavior.Draw();
                    }
                }
                Main.spriteBatch.End();
            }
        }
    }
}
