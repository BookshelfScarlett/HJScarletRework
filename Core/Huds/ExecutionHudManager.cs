using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Configs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace HJScarletRework.Core.Huds
{
    public class ExecutionHudManager : ModSystem
    {
        public static RenderTarget2D ExecutionTarget2D;
        public static float GeneralOpactiy = 0;
        public static float GeneralOffset = 0;
        public static bool DrawFadeOut;
        public static bool DrawFadeIn;
        /// <summary>
        /// Shorthand.
        /// </summary>
        public Player LocalPlayer => Main.LocalPlayer;
        public HJScarletPlayer ModPlayer => LocalPlayer.HJScarlet();
        public static bool FadeInCounter = false;
        public static float GeneralOpacity = 0;
        public static ExecutorBackground Background = new ExecutorBackground();
        public static Vector2 TargetSize = new Vector2(600, 600);
        /// <summary>
        /// 仅仅用于计时
        /// </summary>
        public static int CurExecutionCounter = -1;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                ExecutionTarget2D = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)TargetSize.X, (int)TargetSize.Y);
            });
            On_Main.CheckMonoliths += RenderTarget;
            LoadInit();

        }
        public override void Unload()
        {
            Background = null;
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                ExecutionTarget2D?.Dispose();
                ExecutionTarget2D = null;
            });
            On_Main.CheckMonoliths -= RenderTarget; 

        }
        public override void OnWorldLoad()
        {
            LoadInit();
        }
        public void LoadInit()
        {
            Background.Orig = Background.Texture.Size()/2f;
            Background.Center = TargetSize / 2f + new Vector2(0, 50);
            GeneralOpactiy = 0;
        }

        public static void RenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            //shorthand
            Player localPlayer = Main.LocalPlayer;
            HJScarletConfigClient config = HJScarletConfigClient.Instance;
            SpriteBatch SB = Main.spriteBatch;
            bool noPredraw = Main.dedServ || Main.gameMenu;
            orig();
            if (noPredraw)
                return;
            //如果有人想要绘制。
            if (!config.DrawExecutionCounter)
                return;
            //手持情况
            //可见度为零时时停止更新
            if (GeneralOpacity <= 0f && !localPlayer.HJScarlet().Executor_DrawFadeIn)
                return;
            HJScarletMethods.SwapToTarget(ExecutionTarget2D);
            //第一步：绘制底图
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SB.Draw(Background.Texture, Background.Center , null, Color.DarkRed * GeneralOpacity, 0, Background.Orig, 1f, 0, 0);
            SB.Draw(Background.Texture, Background.Center , null, Color.Red * GeneralOpacity, 0, Background.Orig, 1f, 0, 0);
            //字体。
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = ToRadians(60 * i).ToRotationVector2() * 1.2f;
                DrawNum(Background.Center + offset + Vector2.UnitX.RotatedBy(PiOver4) * 3.5f , Color.Lerp(Color.Red,Color.Black,.95f) * 0.248f);
                DrawNum(Background.Center + offset, Color.Black);
            }
            DrawNum(Background.Center,Color.White);
            void DrawNum(Vector2 pos, Color color)
            {
                DynamicSpriteFont font = HJScarletTexture.Font_MGR.Value;
                Vector2 scale = new(0.60f);
                float offsetValue = 7.30f;
                if (CurExecutionCounter < 10)
                    offsetValue = 5.00f;
                if (CurExecutionCounter > 99 && CurExecutionCounter < 1000 )
                    offsetValue = 15.00f;
                if (CurExecutionCounter > 999 && CurExecutionCounter < 10000)
                    offsetValue = 21.00f;

                Vector2 size = ChatManager.GetStringSize(font, CurExecutionCounter.ToString(), Vector2.One * scale);
                ChatManager.DrawColorCodedString(SB, font, CurExecutionCounter.ToString(), pos - Vector2.UnitX * offsetValue, color * GeneralOpacity, 0, size/2, Vector2.One * scale);
            }
            SB.End();
            Main.graphics.GraphicsDevice.SetRenderTargets(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            HJScarletConfigClient config = HJScarletConfigClient.Instance;
            if (Main.dedServ)
                return;
            if (!config.DrawExecutionCounter)
                return;

            if (LocalPlayer.HeldItem.DamageType != ExecutorDamageClass.Instance)
            {
                ModPlayer.Executor_DrawFadeOut = true;
                ModPlayer.Executor_DrawFadeIn = false;
            }
            else
            {
                ModPlayer.Executor_DrawFadeIn = true;
                ModPlayer.Executor_DrawFadeOut = false;
            }
            //可见度为0的时候停止下方的更新
            if (GeneralOpacity <= 0f && ModPlayer.Executor_DrawFadeIn== false)
                return;
            int heldItem = LocalPlayer.HeldItem.type;
            if (ModPlayer.Executor_DrawFadeOut)
            {
                GeneralOpacity = Lerp(GeneralOpacity, 0f, 0.12f);
                GeneralOffset = Lerp(GeneralOffset, -5f, 0.2f);
                if (GeneralOpacity <= 0.02f)
                {
                    GeneralOpacity = 0f;
                    GeneralOffset = 0f;
                    ModPlayer.Executor_DrawFadeOut = false;
                }
            }
            if (ModPlayer.Executor_DrawFadeIn)
            {
                GeneralOpacity = Lerp(GeneralOpacity, 1f, 0.2f);
                GeneralOffset = Lerp(GeneralOffset, 0f, 0.2f);
                if(GeneralOpacity >= 0.98f)
                {
                    GeneralOpacity = 1f;
                    GeneralOffset = 0f;
                }
            }
            //发射的一瞬间会从字典移除掉对应的值
            //如果最开始的值就不存在，设置为0
            if (ModPlayer.ExecutionListStored.TryGetValue(heldItem, out int num))
            {
                CurExecutionCounter = num;
            }
            else
                CurExecutionCounter = 0;
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.dedServ)
                return;
            if (!HJScarletConfigClient.Instance.DrawExecutionCounter)
                return;
            if (LocalPlayer.HeldItem.DamageType != ExecutorDamageClass.Instance)
                return;
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("HJScarlet: ExecutorBar", delegate ()
                {
                    Main.spriteBatch.Draw(ExecutionTarget2D, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White, 0, ExecutionTarget2D.Size() / 2, 1f, 0, 0f);
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
