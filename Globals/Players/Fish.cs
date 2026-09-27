using ContinentOfJourney;
using HJScarletRework.Globals.Systems.Conditions;
using HJScarletRework.Items.Armor.Diver;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Melee;
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
            int bait = attempt.playerFishingConditions.BaitItemType;
            int power = attempt.playerFishingConditions.BaitPower + attempt.playerFishingConditions.PolePower;
            int questFish = attempt.questFish;
            int poolSize = attempt.waterTilesCount;
            bool water = !attempt.inHoney && !attempt.inLava;
            if (!bitingClaw)
                return;
            if (water)
            {
                int poolSizeAmt = poolSize / 10;
                if (poolSizeAmt > 100)
                    poolSizeAmt = 100;

                if (Player.ZoneBeach && ScarletDownedBoss.DownedEvilBosses)
                    HandleDiverArmor(poolSizeAmt, power, ref itemDrop, ref sonar);
                if (Player.ZoneDesert && DownedBossSystem.downedTimeGod)
                    FastPoolHandler(1800, ItemType<EthrealSwordfish>(), poolSizeAmt, power, ref itemDrop, ref sonar);
                if (Player.ZoneSnow && DownedBossSystem.downedMatterGod)
                    FastPoolHandler(1800, ItemType<MatterSawtoothShark>(), poolSizeAmt, power, ref itemDrop, ref sonar);
                if (Player.ZoneJungle && DownedBossSystem.downedLifeGod)
                    FastPoolHandler(1800, ItemType<LivingReaverShark>(), poolSizeAmt, power, ref itemDrop, ref sonar);
                if (DownedBossSystem.downedBarrier)
                    FastPoolHandler(1800, ItemType<DarkenRockFish>(), poolSizeAmt, power, ref itemDrop, ref sonar);
            }
        }
        public void FastPoolHandler(int chanceAmt, int targetItem, int poolSizeAmt, int power, ref int itemDrop, ref AdvancedPopupRequest sonar)
        {
            int fishPowerDiv = power + poolSizeAmt;
            int chanceToCatch = chanceAmt / fishPowerDiv;
            if (Main.rand.NextBool(chanceToCatch))
            {
                itemDrop = targetItem;
            }
        }
        public void HandleDiverArmor(int poolSizeAmt, int power, ref int itemDrop, ref AdvancedPopupRequest sonar)
        {
            int fishPowerDiv = power + poolSizeAmt;
            int chanceToCatchDiverArmor = 1750 / fishPowerDiv;
            List<int> list = [ItemType<DiverHead>(), ItemType<DiverBody>(), ItemType<DiverLegs>()];
            int increaseChanceTime = 1;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                if (Player.inventory[i].IsAir || Player.inventory[i] == null)
                    continue;
                if (Player.inventory[i].type == ItemType<DiverHead>() && list.Contains(ItemType<DiverHead>()))
                {
                    increaseChanceTime += 1;
                    list.Remove(ItemType<DiverHead>());
                }
                if (Player.inventory[i].type == ItemType<DiverBody>() && list.Contains(ItemType<DiverBody>()))
                {
                    increaseChanceTime += 1;
                    list.Remove(ItemType<DiverBody>());
                }
                if (Player.inventory[i].type == ItemType<DiverLegs>() && list.Contains(ItemType<DiverLegs>()))
                {
                    increaseChanceTime += 1;
                    list.Remove(ItemType<DiverLegs>());
                }
            }
            chanceToCatchDiverArmor /= increaseChanceTime;
            if (Main.rand.NextBool(chanceToCatchDiverArmor))
            {
                if (list.Count != 0)
                {
                    itemDrop = list[Main.rand.Next(list.Count)];
                    sonar.Color = Color.Red;
                }
            }
        }
    }
}
