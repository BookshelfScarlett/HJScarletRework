using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class MachineGunTalisman : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.accessory = true;
            Item.defense = 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyHeal = 35;
            player.HJScarlet().blackKeyDefenseBuff = .25f;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArcBow>().
                AddIngredient(ItemID.SpectreBar, 10).
                AddIngredient(ItemID.SoulofSight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
