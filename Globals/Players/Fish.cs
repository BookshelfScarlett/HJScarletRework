using HJScarletRework.Items.Armor.Diver;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (!bitingClaw)
                return;
            if (attempt.inHoney || attempt.inLava)
                return;
            if (!NPC.downedBoss2)
                return;
            if (attempt.questFish > 0)
                return;
            if (!Player.ZoneBeach)
                return;
            int power = attempt.playerFishingConditions.BaitPower + attempt.playerFishingConditions.PolePower;
            int poolSizeAmt = attempt.waterTilesCount / 10;
            if (poolSizeAmt > 100)
                poolSizeAmt = 100;
            HandleDiverArmor(poolSizeAmt, power, ref itemDrop, ref sonar);
        }
        public void HandleDiverArmor(int poolSizeAmt, int power, ref int itemDrop, ref AdvancedPopupRequest sonar)
        {
            int fishPowerDiv = power + poolSizeAmt;
            if (fishPowerDiv < 45)
                fishPowerDiv = 45;
            int chanceToCatchDiverArmor = 1750 / fishPowerDiv;
            List<int> list = [ItemType<DiverHead>(), ItemType<DiverBody>(), ItemType<DiverLegs>()];
            int increaseChanceTime = 1;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                IncreaseDiverArmorChance(ref increaseChanceTime, ref list, Player.inventory[i]);
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                IncreaseDiverArmorChance(ref increaseChanceTime, ref list, Player.armor[i]);
            }
            for (int i = 0; i < Player.miscEquips.Length; i++)
            {
                IncreaseDiverArmorChance(ref increaseChanceTime, ref list, Player.miscEquips[i]);
            }
            chanceToCatchDiverArmor /= increaseChanceTime;
            if (chanceToCatchDiverArmor < 4)
                chanceToCatchDiverArmor = 4;
            if (chanceToCatchDiverArmor > 60)
                chanceToCatchDiverArmor = 60;
            if (list.Count == 0)
                return;
            if (Main.rand.NextBool(chanceToCatchDiverArmor))
            {
                itemDrop = list[Main.rand.Next(list.Count)];
                sonar.Color = Color.Red;
            }
        }
        private static void IncreaseDiverArmorChance(ref int increaseChanceTime, ref List<int> list, Item item)
        {
            if (item is null || item.IsAir)
                return;
            if (!list.Contains(item.type))
                return;
            increaseChanceTime += 1;
            list.Remove(item.type);
        }
    }
}
