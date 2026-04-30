using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using Steamworks;
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
        public int CalamityValue = HJScarletMethods.HasFuckingCalamity.ToInt();
        public override void PostUpdateMiscEffects()
        {
            UpdateTimer();
            UpdateFlybackBuff();
            if(goldenAppleEnchantedFully)
            {
                if (Player.miscCounter % 3 == 0 && Player.statLife < (int)(Player.statLifeMax2 * 0.9f))
                    Player.Heal(5);
            }
            critDamageAll = 0;
            //星月夜
            //爱心指环
            if (isBeingLove)
            {
                Player.moveSpeed += 0.10f;
                Player.GetAttackSpeed<GenericDamageClass>() += 0.10f;
            }
            if (tacticalExecution && tacticalTime > 0)
            {
                Player.GetDamage<ExecutorDamageClass>() *= 1.1f;
                if (EeecutorsSwordMarkPlus)
                    Player.GetCritChance<ExecutorDamageClass>() += 30;
            }
        }


        public void UpdateFlybackBuff()
        {
            //归零针buff
            bool hasBuff = (flybackInGameTimeBuff > 0) && (Player.HeldItem.type == ItemType<FlybackHandThrown>());
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
            if (flybackhandBuffTime > 0)
                flybackhandBuffTime--;

            if (flybackhandBuffTime == 0)
                flybackhandBuffTimeCurrent = 0;

            if (flybackInGameTimeBuff > 0)
                flybackInGameTimeBuff--;

            if (galvanizedHandDashCD > 0)
            {
                if (galvanizedHandDashCD == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item35, Player.Center);
                    for (int i = 0; i < 25; i++)
                    {
                        new TurbulenceShinyOrb(Player.Center.ToRandCirclePosEdge(10), 2f, RandLerpColor(Color.SkyBlue, Color.White), 120, 0.4f, RandRotTwoPi).Spawn();
                    }
                }
                galvanizedHandDashCD--;
            }

            if (desterranRespawnChargeTimer > 0)
                desterranRespawnChargeTimer--;
            if (desterranRespawnChargeTimer == 0)
            {
                desterrannachtImmortalTime = 0;
            }

            if (flybackhandCloclCD > 0)
                flybackhandCloclCD--;

            if (NoSlowFall > 0)
                NoSlowFall--;

            if (defenderEmblemCD > 0)
                defenderEmblemCD--;

            if (genderChangeTimer > 0)
                genderChangeTimer--;

            if (stardustRuneHitHealTimer > 0)
                stardustRuneHitHealTimer--;

            if (stardustRuneStaticHealTimer > 0)
                stardustRuneStaticHealTimer--;

            if (tacticalTime > 0)
                tacticalTime--;

            if (tacticalPunishTime > 0 && tacticalTime == 0)
                tacticalPunishTime--;

            if (cowboyRevolverTimer > 0)
                cowboyRevolverTimer--;

            if (blackKeyTimer > 0)
                blackKeyTimer--;
            if (floretProtectorTimer > 0)
                floretProtectorTimer--;

            protectorPlantID = Player.HasBuff<HerbBagBuff>() ? protectorPlantID : -1;
            for (int i = 0; i < protectorHerbTimerList.Length; i++)
            {
                if (protectorHerbTimerList[i] != 0)
                    protectorHerbTimerList[i]--;
            }
            if (fishDash > 0)
                fishDash--;
            if (PlayerFinalSpeedStoredTime > 0)
                PlayerFinalSpeedStoredTime--;
            if (PlayerFinalSpeedStoredTime == 0)
                PlayerLastSpeedStored = 0;
            if (exsanguinationBuffTime > 0)
                exsanguinationBuffTime--;
        }


        public override void PostUpdate()
        {
            UpdateNetPacket();
            SwitchWeaponSystem();
            if (monkStaffHeal && Player.statLife < (int)(Player.statLifeMax2 * 0.9f))
            {
                if (Player.miscCounter % 10 == 0)
                    Player.Heal(Main.rand.Next(1, 4));
                Vector2 pos = Player.Center + Vector2.UnitY * (Player.height * 0.5f);
                if (Main.rand.NextBool())
                {
                    pos.X += Main.rand.NextFloat(-1f, 1.1f) * Player.width;
                    pos.Y -= Main.rand.NextFloat(0f, 1f) * Player.height;
                    new StarShape(pos, -Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.4f), Color.Lime, 0.4f, 40).Spawn();
                }
                if (Main.rand.NextBool())
                {
                    pos = Player.Center + Vector2.UnitY * (Player.height * 0.5f);
                    pos.X += Main.rand.NextFloat(-1f, 1.1f) * Player.width;
                    pos.Y -= Main.rand.NextFloat(0f, 1f) * Player.height;
                    new ShinyCrossStar(pos, -Vector2.UnitY * Main.rand.NextFloat(0.1f, .4f), RandLerpColor(Color.Lime, Color.LimeGreen), 40, 0, 1, 0.4f, false).Spawn();
                }
            }
            //进入世界时读取刷新
            HandleWeaponAbility();
        }

        private void HandleWeaponAbility()
        {
            if (!CanWeaponSpecialAbility)
                return;
            CanWeaponSpecialAbility = false;
            if (monkExecutor && !Player.HasProj<MonkStaffProj>())
            {
                //玩家拥有任何手持的棍子都会直接处死掉，不要试图打断玩家的治疗
                foreach (var projID in Main.ActiveProjectiles)
                {
                    if (projID.owner != Player.whoAmI)
                        continue;

                    if (projID.type == ProjectileID.MonkStaffT3 || projID.type == ProjectileID.MonkStaffT3_Alt || projID.type == ProjectileID.MonkStaffT1)
                    {
                        switch (projID.type)
                        {
                            case ProjectileID.MonkStaffT3:
                            case ProjectileID.MonkStaffT3_Alt:
                                for (int i = 0; i < 66; i++)
                                {
                                    Vector2 pos = Player.Center.ToRandCirclePos(16f);
                                    Dust d = Dust.NewDustPerfect(pos, DustID.IceTorch);
                                    d.velocity = -Vector2.UnitY * Main.rand.NextFloat(0.8f, 2.6f);
                                    d.scale *= Main.rand.NextFloat(0.8f, 1.2f);
                                }
                                break;
                            default:
                                for (int i = 0; i < 66; i++)
                                {
                                    Vector2 pos = Player.Center.ToRandCirclePos(16f);
                                    Dust d = Dust.NewDustPerfect(pos, DustID.CursedTorch);
                                    d.velocity = -Vector2.UnitY * Main.rand.NextFloat(0.8f, 2.6f);
                                    d.scale *= Main.rand.NextFloat(0.8f, 1.2f);
                                }
                                break;

                        }
                        projID.Kill();
                    }
                }


                if (Player.HeldItem.type == ItemID.MonkStaffT1)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<MonkStaffProj>(), 0, 0, Player.whoAmI);
                    //标记为1说明是瞌睡章鱼
                    proj.ai[0] = 1;
                }
                if (Player.HeldItem.type == ItemID.MonkStaffT3)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<MonkStaffProj>(), 0, 0, Player.whoAmI);
                    //标记为1说明是瞌睡章鱼
                    proj.ai[0] = 0;
                }
            }
        }

        public override void PostUpdateEquips()
        {
            HandleTerraRecipe();
            ResetTerraRecipe();
            HandleLoveRing();
            UpdateFloretProtectorHerbSpawn();
            UpdateHerbBuff();
            UpdateStardustRune();
            UpdateArmorAbility();
            //小鱼冲刺。动量保存冲刺的初步实现
            if (fishDashStored && fishExecutor)
            {
                if (Main.rand.NextBool())
                    new ShinyRing(Player.ToRandRec(), (-Vector2.UnitY).ToRandVelocity(ToRadians(20f), 0f, .3f), RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, 0.012f).Spawn();
            }
            if (fishDash > 0)
            {
                //在此过程中玩家不会受到击退
                Player.noKnockback = true;
                Player.RemoveAllGrapplingHooks();
                Player.RemoveAllFishingBobbers();
                FishParticles();
                //这里基本上用帧来算
                if (fishDash < 4)
                {
                    //这里用一些硬编码的案例来实现一些可能的冲刺效果
                    float buffer = -10f;
                    float lerpValue = .35f;
                    //除非玩家的速度方向与面朝的方向相同，不然我们不会给这个加速
                    if (Player.direction == Math.Sign(Player.velocity.X))
                    {
                        buffer = 10f;
                        lerpValue = .6f;
                    }
                    Vector2 finalMoveSpeed = Player.velocity.ToSafeNormalize() * (PlayerLastSpeedStored + buffer);
                    //额外的，如果玩家正在向上移动，则根据玩家的速度方向给予一个推开的速度
                    if (Math.Abs(Player.velocity.Y) - Math.Abs(Player.velocity.X) > 0 && Player.velocity.Y < 0)
                    {
                        finalMoveSpeed += Player.direction * Vector2.UnitX * 10f;
                        lerpValue = .4f;
                    }

                    Player.velocity = Vector2.Lerp(Player.velocity, finalMoveSpeed, lerpValue);
                    //需注意的是这里的速度
                }
                else
                {
                    //查看速度情况。
                    float speedValue = PlayerLastSpeedStored > 40 ? PlayerLastSpeedStored : 40;
                    Player.velocity = Vector2.Lerp(Player.velocity, FishDashVector * speedValue, 0.5f);
                }
            }
        }

        public void FishParticles()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Player.ToRandRec(), DustID.GemSapphire);
                d.velocity = Player.velocity.ToSafeNormalize() * Main.rand.NextFloat(1.2f, 2.4f);
                d.scale = 1f;
                d.noGravity = true;
            }
            new ShinyRing(Player.ToRandRec(), (-Vector2.UnitY).ToRandVelocity(ToRadians(20f), 0.5f, 1.3f), RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, 0.032f).Spawn();
        }

        //必须得存储玩家当前的速度动量
        public float CurSpeed = 0;
        public Vector2 FishDashVector;
        private void UpdateArmorAbility()
        {
            if (!CanArmorAbility)
                return;
            CanArmorAbility = false;
            if (fishExecutor && fishDash == 0 && fishDashStored)
            {
                fishDash = 12;
                fishDashStored = false;
                //查看动量保存帧，是否在动量保存帧期间准备执行下一个冲刺
                //如果是，在原来的基础上提供1.10x的速度
                //这个时期给的非常的紧，没什么容错
                PlayerLastSpeedStored = Player.velocity.Length();
                if (PlayerFinalSpeedStoredTime > 0)
                    PlayerLastSpeedStored *= 1.1f;
                FishDashVector = Player.ToMouseVector2();
                PlayerFinalSpeedStoredTime = 4;
                Player.direction = ((Player.Center.X - Main.MouseWorld.X) < 0).ToDirectionInt();
                SoundEngine.PlaySound(HJScarletSounds.Blunt_Swing with { Pitch = 0.2f, MaxInstances = 0 }, Player.Center);
            }
        }
        public void UpdateFloretProtectorHerbSpawn()
        {
            if (floretProtectorTimer == 0 && floretProtectorExecutor)
            {
                if (Main.rand.NextBool())
                {
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
                }
                floretProtectorTimer = 40;
            }

        }

        private void UpdateStardustRune()
        {
            //星月夜。和领标之魂
            if (!souloftheTidalMark)
                return;
            if (Player.statLife < 100 && desterrennacht)
                Player.statLife = 100;
            if (stardustRuneStaticHealTimer == 0)
            {
                if (Player.statLife < Player.statLifeMax2)
                {
                    stardustRuneStaticHealTimer = GetSeconds(10);
                    Player.Heal(Math.Min((Player.statLifeMax2 - Player.statLife - 1), 75));
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

        public void HandleLoveRing()
        {
            if (!loveRing || genderChangeTimer < 1)
                return;
            foreach (var player in Main.ActivePlayers)
            {
                bool isLegalPlayer = player.whoAmI != Player.whoAmI && player.active;
                bool maleFemale = (Player.Male && !player.Male) || (!Player.Male && player.Male);
                float distance = Vector2.Distance(player.Center, Player.Center);
                if (isLegalPlayer && distance < 450f && maleFemale)
                {
                    player.HJScarlet().isBeingLove = true;

                }
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
                for(int i = 0; i < terraRecipe_EatenFoodList.Count;i++)
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
            //
            if(terraRecipe_EatenFoodCounts > 4)
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
            if (resetTerraRecipe)
            {
                //byd你tm不是复制而是类似一个引用的用法啊？？
                terraRecipe_NotEatenFoodList = new List<int>(HJScarletList.LegalFoodList);
                for(int i =0;i<terraRecipe_EatenFoodList.Count;i++)
                {
                    int index = terraRecipe_EatenFoodList[i];
                    if(!terraRecipe_NotEatenFoodList.Contains(index))
                    {
                        terraRecipe_EatenFoodList.RemoveAt(i);
                    }
                }
                for(int i =0;i<terraRecipe_NotEatenFoodList.Count;i++)
                {
                    int index = terraRecipe_NotEatenFoodList[i];
                    if(terraRecipe_EatenFoodList.Contains(index))
                    {
                        terraRecipe_NotEatenFoodList.RemoveAt(i);
                    }
                }
                //重新计算一遍当前值。
                resetEatenFoodCounts= true;
                        terraRecipe_EatenFoodCounts = 0;
                terraRecipe_LifeMaxMultTime = 0;
                resetTerraRecipe = false;
            }
        }


        public override void PostUpdateRunSpeeds()
        {
            if (NoSlowFall > 0)
            {
                Player.slowFall = false;
                Player.maxFallSpeed = maxFallspeedModify;
                Player.GoingDownWithGrapple = true;
            }
            maxFallspeedModify = 0;
        }
    }
}
