using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public partial class HJScarletGlobalNPCs : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool DeepToneThrownMark = false;
        public bool Dialectics_Mark = false;
        public int Dialectics_Timer = 0;
        public int Dialectics_HitTime = 0;
        public override void PostAI(NPC npc)
        {
            if (Dialectics_HitTime > 5)
                Dialectics_HitTime = 5;

            if (Dialectics_Timer > 0)
                Dialectics_Timer--;

            if(Dialectics_Timer <= 0)
            {
                Dialectics_Mark = false;
                Dialectics_HitTime = 0;
            }
        }

    }
}
