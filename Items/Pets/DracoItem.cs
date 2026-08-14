using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Methods.Textbox;
using HJScarletRework.Projs.Pets;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Pets
{
    public class DracoItem : HJScarletPetItem
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<DracoProj>(), BuffType<DracoBuff>());
        }
        public override void ExSD()
        {
            Item.CloneDefaults(ItemID.EyeOfCthulhuPetItem);
        }
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CacheTooltipList = tooltips;
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            string text = this.GetLocalizationKey("FlavorTooltip").ToLangValue();
            TextboxSettings sets = new TextboxSettings
            {
                HasTitle = false,
                BackgroundColor = Color.Black * .24f,
                BackgroundEdgeColor = Color.DarkRed,
                TextColor = Color.White,
                TextEdgeColor = Color.DarkRed,
                MainText = text
            };
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 15).
                AddIngredient(ItemID.JungleRose).
                DisableDecraft().
                AddTile(TileID.Loom).
                Register();
        }
    }
}
