using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Database
{
    public class ScarletDatabase : ModSystem
    {
        public static bool AnyBossHere = false;
        public static Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
        public static Rectangle MouseRectangle;
        public override void UpdateUI(GameTime gameTime)
        {
            ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
        }
        public override void PreUpdateWorld()
        {
            AnyBossHere = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.boss)
                {
                    AnyBossHere = true;
                    return;
                }
            }
        }
    }
}
