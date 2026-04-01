using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace HJScarletRework.Core.Huds
{
    public class ExecutionHudManager :ModSystem
    {
        public static RenderTarget2D ExecutionTarget2D;
        public static float GeneralOpactiy = 0;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            LoadInit();
            Main.QueueMainThreadAction(() =>
            {
                ExecutionTarget2D = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
            On_Main.CheckMonoliths += RenderTarget;

        }
        public override void Unload()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                ExecutionTarget2D?.Dispose();
                ExecutionTarget2D = null;
            });
            On_Main.CheckMonoliths -= RenderTarget; 

        }
        public void LoadInit()
        {
            GeneralOpactiy = 0;
        }

        public static void RenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            if(Main.dedServ||Main.gameMenu)
            {
                orig();
                return;
            }
            orig();
            if (GeneralOpactiy == 0)
                return;
            HJScarletMethods.SwapToTarget(ExecutionTarget2D);
            //第一步：绘制底图

        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
        }
    }
}
