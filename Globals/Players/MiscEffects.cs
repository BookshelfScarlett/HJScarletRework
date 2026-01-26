using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public int CalamityValue = HJScarletMethods.HasFuckingCalamity.ToInt();
        public override void PostUpdateMiscEffects()
        {
            UpdateTimer();
            UpdateFlybackBuff();
            if(Player_RewardofKingdom && RewardofWarriorCounter > 0)
                Player.statDefense += KingdomDefenseTime;
            GeneralCrtiDamageAdd = 0;
        }
        public void UpdateFlybackBuff()
        {
            //归零针buff
            bool hasBuff = (FlybackHitBuffTimer > 0 || HJScarletMethods.HasFuckingCalamity) && (Player.HeldItem.type == ItemType<FlybackHandThrown>());
            if (!hasBuff)
                return;
            //白天上午与夜间前半夜：给予15%近战伤害加成/15防御力加成
            if (HJScarletMethods.TerrariaCurrentHour <= 6)
            {
                if (Main.dayTime)
                {
                    Player.GetDamage<MeleeDamageClass>() += 0.15f + 0.15f * CalamityValue;
                    Player.GetCritChance<MeleeDamageClass>() += 15f * CalamityValue;
                }
                else
                {
                    Player.statDefense += 15 + 35 * CalamityValue;
                    Player.lifeRegen += 5 * CalamityValue;
                }
            }
            //白天下午与夜间后半夜：给予15近战速度加成/15%伤害减免
            else
            {
                if (Main.dayTime)
                {
                    Player.GetAttackSpeed<MeleeDamageClass>() += 0.15f + 0.15f * CalamityValue;
                    Player.GetCritChance<MeleeDamageClass>() += 15f * CalamityValue;
                }
                else
                {
                    Player.endurance += 0.15f + 0.35f * CalamityValue;
                    Player.lifeRegen += 5 * CalamityValue;
                }
            }
        }
        private void UpdateTimer()
        {
            if (FlybackBuffTime > 0)
                FlybackBuffTime--;
            
            if (FlybackBuffTime == 0)
                CurrentFullFlyBackTime = 0;

            if (RewardofWarriorCounter> 0)
                RewardofWarriorCounter-= 0;

            if (RewardofWarriorCounter == 0)
                KingdomDefenseTime--;

            if (KingdomDefenseTime < 0)
                KingdomDefenseTime = 0;

            if (FlybackHitBuffTimer > 0)
                FlybackHitBuffTimer--;
        }

    }
}
