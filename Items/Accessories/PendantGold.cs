using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class PendantGold : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateGolden);
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().pendantLevel = 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PendantGold>().
                AddIngredient<EternalBar>(10).
                AddIngredient<LivingBar>(10).
                AddIngredient<CubistBar>(10).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
