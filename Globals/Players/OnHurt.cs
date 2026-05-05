using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (ShadowCastAcc && Main.rand.NextBool(10))
            {
                return false;
            }
            //星月夜的自活
            if (desterrennacht && desterranRespawnChargeTimer == 0)
            {
                desterranRespawnChargeTimer = GetSeconds(90);
                desterrannachtImmortalTime = 3;
                Player.statLife = Player.statLifeMax2 / 2;
                SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { MaxInstances = 0, Pitch = 0.5f }, Player.Center);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-11f, -6f) + Vector2.UnitX * Main.rand.NextFloat(-10f, 11f);
                    Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-15f, -6f);
                    Vector2 vel = Vector2.UnitY * Main.rand.NextFloat(-8f, -1f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (desterrennacht && desterrannachtImmortalTime > 0)
            {
                SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { MaxInstances = 0, Pitch = 0.9f - desterrannachtImmortalTime * 0.1f }, Player.Center);
                desterrannachtImmortalTime--;
                Player.GetImmnue(ImmunityCooldownID.General, 60, true);
                for (int i = 0; i < 24; i++)
                    new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(20f), 1.2f, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 80, 0.1f, RandRotTwoPi).Spawn();
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                    Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }

                return true;
            }
            return base.ConsumableDodge(info);
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            ////星月夜的自活成功后的闪避，这里应该需要考虑一下……用什么钩子
            return base.FreeDodge(info);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            float totalProjDamageModify = 1f;
            float sourceDamageModify = 1f;
            if (protectorMoonglow)
            {
                if (modifiers.HitDirection == Player.direction)
                {
                    totalProjDamageModify *= 0.65f;
                }
            }
            if(goldenAppleEnchanted)
            {
                sourceDamageModify *= 0.80f;
            }
            if(goldenAppleEnchantedFully)
            {
                sourceDamageModify *= 0.01f;
            }
            modifiers.FinalDamage *= totalProjDamageModify;
            modifiers.SourceDamage *= sourceDamageModify;
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitByProjectile(proj, ref modifiers);
            float totalProjDamageModify = 1f;
            float sourceDamageModify = 1f;
            //月光花的buff，护花员的。
            if (floretProtectorExecutor)
            {
                if (modifiers.HitDirection == Player.direction)
                {
                    if ((protectorHerbTimerList[1] > 0))
                        totalProjDamageModify *= 0.80f;
                    if (protectorMoonglow)
                        totalProjDamageModify *= 0.65f;
                }
            }

            if (raincoatExecutor && (proj.velocity.Y - Player.velocity.Y) > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePos(3f), DustID.Water, RandVelTwoPi(1f, 2.1f));
                }

                totalProjDamageModify *= 0.50f;
                modifiers.Knockback *= 0;
            }
            if(goldenAppleEnchanted)
            {
                sourceDamageModify *= 0.80f;
            }
            if(goldenAppleEnchantedFully)
            {
                sourceDamageModify *= 0.01f;
            }
            if (totalProjDamageModify < 0.05f)
                totalProjDamageModify = 0.1f;

            modifiers.FinalDamage *= totalProjDamageModify;
                modifiers.SourceDamage *= sourceDamageModify;
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (raincoatExecutor && (proj.velocity.Y - Player.velocity.Y) > 0)
            {
                Player.buffImmune[BuffID.Poisoned] = true;
                Player.buffImmune[BuffID.OnFire] = true;
                Player.buffImmune[BuffID.Frostburn] = true;
            }
            base.OnHitByProjectile(proj, hurtInfo);
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (PreciousTargetAcc && info.Damage > 5)
            {
                PreciousTargetCrtis -= 300;
                if (PreciousTargetCrtis < PreciousCritsMin)
                    PreciousTargetCrtis = PreciousCritsMin;
            }
            if (Player.HasProj<MonkStaffSkillProj>())
            {
                foreach (var projID in Main.ActiveProjectiles)
                {
                    if (projID.owner != Player.whoAmI)
                        continue;
                    if (projID.DamageType != ExecutorDamageClass.Instance)
                        continue;
                    if (projID.type != ProjectileType<MonkStaffSkillProj>())
                        continue;
                    projID.Kill();
                }
            }
            monkStaffHeal = false;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
            float finalDamageModiflication = 1f;
            if (Player.HasBuff<BlackKeyExecutionBuff>() && blackKeyDefenseTrigger)
            {
                finalDamageModiflication -= blackKeyDefenseBuff;
                blackKeyDefenseTrigger = false;
            }
            modifiers.FinalDamage *= finalDamageModiflication;
            if (blackKeyHeal != 0)
            {
                Player.Heal(blackKeyHeal);
                SoundEngine.PlaySound(HJScarletSounds.Heal_Minor with { Volume = 0.75f }, Player.Center);
                //一些粒子
                new CrossGlow(Player.Center, Color.RoyalBlue, 40, 1, 0.12f).Spawn();
                new CrossGlow(Player.Center, Color.AliceBlue, 40, 1, 0.08f).Spawn();

                for (int i = 0; i < 10; i++)
                {
                    new StarShape(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 0.25f, 40).Spawn();
                }
                for (int i = 0; i < 8; i++)
                {
                    new KiraStar(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 40, 0, 1, .024f, useAlt: true).Spawn();
                }
                for (int i = 0; i < 15; i++)
                {
                    new HRShinyOrb(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 40, .0824f).Spawn();
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-11f, -6f) + Vector2.UnitX * Main.rand.NextFloat(-10f, 11f);
                    Vector2 vel = Vector2.UnitY * Main.rand.NextFloat(-6f, -1f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }
            }
        }
        private void Modifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            if (info.Cancelled)
                return;
            if (goldenAppleDamageAbsorb != 0)
            {
                //这个机制类似于灾的护盾，但是更加夸张一些
                //会把所有的伤害直接舍去对应值，相当于白给了一个永远不受敌方影响的加算防御
                //这个效果潜在来说会非常非常超模。但是我目前不想改动，为了吸引一批人来玩
                info.Damage -= goldenAppleDamageAbsorb;
                string reduceText = (-goldenAppleDamageAbsorb).ToString();
                Rectangle location = new Rectangle((int)Player.position.X, (int)Player.position.Y - 16, Player.width, Player.height);
                //CombatText.NewText(location, Color.Gold, Language.GetTextValue(reduceText));
            }
        }
    }
}
