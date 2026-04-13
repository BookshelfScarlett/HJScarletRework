using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer :ModPlayer
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
                    for (int i = 0; i<25;i++)
                    {
                        new TurbulenceShinyOrb(Player.Center.ToRandCirclePosEdge(10), 2f, RandLerpColor(Color.SkyBlue, Color.White), 120 , 0.4f, RandRotTwoPi).Spawn();
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
        }


        public override void PostUpdate()
        {
            UpdateNetPacket();
            SwitchWeaponSystem();
        }
        public override void PostUpdateEquips()
        {
            HandleTerraRecipe();
            HandleLoveRing();
            if (stardustRune)
            {
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
