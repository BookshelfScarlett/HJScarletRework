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
