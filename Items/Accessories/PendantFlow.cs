using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class PendantFlow : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Lime);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().pendantLevel = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PendantWitness>().
                AddRecipeGroup(HJScarletRecipeGroup.AnyTitaniumBar, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
