using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        private void ResetAcc()
        {
            pocketMirror = false;
            blackKeyExecutorCriticalChanceAdd = 0;
            blackKeyExecutorDamageAdd = 0;
            preciousTargetLevel = 0;
            pendantLevel = 0;
            cursorID = -1;
            desterrennacht = false;
            manaSavingsJar = 0;
            loveRing = false;
            isBeingLove = false;
            heartoftheCrystal = false;
            tacticalExecution = false;
            sacarbWings = false;
            blackKeyHeal = 0;
            handOfGods = false;
            blackKeyDefenseBuff = 0;
            blackKeyDoT = false;
            artificalManaStar = false;
            executorSwordMarkLevel = -1;
            selfPortraitType = -1;
            souloftheTidalMark = false;
            mayaPumper = false;
            crimsonCharm = false;
            bitingClaw = false;
            powerLily = false;
            powerLilyVanity = false;
            LightofHorizon = false;
            ankhShieldImmnue = false;
            infiniteBreath = false;
            terraSparkBoostImmnue = false;
            celesitalShellEffect = false;
            cycleMadnessLevel = -1;
            spellBreakerLevel = 0;

            emblemVanguard = false;
            emblemColdSteel = false;
            emblemFirearm = false;
            emblemThrown = false;
            emblemExecutor = false;
            emblemGalaxy = false;
            combatSlot = false;
        }
        private void ResetArmor()
        {
            shinobiExecutor = false;
            monkExecutor = false;
            cowboyExecutor = false;
            floretProtectorExecutor = false;
            raincoatExecutor = false;
            redDragonKnight = false;
            protectorShiver = false;
            protectorMoonglow = false;
            diverArmor = false;
            maidReaperArmor = false;
            dragonHunter = false;
            saintChurch = false;

            adamantiteHeadExecutor = false;
            chlorophyteHeadExecutor = false;
            titaniumHeadExecutor = false;
        }
        private void ResetBuff()
        {
            fruitofEthernity = false;
            infiniteFlightTime = false;
            absoluteZeroBuff = false;
            theBleachingBuff = false;
        }
        private void ResetPets()
        {
            petWhale = false;
            petNone = false;
            petShadow = false;
            petSquid = false;
            petWatcher = false;
            petDraco = false;
            petSon = false;
            petLifeWorm = false;
            goldenAppleEnchanted = false;
            goldenAppleDamageAbsorb = 0;
            goldenAppleEnchantedFully = false;
        }
        public override void ResetEffects()
        {
            climaticHawstringLaserCounter *= (Player.HeldItem.type == ItemType<ClimaticHawstring>()).ToInt();
            creationHat = false;
            LifeBalloonAcc = false;
            critDamageAll = 0;
            critDamageExecutor = 0;
            healingPotionMult = 1;
            iFrameHurtAdd = 0;
            ResetAcc();
            ResetPets();
            ResetArmor();
            ResetBuff();

        }
        public override void UpdateDead()
        {
            flybackhandBuffTime = 0;
            flybackhandBuffTimeCurrent = 0;
            cycleMadnessCrit = 0;
            LifeBalloonAcc = false;
            monkStaffHeal = false;
            galvanizedHandDashCD = 0;
            crimsonScytheAttackCounter = 0;
            isExecutionStrikeTriggered = false;
            KnifeMarkIndex = -1;
            theGreatDipperBuff = false;
            ResetAcc();
            ResetPets();
            ResetArmor();
            ResetBuff();
        }
    }
}
