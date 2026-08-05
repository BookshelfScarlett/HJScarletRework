using HJScarletRework.Items.Weapons.Ranged;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        private void ResetAcc()
        {
            PreciousTargetAcc = false;
            PreciousAimAcc = false;
            PreciousCritsMin = 0;
            desterrennacht = false;
            manaSavingsJar = 0;
            loveRing = false;
            isBeingLove = false;
            heartoftheCrystal = false;
            tacticalExecution = false;
            ExecutorSwordMarkPlus = false;
            blackKeyHeal = 0;
            blackKeyDefenseBuff = 0;
            blackKeyDoT = false;
            artificalManaStar = false;
            executorSwordMarkLevel = -1;
            souloftheTidalMark = false;
            mayaPumper = false;
            crimsonCharm = false;
            bitingClaw = false;
            cycleMadness = false;
            powerLily = false;
            powerLilyVanity = false;

            emblemVanguard = false;
            emblemColdSteel = false;
            emblemFirearm = false;
            emblemThrown = false;
            emblemExecutor = false;
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

            adamantiteHeadExecutor = false;
            chlorophyteHeadExecutor = false;
            titaniumHeadExecutor = false;
        }
        private void ResetBuff()
        {
            fruitofEthernity = false;
            infiniteFlightTime = false;
        }
        private void ResetPets()
        {
            WhalePet = false;
            NonePet = false;
            ShadowPet = false;
            SquidPet = false;
            WatcherPet = false;
            dracoPet = false;
            goldenAppleEnchanted = false;
            goldenAppleDamageAbsorb = 0;
            goldenAppleEnchantedFully = false;
        }
        public override void ResetEffects()
        {
            climaticHawstringLaserCounter *= (Player.HeldItem.type == ItemType<ClimaticHawstring>()).ToInt();
            CreationHatSet = false;
            LifeBalloonAcc = false;
            critDamageAll = 0;
            critDamageExecutor = 0;
            healingPotionMult = 1;
            ResetAcc();
            ResetPets();
            ResetArmor();
            ResetBuff();

        }
        public override void UpdateDead()
        {
            flybackhandBuffTime = 0;
            flybackhandCloclCD = 0;
            flybackhandBuffTimeCurrent = 0;
            PreciousTargetCrtis = 10;
            LifeBalloonAcc = false;
            monkStaffHeal = false;
            galvanizedHandDashCD = 0;
            crimsonCharmStopReduce = false;
            crimsonScytheAttackCounter = 0;
            isExecutionStrikeTriggered = false;
            KnifeMarkIndex = -1;
            ResetAcc();
            ResetPets();
            ResetArmor();
            ResetBuff();

        }


    }
}
