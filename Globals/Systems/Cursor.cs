using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Systems
{
    public class HJScarletCustomCursor : ModSystem
    {
        public static float CurFloat = 0f;
        public static int CacheType = -1;
        public static void On_Main_DrawInterface_36_Cursor(On_Main.orig_DrawInterface_36_Cursor orig)
        {
            if(Main.gameMenu)
            {
                orig();
                return;
            }
            int cursorType = Main.LocalPlayer.HJScarlet().cursorID;
            if (cursorType == -1 && CacheType == -1)
            {
                orig();
                return;
            }
            if (cursorType != -1 && CacheType == -1 && CurFloat <= .02f)
            {
                CacheType = cursorType;
            }
            if (cursorType == -1 && CacheType != -1 && CurFloat <= .02f)
            {
                CacheType = -1;
                CurFloat = 0;
            }
            if (Main.gamePaused || Main.LocalPlayer.IsInInventory() || cursorType == -1)
            {
                CurFloat = Lerp(CurFloat, 0f, 0.12f);
                orig();
            }
            else
                CurFloat = Lerp(CurFloat, 1f, 0.12f);
            switch (CacheType)
            {
                case 1:
                    DrawCursorPreciousTarget();
                    break;
                default:
                    orig();
                    break;
            }
        }
        public static void DrawCursorPreciousTarget()

        {
            SpriteBatch sb = Main.spriteBatch;
            Vector2 pos = Main.MouseScreen;
            Texture2D glow = HJScarletTexture.Particle_Smear.Value;
            float overAllScale = 1f*CurFloat; 
            float glowScale = .65f * overAllScale;
            float count = 2;
            float timeForVisual = (float)Main.timeForVisualEffects;
            float oriRotation = 0.25f * timeForVisual;
            Color glowColor = Color.LightSeaGreen.ToAddColor() * .5f * CurFloat;
            for (int i = 0; i < count; i++)
            {
                float rot = (TwoPi / count * i) + oriRotation;
                sb.FastDraw(glow, pos, glowColor, rot, glow.Size() / 2f, glowScale, 0);
                sb.FastDraw(glow, pos, glowColor, -rot + PiOver4, glow.Size() / 2f, glowScale * .75f, 0);
                sb.FastDraw(glow, pos, glowColor, rot + PiOver2, glow.Size() / 2f, glowScale * .8f, 0);
                sb.FastDraw(glow, pos, glowColor, -rot + PiOver4 + PiOver2, glow.Size() / 2f, glowScale * .75f * .8f, 0);
            }

            Texture2D path = HJScarletItemProj.Cursor_Target.Value;
            //CursorTargetCross= Request<Texture2D>("HJScarletRework/Assets/Texture/Items/Equips/PreciousTarget").Value;
            for(int i =0;i<8;i++)
            sb.FastDraw(path, pos+(TwoPi/8f*i).ToRotationVector2()*1.2f*CurFloat, Color.White.ToAddColor(), 0, path.Size() / 2f, overAllScale, 0);
            sb.FastDraw(path, pos, Color.White*CurFloat, 0, path.Size() / 2f, overAllScale, 0);

        }
    }
}
