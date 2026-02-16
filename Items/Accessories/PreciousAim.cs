using Terraria.ID;
using HJScarletRework.Globals.Enums;
using Terraria;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria.ModLoader;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HJScarletRework.Items.Accessories
{
    public class PreciousAim : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().PreciousTargetAcc = true;
            player.HJScarlet().PreciousAimAcc = true;
            player.HJScarlet().PreciousCritsMin = 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PreciousTarget>().
                AddIngredient<EssenceofLife>(10).
                AddTile<FinalAnvil>().
                Register();
        }
    }
}
