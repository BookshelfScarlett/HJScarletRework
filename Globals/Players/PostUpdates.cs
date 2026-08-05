using ContinentOfJourney;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor.ExecutorVanillaHead;
using HJScarletRework.Items.Armor.Reaper;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using HJScarletRework.Items.Weapons.Executor.Misc;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.General;
using HJScarletRework.Rarity.RarityDrawHandler;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public int CalamityValue = HJScarletMethods.HasFuckingCalamity.ToInt();
        public float blackKeyExecutorDamageAdd = 0;
        public int blackKeyExecutorCriticalChanceAdd = 0;
        public override void PostUpdateMiscEffects()
        {
            UpdateFlybackBuff();
            UpdateMisc();
            UpdateRandomMinionSpawn();
            UpdateTimer();
        }

        public override void PostUpdateEquips()
        {
            HandleTerraRecipe();
            ResetTerraRecipe();
            HandleLoveRing();
            UpdateFloretProtectorHerbSpawn();
            UpdateHerbBuff();
            UpdateStardustRune();
            UpdateDiverArmorJellyfishSpawn();
            UpdateMaidReaper();
            UpdateHeadExecutor();
            UpdateExecutorKnifeMark();
        }

        public void UpdateExecutorKnifeMark()
        {
            if (Player.HeldItem.IsExecutorWeapon() && Main.mouseLeft && !Player.IsInInventory() && Player.miscCounter % GhostKnife.MarkGhostKnifeAttackSpeed == 0f && Player.HasProj<GhostKnifeMark>())
            {
                int applyDamage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(GhostKnife.MarkGhostKnifeAttackDamage);
                Vector2 dir = Player.Center.GetNormalVector2(Main.MouseWorld);
                Vector2 off = dir.RotatedBy(PiOver2*Main.rand.NextBool().ToDirectionInt()).ToSafeNormalize();
                Vector2 pos = Player.Center - dir * 60f + off * 60;
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), pos, pos.GetNormalVector2(Main.MouseWorld)* 14f, ProjectileType<GhostKnifeProj>(), applyDamage, 1f, Player.whoAmI);
                proj.ai[1] = 1;
            pos = proj.Center+ proj.Size / 2;
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
        }

        public void UpdateSwordMark()
        {
            //判断是否佩戴处刑者剑章
            if (executorSwordMarkLevel <= 0)
                return;
            int heldType = Player.HeldItem.type;
            //手持是否为代行者武器
            if (!HJScarletList.ExecuteRequests.ContainsKey(heldType))
                return;
            //是否允许处决，且是否已经进入mark状态，这个主要是为了考虑手动处决的情况
            if (CanExecutionStrike && !executorSwordMark)
                executorSwordMark = true;
            int casterMult = executorSwordMarkLevel switch
            {
                1 => ExecutorsSwordMarkSmall.CasterExecutionProgressRegen,
                2 => ExecutorsSwordMark.CasterExecutionProgressRegen,
                3 => ExecutorsSwordMarkPlus.CasterExecutionProgressRegen,
                _ => 0,
            };
            //判定是否发起了处决，并且是否允许开始给予加成
            if (executorSwordMark && !CanExecutionStrike)
            {
                //开始给予加成
                executorSwordMarkPing = true;
                int addTime = executorSwordMarkLevel switch
                {
                    1 => ExecutorsSwordMarkSmall.ExecutionProgressRegen,
                    2 => ExecutorsSwordMark.ExecutionProgressRegen,
                    3 => ExecutorsSwordMarkPlus.ExecutionProgressRegen,
                    _ => 0,
                };
                if (HJScarletList.ExecutorTypes.TryGetValue(heldType, out var weaponType) && weaponType == ExecutorWeaponType.Caster)
                    addTime = HJScarletList.ExecuteRequests[heldType] / casterMult;
                //移除当前列表所有的处决进程
                for (int i = 0; i < ExecutionListStored.Count; i++)
                    Player.RemoveExecutionProgress(i);
                //在直接加上
                Player.AddExecutionTimeDirectly(heldType, addTime);
                //重置mark的标记状态
                executorSwordMark = false;
            }
            //暴击伤害的加成
            if (executorSwordMarkPing)
            {
                float critDamage = executorSwordMarkLevel switch
                {
                    1 => ExecutorsSwordMarkSmall.CritDamage,
                    2 => ExecutorsSwordMark.CritDamage,
                    3 => ExecutorsSwordMarkPlus.CritDamage,
                    _ => 0,
                };
                if (Player.statLife == Player.statLifeMax2)
                    critDamageExecutor += critDamage;
                if (Player.statLife >= Player.statLifeMax2 / 2)
                    critDamageExecutor += critDamage;
            }
        }

        public override void PostUpdate()
        {
            UpdateNetPacket();
            SwitchWeaponSystem();
            PostUpdateMonkHeal();
            HandleWeaponAbility();
            HandleUseableItem();
            HandleBlacKey();
            ResetExecutorCheck();
            UpdateSwordMark();
        }
        public void HandleBlacKey()
        {
            if (blackKeyExecutorDamageAdd != 0)
                Player.GetDamage<ExecutorDamageClass>() += blackKeyExecutorDamageAdd;
            if (blackKeyExecutorCriticalChanceAdd != 0)
                Player.GetCritChance<ExecutorDamageClass>() += blackKeyExecutorCriticalChanceAdd;
        }

        #region 如是我闻
        /// <summary>
        /// 控制如是我闻的核心逻辑：随机召唤物生成
        /// <br>该方法位于<see cref="PostUpdateMiscEffects"/>内</br>
        /// </summary>
        public void UpdateRandomMinionSpawn()
        {
            //生成装饰射弹
            if (!Player.HasProj<RuShiWoWenProj>(out int projID) && powerLilyVanity)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, projID, 0, 0, Player.whoAmI);

            if (!powerLily)
            {
                //确认玩家没有佩戴的情况下立刻杀死召唤物
                //这里对比的是缓存的计时与当前的计时是否处于相同态
                if (powerLilyTimer > 0)
                {
                    KillMinion();
                    powerLilyTimer = 0;
                }
                return;
            }
            //召唤物计时器处于0，即此时被初始化时，再次佩戴会给10秒的帧
            if (powerLilyTimer == 0)
            {
                powerLilyTimer = powerLilyCacheTimer + GetSeconds(5);
                return;
            }
            //大于1帧的时候返回，
            if (powerLilyTimer > 1)
            {
                return;
            }
            //入场清除周围的召唤物
            KillMinion();
            List<Item> hasList = [];
            int applyDmg = AddMinionToList(ref hasList);
            ScarletSound(HJScarletSounds.Misc_ManaClearUse, Player.Center, 0.85f, 1, 0.4f, 0.1f);
            Vector2 spawnPos = Player.MountedCenter - Vector2.UnitY * 100f;
            for (int i = 0; i < hasList.Count; i++)
            {
                Item item = hasList[i];
                Projectile proj = ContentSamples.ProjectilesByType[item.shoot];
                int dmg = (int)Player.GetTotalDamage<SummonDamageClass>().ApplyTo(applyDmg);
                var src = new EntitySource_ItemUse_WithAmmo(Player, item, AmmoID.None);
                ItemLoader.Shoot(item, Player, src, spawnPos, RandDirTwoPi, proj.type, dmg, item.knockBack);
            }
            SetRespawnParticle(spawnPos);
            //重置timer
            powerLilyTimer = GetSeconds(RuShiWoWen.Cooldown) + 1;
        }
        /// <summary>
        /// 粒子生成
        /// </summary>
        /// <param name="spawnPos"></param>
        public void SetRespawnParticle(Vector2 spawnPos)
        {
            float glowScale = .36f;
            ECSParticle.CrossGlow(spawnPos, Color.HotPink, 45, 1, glowScale);
            ECSParticle.CrossGlow(spawnPos, Color.Pink, 45, 1, glowScale * .95f);
            ECSParticle.CrossGlow(spawnPos, Color.White, 45, 1, glowScale * .90f);
            //特效相关
            for (int i = 0; i < 6; i++)
            {
                Color color = RandLerpColor(Color.LightPink, Color.Violet);
                new NoiseShockRing(spawnPos, Vector2.Zero, color, 45, 1f, .5f + i * 0.2f, -1, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 50; i++)
                ECSParticle.TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(60), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.Pink, Color.LightPink), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
            ScreenDarknessSystem.AddScreenDarkness(0.75f, 10, 5, 30, easeOut: EaseInCubic);
        }
        /// <summary>
        /// 将召唤的仆从列表加入表单，并返回当前批次的最低伤害值
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public int AddMinionToList(ref List<Item> items)
        {

            float curSlots = Player.maxMinions - Player.slotsMinions;
            int applyDmg = -1;
            while (curSlots >= 1)
            {
                //武器列表
                int itemID = Main.rand.NextFromCollection(HJScarletList.SummonWeaponList);
                Item item = ContentSamples.ItemsByType[itemID];
                if (applyDmg == -1)
                    applyDmg = item.damage;
                else
                {
                    applyDmg = Math.Min(applyDmg, item.damage);
                }
                Projectile proj = ContentSamples.ProjectilesByType[item.shoot];
                if (curSlots >= proj.minionSlots&&!items.Contains(item))
                {
                    if (itemID < VanillaMaxItem)
                    {
                        if (!ruShiWoWenBanMinionNameList.Contains(itemID.ToString()))
                        {
                            items.Add(item);
                            curSlots -= proj.minionSlots;
                        }
                               
                    }
                    else
                    {
                        if (!ruShiWoWenBanMinionNameList.Contains(item.ModItem.FullName))
                        {
                            items.Add(item);
                            curSlots -= proj.minionSlots;
                        }
                    }
                }
            }
            return applyDmg;
        }
        /// <summary>
        /// 击杀仆从的快捷方法
        /// <br>只用于<see cref="RuShiWoWen"/>如是我闻</br>
        /// </summary>
        public void KillMinion()
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (Main.myPlayer != Player.whoAmI)
                    continue;
                if (proj.owner != Player.whoAmI)
                    continue;
                if (!proj.minion)
                    continue;
                proj.Kill();
                proj.active = false;
            }

        }
        /// <summary>
        /// 如是我闻ban召唤物的逻辑
        /// <br>该方法位于<see cref="HandleUseableItem"/>，即钩子<see cref="PostUpdate"/>内</br>
        /// </summary>
        /// <param name="itemHover"></param>
        public void RuShiWoWenMinionBanHandler(Item itemHover)
        {
            int id = itemHover.type;
            bool isMinion = HJScarletList.SummonWeaponList.Contains(id);
            int totalMinionCount = HJScarletList.SummonWeaponList.Count;
            if (totalMinionCount - ruShiWoWenBanMinionNameList.Count < RuShiWoWen.MinMinionSelected())
                return;
            if (!isMinion)
                return;
            if (!HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                return;
            //判定是否为原版的召唤物
            if (id < VanillaMaxItem)
            {
                //直接存这个id，原版的召唤物id是固定的
                if (!ruShiWoWenBanMinionNameList.Contains(id.ToString()))
                    ruShiWoWenBanMinionNameList.Add(id.ToString());
            }
            else
            {
                //否则模组物品的全名
                if (!ruShiWoWenBanMinionNameList.Contains(itemHover.ModItem.FullName))
                    ruShiWoWenBanMinionNameList.Add(itemHover.ModItem.FullName);
            }
            ScarletSound(HJScarletSounds.Misc_Spell, Player.Center, pitch: .2f, volume: .6f);
        }
        public void HoverRuShiWoWen(ref Item[] inventory, int context, int slot)
        {
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed && ruShiWoWenBanTimer == 0)
            {
                ruShiWoWenBanTimer = 30;
                if (!inventory[slot].IsLegal())
                    return;
                Item item = inventory[slot];
                if (item.type != ItemType<RuShiWoWen>())
                    return;
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                {
                    if (ruShiWoWenBanMinionNameList.Count > 0)
                    {
                        ScarletSound(HJScarletSounds.Misc_Spell, Player.Center, pitch: -.2f, volume: .6f);
                        ruShiWoWenBanMinionNameList.RemoveAt(ruShiWoWenBanMinionNameList.Count - 1);
                    }
                }
            }
        }
        #endregion

        #region PostUpdateMiscEffects的方法
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


        public void UpdateMisc()
        {
            if (goldenAppleEnchantedFully)
            {
                if (Player.miscCounter % 3 == 0 && Player.statLife < (int)(Player.statLifeMax2 * 0.9f))
                    Player.Heal(5);
            }
            critDamageAll = 0;
            //爱心指环
            if (isBeingLove)
            {
                Player.moveSpeed += 0.10f;
                Player.GetAttackSpeed<GenericDamageClass>() += 0.10f;
            }
            //悠久果实
            if (fruitofEthernity)
            {
                foreach (var activeNPC in Main.ActiveNPCs)
                {
                    if (NPC.AnyNPCs(activeNPC.type) && !activeNPC.friendly && activeNPC.lifeMax > 5 && activeNPC.IsLegal() && ScarletNPCIDSets.DivineNPC[activeNPC.type])
                    {
                        //世界范围内存在神明类单位，降低70%伤害
                        Player.GetDamage<GenericDamageClass>() *= FruitofEternity.DamageReduceMultiplier;
                    }
                }
                //这个方法是直接给玩家的防御力加成，也就是增加50%防御力
                Player.statDefense += Player.DefenseMultiplier(FruitofEternity.DefenseMultipler);
            }
            //猩红镰刀
            if (Player.HeldItem.type == ItemType<CrimsonScythe>() && crimsonScytheDefense > 0 && antiKnockbackTime > 0)
            {
                Player.statDefense += (int)crimsonScytheDefense;
                Vector2 pos = Player.ToRandRec();
                if (Player.miscCounter % 4 == 0)
                    BloodyMetaball.SpawnParticle(pos, -Vector2.UnitY, 0.4f, PiOver2);
            }
        }
        #endregion
        public float holdingUseableTimer = 0;
        #region 手持物品管理
        public void HandleUseableItem()
        {
            Item itemMouse = Player.HeldItem;
            Item itemHover = Main.HoverItem;

            if (!itemMouse.IsLegal())
                return;
            if (!itemHover.IsLegal())
                return;
            if (itemMouse.type == ItemType<ProvidenceHolyWater>())
            {
                ProvidenceHolyWaterHandler(itemHover);
            }
            if (itemMouse.type == ItemType<UnregisteredSpiritOrigin>())
            {
                UnRegisteredSpiritOriginHandler(itemHover);
            }
            if (itemMouse.type == ItemType<PurePrismFate>())
            {
                PurePrismFateHandler(itemHover);
            }
            if(itemMouse.type == ItemType<RuShiWoWen>())
            {
                RuShiWoWenMinionBanHandler(itemHover);
            }
        }
        /// <summary>
        /// 天命圣水的使用逻辑。必须得是魔法药水，且鼠标悬停的物品必须是魔法药水
        /// </summary>
        public void ProvidenceHolyWaterHandler(Item itemHover)
        {
            bool isManaPotion = itemHover.damage < 1 && itemHover.pick == 0 && itemHover.axe == 0 && itemHover.hammer == 0 && itemHover.healMana > 0;
            if (isManaPotion)
            {
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                {
                    providenceHolyWaterHealMana = itemHover.healMana;
                    SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { Pitch = .2f }, Player.Center);
                    for (int i = 0; i < 20; i++)
                        new TurbulenceGlowOrb(Main.MouseWorld.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();

                }
            }
        }
        /// <summary>
        /// 无记名灵基的使用逻辑。必须得是武器，或者饰品，或者宝藏袋
        /// </summary>
        public void UnRegisteredSpiritOriginHandler(Item itemHover)
        {
            //必须得有伤害，必须得是武器
            bool isWeapon = itemHover.damage > 0 && itemHover.pick == 0 && itemHover.axe == 0 && itemHover.hammer == 0 && !itemHover.IsACoin && itemHover.ammo == AmmoID.None;
            //必须得有宝藏袋一名
            bool isTreasureBag = ItemID.Sets.BossBag[itemHover.type];
            bool isAccessory = (itemHover.accessory || itemHover.defense > 0) && itemHover.pick == 0 && itemHover.axe == 0 && itemHover.hammer == 0 && !itemHover.IsACoin && itemHover.ammo == AmmoID.None && !itemHover.vanity;
            if (isWeapon || isAccessory || isTreasureBag)
            {
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                {
                    if (Main.mouseItem.IsLegal())
                        Main.mouseItem.stack -= 1;
                    else
                        Player.HeldItem.stack -= 1;
                    Item targetItem = new Item();
                    bool favor = Player.HeldItem.favorited;
                    targetItem.SetDefaults(itemHover.type);
                    targetItem.favorited = favor;
                    targetItem.stack = 1;
                    Player.QuickSpawnItemDirect(Player.GetSource_FromThis(), targetItem, 1);
                    SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { Pitch = .2f }, Player.Center);
                    for (int i = 0; i < 20; i++)
                        new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
                }
            }

        }
        /// <summary>
        /// 纯净棱镜的使用逻辑。必须得是材料，或者是矿石，或者是锭
        /// </summary>
        public void PurePrismFateHandler(Item itemHover)
        {
            //必须得是材料。必须得没有伤害，必须得不是饰品，必须得什么都不会发射，必须得没有任何Buff提供，必须得可叠加（最大叠加数小于零）
            //必须得不能放置任何墙体
            bool isMate = itemHover.material && itemHover.damage < 1 && !itemHover.accessory && itemHover.shoot == ProjectileID.None && itemHover.buffType == 0 && itemHover.maxStack > 1 && itemHover.createWall == -1;
            bool whiteList = SmeltList.BarType.Contains(itemHover.type)
                          || SmeltList.OreType.Contains(itemHover.type)
                          || HJScarletList.BarsHashSet.Contains(itemHover.type)
                          || HJScarletList.OresHashSet.Contains(itemHover.type);

            bool blackList = PurePrismFate._RefusedList.Contains(itemHover.type)
                           || ItemID.Sets.Torches[itemHover.type]
                           || ItemID.Sets.IsFishingCrate[itemHover.type]
                           || ItemID.Sets.IsFishingCrateHardmode[itemHover.type]
                           || ItemID.Sets.Glowsticks[itemHover.type];

            bool blackList2 = false;
            if (itemHover.createTile != -1)
            {
                int tileID = itemHover.createTile;
                blackList2 = TileID.Sets.BasicChest[tileID] || TileID.Sets.BasicDresser[tileID] || TileID.Sets.IsAContainer[tileID];
            }
            bool legalTarget = (isMate || whiteList) && !blackList && !blackList2;
            if (!legalTarget)
                return;
            if (!HJScarletKeybinds.GeneralActionKeybind.Current)
                holdingUseableTimer = 0;

            if (HJScarletKeybinds.GeneralActionKeybind.Current && holdingUseableTimer < 40)
            {
                holdingUseableTimer++;
            }
            bool passTheContorlBarrier = HJScarletKeybinds.GeneralActionKeybind.JustPressed || (holdingUseableTimer > 10 && Player.miscCounter % 10 == 0);
            if (passTheContorlBarrier)
            {
                int stack = Main.mouseItem.IsLegal() ? Main.mouseItem.stack : Player.HeldItem.stack;
                if (stack < 3)
                    return;
                int totalStack = 0;
                for (int i = 1; i <= stack; i++)
                {
                    if (i > 300)
                        break;
                    if (i % 3 == 0)
                    {
                        totalStack++;
                    }
                }
                if (Main.mouseItem.IsLegal())

                    Main.mouseItem.stack -= (totalStack * 3);
                else
                    Player.HeldItem.stack -= (totalStack * 3);
                Item targetItem = new Item();
                bool favor = Player.HeldItem.favorited;
                targetItem.SetDefaults(itemHover.type);
                targetItem.favorited = favor;
                Player.QuickSpawnItemDirect(Player.GetSource_FromThis(), targetItem, totalStack);
                SoundEngine.PlaySound(HJScarletSounds.Misc_Ding, Player.Center);
                for (int i = 0; i < 20; i++)
                    new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
            }
        }
        #endregion
        private void PostUpdateMonkHeal()
        {
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
        }
        public int HoverItemIndex = -1;
        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            mouseHoveringBanWeaponAbility = inventory[slot].IsLegal();
            ClearUpParticle(ref inventory, context, slot);
            HoverSwitchWeapon(ref inventory, context, slot);
            HoverRuShiWoWen(ref inventory, context, slot);
            return false;
        }
        public void HoverSwitchWeapon(ref Item[] inventory, int context, int slot)
        {
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed && swapTimer == 0)
            {
                swapTimer = 30;
                if (!inventory[slot].IsLegal())
                    return;
                Item item = inventory[slot];
                if (WeaponSwapMaps.TryGetValue(item.type, out int value))
                {
                    DoSwapWeapon(ref inventory, item, slot, value, false);
                    return;
                }
                int reverseWeapon = GetReverseWeapon(item.type);
                if (reverseWeapon != -1)
                {
                    Main.NewText(1);
                    DoSwapWeapon(ref inventory, item, slot, reverseWeapon, true);
                    return;
                }
            }
        }
        private void DoSwapWeapon(ref Item[] inventory, Item originalItem, int slot, int targetItemID, bool altPrefix)
        {
            Item targetItem = new Item();
            int prefix = originalItem.prefix;
            bool favor = originalItem.favorited;
            if (altPrefix)
            {
                if (prefix == PrefixID.Demonic || prefix == PrefixID.Godly)
                    prefix = PrefixID.Legendary;
            }
            else if (prefix == PrefixID.Legendary || prefix == PrefixID.Godly)
                prefix = PrefixID.Godly;
            targetItem.SetDefaults(targetItemID);
            if (!targetItem.CanApplyPrefix(PrefixID.Legendary) && prefix == PrefixID.Legendary)
                prefix = PrefixID.Godly;
            targetItem.Prefix(prefix);
            targetItem.favorited = favor;
            inventory[slot] = targetItem;
            ScarletSound(SoundID.ResearchComplete, Player.Center);
            for (int i = 0; i < 20; i++)
                ECSParticle.TurbulenceShinyOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 1, 0.1f, RandRotTwoPi);

        }
        public void ClearUpParticle(ref Item[] inventory, int context, int slot)
        {
            if (!HJScarletConfigClient.Instance.SpecialRarity)
                return;
            Item item = inventory[slot];
            if (!item.IsLegal())
                return;
            if (!HJScarletList.ShinyRarityItemDictionary.ContainsKey(item.type))
                return;
            if (item.type == HoverItemIndex)
                return;
            RarityDrawHelper.CleanUpSparkles();
            HoverItemIndex = item.type;
        }
        private void HandleWeaponAbility()
        {
            if (Player.IsHolding<CrimsonScythe>() && !Player.HasProj<CrimsonScytheSkillProj>() && Main.mouseRight && Main.mouseRightRelease && Main.hoverItemName == "" && DownedBossSystem.downedSunGod)
            {
                Vector2 dir = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX);
                foreach (var id in Main.ActiveProjectiles)
                {
                    if (id.type != ProjectileType<CrimsonScytheHeldProj>())
                        continue;
                    if (id.owner != Player.whoAmI)
                        continue;
                    id.ai[0] = 114514;
                    dir = id.velocity;
                    id.Kill();
                }
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, dir, ProjectileType<CrimsonScytheSkillProj>(), 0, 0, Player.whoAmI);
                ((CrimsonScytheSkillProj)proj.ModProjectile).BeginTargetRotation = 0;
                ((CrimsonScytheSkillProj)proj.ModProjectile).Flip = true;
            }

            if (!CanWeaponSpecialAbility)
                return;
            CanWeaponSpecialAbility = false;
            if (monkExecutor && !Player.HasProj<MonkStaffSkillProj>())
            {
                int[] list = [ProjectileID.MonkStaffT3, ProjectileID.MonkStaffT3_Alt, ProjectileID.MonkStaffT1];
                Player.KillCertainProj(list);
                //玩家拥有任何手持的棍子都会直接处死掉，不要试图打断玩家的治疗
                if (Player.HeldItem.type == ItemID.MonkStaffT1)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<MonkStaffSkillProj>(), 0, 0, Player.whoAmI);
                    //标记为1说明是瞌睡章鱼
                    proj.ai[0] = 1;
                }
                if (Player.HeldItem.type == ItemID.MonkStaffT3)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<MonkStaffSkillProj>(), 0, 0, Player.whoAmI);
                    //标记为1说明是瞌睡章鱼
                    proj.ai[0] = 0;
                }
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
        //钛金碎片
        public void TitaniumHeadExecutorShard()
        {
            if (!(titaniumHeadExecutor && Player.HeldItem.IsExecutorWeapon() && Main.mouseLeft && Player.miscCounter % 8 == 0))
                return;
            int damage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(TitaniumHeadExecutor.ShardDamage);
            Vector2 dir = Player.Center.GetNormalVector2(Main.MouseWorld);
            Vector2 off = dir.RotatedByRandom(PiOver4).ToSafeNormalize();
            Vector2 pos = Player.Center - off * Main.rand.NextFloat(0.7f, 1.1f) * 120f;
            Projectile.NewProjectileDirect(Player.GetSource_FromThis(), pos, dir * 14f, ProjectileType<TitaniumShardHoming>(), damage, 1f, Player.whoAmI);
        }
        //精金闪电
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
            if(chlorophyteHeadExecutor && !Player.HasProj<ChlorophyteCrystalExecutor>(out int crystalLeaf))
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
            bool checkExecution = Player.GetExecutionSrike();
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed && maidReaperIndex != -1 && !checkExecution && Player.HeldItem.DamageType.CountsAsClass<ExecutorDamageClass>() && maidReaperHealTimer==0)
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
            if (Player.HeldItem.type != ItemType<CrimsonScythe>())
                return;
            infiniteFlightTime = true;
            Player.ApplyDash(ScarletContent.DashType<CrimsonScytheDash>());
            Player.jumpSpeed += 1.6f;
            Player.runAcceleration *= 1.40f;
            Player.moveSpeed += .35f;
        }
        public void UpdateDiverArmorJellyfishSpawn()
        {
            if (!diverArmor)
                return;
            if (Player.miscCounter % 45 == 0 && Player.velocity.LengthSquared() > 2f * 2f)
            {
                int damage = (int)Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(150);
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Player.velocity.ToSafeNormalize() * -3f, ProjectileType<DiverJellyFish>(), damage, 0f, Player.whoAmI);
                proj.timeLeft = GetSeconds(10);

            }
        }
        public void UpdateFloretProtectorHerbSpawn()
        {
            if (floretProtectorTimer == 0 && floretProtectorExecutor)
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

