using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class Desterrennacht : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().DesterrennachtAcc = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StardustRune>().
                AddIngredient<EssenceofDeath>(5).
                AddIngredient(ItemID.FragmentStardust, 10).
                AddTile<FinalAnvil>().
                Register();
        }
    }
}
