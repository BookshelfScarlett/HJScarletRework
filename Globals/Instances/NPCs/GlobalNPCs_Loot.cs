using ContinentOfJourney.NPCs.Boss_MarquisMoonsquid;
using ContinentOfJourney.NPCs.Boss_ScarabBelief;
using ContinentOfJourney.NPCs.Boss_TheOverwatcher;
using ContinentOfJourney.NPCs.Boss_WallofShadow;
using ContinentOfJourney.NPCs.Boss_WorldsEndEverlastingFallingWhale;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
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
            switch (npc.type)
            {
                case NPCID.MartianSaucerCore:
                    npcLoot.AddLootSimple(ItemType<TheMars>(), 4);
                    break;
            }
            if (!Main.masterMode)
            {
                switch (npc.type)
                {
                    case NPCID.WallofFlesh:
                        npcLoot.AddLootSimple(ItemType<ExecutorEmblem>(), 3);
                        break;
                }
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
                npcLoot.AddLootSimple(ItemType<NoneItem>(), 4);
        }
        public override bool? CanGoToStatue(NPC npc, bool toKingStatue)
        {
            return base.CanGoToStatue(npc, toKingStatue);
        }
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            bool isMale = HJScarletList.MaleNPC.Contains(npc.type);
            bool isFemale = HJScarletList.FemaleNPC.Contains(npc.type);
            Player player = Main.LocalPlayer;
            if (!player.HJScarlet().loveRing)
                return;
            foreach (var item in items)
            {
                if (item is null || item.IsAir)
                    continue;
                float modify = 0.60f;

                if (player.Male && isFemale)
                    modify -= 0.05f;
                if (!player.Male && isMale)
                    modify -= 0.05f;
                item.shopCustomPrice = (int)((item.shopCustomPrice ?? item.GetStoreValue()) * modify);
            }
        }
    }
}
