using ContinentOfJourney;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.NetCode;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor.RedDragonKnight;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using HJScarletRework.Items.Weapons.Executor.Misc;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Items.Weapons.Ranged;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.General;
using HJScarletRework.Projs.Ranged;
using HJScarletRework.Rarity.RarityDrawHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {

        // ==================== 字段 ====================
        public int CalamityValue = HJScarletMethods.HasFuckingCalamity.ToInt();
        public float blackKeyExecutorDamageAdd = 0;
        public int blackKeyExecutorCriticalChanceAdd = 0;
        public float holdingUseableTimer = 0;
        public int HoverItemIndex = -1;

        // ==================== PostUpdateMiscEffects ====================
        public override void PostUpdateMiscEffects()
        {
            UpdateFlybackBuff();
            HandleBlacKey();
            UpdateMisc();
            UpdateRandomMinionSpawn();
            UpdateTimer();
            UpdateMiscBuff();
        }



        #region PostUpdateMiscEffects 辅助方法
        public void UpdateMiscBuff()
        {
            if (absoluteZeroBuff)
            {
                Player.moveSpeed *= AbsoluteZeroBuff.BadMoveSpeed;
                Player.statDefense -= AbsoluteZeroBuff.BadDefense;
            }
            if (theBleachingBuff)
            {
                Player.statDefense *= 0;
            }
            if (!Player.HasBuff<CycleMadnessBuff>())
            {
                if (cycleMadnessCrit > 0 && Player.miscCounter % 2 == 0)
                    cycleMadnessCrit -= 5;
                if (cycleMadnessCrit < 0)
                    cycleMadnessCrit = 0;
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

        public void UpdateRandomMinionSpawn()
        {
            if (!Player.IsOwnerSide())
                return;
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
                if (curSlots >= proj.minionSlots && !items.Contains(item))
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
        #endregion



        // ==================== PostUpdate ====================
        public override void PostUpdate()
        {
            UpdateNetPacket();
            SwitchWeaponSystem();
            PostUpdateMonkHeal();
            HandleWeaponAbility();
            HandleUseableItem();
            ResetExecutorCheck();
            UpdateRedDragonKnight();
            UpdateSwordMark();
            UpdatePandorasBurgerCleanUp();
        }

        #region PostUpdate 辅助方法
        public void UpdatePandorasBurgerCleanUp()
        {
            if (!Player.IsHolding<PandorasBurger>())
                return;
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner == Player.whoAmI && proj.friendly && proj.type != ProjectileType<PandorasBurgerHeldProj>())
                    {
                        proj.active = false;
                    }
                }
            }
        }
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
            if (itemMouse.type == ItemType<RuShiWoWen>())
            {
                RuShiWoWenMinionBanHandler(itemHover);
            }
            //if(itemMouse.type == ItemType<CrystallizedLore>())
            //{
            //    CrystallizedLoreHandler(ref itemHover);
            //}
        }


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
        /// <summary>
        /// 处理传承结晶词缀问题
        /// <br>这个本质上是超级没有营养的打表</br>
        /// </summary>
        /// <param name="itemHover"></param>
        public void CrystallizedLoreHandler(ref Item itemHover)
        {
            if (itemHover.IsWeapon())
            {
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                {

                }
            }
        }

        #endregion

        public void HandleBlacKey()
        {
            if (blackKeyExecutorDamageAdd != 0)
                Player.GetDamage<ExecutorDamageClass>() += blackKeyExecutorDamageAdd;
            if (blackKeyExecutorCriticalChanceAdd != 0)
                Player.GetCritChance<ExecutorDamageClass>() += blackKeyExecutorCriticalChanceAdd;
        }
        public void UpdateRedDragonKnight()
        {
            if (!redDragonKnight)
                return;
            if (Player.miscCounter % GetSeconds(RedDragonKnightHead.ProgressRegenTime) == 0 && Main.mouseLeft && !Player.IsInInventory() && Player.HeldItem.IsExecutorWeapon())
            {
                foreach (var keys in ExecutionListStored.Keys.ToList())
                {
                    if (HJScarletList.ExecuteRequests.TryGetValue(keys, out int value))
                    {
                        int add = (int)(value * RedDragonKnightHead.ProgressRegenCount);
                        if (add == 0)
                            add = 1;
                        Player.AddExecutionTimeDirectly(keys, add);
                    }
                }
                ScarletSound(SoundID.DD2_BetsyFireballShot, Player.Center, 1, 0, pitch: .2f, .1f);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 pos = Player.ToRandRec();
                    ECSParticle.SmokeParticle(pos, -Vector2.UnitY, RandLerpColor(Color.DarkRed, Color.Red), 40, RandRotTwoPi, 1, 0.15f, blendstate: BlendState.AlphaBlend);
                }
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
        #endregion

        // ==================== PostUpdateRunSpeeds ====================
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

        // ==================== HoverSlot ====================
        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            mouseHoveringBanWeaponAbility = inventory[slot].IsLegal();
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed && swapTimer == 0 && mouseHoveringBanWeaponAbility)
            {
                HoverSevenStarOrGreatDipper(ref inventory, context, slot);
                ClearUpParticle(ref inventory, context, slot);
                HoverSwitchWeapon(ref inventory, context, slot);
                HoverRuShiWoWen(ref inventory, context, slot);
                HoverCryLore(ref inventory, context, slot);
                swapTimer = 30;
            }
            return false;
        }



        #region HoverSlot 辅助方法
        public void HoverCryLore(ref Item[] inventory, int context, int slot)
        {
            if (!inventory[slot].IsLegal())
                return;
            Item itemMouse = Player.HeldItem;
            if (!itemMouse.IsLegal())
                return;
            if (itemMouse.type != ItemType<CrystallizedLore>())
                return;
            Item item = inventory[slot];
            if (!item.CanHavePrefixes())
                return;
            if (item.IsWeapon())
            {
                if (Main.mouseItem.IsLegal())
                    Main.mouseItem.stack -= 1;
                else
                    Player.HeldItem.stack -= 1;
                Item targetItem = item;
                targetItem.ResetPrefix();
                //直接给恶魔，恶魔是最通用的
                HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Demonic);
                bool shouldBreak = false;
                //开始遍历，检查是否为鞭子，或近战类武器
                if (targetItem.DamageType.CountsAsClass<MeleeDamageClass>() || targetItem.DamageType.CountsAsClass<SummonMeleeSpeedDamageClass>() && !shouldBreak)
                {
                    HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Legendary);
                    shouldBreak = true;
                }
                //第二步，检查是否为远程武器
                if (targetItem.DamageType.CountsAsClass<RangedDamageClass>() && !shouldBreak)
                {
                    HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Unreal);
                    shouldBreak = true;
                }
                //第三步，检查是否为魔法武器
                if (targetItem.DamageType.CountsAsClass<MagicDamageClass>() && !shouldBreak)
                {
                    HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Mythical);
                    shouldBreak = true;
                }
                //第四步，检查是否为……召唤师的武器
                if (targetItem.DamageType.CountsAsClass<SummonDamageClass>() & !shouldBreak)
                {
                    HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Ruthless);
                    shouldBreak = true;
                }
                //最后，检查是否为代行者的武器
                if (targetItem.DamageType.CountsAsClass<ExecutorDamageClass>() & !shouldBreak)
                {
                    HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixType<Phantasmic>());
                }
                //将悬浮的物品变为空气
                inventory[slot] = item;
                SoundEngine.PlaySound(HJScarletSounds.Misc_ManaClearUse with { Pitch = .2f }, Player.Center);
                for (int i = 0; i < 20; i++)
                    new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
            }
            else if (item.accessory)
            {
                if (Main.mouseItem.IsLegal())
                    Main.mouseItem.stack -= 1;
                else
                    Player.HeldItem.stack -= 1;
                Item targetItem = item;
                targetItem.ResetPrefix();
                switch (crystallizeLoreReforgeIndex)
                {
                    default:
                        HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Warding);
                        break;
                    case 0:
                        HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Menacing);
                        break;
                    case 1:
                        HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Lucky);
                        break;
                    case 2:
                        HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Quick2);
                        break;
                    case 3:
                        HJScarletMethods.ApplyPrefixToThis(ref targetItem, PrefixID.Violent);
                        break;
                }
                inventory[slot] = targetItem;
                SoundEngine.PlaySound(HJScarletSounds.Misc_ManaClearUse with { Pitch = .2f }, Player.Center);
                for (int i = 0; i < 20; i++)
                    new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();

            }
        }

        public void HoverSevenStarOrGreatDipper(ref Item[] inventory, int context, int slot)
        {
            if (emblemGalaxy)
            {
                if (!inventory[slot].IsLegal())
                    return;
                Item item = inventory[slot];
                if (item.type == ItemType<TheSevenStar>())
                {
                    SwitchToSevenOrDipper(item, ItemType<TheGreatDipper>(), ref inventory, slot);
                }
                else if (item.type == ItemType<TheGreatDipper>())
                {
                    SwitchToSevenOrDipper(item, ItemType<TheSevenStar>(), ref inventory, slot);

                }
            }
        }

        public void SwitchToSevenOrDipper(Item itemHover, int targetID, ref Item[] inventory, int slot)
        {
            Item targetItem = new Item();
            bool favor = Player.HeldItem.favorited;
            targetItem.SetDefaults(targetID);
            targetItem.favorited = favor;
            targetItem.stack = 1;
            targetItem.prefix = itemHover.prefix;
            inventory[slot] = targetItem;
            SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { Pitch = .2f }, Player.Center);
            for (int i = 0; i < 20; i++)
                new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
        }

        public void HoverSwitchWeapon(ref Item[] inventory, int context, int slot)
        {
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
                DoSwapWeapon(ref inventory, item, slot, reverseWeapon, true);
                return;
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

        public void HoverRuShiWoWen(ref Item[] inventory, int context, int slot)
        {
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
        #endregion
    }
}

