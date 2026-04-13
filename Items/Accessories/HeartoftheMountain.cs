using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Placables;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheMountain : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.defense = 5;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().heartoftheCrystal = true;
            player.GetDamage<MagicDamageClass>() *= 5.0f;
            player.GetCritChance<MagicDamageClass>() += 50;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HeartoftheCrystal>().
                AddIngredient<EssenceofBright>(10).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
