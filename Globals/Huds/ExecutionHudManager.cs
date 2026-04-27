using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Configs;
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

namespace HJScarletRework.Globals.Huds
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
            On_Main.DrawDust += On_Main_DrawDust;
            On_Main.CheckMonoliths += RenderTarget;
            LoadInit();

        }

        private void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
        {
            //shorthand
            Player localPlayer = Main.LocalPlayer;
            HJScarletConfigClient config = HJScarletConfigClient.Instance;
            SpriteBatch SB = Main.spriteBatch;
            bool noPredraw = Main.dedServ || Main.gameMenu;
            orig(self);
            if (noPredraw)
                return;
            //如果有人想要绘制。
            if (!config.DrawExecutionCounter)
                return;
            //手持情况
            //可见度为零时时停止更新
            if (GeneralOpacity <= 0f && !localPlayer.HJScarlet().Executor_DrawFadeIn)
                return;
            //HJScarletMethods.SwapToTarget(ExecutionTarget2D);

            //下面的底图似乎因为在某些地方没有被end导致无法正常绘制
            //所以这里直接开了一个空的批次尝试
            //不使用endd是因为会报错，说没有begin
            //怎么底图还是没有?
            //第一步：绘制底图
            Vector2 pos = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) + new Vector2(0, 50);
            Texture2D t2d = HJScarletTexture.Hud_ExecutorCounter.Value;
            SB.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SB.Draw(t2d, pos, null, Color.Red * GeneralOpacity, 0f, t2d.ToOrigin(), 1f, SpriteEffects.None, 0f);
            //SB.Draw(t2d, pos , null, Color.White* GeneralOpacity, 0f, t2d.ToOrigin(), 1f, SpriteEffects.None, 0f);
            //字体。
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = ToRadians(60 * i).ToRotationVector2() * 1.2f;
                DrawNum(pos + offset + Vector2.UnitX.RotatedBy(PiOver4) * 3.5f, Color.Lerp(Color.Red, Color.Black, .95f) * 0.248f);
                DrawNum(pos + offset, Color.Black);
            }
            DrawNum(pos, Color.White);
            void DrawNum(Vector2 pos, Color color)
            {
                DynamicSpriteFont font = HJScarletTexture.Font_MGR.Value;
                Vector2 scale = new(0.60f);
                float offsetValue = 7.30f;
                if (CurExecutionCounter < 10)
                    offsetValue = 5.00f;
                if (CurExecutionCounter > 99 && CurExecutionCounter < 1000)
                    offsetValue = 15.00f;
                if (CurExecutionCounter > 999 && CurExecutionCounter < 10000)
                    offsetValue = 21.00f;

                Vector2 size = ChatManager.GetStringSize(font, CurExecutionCounter.ToString(), Vector2.One * scale);
                ChatManager.DrawColorCodedString(SB, font, CurExecutionCounter.ToString(), pos - Vector2.UnitX * offsetValue, color * GeneralOpacity, 0, size / 2, Vector2.One * scale);
            }
            SB.End();
            //Main.graphics.GraphicsDevice.SetRenderTargets(null);

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
            On_Main.DrawDust -= On_Main_DrawDust;

        }
        public override void OnWorldLoad()
        {
            LoadInit();
        }
        public static void LoadInit()
        {
            GeneralOpactiy = 0;
        }


        public static void RenderTarget(On_Main.orig_CheckMonoliths orig)
        {
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
            if (GeneralOpacity <= 0f && !ModPlayer.Executor_DrawFadeIn)
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
                if (GeneralOpacity >= 0.98f)
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
