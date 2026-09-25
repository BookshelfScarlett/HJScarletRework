using HJScarletRework.Globals.Database;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.RenderTargetsManager
{
    public class RenderTarget2DManager : ModSystem
    {
        public static Vector2 OldScreenSize;
        public static List<RenderTarget2D> RT2D_ScreenSize = [];
        public static List<RenderTarget2D> RT2D_Normal = [];
        public override void Load()
        {
        }
        public override void Unload()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                for (int i = 0; i < RT2D_ScreenSize.Count; i++)
                {
                    RT2D_ScreenSize[i]?.Dispose();
                    RT2D_ScreenSize[i] = null;
                }
                RT2D_ScreenSize.Clear();
                for (int i = 0; i < RT2D_Normal.Count; i++)
                {
                    RT2D_Normal[i]?.Dispose();
                    RT2D_Normal[i] = null;
                }
                RT2D_Normal.Clear();
            });
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.dedServ)
                return;
            if (OldScreenSize != ScarletDatabase.ScreenSize)
            {
                Main.QueueMainThreadAction(() =>
                {
                    for (int i = 0; i < RT2D_ScreenSize.Count; i++)
                    {
                        if (RT2D_ScreenSize[i] != null && !RT2D_ScreenSize[i].IsDisposed)
                        {
                            RT2D_ScreenSize[i].Dispose();
                        }
                        RT2D_ScreenSize[i] = HJScarletMethods.NewRT2D();
                    }
                });
            }
            OldScreenSize = ScarletDatabase.ScreenSize;
        }
        public static void RequestScreenSizeRT2D(out int Index)
        {
            Index = RT2D_ScreenSize.Count;
            RT2D_ScreenSize.Add(null);// 占位
            int capturedIndex = Index; // 捕获索引
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                if (capturedIndex < RT2D_ScreenSize.Count)
                {
                    RT2D_ScreenSize[capturedIndex] = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.ScreenSize.X, Main.ScreenSize.Y);
                }
            });
        }
        public static void RequestRT2D(Vector2 Size, out int Index)
        {
            Index = RT2D_Normal.Count;
            RT2D_Normal.Add(null);// 占位
            int capturedIndex = Index; // 捕获索引
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                if (capturedIndex < RT2D_Normal.Count)
                {
                    RT2D_Normal[capturedIndex] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
                }
            });
        }
        public static RenderTarget2D GetScreenRT2D(int index)
        {
            if (RT2D_ScreenSize.IndexInRange(index))
                return RT2D_ScreenSize[index];
            else
                return null;
        }
    }
}
