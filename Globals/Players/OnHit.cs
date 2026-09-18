using ContinentOfJourney.Buffs;
using ContinentOfJourney.Items;
using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Projs.General;
using rail;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public float critDamageAll = 0f;
        public float critDamageExecutor = 0;
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (dragonHunter)
            {
                hit.DamageType = ExecutorDamageClass.Instance;
            }
            if (monkExecutor && (hit.DamageType.CountsAsClass<MeleeDamageClass>() || hit.DamageType.CountsAsClass<SummonDamageClass>()))
            {
                switch (proj.type)
                {
                    case ProjectileID.MonkStaffT3:
                    case ProjectileID.MonkStaffT3_Alt:
                    case ProjectileID.MonkStaffT3_AltShot:
                    case 1110:
                    case ProjectileID.MonkStaffT1:
                    case ProjectileID.DD2LightningAuraT1:
                    case ProjectileID.DD2LightningAuraT2:
                    case ProjectileID.DD2LightningAuraT3:
                        hit.DamageType = ExecutorDamageClass.Instance;
                        break;
                }

            }
            GlobalOnHitNPCWithSomething(target, hit, damageDone);
            if (proj.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                GlobalExecutorOnHit(target, hit, damageDone);
            }
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (dragonHunter)
            {
                hit.DamageType = ExecutorDamageClass.Instance;
            }
            GlobalOnHitNPCWithSomething(target, hit, damageDone);
        }
        public void ModifyHitGlobal(NPC target, ref NPC.HitModifiers modifiers)
        {
            float finalDamageMult = 1f;
            float srcDamageMult = 1f;
            srcDamageMult *= FloretProtectorModify(target, ref modifiers);
            finalDamageMult *= SpellBreakerModify(target, ref modifiers);
            modifiers.SourceDamage *= srcDamageMult;
            modifiers.FinalDamage *= finalDamageMult;
                        
        }

        public float FloretProtectorModify(NPC target, ref NPC.HitModifiers modifiers)
        {
            float sourceDamageModify = 1f;
            if (floretProtectorExecutor && modifiers.DamageType == ExecutorDamageClass.Instance)
            {
                if (protectorShiver)
                {
                    if (Main.rand.NextBool(4))
                        sourceDamageModify += 0.15f;
                }
                if (protectorHerbTimerList[5] > 0)
                {
                    if (Main.rand.NextBool(4))
                        sourceDamageModify += 0.1f;
                }
            }
            return sourceDamageModify;
        }

        private float SpellBreakerModify(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (spellBreakerLevel > 0 && modifiers.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                int dotTime = GetSeconds(2);
                //先给buff，这里的buff是打表
                switch (spellBreakerLevel)
                {
                    case 1:
                        target.AddBuff(BuffID.OnFire, dotTime);
                        target.AddBuff(BuffID.Poisoned, dotTime);
                        break;
                    case 2:
                        target.AddBuff(BuffID.CursedInferno, dotTime);
                        target.AddBuff(BuffID.Ichor, dotTime);
                        target.AddBuff(BuffID.Venom, dotTime);
                        break;
                    case 3:
                        target.AddBuff(BuffType<DivineFireBuff>(), dotTime);
                        target.AddBuff(BuffType<PlagueBuff>(), dotTime);
                        target.AddBuff(BuffType<VulnerableBuff>(), dotTime);
                        break;
                }
                //这里才开始进判定。
                if (spellBreakerTimer == 0)
                {
                    int count = GetDotCounts(target);
                    //先搜……诶。
                    //这里，正式处理最终的结算逻辑
                    if (count != 0)
                    {
                        ScarletSound(SoundID.DD2_SonicBoomBladeSlash, Player.Center);
                        float dotExtra = spellBreakerLevel switch
                        {
                            1 => SpellBreakerSmall.DamageMult,
                            2 => SpellBreaker.DamageMult,
                            3 => SpellBreakerAdvanced.DamageMult,
                            _ => 0,
                        };
                        float bounces = 1 + dotExtra * count;
                        ECSParticle.ShinyCrossStarSmall(target.Center, Vector2.Zero, Color.Orange, 45, 1, 2f, 0);
                        for (int i = 0; i < 32; i++)
                            ECSParticle.TurbulenceShinyOrb(target.Center.ToRandCirclePos(32), 1.2f, RandLerpColor(Color.Orange, Color.OrangeRed), 45, 1, Main.rand.NextFloat(.9f, 1.1f) * .23f, glowMult: .65f);
                        CombatText.NewText(target.Hitbox, Color.PaleGoldenrod, bounces + "x");
                        spellBreakerTimer = 60;
                        return bounces;
                    }
                }
            }
            //否则返回1
            return 1;
        }
        public int GetDotCounts(NPC target)
        {
            int k = 0;
            for (int i = 0; i < target.buffType.Length; i++)
            {
                int buff = target.buffType[i];
                if (target.buffTime[i] <= 0)
                    continue;
                if (buff <= 0)
                    continue;
                if (HJScarletList.DebuffListTarget.Contains(buff))
                    k+= 1;
            }
            return k;
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyCritDamage(target, ref modifiers);
            ModifyHitGlobal(target, ref modifiers);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyCritDamage(target, ref modifiers);
            float sourceDamageModify = 1f;
            if (monkExecutor && (modifiers.DamageType.CountsAsClass<MeleeDamageClass>() || modifiers.DamageType.CountsAsClass<SummonDamageClass>()))
            {
                switch (proj.type)
                {
                    case ProjectileID.DD2LightningAuraT1:
                    case ProjectileID.DD2LightningAuraT2:
                    case ProjectileID.DD2LightningAuraT3:
                        modifiers.FinalDamage *= 2f + shinobiExecutor.ToInt() * 3f;
                        break;
                }
            }
            if (preciousTargetLevel > 0 && modifiers.DamageType.CountsAsClass<RangedDamageClass>() && target.HJScarlet().isUnderPreciousTargetCross > 0)
            {
                if (preciousTargetLevel == 1)
                {
                    sourceDamageModify += (PreciousTarget.ExtraDamage - 1f);
                    if (Main.rand.NextFloat() < PreciousTarget.ChanceToCrit)
                        modifiers.SetCrit();
                }
                if (preciousTargetLevel == 2)
                {
                    sourceDamageModify += (SteadyBreath.ExtraDamage - 1f);
                    if (Main.rand.NextFloat() < SteadyBreath.ChanceToCrit)
                        modifiers.SetCrit();
                }
            }
            if (emblemColdSteel && modifiers.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                float ratios = (float)Utils.GetLerpValue(180, 0, (double)target.defDefense, true);
                float damageMult = Lerp(1, 1 + EmblemColdSteel.MaxDamageMult, ratios);
                sourceDamageModify += damageMult;
            }
            if (theBleachingBuff)
                sourceDamageModify *= .5f;

            modifiers.SourceDamage *= sourceDamageModify;
            ModifyHitGlobal(target, ref modifiers);
        }
        public void ModifyCritDamage(NPC target, ref NPC.HitModifiers modifiers)
        {
            float totalCritsBonus = 0f;
            if (creationHat && modifiers.DamageType.CountsAsClass(DamageClass.Magic))
            {
                //将所有伤害直接设置为暴击类型，这里先过暴击情况
                modifiers.SetCrit();
                //而后开始依据当前的暴击率设置需要的暴击伤害
                //首先将溢出的暴击概率等价转化
                float baseCritsbuff = Math.Max(0f, GetWantedCrits<MagicDamageClass>());
                //转化成功后，将值/2f，取暴击率的1/2（即20%-> 10%)
                baseCritsbuff /= 2f;
                //最后。直接将暴击伤害设置
                totalCritsBonus += baseCritsbuff;
            }
            if (modifiers.DamageType.CountsAsClass<ExecutorDamageClass>())
            {
                totalCritsBonus += critDamageExecutor;
            }
            if (spellBreakerLevel > 1 && spellBreakerTimer > 0 && modifiers.DamageType.CountsAsClass<MeleeDamageClass>())
                totalCritsBonus += .1f;
            totalCritsBonus += critDamageAll;
            modifiers.CritDamage += totalCritsBonus;

        }
        public float GetWantedCrits<Type>() where Type : DamageClass
        {
            return (Player.GetTotalCritChance<Type>() + 4f - 100f);
        }
        public void MaidReaperHealDamage(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public int CurHit = 0;
        public void GlobalExecutorOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (maidReaperArmor && target.IsLegal())
            {
                maidReaperIndex = target.whoAmI;
            }
        }
        public void GlobalOnHitNPCWithSomething(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (selfPortraitType > 0)
            {
                target.HJScarlet().isPotraitTimer = GetSeconds(5);
                target.HJScarlet().potraityDoT = 5;
                int c = GetDotCounts(target);
                if (c > 0)
                {
                    target.HJScarlet().potraityDoT += c * .5f;
                    if (c > 3)
                        target.AddBuff(BuffType<TheBleachingBuff>(), GetSeconds(1));
                }
            }
            if (souloftheTidalMark && stardustRuneHitHealTimer == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 randPos = target.ToRandRec();
                    Vector2 vel = RandVelTwoPi(18f, 23f);
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), randPos, vel, ProjectileType<DesterrennachtHealProj>(), 0, 0, Player.whoAmI);
                }
                stardustRuneHitHealTimer = GetSeconds(3);
            }
            if (cycleMadnessLevel > 0 && cycleMadnessCrtiStarTimer == 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), target.ToRandRec(), RandVelTwoPi(.5f, 1.1f) * 25f, ProjectileType<CycleMadnessStar>(), 0, 0, Player.whoAmI);
                cycleMadnessCrtiStarTimer = 30;
            }
        }
    }
}
