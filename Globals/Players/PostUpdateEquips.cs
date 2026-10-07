using ContinentOfJourney;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core;
using HJScarletRework.Core.NetCode;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor.ExecutorVanillaHead;
using HJScarletRework.Items.Armor.Reaper;
using HJScarletRework.Items.Armor.SaintChurch;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.General;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        // ==================== PostUpdateEquips ====================
        public override void PostUpdateEquips()
        {
            HandleTerraRecipe();
            ResetTerraRecipe();
            HandleLoveRing();
            UpdateFloretProtectorHerbSpawn();
            UpdateHerbBuff();
            UpdateStardustRune();
            UpdateDiverArmorJellyfishSpawn();
            UpdateHeadExecutor();
            UpdateMaidReaper();
            UpdateExecutorKnifeMark();
            UpdateShorthandFunction();
            UpdateDrowingEffect();
            UpdatePocketMirror();
            UpdateSaintChurch();
            UpdateBloodThornCrown();
        }

        public void UpdateBloodThornCrown()
        {
            if (!bloodThronCrown)
                return;
            if (HJScarletMethods.AnyBossNPCS())
            {
                Player.GetDamage<GenericDamageClass>() += BloodThornCrown.DamageMult;
            }
            else
            {
                Player.GetDamage<GenericDamageClass>() -= BloodThornCrown.DamageReduce;
                Player.GetCritChance<GenericDamageClass>() -= BloodThornCrown.Crit;
                Player.endurance += BloodThornCrown.DR;
            }
        }

        public void UpdateSaintChurch()
        {
            if (!Player.HasBuff<SaintChurchBuff>())
                saintChurchLastStanding = 0;
            if (!(saintChurch && saintChurchLastStanding > 0))
                return;
            foreach (var p in Main.ActivePlayers)
            {
                if (p.team == Player.team && p.whoAmI != Player.whoAmI)
                {
                    p.GetDamage<GenericDamageClass>() += SaintChurchHead.DamageBonus * saintChurchLastStanding;
                    p.GetCritChance<GenericDamageClass>() += SaintChurchHead.CritBonus * saintChurchLastStanding;
                }
            }
            Player.aggro += SaintChurchHead.Aggro;
            Player.GetDamage<GenericDamageClass>() += SaintChurchHead.DamageBonus * saintChurchLastStanding;
            Player.GetCritChance<GenericDamageClass>() += SaintChurchHead.CritBonus * saintChurchLastStanding;
            Player.AddBuff(BuffID.PotionSickness, 2);

            //不死巴卢的粒子与特效


        }

        public void UpdatePocketMirror()
        {
            if (!pocketMirror)
                return;
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                Vector2 spawnPoint = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
                if (Player.SpawnX != -1 && Player.SpawnY != -1)
                    spawnPoint = new Vector2(Player.SpawnX * 16, Player.SpawnY * 16 - 32);
                Player.UnityTeleport(spawnPoint);
            }
        }

        public void HandleTerraRecipe()
        {
            //由于需要提供血上限，这里基本上得往reset这里写内容。
            if (!terraRecipe)
                return;
            //在吃食时，或者进入世界时，都会依据当前的食物表单来查看需要的血上限
            if (resetEatenFoodCounts)
            {
                //记得重置
                //遍历这个表单。
                for (int i = 0; i < terraRecipe_EatenFoodList.Count; i++)
                {
                    //每次达到第五个，我们都重置这个计算用的单位
                    terraRecipe_EatenFoodCounts += 1;
                    if (terraRecipe_EatenFoodCounts > 4)
                    {
                        terraRecipe_EatenFoodCounts = 0;
                        //lifeMaxMultTime会在这个地方递增
                        terraRecipe_LifeMaxMultTime += 1;
                    }
                }
                resetEatenFoodCounts = false;

            }
            if (terraRecipe_EatenFoodCounts > 4)
            {
                terraRecipe_EatenFoodCounts = 0;
                terraRecipe_LifeMaxMultTime += 1;
                SoundEngine.PlaySound(SoundID.ResearchComplete, Player.Center);
                Player.HealEffect(terraRecipe_LifeMaxIncre);
                for (int i = 0; i < 30; i++)
                {
                    float rotArgs = ToRadians((360f / 30 * i));
                    new ShinyCrossStar(Player.Center + Vector2.UnitX.RotatedBy(rotArgs) * 12f, rotArgs.ToRotationVector2() * 2.8f, Color.White, 40, rotArgs, 1, 1f, false).Spawn();
                }
            }
            //全局常态提供血上限。
            Player.statLifeMax2 += terraRecipe_LifeMaxMultTime * terraRecipe_LifeMaxIncre;
        }

        public void ResetTerraRecipe()
        {
            if (!resetTerraRecipe)
                return;
            //byd你tm不是复制而是类似一个引用的用法啊？？
            terraRecipe_NotEatenFoodList = new List<int>(HJScarletList.LegalFoodList);
            for (int i = 0; i < terraRecipe_EatenFoodList.Count; i++)
            {
                int index = terraRecipe_EatenFoodList[i];
                if (!terraRecipe_NotEatenFoodList.Contains(index))
                {
                    terraRecipe_EatenFoodList.RemoveAt(i);
                }
            }
            for (int i = 0; i < terraRecipe_NotEatenFoodList.Count; i++)
            {
                int index = terraRecipe_NotEatenFoodList[i];
                if (terraRecipe_EatenFoodList.Contains(index))
                {
                    terraRecipe_NotEatenFoodList.RemoveAt(i);
                }
            }
            //重新计算一遍当前值。
            resetEatenFoodCounts = true;
            terraRecipe_EatenFoodCounts = 0;
            terraRecipe_LifeMaxMultTime = 0;
            resetTerraRecipe = false;
        }

        public void HandleLoveRing()
        {
            if (!loveRing || genderChangeTimer < 1)
                return;
            foreach (var player in Main.ActivePlayers)
            {
                bool isLegalPlayer = player.whoAmI != Player.whoAmI && player.active;
                bool maleFemale = (player.Male && !player.Male) || (!player.Male && player.Male);
                float distance = Vector2.Distance(player.Center, Player.Center);
                if (isLegalPlayer && distance < 450f && maleFemale)
                {
                    player.HJScarlet().isBeingLove = true;

                }
            }

        }

        public void UpdateFloretProtectorHerbSpawn()
        {
            if (floretProtectorTimer == 0 && floretProtectorExecutor && Player.IsOwnerSide())
            {
                if (Main.rand.NextBool())
                    return;
                bool collision = false;
                Vector2 spawnPos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Player.Center, new Vector2(1300f, 700f)));
                float recDistanceMult = 1f;
                while (!collision)
                {
                    //添加一个安全性的收缩倍率检查，如果收缩的倍率已经少于0.5f,立刻跳出去避免出现可能的死生成
                    //也就是说我们会确保其生成一个，但只会进行一定程度的安全检查
                    if (Collision.SolidCollision(spawnPos, 100, 100) && recDistanceMult > 0.5f)
                    {
                        recDistanceMult -= 0.1f;
                        //一定程度上收缩倍率以查看是否可能玩家处于一些物块内的情况，如洞穴层
                        //这里有个问题是，可能不会很完美地检测所有情况，如玩家处于地表站立在地面上时，有草药生成在了地下，则重新取位时可能会因此收缩了一定的距离
                        //但应该问题不大。
                        spawnPos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Player.Center, new Vector2(1300f * recDistanceMult, 700f * recDistanceMult)));
                    }
                    else
                    {
                        //在最后我们在推开这个草药一定距离。
                        if ((spawnPos - Player.Center).LengthSquared() < 50f * 50f)
                            spawnPos += RandVelTwoPi(30f, 70f);
                        break;
                    }
                }
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), spawnPos, RandVelTwoPi(2f, 6f), ProjectileType<FloatingPlants>(), 0, 0, Player.whoAmI);
                proj.rotation = RandRotTwoPi;
                proj.ai[1] = Main.rand.Next(0, 7);
                floretProtectorTimer = 40;
            }
        }

        public void UpdateHerbBuff()
        {
            if (!floretProtectorExecutor)
                return;
            if (!Player.HasBuff<HerbBagBuff>())
                return;
            //遍历所有的目标准备赋效果
            //妈的，天塌下来了你也只能这么打表
            //太阳花，已有buff
            if (protectorHerbTimerList[0] > 0)
            {
                Player.lifeRegen += 2;
                Player.statDefense += 10;
            }
            //月光花，已有buff
            if (protectorHerbTimerList[1] > 0)
            {
                Player.endurance += 0.08f;
            }
            //闪耀根，已有buff
            if (protectorHerbTimerList[2] > 0)
            {
                Player.pickSpeed -= 0.3f;

            }
            //水叶草，已有Buff
            if (protectorHerbTimerList[3] > 0)
            {
                Player.luck += 25;
            }
            //死亡草，已有Buff
            if (protectorHerbTimerList[4] > 0)
            {
                Player.GetDamage<ExecutorDamageClass>() += 0.10f;
                Player.GetCritChance<ExecutorDamageClass>() += 10f;
            }
            //火焰花，已有Buff
            if (protectorHerbTimerList[6] > 0)
            {
                if (Collision.LavaCollision(Player.Center, Player.width, Player.height))
                {
                    Player.GetDamage<ExecutorDamageClass>() += 0.15f;
                    Player.GetCritChance<ExecutorDamageClass>() += 15f;
                }
            }
        }

        private void UpdateStardustRune()
        {
            //星月夜。和领标之魂
            if (!souloftheTidalMark)
                return;
            int minLife = desterrennacht ? 20 : 5;
            if (Player.statLife < minLife)
                Player.statLife = minLife;
            if (!desterrennacht)
                return;
            if (stardustRuneStaticHealTimer != 0)
                return;
            if (Player.statLife < Player.statLifeMax2)
            {
                stardustRuneStaticHealTimer = GetSeconds(20);
                Player.Heal(Math.Min((Player.statLifeMax2 - Player.statLife - 1), 20));
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

        public void UpdateDiverArmorJellyfishSpawn()
        {
            if (!diverArmor)
                return;
            if (Player.miscCounter % 45 == 0 && Player.velocity.LengthSquared() > 2f * 2f && Player.IsOwnerSide())
            {
                int damage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(50);
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Player.velocity.ToSafeNormalize() * -3f, ProjectileType<DiverJellyFish>(), damage, 0f, Player.whoAmI);
                proj.timeLeft = GetSeconds(10);

            }
        }
        #region 代行者的矿石套
        public void UpdateHeadExecutor()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;
            TitaniumHeadExecutorShard();
            AdamantiteHeadExecutorThunder();
            ChlorophyteHeadExecutorCrystal();
        }

        public void TitaniumHeadExecutorShard()
        {
            bool canUseTitan = titaniumHeadExecutor && Player.HeldItem.IsExecutorWeapon() && Player.MouseLeftOf() && Player.miscCounter % 9 == 0 && !Player.IsInInventory();
            if (!canUseTitan)
                return;
            int damage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(TitaniumHeadExecutor.ShardDamage);
            Vector2 dir = Player.Center.GetNormalVector2(Main.MouseWorld);
            Vector2 off = dir.RotatedByRandom(PiOver4).ToSafeNormalize();
            Vector2 pos = Player.Center - off * Main.rand.NextFloat(0.7f, 1.1f) * 120f;
            Projectile.NewProjectileDirect(Player.GetSource_FromThis(), pos, dir * 14f, ProjectileType<TitaniumShardHoming>(), damage, 1f, Player.whoAmI);
        }

        public void AdamantiteHeadExecutorThunder()
        {
            if (!(adamantiteHeadExecutor && adamantiteHeadExecutorThunderTimer == 0))
                return;
            float searchDist = 1100f;
            List<NPC> availableTarget = [];
            foreach (NPC needTar in Main.ActiveNPCs)
            {
                if (availableTarget.Count >= AdamantiteHeadExecutor.ThunderCount)
                    break;
                bool legalTarget = needTar.CanBeChasedBy();
                float distPerTar = Vector2.Distance(needTar.Center, Player.Center);
                if (legalTarget && distPerTar < searchDist)
                {
                    availableTarget.Add(needTar);
                }
            }
            if (availableTarget.Count == 0)
            {
                return;
            }
            for (int i = 0; i < availableTarget.Count; i++)
            {
                NPC target = availableTarget[i];
                if (!target.IsLegal())
                    continue;

                ScarletSound(HJScarletSounds.Lightning_Strike, Player.Center, 0.4f, 1, 0.35f);
                Vector2 pos = Player.Center - Vector2.UnitY * Main.rand.NextFloat(800f, 900f) + Vector2.UnitX * Main.rand.NextFloat(0f, 20f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 vel = (target.Center - pos).ToSafeNormalize() * Main.rand.NextFloat(4f, 9f);
                int damage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(AdamantiteHeadExecutor.ThunderDamage);
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), pos, vel, ProjectileType<AdamantiteThunder>(), damage, 3f, Player.whoAmI);
                ((AdamantiteThunder)proj.ModProjectile).CurTarget = target;
            }
            adamantiteHeadExecutorThunderTimer = GetSeconds(AdamantiteHeadExecutor.StrikeChance);
        }

        public void ChlorophyteHeadExecutorCrystal()
        {
            if (chlorophyteHeadExecutor && !Player.HasProj<ChlorophyteCrystalExecutor>(out int crystalLeaf))
            {
                int damage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(ChlorophyteHeadExecutor.BoltDamage);
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, crystalLeaf, damage, 2, Player.whoAmI);
                proj.originalDamage = damage;
            }

        }
        #endregion

        public void UpdateMaidReaper()
        {
            if (!maidReaperArmor)
                return;
            if (Player.IsHolding<CrimsonScythe>())
            {
                infiniteFlightTime = true;
                Player.ApplyDash(ScarletContent.DashType<CrimsonScytheDash>());
                Player.jumpSpeed += 1.6f;
                Player.runAcceleration *= 1.40f;
                Player.moveSpeed += .35f;
                Player.GetDamage<ExecutorDamageClass>() += ReaperHead.TlipocaScytheDamage;
                Player.GetCritChance<ExecutorDamageClass>() += ReaperHead.TlipocaScytheCrits;
                Player.statDefense += ReaperHead.TlipocaDefense;
                critDamageExecutor += ReaperHead.TlipocaCritDamage;
            }

            bool checkExecution = Player.CheckCurWeaponExecution(Player.HeldItem.type);
            if (checkExecution)
                return;
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed && maidReaperIndex != -1 &&
                Player.HeldItem.DamageType.CountsAsClass<ExecutorDamageClass>() && maidReaperHealTimer == 0 && Player.IsOwnerSide())
            {
                NPC npc = Main.npc[maidReaperIndex];
                if (npc.IsLegal())
                {
                    ScarletSound(HJScarletSounds.Tlipoca_SoulAbsorb, Player.Center, 0.85f, 1, 0.3f);
                    float ratios = Clamp((float)ExecutionListStored[Player.HeldItem.type] / (float)HJScarletList.ExecuteRequests[Player.HeldItem.type], 0, 1);
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), npc.Center, RandVelTwoPi(6, 9), ProjectileType<MaidReaperHeal>(), 0, 0, Player.whoAmI);
                    proj.ai[2] = ratios;
                    ((MaidReaperHeal)proj.ModProjectile).CurTarget = npc;
                    Player.RemoveExecutionProgress(Player.HeldItem.type);
                    maidReaperHealTimer = GetSeconds((int)(Lerp(0, ReaperHead.MaidReaperMaxHealCooldown, ratios)));
                }
            }
        }

        public void UpdateExecutorKnifeMark()
        {
            if (!Player.HeldItem.IsExecutorWeapon())
                return;
            //希望之星
            if (KnifeMarkIndex == ProjectileType<StarofHoperMark>()
                && !Player.IsInInventory() && Main.mouseLeft && Player.miscCounter % 25f == 0 && Player.IsOwnerSide())
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = Player.Center - Vector2.UnitY * Main.rand.NextFloat(600, 700) + Main.rand.NextFloat(80, 120) * i * Vector2.UnitX * Main.rand.NextBool().ToDirectionInt() - ((Main.MouseWorld.X - Player.Center.X) > 0).ToDirectionInt() * Vector2.UnitX * 100;
                    int dmg = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(20);
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), pos, pos.GetNormalVector2(Main.MouseWorld) * 30f, ProjectileType<StarofHopeStar>(), dmg, 1f, Player.whoAmI);
                    proj.HJScarlet().HasExecutionMechanic = true;
                }

            }
            //克苏鲁的眼泪
            if (tearEyeBuff > 0)
            {
                Player.AddBuff(BuffID.Wet, GetSeconds(1));
                Player.endurance += (1 + Condition.Hardmode.IsMet().ToInt() + DownedBossSystem.downedBarrier.ToInt()) * .05f;
            }
            //幽灵短匕
            if (Main.mouseLeft && !Player.IsInInventory()
                && Player.miscCounter % GhostKnife.MarkGhostKnifeAttackSpeed == 0f
                && KnifeMarkIndex == GetInstance<GhostKnifeMark>().Type
                && Player.IsOwnerSide())
            {
                int applyDamage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(GhostKnife.MarkGhostKnifeAttackDamage);
                Vector2 dir = Player.Center.GetNormalVector2(Main.MouseWorld);
                Vector2 off = dir.RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt()).ToSafeNormalize();
                Vector2 pos = Player.Center - dir * 60f + off * 60;
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), pos, pos.GetNormalVector2(Main.MouseWorld) * 14f, ProjectileType<GhostKnifeProj>(), applyDamage, 1f, Player.whoAmI);
                proj.ai[1] = 1;
                pos = proj.Center + proj.Size / 2;
                for (int i = 0; i < 8; i++)
                {
                    ECSParticle.ShinyCrossStarECS(pos, RandVelTwoPi(1.2f, 2.2f), Color.White, 40, 1, 0.4f);
                }
                for (int i = 0; i < 6; i++)
                {
                    ECSParticle.SmokeParticle(pos, RandVelTwoPi(1.2f, 3.2f), RandLerpColor(Color.White, Color.LightSkyBlue), 40, 1, 1, 0.21f, blendstate: BlendState.Additive);
                }
                ECSParticle.StarShape(pos, proj.velocity.ToSafeNormalize() * .01f, Color.LightBlue, 40, 1, 0.94f);
                ECSParticle.StarShape(pos, proj.velocity.RotatedBy(PiOver2).ToSafeNormalize() * .01f, Color.LightBlue, 40, 1, 0.94f);
            }
            //公爵长牙
            if (KnifeMarkIndex == ProjectileType<FishronKnifeMark>())
            {
                Player.GetDamage<ExecutorDamageClass>() += FishronKnife.DamageBuff;
            }
            //钢制匕首
            if (KnifeMarkIndex == ProjectileType<DungeonKnifeMark>())
            {
                Player.statDefense += 8;

            }
        }

        public void UpdateShorthandFunction()
        {
            if (infiniteBreath)
            {
                Player.breathCD = 0;
            }
            if (ankhShieldImmnue)
            {
                Player.buffImmune[67] = true;
                Player.buffImmune[24] = true;
                Player.buffImmune[323] = true;
                Player.buffImmune[33] = true;
                Player.buffImmune[30] = true;
                Player.buffImmune[20] = true;
                Player.buffImmune[36] = true;
                Player.buffImmune[31] = true;
                Player.buffImmune[22] = true;
                Player.buffImmune[35] = true;
                Player.buffImmune[46] = true;
                Player.buffImmune[32] = true;
                Player.buffImmune[23] = true;
                Player.buffImmune[156] = true;
                Player.buffImmune[70] = true;
                Player.buffImmune[39] = true;
                Player.buffImmune[69] = true;
            }
            if (terraSparkBoostImmnue)
            {
                Player.iceSkate = true;
                Player.waterWalk = true;
                Player.lavaImmune = true;
                Player.lavaRose = true;
            }
            if (celesitalShellEffect)
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
                Player.GetDamage(DamageClass.Generic) += 0.1f;
                Player.GetCritChance(DamageClass.Generic) += 3f;
                Player.GetKnockback(DamageClass.Summon) += 0.5f;
                Player.pickSpeed += 0.15f;
            }
        }

        public void UpdateDrowingEffect()
        {
            if (Player.IsInwater())
            {
                if (LightofHorizon)
                {
                    Lighting.AddLight(Player.Center, new Vector3(255, 255, 255) / 35f);
                }
            }
            else
            {
                if (LightofHorizon)
                    Lighting.AddLight(Player.Center, new Vector3(255, 255, 255) / 70f);

            }
        }
    }
}
