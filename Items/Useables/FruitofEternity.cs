using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class FruitofEternity : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public static float DamageReduceMultiplier = 0.5f;
        public static float DefenseMultipler = 1.5f;
        public static int TeleportChance = 5;
        public static int LifeRegenSpeed = 4;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.Gold);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DefenseMultipler - 1).ToPercent(), LifeRegenSpeed / 2, DamageReduceMultiplier.ToPercent(), TeleportChance);
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.value = Item.buyPrice(gold: 5, silver: 30);
            Item.consumable = true;
        }
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
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
