using ContinentOfJourney;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.ExecutorAlter;
using HJScarletRework.Items.Useables;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        /// 排序要求：相同类型的字段并一块，按字母表排序
        // bool 字段
        public bool adamantiteHeadExecutor = false;
        public bool ankhShieldImmnue = false;
        public bool artificalManaStar = false;
        public bool bitingClaw = false;
        public bool blackKeyDefenseTrigger = false;
        public bool blackKeyDoT = false;
        public bool bloodThronCrown = false;
        public bool brimstoneHeartKilling = false;
        public bool celesitalShellEffect = false;
        public bool chlorophyteHeadExecutor = false;
        public bool combatSlot = false;
        public bool combatSlot2 = false;
        public bool cowboyExecutor = false;
        public bool creationHat = false;
        public bool crimsonCharm = false;
        public bool cycleMadness = false;
        public bool desterrennacht = false;
        public bool diverArmor = false;
        public bool petDraco = false;
        public bool dragonHunter = false;
        public bool emblemColdSteel = false;
        public bool emblemExecutor = false;
        public bool emblemFirearm = false;
        public bool emblemGalaxy = false;
        public bool emblemThrown = false;
        public bool emblemVanguard = false;
        public bool executorSwordMark = false;
        public bool executorSwordMarkPing = false;
        public bool Executor_DrawFadeIn = false;
        public bool Executor_DrawFadeOut = false;
        public bool firstTimeCraftGaia = false;
        public bool floretProtectorExecutor = false;
        public bool fruitofEthernity = false;
        public bool giveMagicStorage = false;
        public bool givePaper = true;
        public bool goldenAppleEnchanted = false;
        public bool goldenAppleEnchantedFully = false;
        public bool handOfGods = false;
        public bool heartoftheCrystal = false;
        public bool infiniteBreath = false;
        public bool infiniteFlightTime = false;
        public bool isBeingLove = false;
        public bool LifeBalloonAcc = false;
        public bool LightofHorizon = false;
        public bool loveRing = false;
        public bool maidReaperArmor = false;
        public bool maidReaperHealUp = false;
        public bool mayaPumper = false;
        public bool mayaPumperParty = false;

        public bool monkExecutor = false;
        public bool monkStaffHeal = false;
        public bool mouseHoveringBanWeaponAbility = false;
        public bool parrying = false;
        public bool pocketMirror = false;
        public bool powerLily = false;
        public bool powerLilyVanity = false;
        public bool protectorMoonglow = false;
        public bool protectorShiver = false;
        public bool raincoatExecutor = false;
        public bool redDragonKnight = false;
        public bool resetEatenFoodCounts = false;
        public bool resetTerraRecipe = false;
        public bool sacarbWings = false;
        public bool petNone = false;
        public bool petShadow = false;
        public bool petSon = false;
        public bool petSquid = false;
        public bool petWatcher = false;
        public bool petWhale = false;
        public bool petLifeWorm = false;
        public bool saintChurch = false;
        public bool shinobiExecutor = false;
        public bool souloftheTidalMark = false;
        public bool terraRecipe = false;
        public bool terraSparkBoostImmnue = false;
        public bool theGreatDipperBuff = false;
        public bool titaniumHeadExecutor = false;
        public bool weaponUpgradePostSon = false;

        // int 字段
        public int adamantiteHeadExecutorThunderTimer = 0;
        public int antiKnockbackTime = 0;
        public int arcBowLevel = -1;
        public int rocketCharmLevel = -1;
        public int ASMDBuffTime = 0;
        public int blackKeyHeal = 0;
        public int blackKeyReduceDefense = 0;
        public int blackKeyTimer = 0;
        public int bloodThornCrownHit = 0;
        public int climaticHawstringLaserCounter = 0;
        public int conferenceCallBuffTime = 0;
        public int containedBlastBuffTime = 0;
        public int cowboyRevolverTimer = 0;
        public int crimsonCharmReduceTime = 0;
        public int crimsonScytheAttackCounter = 0;
        public int crimsonScytheDefense = 0;
        public int crimsonScytheSlayNPCType = 0;
        public int crystallizeLoreReforgeIndex = 0;
        public int cursorID = -1;
        public int cycleMadnessLevel = -1;
        public int cycleMadnessCrit = 0;
        public int cycleMadnessTimer = 0;
        public int defenderEmblemCD = 0;
        public int desterrannachtImmortalTime = 0;
        public int desterranRespawnChargeTimer = 0;
        public int drawUseableItemIcon = -1;
        public int executorSwordMarkLevel = -1;
        public int Executor_AFKTimer = 0;
        public int exsanguinationBuffTime = 0;
        public int floretProtectorTimer = 0;
        public int flybackhandBuffTime = 0;
        public int flybackhandBuffTimeCurrent = 0;
        public int flybackhandHealthRecord = 0;
        public int flybackHandManaRecord = 0;
        public int flybackInGameTimeBuff = 0;
        public int galvanizedHandDashCD = 0;
        public int genderChangeTimer = 0;
        public int goldenAppleDamageAbsorb = 0;
        public int iFrameHurtAdd = 0;
        public int jellyfishGroupIndex = -1;
        public int lastHeldItemIndex = -1;
        public int LifeBalloonAccJumps;
        public int maidReaperHealTimer = 0;
        public int maidReaperIndex = -1;
        public int manaSavingsJar = 0;
        public int mayaPumperDashTime = 0;
        public int mayaPumperDashType = -1;
        public int NoSlowFall = 0;
        public int parryShieldType = -1;
        public int pendantLevel = 0;
        public int powerLilyCacheTimer = 0;
        public int powerLilyTimer = 0;
        public int preciousTargetLevel = 0;
        public int protectorPlantID = -1;
        public int providenceHolyWaterHealMana = 0;
        public int saintChurchLastStanding = 0;
        public int selfPortraitType = -1;
        public int spellBreakerLevel = 0;
        public int spellBreakerTimer = 0;
        public int stardustRuneHitHealTimer = 0;
        public int stardustRuneStaticHealTimer = 0;
        public int tearEyeBuff = 0;
        public int terrariaRecipeEatenFoodCounts = 0;
        public int terrariaRecipeEatenMultTimes = 0;

        // float 字段
        public float blackKeyDefenseBuff = 0;
        public float containedBlastBoomCount = 0;
        public float Executor_BarOpacity = 0;
        public float healingPotionMult = 1f;
        public float heldProjReUseTime = 0;
        public float maxFallspeedModify = 0;
        public float PlayerFinalSpeedStoredTime = 0f;
        public float PlayerLastSpeedStored = 0f;

        // List<string> 字段
        public List<string> ruShiWoWenBanMinionNameList = new List<string>();
        public List<string> ruShiWoWenBanMinionNameTrashList = new List<string>();

        public List<string> terrariaRecipeEatenList = new List<string>();
        public List<string> terrariaRecipeNotEatenList = new List<string>();
        public List<string> terrariaRecipeEatenListTrash = new List<string>();
        public List<string> terrariaRecipeNotEatenListTrash = new List<string>();

        // int[] 字段
        public int[] protectorHerbTimerList = [0, 0, 0, 0, 0, 0, 0];
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
        public void DrawLoveRingParticle(Vector2 position, Player drawPlayer)
        {
            if (Main.rand.NextBool(12))
            {
                Rectangle rec = Utils.CenteredRectangle(drawPlayer.Center, new Vector2(drawPlayer.width, drawPlayer.height));
                Vector2 pos = Main.rand.NextVector2FromRectangle(rec) + Vector2.UnitY * 20f + Vector2.UnitX * Main.rand.NextFloat(10f, 20f) * Main.rand.NextBool().ToDirectionInt();
                new HeartParticle(pos, Vector2.UnitY * -Main.rand.NextFloat(0.51f, 2.3f), RandLerpColor(Color.Crimson, Color.HotPink), 40, 0.08f, 0.8f, fadeIn: true).Spawn();
            }
        }
        public void SwapListForNeeded(IReadOnlyList<string> matcher, ref List<string> trasher, ref List<string> apply, bool shouldReverse)
        {
            if (shouldReverse)
            {
                for (int i = 0; i < trasher.Count; i++)
                {
                    string nameType = trasher[i];
                    if (!matcher.Contains(nameType))
                    {
                        trasher.RemoveAt(i);
                        apply.Add(nameType);
                    }
                }
            }
            else
            {
                for (int i = 0; i < trasher.Count; i++)
                {
                    string nameType = trasher[i];
                    if (matcher.Contains(nameType))
                    {
                        trasher.RemoveAt(i);
                        apply.Add(nameType);
                    }
                }
            }
        }
        public override void OnEnterWorld()
        {
            if (givePaper)
            {
                Player.QuickSpawnItem(Player.GetSource_FromThis(), ItemType<StarterBag>());
                givePaper = false;
            }
            OnEnterWorldReset();
            resetTerraRecipe = true;
            if (terrariaRecipeNotEatenList.Count < 1)
            {
                terrariaRecipeNotEatenList = [.. HJScarletList.LegalFoodListName];
            }
            //初始化
            for (int i = 0; i < HJScarletList.LegalFoodListName.Count; i++)
            {
                string name = HJScarletList.LegalFoodListName[i];

                if (!terrariaRecipeNotEatenList.Contains(name))
                    terrariaRecipeNotEatenList.Add(name);
            }

            SwapListForNeeded(HJScarletList.SummonWeaponFullName, ref ruShiWoWenBanMinionNameTrashList, ref ruShiWoWenBanMinionNameList, false);
            SwapListForNeeded(HJScarletList.SummonWeaponFullName, ref ruShiWoWenBanMinionNameList, ref ruShiWoWenBanMinionNameTrashList, true);
            //把垃圾内容的表单丢进去
            SwapListForNeeded(HJScarletList.LegalFoodListName, ref terrariaRecipeEatenListTrash, ref terrariaRecipeEatenList, false);
            SwapListForNeeded(HJScarletList.LegalFoodListName, ref terrariaRecipeEatenList, ref terrariaRecipeEatenListTrash, true);
            SwapListForNeeded(HJScarletList.LegalFoodListName, ref terrariaRecipeNotEatenListTrash, ref terrariaRecipeNotEatenList, false);
            SwapListForNeeded(HJScarletList.LegalFoodListName, ref terrariaRecipeNotEatenList, ref terrariaRecipeNotEatenListTrash, true);
            for (int i = 0; i < terrariaRecipeEatenList.Count; i++)
            {
                string name = terrariaRecipeEatenList[i];
                terrariaRecipeNotEatenList.Remove(name);
            }


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
