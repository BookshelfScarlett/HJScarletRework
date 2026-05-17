using ContinentOfJourney;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.ExecutorAlter;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Arabic;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public int flybackhandCloclCD = 0;
        public int flybackhandBuffTime = 0;
        public int flybackhandBuffTimeCurrent = 0;
        //用给归零针，查阅玩家当前损失的HP量
        public int flybackhandHealthRecord = 0;
        public int flybackHandManaRecord = 0;
        public int flybackInGameTimeBuff = 0;

        public bool CreationHatSet = false;
        //电表镀针的冲刺冷却
        public int galvanizedHandDashCD = 0;

        // 用于向上向下冲刺禁用羽落
        public int NoSlowFall = 0;
        public float maxFallspeedModify = 0;
        public int ownerMinionHammerCount = 0;

        public int climaticHawstringLaserCounter = 0;
        public bool goldenApple = false;
        public bool goldenAppleEnchanted = false;
        public int goldenAppleDamageAbsorb = 0;
        public bool goldenAppleEnchantedFully = false;
        public bool givePaper = true;
        #region 护甲
        public bool shinobiExecutor = false;
        public bool monkExecutor = false;
        public bool runeWizardExecutor = false;
        public bool cowboyExecutor = false;
        public bool monkStaffHeal = false;
        public bool fishExecutor = false;
        public int fishDash = 0;
        public bool fishDashStored = false;
        public int cowboyRevolverTimer = 0;

        public bool floretProtectorExecutor = false;
        public int floretProtectorTimer = 0;
        public bool raincoatExecutor = false;
        public bool redDragonKnight = false;
        public int protectorPlantID = -1;
        public int[] protectorHerbTimerList = [0, 0, 0, 0, 0, 0, 0];
        public bool protectorShiver = false;
        public bool protectorMoonglow = false;
        #endregion

        #region Accessories
        public bool heartoftheCrystal = false;
        public bool loveRing = false;
        public bool isBeingLove = false;
        public int genderChangeTimer = 0;
        public bool artificalManaStar = false;

        public bool ShadowCastAcc = false;
        public bool LifeBalloonAcc = false;
        public int LifeBalloonAccJumps;

        public bool souloftheTidalMark = false;
        public bool desterrennacht = false;
        public int stardustRuneHitHealTimer = 0;
        public int stardustRuneStaticHealTimer = 0;
        public int desterrannachtImmortalTime = 0;
        public int desterranRespawnChargeTimer = 0;

        public bool PreciousTargetAcc = false;
        public bool PreciousAimAcc = false;
        public int PreciousTargetCrtis = 10;
        public int PreciousCritsMin = 0;
        public int manaSavingsJar = 0;

        public bool vanguardEmblem = false;
        public int defenderEmblemCD = 0;
        public int blackKeyHeal = 0;
        public float blackKeyDefenseBuff = 0;
        public int blackKeyTimer = 0;
        public bool blackKeyDoT = false;
        public int blackKeyReduceDefense = 0;
        public bool blackKeyDefenseTrigger = false;
        public bool executorSwordMark = false;
        #endregion

        #region Pets
        public bool WhalePet = false;
        public bool NonePet = false;
        public bool ShadowPet = false;
        public bool SquidPet = false;
        public bool WatcherPet = false;
        #endregion
        #region Player Movement
        /// <summary>
        /// 在进入保存动量的冲刺之前，玩家当前的速度
        /// </summary>
        public float PlayerLastSpeedStored = 0f;
        /// <summary>
        /// 玩家是否按下了跳跃键
        /// </summary>
        public bool PlayerHasUseJump = false;
        /// <summary>
        /// 玩家的动量保存的时间
        /// </summary>
        public float PlayerFinalSpeedStoredTime = 0f;
        #endregion
        #region 处刑攻击
        /// <summary>
        /// <para>是否允许手动处决模式</para>
        /// <para>用于标识玩家全局的处决能力类型</para>
        /// </summary>
        public bool tacticalExecution = false;
        /// <summary>
        /// <para>临时手动处决形态切换标志（与 <see cref="tacticalExecution"/> 无关）。</para>
        /// <para>该字段专门用于处决攻击时动态切换玩家的手持形态（包括手持射弹）。 </para>
        /// <para>当玩家按下处决键且满足条件时，此字段会被设为 <c>true</c>，并在处决动画结束后需由调用者手动重置为 <c>false</c></para>
        /// <para>此开关不依赖任何装备，适用于需要临时改变攻击形态的场景（例如使用特殊射弹替换普通投掷物）</para>
        /// </summary>
        public bool tacticalExecutionManual = false;
        public int tacticalTime = 0;
        public bool ExecutorSwordMarkPlus = false;
        public int tacticalPunishTime = 0;
        public int ExecutionTime = 0;
        public int bonusExecutionReduce = 0;
        public Dictionary<int, int> ExecutionListStored = new Dictionary<int, int>();
        public bool StopExecutionInit = false;

        //用于hud绘制的计时器
        public int Executor_AFKTimer = 0;
        public float Executor_BarOpacity = 0;
        public bool Executor_DrawFadeIn = false;
        public bool Executor_DrawFadeOut = false;

        public int exsanguinationBuffTime = 0;

        #endregion
        public bool terraRecipe = false;
        public bool resetTerraRecipe = false;
        public bool resetEatenFoodCounts = false;
        public int terraRecipe_EatenFoodCounts = 0;
        public int terraRecipe_LifeMaxMultTime = 0;
        public int terraRecipe_LifeMaxIncre = 10;
        public List<int> terraRecipe_EatenFoodList = new List<int>();
        public List<int> terraRecipe_NotEatenFoodList = new List<int>();
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff<HoneyRegenAlt>())
            {
                Player owner = drawInfo.drawPlayer;
                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(drawInfo.Position, owner.width + 4, owner.height + 4, Main.rand.NextBool() ? DustID.Honey : DustID.Honey2);
                    Main.dust[d].velocity = new Vector2(Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[d].alpha = 100;
                    Main.dust[d].scale *= 1f;
                    drawInfo.DustCache.Add(d);
                }
            }
            if (isBeingLove)
            {
                DrawLoveRingParticle(drawInfo.Position, drawInfo.drawPlayer);
            }
        }
        public override void DrawPlayer(Camera camera)
        {
        }
        public void DrawLoveRingParticle(Vector2 position, Player drawPlayer)
        {
            if (Main.rand.NextBool(12))
            {
                Rectangle rec = Utils.CenteredRectangle(drawPlayer.Center, new Vector2(drawPlayer.width, drawPlayer.height));
                Vector2 pos = Main.rand.NextVector2FromRectangle(rec) + Vector2.UnitY * 20f + Vector2.UnitX * Main.rand.NextFloat(10f, 20f) * Main.rand.NextBool().ToDirectionInt();
                new HeartParticle(pos, Vector2.UnitY * -Main.rand.NextFloat(0.51f, 2.3f), RandLerpColor(Color.Crimson, Color.HotPink), 40, 0.08f, 0.8f, fadeIn: true).Spawn();
            }
        }
        public override void OnEnterWorld()
        {
            if(givePaper)
            {
                Player.QuickSpawnItem(Player.GetSource_FromThis(), ItemType<DescriptionPaper>());
                givePaper = false;
            }
            resetTerraRecipe = true;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (item.IsAir || item is null)
                    continue;
                if (item.HJScarlet().EnableExecutorVersion)
                {
                    if (!ArmorMaps.Contains(item.type))
                        continue;
                    //这里实际上没什么办法，只能这样打表
                    SwitchArmorType2(item, i);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                Item item = Player.armor[i];
                if (item.IsAir || item is null)
                    continue;
                if (item.HJScarlet().EnableExecutorVersion)
                {
                    if (!ArmorMaps.Contains(item.type))
                        continue;
                    //这里实际上没什么办法，只能这样打表
                    SwitchArmorType2(item, i, true);
                }
            }
        }
        private void SwitchArmorType2(Item item, int i, bool armorSlot = false)
        {
            switch (item.type)
            {
                case ItemID.RuneHat:
                    AlterArmorType2(item.type, i, 14, false, armorSlot: armorSlot);
                    break;
                case ItemID.RuneRobe:
                    AlterArmorType2(item.type, i, 22, false, armorSlot: armorSlot);
                    break;
                case ItemID.CowboyHat:
                    AlterArmorType2(item.type, i, CowboyHelmet.Defense, false, ItemRarityID.Orange, armorSlot);
                    break;
                case ItemID.CowboyJacket:
                    AlterArmorType2(item.type, i, CowboyChestplate.Defense, false, ItemRarityID.Orange, armorSlot);
                    break;
                case ItemID.CowboyPants:
                    AlterArmorType2(item.type, i, CowboyHelmet.Defense, false, ItemRarityID.Orange, armorSlot);
                    break;
                case ItemID.RainHat:
                    AlterArmorType2(item.type, i, RaincoatHelmet.Defense, false, armorSlot: armorSlot);
                    break;
                case ItemID.RainCoat:
                    AlterArmorType2(item.type, i, RaincoatChestplate.Defense, false, armorSlot: armorSlot);
                    break;
            }
            if (Condition.DownedBrainOfCthulhu.IsMet() || Condition.DownedEaterOfWorlds.IsMet())
            {
                switch (item.type)
                {
                    case ItemID.FishCostumeMask:
                        AlterArmorType2(item.type, i,FishCostumeHelmet.Defense, false, ItemRarityID.Orange,armorSlot:armorSlot);
                        break;
                    case ItemID.FishCostumeShirt:
                        AlterArmorType2(item.type, i,FishCostumeChestplate.Defense, false, ItemRarityID.Orange,armorSlot:armorSlot);
                        break;
                    case ItemID.FishCostumeFinskirt:
                        AlterArmorType2(item.type,i, FishCostumeLegs.Defense, false, ItemRarityID.Orange,armorSlot:armorSlot);
                        break;

                }
            }

            if (DownedBossSystem.downedLifeGod)
            {
                switch (item.type)
                {
                    case ItemID.FloretProtectorHelmet:
                        AlterArmorType2(item.type, i, FloretProtectorHelmetAlter.Defense, false, ItemRarityID.Red, armorSlot);
                        break;
                    case ItemID.FloretProtectorChestplate:
                        AlterArmorType2(item.type, i, FlorectProtectorChestplateAlter.Defense, false, ItemRarityID.Red, armorSlot);
                        break;
                    case ItemID.FloretProtectorLegs:
                        AlterArmorType2(item.type, i, FlorectProtectorLegsAlter.Defense, false, ItemRarityID.Red, armorSlot);
                        break;
                }
            }
            if (Condition.DownedPlantera.IsMet())
            {
                switch (item.type)
                {
                    case ItemID.MaidHead:
                        AlterArmorType2(item.type, i, MaidHelmetAlter.Defense, false, ItemRarityID.Yellow,armorSlot);
                        break;
                    case ItemID.MaidShirt:
                        AlterArmorType2(item.type, i,MaidChestplateAlter.Defense, false, ItemRarityID.Yellow,armorSlot);
                        break;
                    case ItemID.MaidPants:
                        AlterArmorType2(item.type, i,MaidLegsAlter.Defense, false, ItemRarityID.Yellow,armorSlot);
                        break;
                }
            }
        }

        private void AlterArmorType2(int targetArmor, int targetindex, int defense = 0, bool vanity = true, int rarityID = -1, bool armorSlot = false)
        {
            Item targetItem = new Item();
            Item inventItem;
            if (armorSlot)
            {
                inventItem = Player.armor[targetindex];
            }
            else
                inventItem = Player.inventory[targetindex];
            bool favor = inventItem.favorited;
            bool alterVersion = inventItem.HJScarlet().EnableExecutorVersion;
            if (!alterVersion)
            {
                targetItem.SetDefaults(targetArmor);
            }
            else
            {
                targetItem.SetDefaults(targetArmor);
                targetItem.vanity = vanity;
                targetItem.HJScarlet().EnableExecutorVersion = true;
                targetItem.defense = defense;
                targetItem.favorited = favor;
                if (rarityID != -1)
                    targetItem.rare = rarityID;
            }
            if (armorSlot)
            {
                Player.armor[targetindex] = targetItem;
            }
            else
                Player.inventory[targetindex] = targetItem;
        }
    }
}
