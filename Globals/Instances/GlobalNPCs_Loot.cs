using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public partial class HJScarletGlobalNPCs : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch(npc.type)
            {
                case NPCID.MartianSaucerCore:
                    npcLoot.AddLootSimple(ItemType<TheMars>(), 4);
                    break;
            }
        }
    }
}
