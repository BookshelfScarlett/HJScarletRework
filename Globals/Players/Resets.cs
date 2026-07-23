using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using HJScarletRework.Projs.Executor;
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
            vanguardEmblem = false;
            loveRing = false;
            isBeingLove = false;
            heartoftheCrystal = false;
            tacticalExecution = false;
            ExecutorSwordMarkPlus = false;
            blackKeyHeal = 0;
            blackKeyDefenseBuff = 0;
            blackKeyDoT = false;
            artificalManaStar = false;
            executorSwordMark = false;
            executorSwordMarkLevel = -1;
            frostHammerHoming = false;
            souloftheTidalMark = false;
            mayaPumper = false;
            crimsonCharm = false;
            bitingClaw = false;
            cycleMadness = false;
            accVanityID = -1;
        }
        private void ResetArmor()
        {
            runeWizardExecutor = false;
            fishExecutor = false;
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
        }

        private void ResetPets()
        {
            WhalePet = false;
            NonePet = false;
            ShadowPet = false;
            SquidPet = false;
            WatcherPet = false;
            goldenApple = false;
            goldenAppleEnchanted = false;
            goldenAppleDamageAbsorb = 0;
            goldenAppleEnchantedFully = false;
        }
        public override void ResetEffects()
        {
            climaticHawstringLaserCounter *= (Player.HeldItem.type == ItemType<ClimaticHawstring>()).ToInt();
            CreationHatSet = false;
            ShadowCastAcc = false;
            LifeBalloonAcc = false;
            critDamageAll = 0;
            critDamageExecutor = 0;
            bonusExecutionReduce = 0;
            healingPotionMult = 1;
            //drawUseableItemIcon = -1;
            if (!Player.HasProj<GaiaStrikerHeldProj>())
                holdingGaiaStaff = false;
            ResetAcc();
            ResetPets();
            ResetArmor();

        }
        public override void UpdateDead()
        {
            ExecutionProgress = 0;
            flybackhandBuffTime = 0;
            flybackhandCloclCD = 0;
            flybackhandBuffTimeCurrent = 0;
            PreciousTargetCrtis = 10;
            LifeBalloonAcc = false;
            galvanizedHandDashCD = 0;
            climaticHawstringLaserCounter = 0;

            desterrannachtImmortalTime = 0;
            desterranRespawnChargeTimer = 0;
            stardustRuneHitHealTimer = 0;
            defenderEmblemCD = 0;
            exsanguinationBuffTime = 0;
            tacticalTime = 0;
            tacticalPunishTime = 0;
            tacticalExecutionInputCache = 0;
            blackKeyTimer = 0;
            heldProjReUseTime = 0;
            holdingGaiaStaff = false;
            ResetAcc();
            ResetPets();
            ResetArmor();
            crimsonCharmReduceTime = 0;
            crimsonCharmStopReduce = false;
            cycleMadenessCrit = 0;
            cycleMadenssTimer = 0;
            cowboyRevolverTimer = 0;
            floretProtectorTimer = 0;
            monkStaffHeal = false;
            containedBlastBoomCount = 0;
            containedBlastBuffTime = 0;
            executionUpdatesFrame = 0;
        }


    }
}
