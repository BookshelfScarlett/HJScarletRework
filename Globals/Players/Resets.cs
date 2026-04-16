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
            fakeManaContainer = 0;
            defenderEmblem = false;
            loveRing = false;
            isBeingLove = false;
            heartoftheCrystal = false;
            tacticalExecution = false;
            executorAscension = false;
            blackKeyHeal = 0;
            blackKeyDefenseBuff = 0;
            blackKeyDoT = false;
        }
        private void ResetArmor()
        {
            runeWizardExecutor = false;
            cowboyExecutor = false;
            floretProtectorExecutor = false;
            raincoatExecutor = false;
            redDragonKnight = false;
            protectorShiver = true;
        }

        private void ResetPets()
        {
            WhalePet = false;
            NonePet = false;
            ShadowPet = false;
            SquidPet = false;
            WatcherPet = false;
        }


    }
}
