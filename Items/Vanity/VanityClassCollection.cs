using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using HJScarletRework.Globals.Players.VanitySets;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            if (HasFlavorTooltip)
            {
                TextboxSettings sets = new TextboxSettings()
                {
                    HasTitle = false,
                    BackgroundColor = Color.Lerp(VanityData.GlowColor, Color.Black, .4f) * .45f,
                    TextEdgeColor = VanityData.EdgeColor,
                    MainText = this.GetLocalizationKey("DetailTooltip").ToLangValue(),
                    TextColor = VanityData.MainColor,
                };
                TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            }
            return true;
        }

        public virtual bool ExDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) => false;
    }
}
