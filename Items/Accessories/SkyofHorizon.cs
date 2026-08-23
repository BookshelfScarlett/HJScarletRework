using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class SkyofHorizon :HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Matter);
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.expert = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.empressBrooch)
            {
                player.moveSpeed += .075f;
                player.jumpSpeedBoost += 1.8f;
                player.runAcceleration *= 1.75f;
            }
            player.HJScarlet().infiniteFlightTime = true;
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EmpressFlightBooster).
                AddIngredient<SolarFlareScoria>(50).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
