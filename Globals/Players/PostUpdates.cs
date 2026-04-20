using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using System;
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
                if (executorAscension)
                    Player.GetCritChance<ExecutorDamageClass>() += 30;
            }
            if (CanWeaponSpecialAbility)
            {
                CanWeaponSpecialAbility = false;
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
        }


        public override void PostUpdate()
        {
            UpdateNetPacket();
            SwitchWeaponSystem();
            if (resetTerraRecipe)
            {
                for (int i = 0; i < terraRecipe_haventEat.Count; i++)
                {
                    int index = terraRecipe_haventEat[i];
                    if (HJScarletList.LegalFoodList.Contains(index))
                        continue;
                    terraRecipe_haventEat.Remove(i);
                }

                    resetTerraRecipe = false;
            }
        }
        public override void PostUpdateEquips()
        {
            HandleTerraRecipe();
            HandleLoveRing();
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
            UpdateHerbBuff();
            UpdateStardustRune();
        }

        private void UpdateStardustRune()
        {
            //星月夜。和领标之魂
            if (!stardustRune)
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
                        new HRShinyOrb(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 40, 0, 1, .0824f).Spawn();
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-11f, -6f) + Vector2.UnitX * Main.rand.NextFloat(-10f, 11f);
                        Vector2 vel = Vector2.UnitY * Main.rand.NextFloat(-6f, -1f);
                        new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, 0, 1, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
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
            //如果列表什么都没有，我们才注册这个表单
            if (terraRecipe_haventEat.Count == 0)
                terraRecipe_haventEat = HJScarletList.LegalFoodList;
            //满足这个count的时候。我们就准提供一个血上限
            if(terraRecipe_EatenFoods > 4)
            {
                //记得重置
                terraRecipe_EatenFoods = 0;
                terraRecipe_LifeMaxMultTime += 1;
                SoundEngine.PlaySound(SoundID.ResearchComplete, Player.Center);
                Player.HealEffect(terraRecipe_LifeMaxIncre);
                for (int i = 0; i < 30; i++)
                {
                    float rotArgs = ToRadians((360f / 30 * i));
                    new ShinyCrossStar(Player.Center + Vector2.UnitX.RotatedBy( rotArgs) * 12f, rotArgs.ToRotationVector2() * 2.8f, Color.White, 40, rotArgs, 1, 1f, false).Spawn();
                }
            }
            //全局常态提供血上限。
            Player.statLifeMax2 +=  terraRecipe_LifeMaxMultTime * terraRecipe_LifeMaxIncre;
        }


        public override void PostUpdateRunSpeeds()
        {
            if (NoSlowFall > 0)
            {
                Player.slowFall = false;
                Player.maxFallSpeed = 10000;
                Player.GoingDownWithGrapple = true;
            }
        }
    }
}
