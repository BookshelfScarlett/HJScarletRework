using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Diver
{
    [AutoloadEquip(EquipType.Legs)]
    public class DiverLegs : HJScarletArmor
    {
        public float CritChance = 5f;
        public override void ExSD()
        {
            Item.defense = 4;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritChance + "%");
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChance;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemType<AnglerCoin>(), 36).
                AddTile(TileType<FishmenFreeMarketTradingSystem>()).
                Register();
        }

    }
}
