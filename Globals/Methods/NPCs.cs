using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static bool IsLegalTarget(this NPC npc) => !(npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active);
        /// <summary>
        /// 给你的目标来一拳
        /// <br>用于强制移动你的目标，你可以用这个去强制击退</br>
        /// </summary>
        public static void PunchTarget(this NPC target, Vector2 punchDirection, float punchStrength)
        {
            if (!target.IsLegal())
                return;
            Vector2 punchVel = punchDirection.ToSafeNormalize() * punchStrength;
            target.velocity = punchVel;
            //代办：给一拳的击退是需要做多人同步的
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
        }
    }
}
