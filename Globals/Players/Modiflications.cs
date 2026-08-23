using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor;
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            int totalCrit = 0;
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
                if (item.type == ItemID.MonkStaffT3 || item.type == ItemID.MonkStaffT1)
                    crit += 15;
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
            int totalReduce = 0;
            if (heartoftheCrystal)
            {
                mult = 0;
            }
            if (CreationHat.Staffs.Contains(item.type))
            {
                totalReduce += 3;
            }
            if (artificalManaStar)
            {
                totalReduce += 1;
            }
            reduce -= totalReduce;
        }
        //潜在的问题是，这里实际上有可能因为写法差异导致出现多乘区
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (dragonHunter && !item.DamageType.CountsAsClass<ExecutorDamageClass>() && !item.DamageType.CountsAsClass<GenericDamageClass>() && item.damage > 0)
            {
                damage = StatModifier.Default;
                float ratios = Player.GetDamageBonusRatio(item.damage, ExecutorDamageClass.Instance);
                damage *= (1f + ratios);
            }
            if (monkExecutor)
            {
                if (item.type == ItemID.MonkStaffT3)
                {
                    damage = StatModifier.Default;
                    float ratios = Player.GetDamageBonusRatio(item.damage, ExecutorDamageClass.Instance);
                    damage *= (1 + ratios);
                    damage *= 1.35f;
                }
                if (item.type == ItemID.MonkStaffT1)
                {
                    damage = StatModifier.Default;
                    float ratios = Player.GetDamageBonusRatio(item.damage, ExecutorDamageClass.Instance);
                    damage *= (1 + ratios);
                    damage *= 1.2f;
                }
            }
            if (CreationHatSet && item.DamageType.CountsAsClass<MagicDamageClass>() && CreationHat.Staffs.Contains(item.type))
            {
                damage = StatModifier.Default;
                float ratios = Player.GetDamageBonusRatio(item.damage, DamageClass.Magic);
                damage *= (1 + ratios);
                damage *= 10;
            }
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = (int)(healValue * healingPotionMult);
            if (!crimsonCharm)
                return;
            if (item.healLife <= 0)
                return;
            int maxLife = Player.statLifeMax2;
            bool hasOverSatu = Player.HasBuff<CrimsonCharmBuff>();
            if (!hasOverSatu)
            {
                healValue = maxLife;
                return;
            }
            float percent = 1f - CrimsonCharm.MinusRatios * crimsonCharmReduceTime;
            if (percent < 0f)
                percent = 0;
            healValue = (int)(maxLife * percent);
            if (healValue < CrimsonCharm.MininumHeal)
                healValue = CrimsonCharm.MininumHeal;
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
