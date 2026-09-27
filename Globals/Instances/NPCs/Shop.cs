using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Useables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.NPCs
{
    public partial class HJScarletGlobalNPCs : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.BestiaryGirl)
            {
                shop.ToCustomValue<RuShiWoWen>(0, 30, 0, 0);
            }
            if (shop.NpcType == NPCID.Merchant)
            {
                shop.ToCustomValue<AxeCharm>(0, 5, 0, 0);
            }
            if (shop.NpcType == NPCID.Wizard)
            {
                shop.ToCustomValue<CrystallizedLore>(0, 60, 0, 0);
            }
            if (shop.NpcType == NPCID.Clothier)
            {
                shop.ToCustomValue<CombatSlot>(0, 50, 0, 0);
            }
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
