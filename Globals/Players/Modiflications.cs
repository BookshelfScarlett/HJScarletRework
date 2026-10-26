using ContinentOfJourney.Items;
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
            if (cycleMadnessLevel > 0)
            {
                int critCounts = cycleMadnessCrit * cycleMadnessLevel;
                if (critCounts > CycleMadness.MaxCrits * cycleMadnessLevel)
                    critCounts = CycleMadness.MaxCrits * cycleMadnessLevel;
                crit = critCounts;
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
                ApplyWeaponDamageMult(ref damage, item.damage, ExecutorDamageClass.Instance, 1);
            }
            if (monkExecutor)
            {
                if (item.type == ItemID.MonkStaffT3)
                    ApplyWeaponDamageMult(ref damage, item.damage, ExecutorDamageClass.Instance, 1.35f);
                if (item.type == ItemID.MonkStaffT1)
                    ApplyWeaponDamageMult(ref damage, item.damage, ExecutorDamageClass.Instance, 1.2f);
            }
            if (creationHat && item.DamageType.CountsAsClass<MagicDamageClass>() && CreationHat.Staffs.Contains(item.type))
                ApplyWeaponDamageMult(ref damage, item.damage, DamageClass.Magic, 10);
            if (creationHat && item.type == ItemType<OnyxStaff>())
                ApplyWeaponDamageMult(ref damage, item.damage, DamageClass.Magic, 5);
        }
        public void ApplyWeaponDamageMult(ref StatModifier damage, int originalDamage, DamageClass damageClass, float mult)
        {
            damage = StatModifier.Default;
            float ratios = Player.GetDamageBonusRatio(originalDamage, damageClass);
            damage *= (1 + ratios);
            damage *= mult;
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
