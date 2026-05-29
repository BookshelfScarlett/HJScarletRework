using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Globals.Systems
{
     public class HJScarletSnowCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public HJScarletSnowCondition()
        {
            Description ??= Language.GetOrRegister("Mods.HJScarletRework.Conditions.Drop.SnowBiome");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return Main.hardMode
                && !NPCID.Sets.CannotDropSouls[npc.type]
                && !npc.boss
                && !npc.friendly
                && npc.lifeMax > 1
                && npc.value >= 1f
                && info.player.ZoneSnow;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }

    public class ScarletWearingFullCowboy : IItemDropRuleCondition
    {
        private static LocalizedText Description;
        public ScarletWearingFullCowboy()
        {
            Description ??= Language.GetOrRegister("Mods.HJScarletRework.Conditions.Drop.FullCowboy");
        }
        public bool CanDrop(DropAttemptInfo info)
        {
            Player player = info.player;
            return (player.armor[0].type == ItemID.CowboyHat && player.armor[1].type == ItemID.CowboyJacket && player.armor[2].type == ItemID.CowboyPants);
        }
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
        public class HJScarletUnderworldCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public HJScarletUnderworldCondition()
        {
            Description ??= Language.GetOrRegister("Mods.HJScarletRework.Conditions.Drop.UnderworldBiome");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return Main.hardMode
                && !NPCID.Sets.CannotDropSouls[npc.type]
                && !npc.boss
                && !npc.friendly
                && npc.lifeMax > 1
                && npc.value >= 1f
                && info.player.ZoneUnderworldHeight;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }

}
