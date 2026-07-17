using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static HJScarletRework.Rarity.RarityShiny.RareItemRarity;

namespace HJScarletRework.Items.ExecutorCards
{
    public class ShieldCoreRedDragon : ExecutorShieldCore
    {
        public override RareType RareType => RareType.Gold;
    }
    public abstract class ExecutorShieldCore : HJScarletItemClass, ILocalizedModType
    {
        public override string AssetPath => AssetHandler.Useables;
        public override string Texture => $"HJScarletRework/Assets/Texture/Items/ExecutorShieldCore/{GetType().Name}";
        public virtual ShieldCoreType CoreType => ShieldCoreType.Assault;
        public new string LocalizationCategory => "Items.ExecutorShieldCore";
        public virtual RareType RareType => RareType.Copper;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, RareType);
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.HJScarlet().CanDrawIcon = true;
            Item.accessory = true;
            ExSD();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CacheTooltipList = tooltips;
            int isRight = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            string coreTypeTooltip = Mod.GetLocalizationKey(LocalizationCategory + ".GenericData.Types." + CoreType).ToLangValue();
            string generalTooltip = Mod.GetLocalizationKey(LocalizationCategory + ".GenericData.ActiveDescription").ToLangValue();
            var coreTypeline = new TooltipLine(Mod, "ShieldCoreTypeName", coreTypeTooltip)
            {
                OverrideColor = Color.LightGoldenrodYellow
            };
            var generalLine = new TooltipLine(Mod, "ShieldCoreGeneralName", generalTooltip)
            {
                OverrideColor = Color.GreenYellow
            };
            tooltips.Insert(isRight, generalLine);
            tooltips.Insert(isRight, coreTypeline);
            ExModifyTooltips(tooltips);
        }

        public virtual void ExModifyTooltips(List<TooltipLine> tooltips) { }

        public override void UpdateInfoAccessory(Player player)
        {
            //base.UpdateInfoAccessory(player);
        }

        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public int FirstLineY = -1;
        public float PrevLerpValue = -1;
        public float LerpValue = 0;
        public float EdegValue = 0;

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            //记录起始点坐标。
            //通常情况下，物品不可能没有名字，而物品名称通常都在第一行，所以可以用这个来记录第一行的坐标
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            //line.Index如果不等于最后一行我们都不会绘制，以确保其绘制一次。
            //CacheTooltipList是事先在modifyTooltipline里缓存的列表。
            if (CacheTooltipList is null || line.Index != CacheTooltipList.Count - 1)
                return true;

            var setsList = new List<TextboxSettings>();
            for (int j = 1; j <= 3; j++)
            {
                string titleText = Mod.GetLocalizationKey(LocalizationCategory + ".GenericData.LevelName").ToLangValue().ToFormatValue(j);
                //实际文本描述，这里都是一行作结
                string detailPath = $"CoreLevel" + j;
                string detailText = this.GetLocalizationKey(detailPath).ToLangValue();
                detailText += "\n";

                //最后，我们再画需要的文本内容。
                var edgeColor = j switch
                {
                    1 => Color.Black,
                    2 => Color.DarkBlue,
                    _ => Color.Purple,
                };
                TextboxSettings sets = new TextboxSettings()
                {
                    TitleText = titleText,
                    TitleTextColor = Color.White,
                    TitleEdgeColor = edgeColor,
                    HasTitle = true,
                    BackgroundColor = Color.Black * .35f,
                    BackgroundEdgeColor = Color.Transparent,
                    TitleTextSize = 1.15f,
                    MainText = detailText,
                    TextColor = Color.White,
                    TextEdgeColor = Color.Black,
                    MultboxSpacing = 0
                };
                setsList.Add(sets);
            }
            TextboxMethods.DrawMultipleTextboxes(line, CacheTooltipList, setsList, 30);
            return true;
        }
    }
}
