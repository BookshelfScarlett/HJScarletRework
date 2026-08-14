using HJScarletRework.Buffs;
using HJScarletRework.Globals.Graphics.Particles;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public int defenseBuffTimer = 0;
        public int defenseBuff = 0;
        public int swapTimer = 0;
        public int ruShiWoWenBanTimer = 0;
        public int globalSoundDelay = 0;
        public int bookcaseBuffTime = 0;
        public void ResetTimer()
        {
            climaticHawstringLaserCounter = 0;
            desterrannachtImmortalTime = 0;
            desterranRespawnChargeTimer = 0;
            stardustRuneHitHealTimer = 0;
            defenderEmblemCD = 0;
            exsanguinationBuffTime = 0;
            tacticalExecutionInputCache = 0;
            blackKeyTimer = 0;
            heldProjReUseTime = 0;
            antiKnockbackTime = 0;
            crimsonCharmReduceTime = 0;
            cycleMadenessCrit = 0;
            cycleMadenssTimer = 0;
            cowboyRevolverTimer = 0;
            floretProtectorTimer = 0;
            containedBlastBoomCount = 0;
            containedBlastBuffTime = 0;
            defenseBuffTimer = 0;
            defenseBuff = 0;
            swapTimer = 0;
            adamantiteHeadExecutorThunderTimer = 0;
            powerLilyTimer = 0;
            ruShiWoWenBanTimer = 0;
            globalSoundDelay = 0;
            maidReaperHealTimer = 0;
            conferenceCallBuffTime = 0;
            tearEyeBuff = 0;
            bookcaseBuffTime = 0;
        }
        public void UpdateTimer()
        {
            if (bookcaseBuffTime > 0)
                bookcaseBuffTime--;
            if (tearEyeBuff > 0)
                tearEyeBuff--;
            if (conferenceCallBuffTime > 0)
                conferenceCallBuffTime--;
            if (maidReaperHealTimer > 0)
                maidReaperHealTimer--;
            if (globalSoundDelay > 0)
                globalSoundDelay--;
            if (ruShiWoWenBanTimer > 0)
                ruShiWoWenBanTimer--;
            if (powerLilyTimer > 1)
            {
                powerLilyTimer--;
                powerLilyCacheTimer = powerLilyTimer;
            }
            if (!powerLily && powerLilyTimer > 0)
                powerLilyTimer = 0;
            if (adamantiteHeadExecutorThunderTimer > 0)
                adamantiteHeadExecutorThunderTimer--;
            if (swapTimer > 0)
                swapTimer--;
            if (tacticalExecutionInputCache > 0)
                tacticalExecutionInputCache--;


            if (cycleMadenssTimer > 0)
                cycleMadenssTimer--;
            if (cycleMadenssTimer == 0)
                cycleMadenessCrit = 0;

            if (flybackhandBuffTime > 0)
                flybackhandBuffTime--;

            if (flybackhandBuffTime == 0)
                flybackhandBuffTimeCurrent = 0;

            if (flybackInGameTimeBuff > 0)
                flybackInGameTimeBuff--;

            if (galvanizedHandDashCD > 0)
            {
                if (galvanizedHandDashCD == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item35, Player.Center);
                    for (int i = 0; i < 25; i++)
                    {
                        new TurbulenceShinyOrb(Player.Center.ToRandCirclePosEdge(10), 2f, RandLerpColor(Color.SkyBlue, Color.White), 120, 0.4f, RandRotTwoPi).Spawn();
                    }
                }
                galvanizedHandDashCD--;
            }

            if (desterranRespawnChargeTimer > 0)
                desterranRespawnChargeTimer--;
            if (desterranRespawnChargeTimer == 0)
                desterrannachtImmortalTime = 0;

            if (flybackhandCloclCD > 0)
                flybackhandCloclCD--;

            if (NoSlowFall > 0)
                NoSlowFall--;

            if (defenderEmblemCD > 0)
                defenderEmblemCD--;

            if (genderChangeTimer > 0)
                genderChangeTimer--;

            if (stardustRuneHitHealTimer > 0)
                stardustRuneHitHealTimer--;

            if (stardustRuneStaticHealTimer > 0)
                stardustRuneStaticHealTimer--;

            if (cowboyRevolverTimer > 0)
                cowboyRevolverTimer--;

            if (blackKeyTimer > 0)
                blackKeyTimer--;
            if (floretProtectorTimer > 0)
                floretProtectorTimer--;
            if (heldProjReUseTime > 0)
                heldProjReUseTime--;

            protectorPlantID = Player.HasBuff<HerbBagBuff>() ? protectorPlantID : -1;
            for (int i = 0; i < protectorHerbTimerList.Length; i++)
            {
                if (protectorHerbTimerList[i] != 0)
                    protectorHerbTimerList[i]--;
            }
            if (PlayerFinalSpeedStoredTime > 0)
                PlayerFinalSpeedStoredTime--;
            if (PlayerFinalSpeedStoredTime == 0)
                PlayerLastSpeedStored = 0;
            if (exsanguinationBuffTime > 0)
                exsanguinationBuffTime--;
            if (hasSendExecutionTintTimer > 0)
                hasSendExecutionTintTimer--;

            if (!Player.HasBuff<BlackKeyExecutionBuff>())
                blackKeyDefenseTrigger = false;
            if (!Player.HasBuff<CrimsonCharmBuff>())
            {
                crimsonCharmStopReduce = false;
                if (Player.miscCounter % 10 == 0 && crimsonCharmReduceTime > 0)
                    crimsonCharmReduceTime--;
            }
            if (containedBlastBuffTime > 0)
                containedBlastBuffTime--;
            if (containedBlastBuffTime == 0)
                containedBlastBoomCount = 0;
            if (antiKnockbackTime > 0)
            {
                Player.noKnockback = true;
                antiKnockbackTime--;
            }
            if (defenseBuffTimer > 0)
            {
                Player.statDefense += defenseBuff;
                defenseBuffTimer--;
            }
            if (Player.HeldItem.type != lastHeldItemIndex)
            {
                lastHeldItemIndex = Player.HeldItem.type;
                hasSendExecutionTint = false;
                executorSwordMarkPing = false;
            }
            if (crimsonScytheDefense > 0 && crimsonScytheAttackCounter < 1)
                crimsonScytheDefense -= 1;
        }
    }
}
