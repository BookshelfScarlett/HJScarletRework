using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Materials
{
    public class DisasterBar : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Materials;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Solar);
        }
        public override void ExSD()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Red;
            Item.material = true;
            Item.value = Item.sellPrice(gold: 4, silver: 30);
        }
        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient<DisasterEssence>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
