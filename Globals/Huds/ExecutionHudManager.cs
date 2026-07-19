using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
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
        public static int StopCounter = 0;
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
            LoadInit();

        }

        private void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (Main.dedServ || Main.gameMenu)
                return;
            HJScarletConfigClient config = HJScarletConfigClient.Instance;
            if (!config.DrawExecutionCounter)
                return;
            Player localPlayer = Main.LocalPlayer;
            if (GeneralOpacity <= 0f && !localPlayer.HJScarlet().Executor_DrawFadeIn)
                return;
            SpriteBatch SB = Main.spriteBatch;
            Vector2 pos = LocalPlayer.Center + new Vector2(0, 50) - Main.screenPosition;
            Texture2D t2d = HJScarletTexture.Hud_ExecutorCounter.Value;

            SB.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            SB.Draw(t2d, pos, null, Color.Red * GeneralOpacity, 0f, t2d.ToOrigin(), 1f, SpriteEffects.None, 0f);
            DrawNumberWithEffect(SB, pos, CurExecutionCounter, GeneralOpacity);

            SB.End();

        }
        private void DrawNumberWithEffect(SpriteBatch sb, Vector2 basePos, int number, float opa)
        {
            string numStr = number.ToString();
            DynamicSpriteFont font = HJScarletTexture.Font_MGR.Value;
            Vector2 scale = new Vector2(.6f);
            float offsetX = GetNumberOffsetX(number);
            Vector2 size = ChatManager.GetStringSize(font, numStr, scale);
            Vector2 textPos = basePos - new Vector2(offsetX, 0);

            Color shadowColor1 = Color.Lerp(Color.Red, Color.Black, 0.95f) * (opa * 0.248f);

            Color shadowColor2 = Color.Black * opa;
            Color mainColor = Color.White * opa;
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = (TwoPi * i / 8f).ToRotationVector2() * 1.2f;

                //第一层阴影
                Vector2 pos1 = textPos + offset + new Vector2(3.5f, 3.5f);
                ChatManager.DrawColorCodedString(sb, font, numStr, pos1, shadowColor1, 0f, size * 0.5f, scale);

                //第二层阴影
                Vector2 pos2 = textPos + offset;
                ChatManager.DrawColorCodedString(sb, font, numStr, pos2, shadowColor2, 0f, size * 0.5f, scale);
            }

            //中心白色文字
            ChatManager.DrawColorCodedString(sb, font, numStr, textPos, mainColor, 0f, size * 0.5f, scale);

        }
        private float GetNumberOffsetX(int number)
        {
            if (number < 10)
                return 5f;
            if (number < 100)
                return 7.30f;
            if (number < 1000)
                return 15f;
            if (number < 10000)
                return 21f;
            return 21f;
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
        public bool StopNow = false;
        public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
        {
            base.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            HJScarletConfigClient config = HJScarletConfigClient.Instance;
            if (Main.dedServ)
                return;
            if (!config.DrawExecutionCounter)
                return;
            Player localPlayer = Main.LocalPlayer;
            Item heldItem = localPlayer.HeldItem;
            bool hasReforged = ModLoader.HasMod("ExpandedReforge");
            bool isExectuorWeapon = heldItem.DamageType.CountsAsClass<ExecutorDamageClass>();
            isExectuorWeapon = HJScarletList.ExecutorWeaponDictionary.ContainsKey(heldItem.type);
            if (isExectuorWeapon)
            {
                if (!ModPlayer.Executor_DrawFadeIn)
                {
                    ModPlayer.Executor_DrawFadeOut = false;
                    ModPlayer.Executor_DrawFadeIn = true;
                }
            }
            else
            {
                if (!ModPlayer.Executor_DrawFadeOut)
                {
                    ModPlayer.Executor_DrawFadeOut = true;
                    ModPlayer.Executor_DrawFadeIn = false;
                }
            }
            if (ModPlayer.Executor_DrawFadeOut)
            {
                GeneralOpacity = Lerp(GeneralOpacity, 0, 0.12f);
                GeneralOffset = Lerp(GeneralOffset, -5f, 0.2f);
                if (GeneralOpacity <= .02f)
                {
                    GeneralOpacity = 0f;
                    GeneralOffset = 0f;
                    ModPlayer.Executor_DrawFadeOut = false;
                }
            }
            else if (ModPlayer.Executor_DrawFadeIn)
            {
                GeneralOpacity = Lerp(GeneralOpacity, 1f, 0.2f);
                GeneralOffset = Lerp(GeneralOffset, 0f, 0.2f);
                if (GeneralOpacity >= 0.98f)
                {
                    GeneralOpacity = 1f;
                    GeneralOffset = 0f;
                }
            }
            //可见度为0的时候停止下方的更新
            if (GeneralOpacity <= 0f && !ModPlayer.Executor_DrawFadeIn)
                return;

            //发射的一瞬间会从字典移除掉对应的值
            //如果最开始的值就不存在，设置为0
            if (ModPlayer.ExecutionListStored.TryGetValue(heldItem.type, out int num))
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
