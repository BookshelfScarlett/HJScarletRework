using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
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
        public override int PetProjType => ProjectileType<DracoProj>();
        public override int PetBuffType => BuffType<DracoBuff>();
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
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
                (
                hasTitle: false,
                backgroundColor: Color.Black * .24f,
                backgroundEdgeColor: Color.DarkRed,
                textColor: Color.White,
                textEdgeColor: Color.DarkRed,
                mainText: text
                );
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
