using ContinentOfJourney.NPCs.Boss_MarquisMoonsquid;
using ContinentOfJourney.NPCs.Boss_ScarabBelief;
using ContinentOfJourney.NPCs.Boss_TheOverwatcher;
using ContinentOfJourney.NPCs.Boss_WallofShadow;
using ContinentOfJourney.NPCs.Boss_WorldsEndEverlastingFallingWhale;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Pets;
using HJScarletRework.Items.Weapons.Melee;
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
            if (npc.type == NPCType<WorldsEndEverlastingFallingWhale>())
                npcLoot.AddLootSimple(ItemType<WhaleItem>(), 4);
            if (npc.type == NPCType<TheOverwatcher>())
                npcLoot.AddLootSimple(ItemType<WatcherItem>(), 4);
            if (npc.type == NPCType<WallofShadow>())
                npcLoot.AddLootSimple(ItemType<ShadowItem>(), 4);
            if (npc.type == NPCType<MarquisMoonsquid>())
                npcLoot.AddLootSimple(ItemType<SquidItem>(), 4);
            if (npc.type == NPCType<ScarabBelief>())
            {
                npcLoot.AddLootSimple(ItemType<NoneItem>(), 4);
            }
        }
    }
}
