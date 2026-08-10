using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class EmblemGalaxy : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Donator);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.HJScarlet().OwnerName = "冰川咲";
            Item.HJScarlet().ItemBelongTo = EnumItemOwner.Donator;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().emblemGalaxy = true;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EssenceofTime>(5).
                AddIngredient<EssenceofLife>(5).
                AddIngredient<EssenceofMatter>(5).
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
