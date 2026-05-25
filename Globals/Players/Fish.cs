using ContinentOfJourney;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            bool water = !attempt.inHoney && !attempt.inLava;
            if(water)
            {
                if(Player.ZoneJungle)
                {

                }
            }
        }
    }
}
