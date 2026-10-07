using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.NetCode;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Database.Localization;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor.SaintChurch;
using HJScarletRework.Items.Useables;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.General;
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
            //星月夜的自活
            if (desterrennacht && desterranRespawnChargeTimer == 0)
            {
                desterranRespawnChargeTimer = GetSeconds(90);
                desterrannachtImmortalTime = 2;
                Player.RestoreHealthByPercent(.15f);
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
            if (saintChurch)
            {
                //是否首次死亡，如果是，将这个lastStand标记为True
                if (saintChurchLastStanding == 0)
                {
                    Player.AddBuff(BuffType<SaintChurchBuff>(), GetSeconds(10) * 60);
                    Player.RestoreHealthByPercent(SaintChurchHead.RespawnLifePercentFirst);
                    saintChurchLastStanding += 1;
                    SaintChurchUndead();
                    return false;
                }
                else
                {
                    if (Main.rand.NextFloat() < .8f)
                    {
                        saintChurchLastStanding += 1;
                        Player.RestoreHealthByPercent(SaintChurchHead.RespawnLifePercent);
                        SaintChurchUndead();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public void SaintChurchUndead()
        {
            Player.GetImmnue(ImmunityCooldownID.General, 60, true);
            ScarletSound(HJScarletSounds.Misc_Spell, Player.Center);
            new CrossGlow(Player.Center, Color.White, 45, 1, .31f).Spawn();
            //ECSParticle.CrossGlow(Player.Center, Color.White, 45, 1, .91f);
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Player.Center.ToRandCirclePos(3);
                Vector2 dir = Player.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                int lifeTime = Main.rand.Next(30, 70);
                float rot = RandRotTwoPi;
                ECSParticle.SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .8f, .35f, true, BlendState.NonPremultiplied);
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.White, .1f)), lifeTime, rot, 1f, .3f, true, BlendState.NonPremultiplied);
            }
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
            if (mayaPumper)
            {
                PumperHandler(npc.knockBackResist, ref modifiers, npc.Center);
            }
            float totalProjDamageModify = 1f;
            float sourceDamageModify = 1f;
            if (protectorMoonglow)
            {
                if (modifiers.HitDirection == Player.direction)
                {
                    totalProjDamageModify *= 0.65f;
                }
            }
            if (goldenAppleEnchanted)
            {
                sourceDamageModify *= 0.80f;
            }
            if (goldenAppleEnchantedFully)
            {
                sourceDamageModify *= 0.01f;
            }
            modifiers.FinalDamage *= totalProjDamageModify;
            modifiers.SourceDamage *= sourceDamageModify;
        }
        public void PumperHandler(float kbResistOrKB, ref Player.HurtModifiers modifiers, Vector2 beginCenter)
        {
            modifiers.Knockback *= kbResistOrKB;
            modifiers.Knockback *= 5f;
            float pitch = Main.rand.NextFromList<float>(0f, 0.13f, -0.13f);
            if (kbResistOrKB != 0)
            {
                ScreenShakeSystem.AddScreenShakes(Player.Center, 15f, 40, Player.velocity.ToRotation());
            }
            else
                ScreenShakeSystem.AddScreenShakes(Player.Center, 15f, 40, RandRotTwoPi);
            ScarletSound(HJScarletSounds.Misc_MayaPumper, Player.Center, 1, 1, pitch, 0, 1);

            if (mayaPumperParty)
            {
                int type = mayaPumperDashType;
                if (type == -1)
                    type = Main.rand.Next(PinballPurgatory.WhiteType, PinballPurgatory.PinkType + 1);
                //粉色冲刺
                mayaPumperDashType = type;
                if (type == PinballPurgatory.PinkType)
                {
                    if (mayaPumperDashTime == 0)
                        mayaPumperDashTime = GetSeconds(4);
                    SpawnPumpperParticleAlt(beginCenter.GetNormalVector2(Player.Center), Color.HotPink, Color.Pink);
                }
                else if (type == PinballPurgatory.BlueType)
                {
                    if (mayaPumperDashTime == 0)
                        mayaPumperDashTime = GetSeconds(2);
                    SpawnPumpperParticleAlt(beginCenter.GetNormalVector2(Player.Center), Color.AliceBlue, Color.SkyBlue);
                }
                else if (type == PinballPurgatory.OrangeType)
                {
                    SpawnPumpperParticleAlt(beginCenter.GetNormalVector2(Player.Center), Color.OrangeRed, Color.Orange);
                    if (mayaPumperDashTime == 0)
                        mayaPumperDashTime = GetSeconds(4);
                }
                else
                {
                    SpawnPumpperParticleAlt(beginCenter.GetNormalVector2(Player.Center), Color.White, Color.WhiteSmoke);
                    if (mayaPumperDashTime == 0)
                        mayaPumperDashTime = GetSeconds(3);
                }
            }
            else
                SpawnPumpperParticle(beginCenter.GetNormalVector2(Player.Center));
        }
        public void SpawnPumpperParticleAlt(Vector2 vel, Color beginColor, Color endColor)
        {
            for (int i = 0; i < 40; i++)
                ECSParticle.SmokeParticle(Player.Center, vel.ToRandVelocity(ToRadians(30f), 1f, 22f), RandLerpColor(beginColor, endColor), 45, RandRotTwoPi, 0.5f, 0.25f, true, BlendState.AlphaBlend);
        }

        public void SpawnPumpperParticle(Vector2 vel)
        {
            for (int i = 0; i < 40; i++)
                ECSParticle.SmokeParticle(Player.Center, vel.ToRandVelocity(ToRadians(30f), 1f, 22f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 45, RandRotTwoPi, 0.5f, 0.25f, true, BlendState.AlphaBlend);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitByProjectile(proj, ref modifiers);
            float totalProjDamageModify = 1f;
            float sourceDamageModify = 1f;
            if (mayaPumper)
                PumperHandler(proj.knockBack, ref modifiers, proj.Center);
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
            if (goldenAppleEnchanted)
            {
                sourceDamageModify *= 0.80f;
            }
            if (goldenAppleEnchantedFully)
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
            if (ScarletNPCIDSets.DivineNPC[npc.type] && fruitofEthernity)
            {
                //悠久果实效果下的玩家受到NPC攻击时，重置玩家的移动状态
                Player.velocity *= 0;
                Player.mount.Dismount(Player);
                Player.RemoveAllGrapplingHooks();
                //1/5的概率令玩家随机传送
                if (Main.rand.NextBool(FruitofEternity.TeleportChance))
                {
                    Player.UnityTeleport(Player.Center.ToRandCirclePos(150));
                }
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (fruitofEthernity)
            {
                bool hasAnyDivineNPC = false;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.IsLegal() && ScarletNPCIDSets.DivineNPC[npc.type])
                    {
                        hasAnyDivineNPC = true;
                        break;
                    }
                }
                if (hasAnyDivineNPC)
                {
                    //悠久果实效果下的玩家受到NPC攻击时，重置玩家的移动状态
                    Player.velocity *= 0;
                    Player.mount.Dismount(Player);
                    Player.RemoveAllGrapplingHooks();
                    //1/5的概率令玩家随机传送
                    if (Main.rand.NextBool(FruitofEternity.TeleportChance))
                    {
                        Player.UnityTeleport(Player.Center.ToRandCirclePos(150));
                    }
                }
            }
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
            if (mayaPumperParty && mayaPumperDashTime > 0 && mayaPumperDashType == PinballPurgatory.OrangeType)
            {
                ScarletSound(SoundID.DD2_BetsyFlameBreath, Player.Center, pitchVariance: .1f, instances: 0);
                for (int i = 0; i < 9; i++)
                {
                    int baseDamage = info.Damage;
                    Vector2 spawnPos = Player.Center - Vector2.UnitY * Main.rand.NextFloat(600f, 900f) + Vector2.UnitX * Main.rand.NextFloat(-1200f, 1200f);
                    Vector2 vel = Vector2.UnitY.ToRandVelocity(ToRadians(10), 16, 21);
                    if (Player.IsOwnerSide())
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), spawnPos, vel, ProjectileType<PinballPurgatoryFireball>(), baseDamage * 10, 1, Player.whoAmI);
                        proj.extraUpdates += 1;
                    }
                }
            }
            if (bloodThronCrown)
            {
                bloodThornCrownHit += GetSeconds(5);
                if (bloodThornCrownHit >= GetSeconds(5) * BloodThornCrown.MaxHitCounter)
                {
                    NetworkText text = Mod.GetLocalization(ScarletTextSets.CustomDeath.SuicidePath).ToNetworkText();
                    Player.Suicide(PlayerDeathReason.ByCustomReason(text), 99999, 0, false, true);
                }
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

            //处刑者剑章的判定
            if (executorSwordMarkLevel > 0)
            {
                var recoverExecutionTime = executorSwordMarkLevel switch
                {
                    1 => ExecutorsSwordMarkSmall.ExecutionProgressRegen,
                    2 => ExecutorsSwordMark.ExecutionProgressRegen,
                    3 => ExecutorsSwordMarkPlus.ExecutionProgressRegen,
                    _ => 0,
                };
                int heldType = Player.HeldItem.type;
                if (HJScarletList.ExecuteRequests.TryGetValue(heldType, out int value))
                {
                    Player.AddExecutionTimeDirectly(Player.HeldItem.type, recoverExecutionTime);
                }
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
            float finalDamageModiflication = 1f;
            if (KnifeMarkIndex == ProjectileType<DungeonKnifeMark>() && Player.ZoneDungeon && NPC.AnyDanger(true, true))
            {
                finalDamageModiflication *= .85f;
            }
            if (Player.HasBuff<BlackKeyExecutionBuff>() && blackKeyDefenseTrigger)
            {
                finalDamageModiflication -= blackKeyDefenseBuff;
                blackKeyDefenseTrigger = false;
            }
            modifiers.FinalDamage *= finalDamageModiflication;
            if (blackKeyHeal != 0 && modifiers.CooldownCounter != ImmunityCooldownID.TileContactDamage)
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
        public override void PostHurt(Player.HurtInfo info)
        {
            int iTime = iFrameHurtAdd;
            if (info.CooldownCounter != -1)
                Player.hurtCooldowns[info.CooldownCounter] += iTime;
            else
                Player.immuneTime += iTime;

            base.PostHurt(info);
        }
    }
}
