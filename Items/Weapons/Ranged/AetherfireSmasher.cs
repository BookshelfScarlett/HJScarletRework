using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static HJScarletRework.Projs.Ranged.AetherfireSmasherName;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class AetherfireSmasher: ThrownHammerItem
    {
        public override string Texture => HJScarletItemProj.Item_AetherfireSmasher.Path;
        public override int ShootProjID => ProjectileType<AetherfireSmasherMain>();
        public override int NeedFocusStrikeTime => 40;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 70;
            //这里的ut有意为之
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.Yellow;
        }
        public override float StealthDamageMultipler => 0.25f;
        private void DrawSpecialColor(DrawableTooltipLine line, Color drawColor, string colorValue)
        {
            //获取全局Sin值，让描边发生一定程度的动态变化
            float sine = (float)((1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f)) / 2);
            float sineOffset = Lerp(0.5f, 1f, sine);
            string textValue = line.Text;
            Vector2 textPos = new(line.X, line.Y);
            string colorTextPath = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.Color.{colorValue}");
            string needReplacedText = textValue + " - " + colorTextPath.ToLangValue();
            //绘制发光描边
            for (int i = 0; i < 12; i++)
            {
                Vector2 afterimageOffset = (TwoPi * i / 12f).ToRotationVector2() * (1.8f * sineOffset);
                ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, needReplacedText, (textPos + afterimageOffset).RotatedBy(TwoPi * (i / 12)), drawColor * 0.9f, line.Rotation, line.Origin, line.BaseScale);
            }
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, needReplacedText, textPos, Color.White, line.Rotation, line.Origin, line.BaseScale);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
           //除非你的武器不存在武器名。否则这个判定不可能过不去
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                //暴力枚举，便于引用
                switch(Main.LocalPlayer.name.SelectedName())
                {
                    case NameType.Emma:
                        DrawSpecialColor(line, Color.HotPink, "Sakura");
                        return false;
                    case NameType.TrueScarlet:
                        DrawSpecialColor(line, Color.Red, "Red");
                        return false;
                    case NameType.Shizuku:
                        DrawSpecialColor(line, Color.MidnightBlue, "IcyBlue");
                        return false;
                    case NameType.SerratAntler:
                        DrawSpecialColor(line, Color.Purple, "Purple");
                        return false;
                    case NameType.WutivOrChaLost:
                        DrawSpecialColor(line, Color.Gold, "Gold");
                        return false;
                    case NameType.SherryOrAnnOrKino:
                        DrawSpecialColor(line, Color.RoyalBlue, "Blue");
                        return false;
                    case NameType.Hanna:
                        DrawSpecialColor(line, Color.Green, "Green");
                        return false;
                    default:
                        break;
                }
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color edgeColor = Color.White;
            edgeColor = Main.LocalPlayer.name.SelectedName() switch
            {
                NameType.TrueScarlet => Color.Red,
                //查 -- 金
                NameType.WutivOrChaLost => new Color(255, 178, 36),
                NameType.Emma => Color.HotPink,
                //锯角 - 紫
                NameType.SerratAntler => Color.Purple,
                //Kino - 蓝
                NameType.SherryOrAnnOrKino => Color.RoyalBlue,
                NameType.Shizuku => Color.LightSkyBlue,
                //绿
                NameType.Hanna => Color.Green,
                _ => Color.White,
            };
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, edgeColor with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.GhostWhite, 0f, tex.Size() / 2, scale, 0f, 0f);
            return false;
        }
        //实际合成材料可随意，我个人推荐为花后
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Judgement>().
                AddIngredient(ItemID.PaladinsHammer).
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
