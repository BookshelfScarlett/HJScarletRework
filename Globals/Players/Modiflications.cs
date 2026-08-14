using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor.DragonHunter;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            if (dragonHunter && !item.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                crit = Player.GetTotalCritChance<ExecutorDamageClass>();
                if (item.DamageType.CountsAsClass<RangedDamageClass>())
                    crit += DragonHunterHead.RangedCrit;
            }
            if (Player.HasProj<GhostKnifeMark>() && item.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                crit += 10;
            }
            if (monkExecutor)
            {
                crit = Player.GetTotalCritChance<ExecutorDamageClass>() + 4;
                if (item.type == ItemID.MonkStaffT3)
                {
                    crit += 15;
                }
                if (item.type == ItemID.MonkStaffT1)
                {
                    crit += 15;
                }
            }

            //下面这个必须得最后执行
            if (PreciousTargetAcc && item.damage > 0)
            {
                crit = PreciousTargetCrtis;
                int limitedCrit = PreciousAimAcc ? 125 : 115;
                if (PreciousTargetCrtis > limitedCrit)
                    PreciousTargetCrtis = limitedCrit;
            }
            if (cycleMadness && item.damage > 0 && item.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                crit = cycleMadenessCrit;
                if (cycleMadenessCrit > 200)
                    cycleMadenessCrit = 200;
            }
        }
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if ((heartoftheCrystal || redDragonKnight) && item.DamageType.CountsAsClass(DamageClass.Magic))
            {
                mult = 0;
            }
            if (artificalManaStar)
            {
                reduce = 1;
            }

            base.ModifyManaCost(item, ref reduce, ref mult);
        }
        //潜在的问题是，这里实际上有可能因为写法差异导致出现多乘区
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (dragonHunter && !item.DamageType.CountsAsClass<ExecutorDamageClass>() && !item.DamageType.CountsAsClass<GenericDamageClass>() && item.damage > 0)
            {
                damage = StatModifier.Default;
                float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                damage *= (1f + ratios);
            }
            if (monkExecutor)
            {
                if (item.type == ItemID.MonkStaffT3)
                {
                    damage = StatModifier.Default;
                    float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                    damage *= (1 + ratios);
                    damage *= 1.35f;
                }
                if (item.type == ItemID.MonkStaffT1)
                {
                    damage = StatModifier.Default;
                    float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                    damage *= (1 + ratios);
                    damage *= 1.2f;
                }
            }
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = (int)(healValue * healingPotionMult);
            HandleCrimsonCharmEffect(item, quickHeal, ref healValue);
        }

        public void HandleCrimsonCharmEffect(Item item, bool quickHeal, ref int healValue)
        {
            bool isOverSatu = Player.HasBuff(BuffType<CrimsonCharmBuff>());
            bool pass = quickHeal || crimsonCharm || isOverSatu;
            if (!pass)
                return;
            if (isOverSatu)
            {
                int healAmt = (int)(item.healLife * healingPotionMult);
                CalOverHeal(healAmt, ref healValue);
            }
        }
        public void CalOverHeal(int healAmt, ref int healValue)
        {
            int shouldHeal = healAmt;
            shouldHeal -= CrimsonCharm.MinusHeal * (1 + crimsonCharmReduceTime);
            if (shouldHeal <= 0f)
            {
                healValue = 1;
                crimsonCharmStopReduce = true;
                return;
            }
            healValue = shouldHeal;
        }

        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            float percent = 1f;
            if (artificalManaStar)
            {
                percent -= 0.15f;
            }
            if (percent != 1f)
            {
                healValue = (int)(healValue * percent);
            }
        }
    }
}
