using HJScarletRework.Buffs;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Armor.DragonHunter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool EnableCritDamage = false;
        public bool CanDrawIcon = false;
        public float CritsDamageBonus = 0f;
        private int GhostTimer = 0;
        private int GhostFrame = 0;
        public bool EnableExecutorVersion = false;
        public bool CanDrawGhost = false;
        public int ExecutionProj = -1;
        public bool ForceTacticalExecution = false;
        public bool ForceAutomaticExecution = false;
        public bool NotFinished = false;
        //控制purePrism的运动
        public float tintIconDrawLerp = 0;
        public bool purePrismLerpIn = false;
        public bool purePrismLerpOut = false;
        public bool setTintIcon = false;
        public bool isShivering = false;
        public EnumItemOwner ItemBelongTo = EnumItemOwner.None;
        public float simpleImmersiveBackpackValue = 1f;
        public float simpleImmersiveBackpackValueAlt = 1f;
        public bool DrawFloatingTextBox = false;
        public bool borderlandWeapon = false;
        /// <summary>
        /// shorthand
        /// </summary>
        public override void SetStaticDefaults()
        {
            HJScarletMethods.ShimmerEach(ItemID.PaladinsHammer, ItemID.PaladinsShield);
        }

        public Player LocalPlayer => Main.LocalPlayer;
        public void HandleLerpValue(Item item, Vector2 position, float scale)
        {
            if (!HJScarletConfigClient.Instance.SimpleImmersiveInventory)
            {
                simpleImmersiveBackpackValue = 1f;
                simpleImmersiveBackpackValueAlt = 1f;
            }
            bool hasImmersiveInventory = ModLoader.HasMod("ImmersiveInventory");
            bool isHovering = MouseHoveringAnySlot(position, scale);
            float maxScale = isHovering ? 1.35f : 1f;
            //开启沉浸背包mod下会禁用这一条的更改
            if (isHovering && hasImmersiveInventory)
                simpleImmersiveBackpackValue = 1f;
            simpleImmersiveBackpackValue = Lerp(simpleImmersiveBackpackValue, maxScale, 0.15f);
            //这个用于控模组图标的放缩
            simpleImmersiveBackpackValueAlt = Lerp(simpleImmersiveBackpackValueAlt, maxScale, 0.15f);
        }

        private bool MouseHoveringAnySlot(Vector2 slotPos, float drawScale)
        {
            float hitRadius = 28f * Main.inventoryScale;
            return Vector2.Distance(Main.MouseScreen, slotPos) < hitRadius;
        }
        public override void RightClick(Item item, Player player)
        {
            base.RightClick(item, player);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            var usPlayer = player.HJScarlet();
            if (usPlayer.terraRecipe)
            {
                if (item.type < VanillaMaxItem)
                {
                    string name = ItemID.Search.GetName(item.type);
                    if (!usPlayer.terraRecipeEatenFoodNameList.Contains(name))
                    {
                        usPlayer.terraRecipeEatenFoodNameList.Add(name);
                        usPlayer.terraRecipeNotEatenFoodNameList.Remove(name);
                        usPlayer.terraRecipe_EatenFoodCounts++;
                    }
                }
                else
                {
                    string name = item.ModItem.FullName;
                    if (!usPlayer.terraRecipeEatenFoodNameList.Contains(name))
                    {
                        usPlayer.terraRecipeEatenFoodNameList.Add(name);
                        usPlayer.terraRecipeNotEatenFoodNameList.Remove(name);
                        usPlayer.terraRecipe_EatenFoodCounts++;
                    }
                }
                if (HJScarletList.LegalFoodList.Contains(item.type))
                {
                    //物品都是独立的实例，这里必须得把表单直接扔到玩家类里进行保存
                    if (!usPlayer.terraRecipe_EatenFoodList.Contains(item.type))
                    {
                        usPlayer.terraRecipe_EatenFoodList.Add(item.type);
                        //这里也会尝试删除这个表的一个元素
                        usPlayer.terraRecipe_NotEatenFoodList.Remove(item.type);
                        usPlayer.terraRecipe_EatenFoodCounts++;
                    }
                }
            }

            if (item.healLife > 0 && player.HJScarlet().crimsonCharm)
            {
                player.AddBuff(BuffType<CrimsonCharmBuff>(), CrimsonCharm.OverSatuTime * 60);
                player.HJScarlet().crimsonCharmReduceTime += 1;
            }
            return true;
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            if (item.type == ItemID.GenderChangePotion)
            {
                player.HJScarlet().genderChangeTimer = GetSeconds(300);
            }
            if (player.HJScarlet().protectorShiver)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(item.type), ItemID.GoldCoin, 50);
                if (Main.rand.NextBool(4))
                    player.QuickSpawnItem(player.GetSource_OpenItem(item.type), ItemID.PlatinumCoin, 30);
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            var usPlayer = player.HJScarlet();
            if (usPlayer.GeneralWeaponBuffTimer == 0 && ScarletItemIDSets.SharedSameBuffTimer[item.type])
            {
                usPlayer.GeneralWeaponIndex = item.type;
            }
            //这里的判断如下：
            //tacticalExecutionManual (HJScarletPlayer类内) 只用于板正斧头，给代行者玩家自由切换手动处决与自动处决的模式
            //ForceToCustomExecute 则必须得否，即这个武器不能经过这里默认提供的管理
            //ForceToTactialExecute 便为强制自动处决
            bool usetactical = (ScarletItemIDSets.ForceToTacticalExecute[item.type] || usPlayer.tacticalExecutionManual) && (!ScarletItemIDSets.ForceToCustomExecute[item.type]);
            if (usetactical && !ScarletItemIDSets.ForceToAutomaticExecute[item.type])
            {
                usPlayer.tacticalExecution = true;
            }
            if (EnableCritDamage)
            {
                usPlayer.critDamageAll += CritsDamageBonus;
            }
            isShivering = usPlayer.protectorShiver;
            if (item.IsWeapon() && player.HJScarlet().dragonHunter)
            {
                usPlayer.critDamageAll += DragonHunterHead.FixedDamage;
            }
        }

        public override bool? UseItem(Item item, Player player)
        {
            var usPlayer = player.HJScarlet();
            if (usPlayer.terraRecipe)
            {

            }
            return base.UseItem(item, player);
        }
    }
}
