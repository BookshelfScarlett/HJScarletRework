using ContinentOfJourney.Items.ThrowerWeapons;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
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
        //控制purePrism的运动
        public float purePrismAnimationCounter = 0;
        public bool purePrismLerpIn = false;
        public bool purePrismLerpOut = false;
        public bool purePrismLegalTarget = false;
        public bool isShivering = false;

        public float simpleImmersiveBackpackValue = 1f;
        public float simpleImmersiveBackpackValueAlt = 1f;
        /// <summary>
        /// shorthand
        /// </summary>
        public override void SetStaticDefaults()
        {
            HJScarletMethods.ShimmerEach(ItemID.PaladinsHammer, ItemID.PaladinsShield);
        }
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
            if(isHovering)

            //开启沉浸背包mod下会禁用这一条的更改
            if (hasImmersiveInventory)
                simpleImmersiveBackpackValue = 1f;
                simpleImmersiveBackpackValue = Lerp(simpleImmersiveBackpackValue, maxScale, 0.15f);
            //这个用于控模组图标的放缩
            simpleImmersiveBackpackValueAlt = Lerp(simpleImmersiveBackpackValueAlt, maxScale, 0.15f);
        }
        public Player LocalPlayer => Main.LocalPlayer;
        public override void UpdateInventory(Item item, Player player)
        {
            //暂时禁用，即将实装特莉波卡系列物品时回归
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawGhost)
            {
                //在UpdateInventory内更新帧图的绘制，因为tooltip的draw实际上只会执行一次
                GhostTimer++;
                if (GhostTimer > 5)
                {
                    GhostFrame++;
                    GhostTimer = 0;
                }
                if (GhostFrame >= 16)
                    GhostFrame = 1;
            }
            if (!purePrismLegalTarget)
            {
                purePrismAnimationCounter = Lerp(purePrismAnimationCounter, 0f, 0.2f);
                if (purePrismAnimationCounter <= 0.02f)
                    purePrismAnimationCounter = 0;
            }
            else
            {
                purePrismAnimationCounter = Lerp(purePrismAnimationCounter, 1f, 0.2f);
                if (purePrismAnimationCounter >= 0.98f)
                    purePrismAnimationCounter = 1;
            }

            purePrismLegalTarget = false;
        }
        private bool MouseHoveringAnySlot(Vector2 slotPos, float drawScale)
        {
            float hitRadius = 28f * Main.inventoryScale;
            return Vector2.Distance(Main.MouseScreen, slotPos) < hitRadius;
        }
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(item, player, ref reduce, ref mult);
        }
        public override void RightClick(Item item, Player player)
        {
            base.RightClick(item, player);
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
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!item.IsLegal() || Main.dedServ)
                return true;
            HandleLerpValue(item, position, scale);
            if (Math.Abs(1f - simpleImmersiveBackpackValueAlt) > .03f)
            {
                spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, new Rectangle?(frame), drawColor, 0, origin, scale * simpleImmersiveBackpackValue, 0, 0f);
                return false;
            }
            return true;
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawSpecialIconDisplay(item, spriteBatch, position);
            if(HJScarletConfigClient.Instance.DrawIcon && CanDrawGhost && (purePrismAnimationCounter) <= .98f)
            {
                Vector2 iconPosition = position + new Vector2(10f * Main.inventoryScale, 10f * Main.inventoryScale);
                float iconScale = 0.31f * simpleImmersiveBackpackValueAlt;
                Rectangle rect = new(0, GhostFrame * 44, 46, 42);
                Vector2 recorigin = new(23, 21);
                Texture2D tex = HJScarletTexture.ScarletGhost.Value;
                for (int i = 0; i < 6; i++)
                    spriteBatch.Draw(tex, iconPosition + ToRadians(60f * i).ToRotationVector2() * 2f, rect, Color.White.ToAddColor() * (1f - purePrismAnimationCounter), 0f, recorigin, iconScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, iconPosition, rect, Color.White * (1f -purePrismAnimationCounter), 0f, recorigin, iconScale, SpriteEffects.None, 0f);

            }
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawIcon && (purePrismAnimationCounter) <= 0.98f)
            {
                Vector2 iconPosition = position + new Vector2(15f * Main.inventoryScale, 12f * Main.inventoryScale);
                float iconScale = 0.34f * simpleImmersiveBackpackValueAlt;
                //Rectangle rect = new(0, GhostFrame * 44, 46, 42);
                //Vector2 recorigin = new(23, 21);
                Texture2D tex = HJScarletTexture.LostbeltJourneyIcon.Value;
                for (int i = 0; i < 6; i++)
                    spriteBatch.Draw(tex, iconPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * (1f - purePrismAnimationCounter), 0f, tex.ToOrigin(), iconScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, iconPosition, null, Color.White * (1f -purePrismAnimationCounter), 0f, tex.ToOrigin(), iconScale, SpriteEffects.None, 0f);
            }
        }
        public void DrawSpecialIconDisplay(Item item, SpriteBatch spriteBatch, Vector2 position)
        {
            if ((purePrismAnimationCounter) <= 0.02f)
                return;
            //缩写。
            float time = (float)Main.timeForVisualEffects;
            //算提示图标的摆动
            float amp = 1.2f;
            float omega = .03f;
            float xOffset = (float)Math.Sin(time * omega) * amp * .7f;
            float yOffset = (float)Math.Cos(time * omega) * amp * 0.5f;
            //Vector2 lerpPos = Vector2.Lerp(new Vector2(10f, 15f), new Vector2(10f, 10f), purePrismAnimationCounter);
            Vector2 iconPosition = position - new Vector2(10) + new Vector2(xOffset, yOffset);
            iconPosition.Y -= Lerp(5f, 0f, purePrismAnimationCounter);
            //算放缩
            float iconScale = Lerp(0.31f, 0.34f, (float)(Math.Abs(Math.Sin(time)))) * simpleImmersiveBackpackValueAlt;

            //算颜色情况
            Color lerpColor = Color.Lerp(Color.White, Color.Silver, (float)Math.Abs(Math.Sin(time * .05f)));

            //最后我们再尝试将其画出
            Texture2D tex = HJScarletTexture.InvisAsset.Value;
            //这个Type是个打表，用来打表mod内有特殊搭配的物品
            //现在我们没有wiki，因此是得这么做
            if (LocalPlayer.HJScarlet().drawUseableItemIcon != -1)
                tex = TextureAssets.Item[LocalPlayer.HJScarlet().drawUseableItemIcon].Value;
            Vector2 recorigin = tex.ToOrigin();
            for (int i = 0; i < 8; i++)
                spriteBatch.Draw(tex, iconPosition + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * purePrismAnimationCounter, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, iconPosition, null, lerpColor * purePrismAnimationCounter, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
        }
        public override void HoldItem(Item item, Player player)
        {
            var usPlayer = player.HJScarlet();
            if (EnableCritDamage)
            {
                usPlayer.critDamageAll += CritsDamageBonus;
            }
            isShivering = usPlayer.protectorShiver;
        }
        public override bool? UseItem(Item item, Player player)
        {
            var usPlayer = player.HJScarlet();
            if (usPlayer.terraRecipe)
            {
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
            return base.UseItem(item, player);
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            return base.IsArmorSet(head, body, legs);
        }
           }
}
