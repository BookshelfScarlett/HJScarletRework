using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.NPCs.Bosses.VanillaOverride.EyeofCthulhu
{
    public class EyeofCthulhuAI : LostbeltModeBehaviour
    {
        public override NPCEqualsTo ApplyEquals() => new NPCEqualsTo().EqualsToType(NPCID.EyeofCthulhu);
        public override void NpcAI(NPC npc)
        {
            base.NpcAI(npc);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }
    }
}
