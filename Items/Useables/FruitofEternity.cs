using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class FruitofEternity : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.Gold);
        }
        public override void UpdateInfoAccessory(Player player)
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
        public override void ExSD()
        {
            Item.accessory = true;
        }
        public float FirstLineY = -1;
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public float LerpValue = 0;
        public float EdegValue = 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CacheTooltipList = tooltips;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            string text = this.GetLocalizationKey("DetailTooltip").ToLangValue();
            TextboxSettings sets = new TextboxSettings
            {
                HasTitle = false,
                BackgroundColor = Color.White * .24f,
                TextColor = Color.White,
                TextEdgeColor = Color.Lerp(Color.HotPink, Color.Black, .74f),
                MainText = text
            };
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            return true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
