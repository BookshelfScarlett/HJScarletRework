using ContinentOfJourney;
using ContinentOfJourney.Tiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Configuration;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class DeathTolls: ThrownHammerItem
    {
        public override int ShootProjID => ProjectileType<DeathTollsMainProj>();
        public override int NeedFocusStrikeTime => 20;
        public override void ExSSD()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemType<DeathTolls>()] = ItemType<BinaryStars>();
        }
        public override void ExSD()
        {
            Item.width = 88;
            Item.height = 94;
            Item.damage = 197;
            Item.useTime = 18;
            //这里的UseTime是有意改的很慢的
            Item.useAnimation = 18;
            Item.shootSpeed = 24f;
            Item.rare = RarityType<NightRarity>();
            //这里不会给音效，因为要考虑一些射弹的联动
            //实际音效会在射弹初始化的时候提供
            Item.UseSound = null;
            Item.value = Item.buyPrice(gold: 12);

        }
        public override float StealthDamageMultipler => 0.35f;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                NightRarity.DrawRarity(line);
                return false;
            }
            if (line.Name == "FlavorTooltipsName" && line.Mod == Mod.Name)
            {
                NightRarity.DrawFlavorRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string path = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.ShimmerTooltip");
            bool downedAnyGods = DownedBossSystem.downedMatterGod || DownedBossSystem.downedLifeGod || DownedBossSystem.downedTimeGod;
            if (downedAnyGods && !Main.LocalPlayer.HJScarlet().NoGuideForBinaryStars)
                tooltips.CreateTooltip(path, Color.LightPink);
            int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            //通过本地化路径搜索需要的特殊文本
            string value = this.GetLocalizedValue("FlavorTooltips").ToLangValue();
            //实例化toolti并注册名字
            TooltipLine flavorTooltips = new TooltipLine(Mod, "FlavorTooltipsName", value);
            //植入Tooltip
            tooltips.Insert(flavorTooltipIndex + 1, flavorTooltips);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.Purple with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.UltraBright);
            return false;
        }
        private static float UpdatePos
        {
            get
            {
                float value = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1f) * 1.2f + 1.4f);
                return Clamp(value, 1.0f, 2.4f);
            }
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //草拟吗瑞德
            //没有击倒任意3神，正常绘制这把锤子
            bool downedAnyGods = DownedBossSystem.downedMatterGod || DownedBossSystem.downedLifeGod || DownedBossSystem.downedTimeGod;
            if (!downedAnyGods)
                return true;
            //第二个判定，如果已经获得了弑神锤，返回

            if (Main.LocalPlayer.HJScarlet().NoGuideForBinaryStars)
                return true;

            //否则绘制这把锤子的其他效果。
            Texture2D tex = TextureAssets.Item[Type].Value;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * UpdatePos, null, Color.Pink with { A = 0 }, 0f, origin, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AetherfireSmasher>().
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient(ItemID.SoulofNight, 10).
                AddIngredient(ItemID.LargeAmethyst).
                DisableDecraft().
                AddTile<FinalAnvil>().
                Register();
        }
    }
}
