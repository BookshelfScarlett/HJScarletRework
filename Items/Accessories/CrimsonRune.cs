using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class CrimsonRune : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.defense = 2;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.pStone = true;
            player.crimsonRegen = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CharmofMyths).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
