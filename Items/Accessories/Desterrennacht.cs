using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class Desterrennacht : HJScarletItems
    {
        public override bool IsLoadingEnabled(Mod mod) => true;
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
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
