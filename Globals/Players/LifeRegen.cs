using HJScarletRework.Buffs;
using HJScarletRework.Items.Useables;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public bool absoluteZeroBuff = false;
        public bool theBleachingBuff = false;
        public override void UpdateLifeRegen()
        {
            if (fruitofEthernity)
            {
                Player.lifeRegen += FruitofEternity.LifeRegenSpeed;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (saintChurch && saintChurchLastStanding > 0)
                ApplyDoT(0);
            if (absoluteZeroBuff)
            {
                ApplyDoT(AbsoluteZeroBuff.BadLifeRegen);
            }
            if (theBleachingBuff)
            {
                ApplyDoT(TheBleachingBuff.BadLifeRegen);
            }
        }
        public void ApplyDoT(int badLifeRegen)
        {
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            Player.lifeRegen -= badLifeRegen * 2;
        }

    }
}
