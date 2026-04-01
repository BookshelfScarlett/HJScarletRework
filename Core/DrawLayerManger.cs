using HJScarletRework.Core.MetaballSystem;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.ParticleSystem;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core
{
    public class DrawLayerManger : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawDust += MetaballManager.DrawRenderTarget;
            On_Main.DrawDust += BaseParticleManager.DrawParticles;
            On_Main.DrawPlayers_BehindNPCs += MetaballManager.DrawRenderTargetPiority;
            On_Main.DrawProjectiles += PixelatedRenderManager.On_Main_DrawProjectiles;
            On_Main.DrawDust += PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles += PixelatedRenderManager.DrawTarget_BeforePlayers;
        }

        public override void Unload()
        {
            On_Main.DrawDust -= MetaballManager.DrawRenderTarget;
            On_Main.DrawDust -= BaseParticleManager.DrawParticles;
            On_Main.DrawPlayers_BehindNPCs -= MetaballManager.DrawRenderTargetPiority;
            On_Main.DrawProjectiles -= PixelatedRenderManager.On_Main_DrawProjectiles;
            On_Main.DrawDust -= PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles -= PixelatedRenderManager.DrawTarget_BeforePlayers;
        }
    }
}
