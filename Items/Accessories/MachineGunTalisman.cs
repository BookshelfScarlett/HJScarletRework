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
            Item.width = Item.height = 33;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().critDamageExecutor += 0.12f;
            player.HJScarlet().blackKeyHeal = 35;
            
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeepBar>(10).
                AddIngredient<ArcBow>().
                AddIngredient(ItemID.SoulofMight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
