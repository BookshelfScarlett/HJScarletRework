using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
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
            if (redDragonKnight)
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
            if (proj.DamageType == ExecutorDamageClass.Instance)
                GlobalExecutorOnHit(target, hit, damageDone);
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (redDragonKnight)
            {
                hit.DamageType = ExecutorDamageClass.Instance;
            }

            GlobalOnHitNPCWithSomething(target, hit, damageDone);
            if (item.DamageType == ExecutorDamageClass.Instance)
                GlobalExecutorOnHit(target, hit, damageDone);
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyCritDamage(target, ref modifiers);
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
            modifiers.SourceDamage *= sourceDamageModify;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyCritDamage(target, ref modifiers);
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
            if (monkExecutor && (modifiers.DamageType.CountsAsClass<MeleeDamageClass>() || modifiers.DamageType.CountsAsClass<SummonDamageClass>()))
            {
                switch (proj.type)
                {
                    case ProjectileID.DD2LightningAuraT1:
                    case ProjectileID.DD2LightningAuraT2:
                    case ProjectileID.DD2LightningAuraT3:
                        modifiers.FinalDamage *= 2f + shinobiExecutor.ToInt() * 8f;
                        break;
                }
            }
            modifiers.SourceDamage *= sourceDamageModify;

        }
        public void ModifyCritDamage(NPC target, ref NPC.HitModifiers modifiers)
        {
            float totalCritsBonus = 0f;
            if (CreationHatSet && modifiers.DamageType.CountsAsClass(DamageClass.Magic))
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
            //精确打击取溢出暴击率的100伤害。
            if (PreciousTargetCrtis > 0 && PreciousTargetAcc)
            {
                float critBuff = PreciousTargetCrtis / 100f;
                totalCritsBonus += critBuff;
            }
            if (modifiers.DamageType == ExecutorDamageClass.Instance)
            {
                totalCritsBonus += critDamageExecutor;
            }
            totalCritsBonus += critDamageAll;
            modifiers.CritDamage += totalCritsBonus;

        }
        public float GetWantedCrits<Type>() where Type : DamageClass
        {
            return (Player.GetTotalCritChance<Type>() + 4f - 100f);
        }
        public void GlobalExecutorOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (cowboyExecutor && cowboyRevolverTimer == 0)
            {
                cowboyRevolverTimer = Player.ApplyWeaponAttackSpeed(Player.HeldItem, Player.HeldItem.useTime, 5);
                int revolverDamage = Player.GetWeaponDamage(Player.HeldItem);
                if (Main.rand.NextBool(6))
                    revolverDamage *= 5;
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), target.Center, (-Vector2.UnitY).ToRandVelocity(ToRadians(35f), 9f, 11f), ProjectileType<CowboyRevolverProj>(), revolverDamage, 0f, Player.whoAmI);
                proj.timeLeft = 300;
                if (target.CanBeChasedBy())
                    ((CowboyRevolverProj)proj.ModProjectile).CurTarget = target;
            }
        }


        public void GlobalOnHitNPCWithSomething(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (stardustRune && stardustRuneHitHealTimer == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 randPos = target.ToRandRec();
                    Vector2 vel = RandVelTwoPi(18f, 23f);
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), randPos, vel, ProjectileType<DesterrennachtHealProj>(), 0, 0, Player.whoAmI);
                }
                stardustRuneHitHealTimer = GetSeconds(3);
            }
            if (PreciousTargetAcc && hit.Crit)
                PreciousTargetCrtis += 5;
        }
    }
}
