using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace HJScarletRework.Items
{
    public class Wreach : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletItemProj.Wreach.Path;
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 20;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<CoronaFireball>();
            Item.shootSpeed = 17f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
        }
        private IReadOnlyList<TooltipLine> AlterTooltip = null;
        private float LineY = -1;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            AlterTooltip = tooltips;
            base.ModifyTooltips(tooltips);
        }
        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            base.PostDrawTooltip(lines);
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            base.PostDrawTooltipLine(line);
        }
        public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            return base.PreDrawTooltip(lines, ref x, ref y);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            //记录起始点坐标。
            //通常情况下，物品不可能没有名字，而物品名称通常都在第一行，所以可以用这个来记录第一行的坐标
            if (line.IsItemName())
            {
                LineY = line.Y;
            }
            //line.Index如果不等于最后一行我们都不会绘制，以确保其绘制一次。
            //AlterTooltip是事先在modifyTooltipline里缓存的列表。
            if (AlterTooltip is null || line.Index != AlterTooltip.Count - 1)
                return true;
            //获取当前行的字体和缩放，通常情况下所有行通常一致，但以防万一。
            DynamicSpriteFont font = line.Font ?? FontAssets.MouseText.Value;
            Vector2 scale = line.BaseScale;
            if (scale == Vector2.Zero)
                scale = Vector2.One;

            //计算整个 Tooltip 框的最大文本宽度，用于定位右侧内容
            float maxWidth = 0f;
            foreach (var t1 in AlterTooltip)
            {
                Vector2 size = ChatManager.GetStringSize(font, t1.Text, scale);
                if (size.X > maxWidth)
                    maxWidth = size.X;
            }
            //准备要绘制的自定义内容，这里目前包括了标题和详细说明
            string titleText = "处决攻击";
            Vector2 titleTextSize = ChatManager.GetStringSize(font, titleText, scale);
            //与原本tooltip边框的边距大小
            float spacing = 30f;

            //默认从tooltip边框右侧开始
            float titleTextDrawX = line.X + maxWidth + spacing;
            //第一行的起始点。即和第一行对齐
            float titleTextDrawY = LineY;
            //屏幕边界处理，若右侧超出屏幕则绘制在左侧
            if (titleTextDrawX + titleTextSize.X > Main.screenWidth)
            {
                titleTextDrawX = line.X - titleTextSize.X - spacing;
                if (titleTextDrawX < 0)
                    titleTextDrawX = 0;
            }
            Vector2 titlePos = new Vector2(titleTextDrawX, titleTextDrawY);

            //实际文本描述，这里都是一行作结
            string detailText = "这是一行测试用例";
            Vector2 detailTextSize = ChatManager.GetStringSize(font, detailText, scale);
            //Y值需要额外加上本身标题的那行。
            Vector2 detailPos = new Vector2(titleTextDrawX, titleTextDrawY + titleTextSize.Y + 5);

            float padding = 8f;
            //开始画背景
            int minX = (int)(Math.Min(titlePos.X, detailPos.X) - padding);
            int minY = (int)(Math.Min(titlePos.Y, detailPos.Y) - padding);
            int maxX = (int)(Math.Max(titlePos.X + titleTextSize.X, detailPos.X + detailTextSize.X) + padding);
            int maxY = (int)(Math.Max(titlePos.Y + titleTextSize.Y, detailPos.Y + detailTextSize.Y) + padding);
            //设定这个矩形大小
            Rectangle rec = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            //绘制背景，这个背景是一个超级巨大的方块，由于已经超出屏幕，可以直接使用rec的形式随意切割来实现我们需要的效果。
            Texture2D background = HJScarletTexture.Texture_WhiteCubeBig.Value;
            SpriteBatch sb = Main.spriteBatch;
            for (int i = 0; i < 16; i++)
                sb.Draw(background, titlePos - new Vector2(padding) + (TwoPi / 16f * i).ToRotationVector2() * 2, rec, Color.WhiteSmoke * .5f, 0, Vector2.Zero, 1, 0, 0);
            sb.Draw(background, titlePos - new Vector2(padding), rec, Color.Black * .9f, 0, Vector2.Zero, 1, 0, 0);

            //最后，我们再画需要的文本内容。
            ChatManager.DrawColorCodedString(sb, font, titleText, titlePos, Color.Crimson, 0, Vector2.Zero, scale);
            ChatManager.DrawColorCodedString(sb, font, detailText, detailPos, Color.White, 0, Vector2.Zero, scale);
            //由于我们是往predraw进行的绘制，这里返回默认值来保证其余tooltip正常出现
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Stopwatch.StartNew();
            Stopwatch sw = Stopwatch.StartNew();
            //foreach(var proj2 in Main.ActiveProjectiles)
            //{
            //    if (Main.myPlayer != player.whoAmI)
            //        continue;
            //    if (!proj2.minion)
            //        continue;
            //    if (proj2.owner != player.whoAmI)
            //        continue;
            //    //proj2.Kill();
            //    proj2.active = false;
            //}
            //float curSlots = player.maxMinions - player.slotsMinions;
            //List<Item> hasList = [];
            //SoundEngine.PlaySound(HJScarletSounds.Misc_Spell);
            //while (curSlots >= 1)
            //{
            //    int itemID = Main.rand.NextFromCollection(HJScarletList.SummonWeaponList);
            //    Item item = ContentSamples.ItemsByType[itemID];

            //    Projectile proj = ContentSamples.ProjectilesByType[item.shoot];

            //    if (curSlots >= proj.minionSlots && !hasList.Contains(item))
            //    {
            //        ItemLoader.Shoot(item, player, source, position, velocity, proj.type, item.damage, knockback);
            //        curSlots -= proj.minionSlots;
            //        hasList.Add(item);
            //    }
            //}
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity.ToSafeNormalize() * 9f, ProjectileType<GaiaStrikerHeldProj>(), 1, knockback, player.whoAmI);
            sw.Stop();
            // 输出经过的时间（毫秒）
            Main.NewText($"执行耗时: {sw.ElapsedMilliseconds} ms");
            // 更高精度输出
            Main.NewText($"精确耗时: {sw.Elapsed.TotalMilliseconds:F4} ms");
            return false;
            //Vector2 ownerMW = player.LocalMouseWorld();
            //添加需要的攻击单位
        }
    }

}
