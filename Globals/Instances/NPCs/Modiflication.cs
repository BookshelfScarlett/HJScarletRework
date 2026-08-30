using HJScarletRework.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.NPCs
{
    public partial class HJScarletGlobalNPCs:GlobalNPC
    {
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (theBleachingBuffEnemy)
            {
                modifiers.FinalDamage *= TheBleachingBuff.HitDamageMultEnemy;
            }
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (absoluteZeroBuffEnemy)
            {
                modifiers.SourceDamage *= (1 + AbsoluteZeroBuff.BadDamageMult);
            }
            if (theBleachingBuffEnemy)
            {
                modifiers.DefenseEffectiveness *= 0;
            }
        }

    }
}
