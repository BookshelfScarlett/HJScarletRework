using Terraria.ID;
using Terraria;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria.ModLoader;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;

namespace HJScarletRework.Items.Accessories
{
    public class PreciousAim : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);

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
