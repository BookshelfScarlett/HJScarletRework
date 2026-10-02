using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class CrystallizedLore : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateGolden);
        }
        public override void ExSD()
        {
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(0, 1, 0, 0);
        }
        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            player.HJScarlet().crystallizeLoreReforgeIndex++;
            if (player.HJScarlet().crystallizeLoreReforgeIndex > 4)
                player.HJScarlet().crystallizeLoreReforgeIndex = 0;
        }
        public override bool ConsumeItem(Player player)
        {
            return false;
        }
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CacheTooltipList = tooltips;
            int index = tooltips.FindLastIndex(0, t => t.Name.Contains("Tooltip") && t.Mod == "Terraria");
            string prefixID = "Mencing";
            prefixID = Main.LocalPlayer.HJScarlet().crystallizeLoreReforgeIndex switch
            {
                0 => "Mencing",
                1 => "Lucky",
                2 => "Quick",
                3 => "Violent",
                _ => "Warding",
            };
            string text = Mod.GetLocalizationKey("Database.PrefixData." + prefixID).ToLangValue();
            tooltips.CreateTooltip(this.GetLocalizationKey("PrefixLine"), Color.SkyBlue, Mod, "CrystallizedLorePrefixName", index, text);
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            string text = this.GetLocalizationKey("DetailTooltip").ToLangValue();
            TextboxSettings sets = new TextboxSettings(
                backgroundColor: Color.White * .24f,
                textColor: Color.White,
                textEdgeColor: Color.Lerp(Color.Black, Color.Black, .74f),
                mainText: text,
                backgroundEdgeColor: Color.White,
                hasTitle: false
                );
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
        }
    }
}
