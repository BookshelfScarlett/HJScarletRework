using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using HJScarletRework.Globals.Players.VanitySets;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace HJScarletRework.Items.Vanity
{
    public struct VanityData(Color glowColor, Color edgeColor, Color mainColor, float glowMult = 1)
    {
        public Color GlowColor = glowColor;
        public Color EdgeColor = edgeColor;
        public Color MainColor = mainColor;
        public float GlowMult = glowMult;
    }
    public class VanityEffectClass : ModRarity
    {
        public override Color RarityColor => Color.Red;
        public static List<RaritySparkle> RaritySparkles = [];
        public static List<RaritySparkle> FlavorSparkles = [];
        public static void DrawItemName(DrawableTooltipLine line, VanityData data, Color particleColor1, Color particleColor2)
        {
            PostDrawRarity(ref RaritySparkles, line, particleColor1, particleColor2);
            RarityDrawHelper.DrawCustomTooltipLine(line, data.GlowColor, data.EdgeColor, data.MainColor, data.GlowMult);
        }
        public static void DrawFlavorTooltipName(DrawableTooltipLine line, VanityData data, Color particleColor1, Color particleColor2)
        {
            PostDrawRarity(ref FlavorSparkles, line, particleColor1, particleColor2, true);
            RarityDrawHelper.DrawCustomTooltipLine(line, data.GlowColor, data.EdgeColor, data.MainColor, data.GlowMult);

        }
        public static void DrawMisc(DrawableTooltipLine line, VanityData data, Color particleColor1, Color particleColor2)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, data.GlowColor, data.EdgeColor, data.MainColor, 0f);
        }

        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine, Color c, Color c2, bool slowdown = false)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f) * (1 + slowdown.ToInt() * -0.75f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(c, c2), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
        public static Vector2 GetParticlePosition(DrawableTooltipLine line)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            return Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
        }
    }
    public abstract class AccVanityItem : HJScarletItemClass
    {
        public virtual VanityData VanityData { get; set; }
        public virtual Color ParticleColor1 => Color.White;
        public virtual Color ParticleColor2 => Color.White;
        public static string ItemPath => "HJScarletRework/Assets/Texture/Items/Vanity/";
        public virtual bool DrawFlavorTooltip => false;
        public virtual string VanityName => string.Empty;
        public string VanityPrefix => ItemPath + VanityName;
        public override string AssetPath => AssetHandler.Vanity;
        public virtual bool HasFlavorTooltip => true;
        public override bool AllowPrefix(int pre) => false;
        public override bool ConsumeItem(Player player) => false;
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.rare = RarityType<VanityEffectClass>();
            Item.vanity = true;
            Item.accessory = true;
            Item.HJScarlet().CanDrawIcon = true;
        }
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Head", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Body", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Legs", EquipType.Legs, this);
            ExLoad();

        }

        public virtual void ExLoad()
        {
        }
        public override void SetStaticDefaults()
        {

            HJScarletList.VanityItemDictionary.Add(Type, GetType().Name);
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
        public static int FlavorTooltipIndex = -1;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!HasFlavorTooltip)
                return;
            //遍历一遍寻找需要绘制的特殊台词位置的索引
            //这里需要把对应的原罪名与原罪文本直接植入到物品名的下方，让其看起来比较协调
            FlavorTooltipIndex = tooltips.FindIndex(line => line.Name == "ItemName" && line.Mod == "Terraria");
            //通过本地化路径获取相应的内容，一个是原罪名，第二个为原罪文本
            string value = HJScarletMethods.ToLangValue(this.GetLocalizedValue("FlavorTooltip"));
            //实例化TooltipLine，这里的名字不能乱写，需要作为后面绘制特殊效果用的一个索引
            TooltipLine flavorTooltip = new TooltipLine(Mod, "FlavorTooltipName", value);
            //植入Tooltip。
            tooltips.Insert(FlavorTooltipIndex + 1, flavorTooltip);
            CacheTooltipList = tooltips;
            ExModifyTooltip(tooltips);
        }
        public virtual void ExModifyTooltip(List<TooltipLine> tooltips) { }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
                player.GetModPlayer<ScarletVanityPlayer>().accVanityID = Type;
        }
        public float FirstLineY = -1;
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public float LerpValue = 0;
        public float EdegValue = 0;
        public override void UpdateInfoAccessory(Player player)
        {
            if (HasFlavorTooltip)
            {
                if (Main.HoverItem.IsAir || Main.HoverItem is null)
                {
                    LerpValue = 0;
                    EdegValue = 0;
                }

                if (Main.HoverItem.type == Type)
                {
                    LerpValue = Lerp(LerpValue, 1.0f, 0.21f);
                    if (LerpValue > 0.98f)
                    {
                        EdegValue = Lerp(EdegValue, 1f, 0.21f);
                        LerpValue = 1f;
                    }
                }
            }
            base.UpdateInfoAccessory(player);
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<ScarletVanityPlayer>().accVanityID = Type;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
            //记录起始点坐标。
            //通常情况下，物品不可能没有名字，而物品名称通常都在第一行，所以可以用这个来记录第一行的坐标

                TextboxManager.FirstLineY = line.Y;
                VanityEffectClass.DrawItemName(line, VanityData, ParticleColor1, ParticleColor2);
                return false;
            }
            if (line.Name == "FlavorTooltipName" && line.Mod == Mod.Name)
            {
                VanityEffectClass.DrawFlavorTooltipName(line, VanityData, ParticleColor1, ParticleColor2);
                return false;

            }
            if (((line.Name == "Vanity" || line.Name == "Equipable") && line.Mod == "Terraria"))
            {
                VanityEffectClass.DrawMisc(line, VanityData, ParticleColor1, ParticleColor2);
                return false;
            }
            //if (ExDrawTooltipLine(line, ref yOffset))
            //{
            //    return false;
            //}
            if (HasFlavorTooltip)
            {
                TextboxSettings sets = new TextboxSettings()
                {
                    HasTitle = false,
                    BackgroundColor = Color.Lerp(VanityData.GlowColor, Color.Black, .4f) * .35f,
                    TextEdgeColor = VanityData.EdgeColor,
                    MainText = this.GetLocalizationKey("DetailTooltip").ToLangValue(),
                    TextColor = VanityData.MainColor,

                };
                TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
                //if (CacheTooltipList is null || line.Index != CacheTooltipList.Count - 1)
                //    return true;
                ////获取当前行的字体和缩放，通常情况下所有行通常一致，但以防万一。
                //DynamicSpriteFont font = line.Font ?? FontAssets.MouseText.Value;
                //Vector2 scale = line.BaseScale;
                //if (scale == Vector2.Zero)
                //    scale = Vector2.One;

                ////计算整个 Tooltip 框的最大文本宽度，用于定位右侧内容
                //float maxWidth = 0f;
                //foreach (var t1 in CacheTooltipList)
                //{
                //    Vector2 size = ChatManager.GetStringSize(font, t1.Text, scale);
                //    if (size.X > maxWidth)
                //        maxWidth = size.X;
                //}

                ////准备要绘制的自定义内容，这里目前包括了标题和详细说明
                //string titleText = this.GetLocalizationKey("DetailTooltip").ToLangValue();
                //Vector2 titleTextSize = ChatManager.GetStringSize(font, titleText, scale);
                ////与原本tooltip边框的边距大小
                //float spacing = 30f;

                ////默认从tooltip边框右侧开始
                //float titleTextDrawX = line.X + maxWidth + spacing;
                ////第一行的起始点。即和第一行对齐
                //float titleTextDrawY = FirstLineY;
                ////屏幕边界处理，若右侧超出屏幕则绘制在左侧
                //if (titleTextDrawX + titleTextSize.X > Main.screenWidth)
                //{
                //    titleTextDrawX = line.X - titleTextSize.X - spacing;
                //    if (titleTextDrawX < 0)
                //        titleTextDrawX = 0;
                //}
                //Vector2 titlePos = new Vector2(titleTextDrawX, titleTextDrawY);


                //float padding = 8f;
                ////开始画背景
                //int minX = (int)((titlePos.X) - padding);
                //int minY = (int)((titlePos.Y) - padding);
                //int maxX = (int)(titlePos.X + titleTextSize.X + padding);
                //int maxY = (int)(titlePos.Y + titleTextSize.Y + padding);
                ////设定这个矩形大小
                //Rectangle rec = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                ////绘制背景，这个背景是一个超级巨大的方块，由于已经超出屏幕，可以直接使用rec的形式随意切割来实现我们需要的效果。
                //Texture2D background = HJScarletTexture.Texture_WhiteCubeBig.Value;
                //SpriteBatch sb = Main.spriteBatch;
                //Vector2 posOffset = Vector2.Lerp(Vector2.UnitY * -50f, Vector2.Zero, LerpValue) + Vector2.UnitY * (float)(Math.Sin(Main.GlobalTimeWrappedHourly)) * 10f;

                //Vector2 backgroundPos = titlePos + posOffset - new Vector2(padding);
                //Color edgeColor = Color.Lerp(VanityData.EdgeColor,Color.Black,.25f) * .10f * LerpValue * EdegValue;
                ////描边
                //sb.Draw(background, backgroundPos, rec, Color.Lerp(VanityData.GlowColor,Color.Black,.4f) * .35f * LerpValue, 0, Vector2.Zero, 1, 0, 0);

                ////最后，我们再画需要的文本内容。
                //for (int i = 0; i < 16; i++)
                //    ChatManager.DrawColorCodedString(sb, font, titleText, titlePos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f + posOffset, VanityData.EdgeColor* LerpValue, 0, Vector2.Zero, scale);
                //ChatManager.DrawColorCodedString(sb, font, titleText, titlePos + posOffset, VanityData.MainColor * LerpValue, 0, Vector2.Zero, scale);
            }
            return true;
        }

        public virtual bool ExDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) => false;
    }
}
