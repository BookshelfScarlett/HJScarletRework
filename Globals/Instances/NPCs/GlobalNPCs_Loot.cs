using ContinentOfJourney.Items;
using ContinentOfJourney.NPCs;
using ContinentOfJourney.NPCs.Boss_MarquisMoonsquid;
using ContinentOfJourney.NPCs.Boss_PriestessRod;
using ContinentOfJourney.NPCs.Boss_ScarabBelief;
using ContinentOfJourney.NPCs.Boss_TheOverwatcher;
using ContinentOfJourney.NPCs.Boss_WallofShadow;
using ContinentOfJourney.NPCs.Boss_WorldsEndEverlastingFallingWhale;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Pets;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.CodeAnalysis.Operations;
using System.Security.AccessControl;
using Terraria;
using Terraria.GameContent.ItemDropRules;
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
            switch (npc.type)
            {
                case NPCID.Golem:
                    HJScarletMethods.ApplyNoBossBagLoot(ref npcLoot, ItemType<DisasterEssence>(), 1, 10, 20);
                    break;
                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new ScarletWearingFullCowboy(), ItemType<ExecutorEmblem>()));
                    HJScarletMethods.ApplyNoBossBagLoot(ref npcLoot, ItemType<ExecutorEmblem>(), 4);
                    break;
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicHallow:
                case NPCID.BigMimicJungle:
                    npcLoot.AddLootSimple(ItemType<PurePrismFate>(), 1, 20, 40);
                    break;
                case NPCID.Mimic:
                case NPCID.IceMimic:
                    npcLoot.AddLootSimple(ItemType<PurePrismFate>(), 1, 10, 30);
                    break;

                case NPCID.MoonLordCore:
                    HJScarletMethods.ApplyNoBossBagLoot(ref npcLoot, ItemType<PrunusMume>(), 4);
                    break;
            }
            if(npc.boss && npc.type != NPCID.KingSlime)
            {
                HJScarletMethods.ApplyMasterLoot(ref npcLoot, ItemType<UnregisteredSpiritOrigin>(), 5);
            }
            if (npc.type == NPCType<WorldsEndEverlastingFallingWhale>())
                HJScarletMethods.ApplyMasterLoot(ref npcLoot, ItemType<WhaleItem>(), 4);
            if (npc.type == NPCType<TheOverwatcher>())
                HJScarletMethods.ApplyMasterLoot(ref npcLoot, ItemType<WatcherItem>(), 4);
            if (npc.type == NPCType<WallofShadow>())
            {
                HJScarletMethods.ApplyMasterLoot(ref npcLoot, ItemType<ShadowItem>(), 4);
                HJScarletMethods.ApplyNoBossBagLoot(ref npcLoot, ItemType<DeathTolls>(), 4);
                HJScarletMethods.ApplyNoBossBagLoot(ref npcLoot, ItemType<ExecutorBadge>(), 4);
            }
            if (npc.type == NPCType<MarquisMoonsquid>())
                HJScarletMethods.ApplyMasterLoot(ref npcLoot, ItemType<SquidItem>(), 4);
            if (npc.type == NPCType<ScarabBelief>())
                HJScarletMethods.ApplyMasterLoot(ref npcLoot, ItemType<NoneItem>(), 4);
            if (npc.type == NPCType<DesertMimic>() || npc.type == NPCType<PolarMimic>() || npc.type == NPCType<TempleMimic>())
            {
                npcLoot.AddLootSimple(ItemType<PurePrismFate>(), 1, 30, 50);
            }
            if(npc.type == NPCType<PriestessRod>())
            {
                HJScarletMethods.ApplyNoBossBagLoot(ref npcLoot, ItemType<ClimaticHawstring>(), 4);
            }
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            LeadingConditionRule leadingConditionRule2 = new(Condition.InSnow.ToDropCondition(ShowItemDropInUI.Always));
            leadingConditionRule2.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<FrostHammer>(), 250));
            globalLoot.Add(leadingConditionRule2);

            LeadingConditionRule leadingConditionRule = new(Condition.InUnderworld.ToDropCondition(ShowItemDropInUI.Always));
            IItemDropRuleCondition rule = (Condition.DownedGolem.ToDropCondition(ShowItemDropInUI.Always));
            leadingConditionRule.OnSuccess(ItemDropRule.ByCondition(rule, ItemType<DisasterEssence>(), 6));
            globalLoot.Add(leadingConditionRule);
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
