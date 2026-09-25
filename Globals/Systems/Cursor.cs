using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Systems
{
    public class HJScarletCustomCursor : ModSystem
    {
        public static Texture2D CursorTargetCross { get; set; }
        public override void Load()
        {
            CursorTargetCross = Request<Texture2D>("HJScarletRework/Assets/Texture/Items/Equips/PreciousTarget").Value;
        }
        public override void Unload()
        {
            CursorTargetCross = null;
        }
        public static void On_Main_DrawInterface_36_Cursor(On_Main.orig_DrawInterface_36_Cursor orig)
        {
            if (Main.gameMenu)
            {
                orig();
                return;
            }
            int cursorType = Main.LocalPlayer.HJScarlet().cursorID;
            switch (cursorType)
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
            Vector2 pos = Main.MouseWorld - Main.screenPosition;
            //CursorTargetCross= Request<Texture2D>("HJScarletRework/Assets/Texture/Items/Equips/PreciousTarget").Value;
            sb.FastDraw(CursorTargetCross, pos, Color.White, 0, CursorTargetCross.Size() / 2f, 1, 0);
        }
    }
}
