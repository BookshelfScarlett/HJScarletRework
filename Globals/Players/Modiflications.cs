using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
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
            if(cycleMadness && item.damage > 0 && item.DamageType.CountsAsClass<ExecutorDamageClass>())
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
            if (redDragonKnight && item.DamageType != ExecutorDamageClass.Instance && !item.DamageType.CountsAsClass<GenericDamageClass>() && item.damage > 0)
            {
                damage = StatModifier.Default;
                float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                damage *= (1f + ratios);

                if (item.consumable && item.DamageType.CountsAsClass<RangedDamageClass>())
                    damage *= 1.10f;
                if (item.DamageType.CountsAsClass<MagicDamageClass>())
                    damage *= .80f;
            }
            if (monkExecutor)
            {
                if (item.type == ItemID.MonkStaffT3)
                {
                    damage = StatModifier.Default;
                    float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                    damage *= (1 + ratios);
                    damage *= 1.5f;
                }
                if (item.type == ItemID.MonkStaffT1)
                {
                    damage = StatModifier.Default;
                    float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                    damage *= (1 + ratios);
                    damage *= 1.2f;
                }
            }
            base.ModifyWeaponDamage(item, ref damage);
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (runeWizardExecutor && item.DamageType == ExecutorDamageClass.Instance)
            {
                int count = Main.rand.Next(2, 4);
                SoundEngine.PlaySound(SoundID.Item43 with { MaxInstances = 1, Pitch = 0.4f, PitchVariance = 0.2f, Volume = 0.75f }, Player.Center);
                NPC target = Main.MouseWorld.FindClosestTarget(400f, ignoreTiles: false);
                for (int i = 0; i < count; i++)
                {
                    Vector2 vel = Player.ToMouseVector2().ToRandVelocity(ToRadians(15f), 11f, 13f);
                    Vector2 spawnPos = Player.Center - Player.ToMouseVector2() * Main.rand.NextFloat(150f, 190f);
                    Projectile proj = Projectile.NewProjectileDirect(source, spawnPos.ToRandCirclePosEdge(50f), vel, ProjectileType<RuneWizardProj>(), (damage / 2) / count, 1f, Player.whoAmI);
                    if (target.IsLegal())
                        proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                }
            }
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
