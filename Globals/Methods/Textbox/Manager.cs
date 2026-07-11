using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods.Textbox
{
    public class TextboxManager : ModSystem
    {
        public static float FirstLineY = 0;
        public static float LerpValue = 0;
        public static float EdgeValue = 0;
        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.dedServ)
                return;
            if (!Main.HoverItem.IsLegal())
            {
                LerpValue = 0;
                EdgeValue = 0;
                FirstLineY = 0;
                return;
            }
                LerpValue = Lerp(LerpValue, 1.0f, 0.21f);
                if (LerpValue > 0.98f)
                {
                    EdgeValue = Lerp(EdgeValue, 1f, 0.21f);
                    LerpValue = 1f;
                }

        }
    }
}
