using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class CycleMadness : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static int CritsAdd = 5;
        public static int CritsPerSecond = 3;
        public static int MaxCrits = 250;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritsAdd + "%", CritsPerSecond, MaxCrits + "%");
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().cycleMadnessLevel = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofLight, 10).
                AddIngredient(ItemID.SoulofNight, 10).
                AddIngredient(ItemID.LightShard, 1).
                AddIngredient(ItemID.DarkShard, 1).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
