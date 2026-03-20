using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheCrystal: HJScarletItemClass
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override string AssetPath => AssetHandler.Equips;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
