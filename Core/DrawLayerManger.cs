using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.MetaballSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Core.SeperateVisualEffect;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Globals.Systems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace HJScarletRework.Core
{
    public class DrawLayerManger : ModSystem
    {
        public override void Load()
        {
            //屏幕暗化效果
            On_Main.DrawBackground += ScreenDarknessSystem.DrawScreenDarkness;
            //Metaball层级，可以考虑直接分离出去
            On_Main.DrawDust += MetaballManager.DrawRenderTarget;
            //ECS粒子
            On_Main.DrawDust += ECSParticleDataManager.DrawParticle_ECS;
            //使用类射弹的实例化粒子
            On_Main.DrawDust += BaseParticleManager.DrawParticles;
            //缩放遮罩
            On_Main.DrawDust += ScreenZoomSystem.DrawCameraMask;
            On_Main.DrawProjectiles += SeperateVisualManager.SeperateVisual_PreProjectiles;

            On_Main.DrawDust += SeperateVisualManager.SeperateVisual_PostDust;
            //未使用，待删除
            On_Main.DrawPlayers_BehindNPCs += MetaballManager.DrawRenderTargetPiority;
            //未使用，待删除
            On_Main.DrawProjectiles += PixelatedRenderManager.On_Main_DrawProjectiles;
            //像素化渲染
            On_Main.DrawDust += PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles += PixelatedRenderManager.DrawTarget_BeforePlayers;
            //指针绘制
            On_Main.DrawInterface_36_Cursor += HJScarletCustomCursor.On_Main_DrawInterface_36_Cursor;
            //DeepGlow
            On_Main.DrawProjectiles += DeepGlow.Hook_AfterProjectile;
            On_Main.DoDraw_Tiles_Solid += DeepGlow.Hook_BeforeTile;
            On_Main.DrawDust += DeepGlow.Hook_AfterDust;
            On_FilterManager.EndCapture += DeepGlow.DrawDeepGlow;
        }

        public override void Unload()
        {
            //屏幕暗化效果
            On_Main.DrawBackground -= ScreenDarknessSystem.DrawScreenDarkness;
            //Metaball层级，可以考虑直接分离出去
            On_Main.DrawDust -= MetaballManager.DrawRenderTarget;
            //ECS粒子
            On_Main.DrawDust -= ECSParticleDataManager.DrawParticle_ECS;
            //使用类射弹的实例化粒子
            On_Main.DrawDust -= BaseParticleManager.DrawParticles;
            //缩放遮罩
            On_Main.DrawDust -= ScreenZoomSystem.DrawCameraMask;
            On_Main.DrawProjectiles -= SeperateVisualManager.SeperateVisual_PreProjectiles;
            On_Main.DrawDust -= SeperateVisualManager.SeperateVisual_PostDust;
            //待删除
            On_Main.DrawPlayers_BehindNPCs -= MetaballManager.DrawRenderTargetPiority;
            //待删除
            On_Main.DrawProjectiles -= PixelatedRenderManager.On_Main_DrawProjectiles;
            //像素化渲染
            On_Main.DrawDust -= PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles -= PixelatedRenderManager.DrawTarget_BeforePlayers;
            //指针绘制
            On_Main.DrawInterface_36_Cursor -= HJScarletCustomCursor.On_Main_DrawInterface_36_Cursor;
            //DeepGlow
            On_Main.DrawProjectiles -= DeepGlow.Hook_AfterProjectile;
            On_Main.DoDraw_Tiles_Solid -= DeepGlow.Hook_BeforeTile;
            On_Main.DrawDust -= DeepGlow.Hook_AfterDust;
            On_FilterManager.EndCapture -= DeepGlow.DrawDeepGlow;
        }
    }
}
