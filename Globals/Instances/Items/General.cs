using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public bool isShivering = false;
        /// <summary>
        /// shorthand
        /// </summary>
        public override void SetStaticDefaults()
        {
            HJScarletMethods.ShimmerEach(ItemID.PaladinsHammer, ItemID.PaladinsShield);
        }
        public Player LocalPlayer => Main.LocalPlayer;
        public override void UpdateInventory(Item item, Player player)
        {
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawIcon)
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
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawIcon)
            {
                Vector2 iconPosition = position + new Vector2(8f, 8f);
                float iconScale = 0.35f;
                Rectangle rect = new(0, GhostFrame * 44, 46, 42);
                Vector2 recorigin = new(23, 21);
                spriteBatch.Draw(HJScarletTexture.ScarletGhost.Value, iconPosition, rect, Color.White, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
            }
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
