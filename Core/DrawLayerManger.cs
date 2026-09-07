using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.MetaballSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.ParticleSystem;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace HJScarletRework.Core
{
    public class DrawLayerManger : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawBackground += ScreenDarknessSystem.DrawScreenDarkness;
            On_Main.DrawDust += MetaballManager.DrawRenderTarget;
            On_Main.DrawDust += BaseParticleManager.DrawParticles;
            On_Main.DrawDust += ECSParticleDataManager.DrawParticle_ECS;
            On_Main.DrawPlayers_BehindNPCs += MetaballManager.DrawRenderTargetPiority;
            On_Main.DrawProjectiles += PixelatedRenderManager.On_Main_DrawProjectiles;
            On_Main.DrawDust += PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles += PixelatedRenderManager.DrawTarget_BeforePlayers;
            On_FilterManager.EndCapture += DeepGlow.DrawDeepGlow;
        }
        public override void Unload()
        {
            On_Main.DrawBackground -= ScreenDarknessSystem.DrawScreenDarkness;
            On_Main.DrawDust -= MetaballManager.DrawRenderTarget;
            On_Main.DrawDust -= BaseParticleManager.DrawParticles;
            On_Main.DrawDust -= ECSParticleDataManager.DrawParticle_ECS;
            On_Main.DrawPlayers_BehindNPCs -= MetaballManager.DrawRenderTargetPiority;
            On_Main.DrawProjectiles -= PixelatedRenderManager.On_Main_DrawProjectiles;
            On_Main.DrawDust -= PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles -= PixelatedRenderManager.DrawTarget_BeforePlayers;
            On_FilterManager.EndCapture -= DeepGlow.DrawDeepGlow;
        }
    }
}
