using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class MachineGunTalisman : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public int HealAmt = 35;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.accessory = true;
            Item.defense = 5;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealAmt);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyHeal = HealAmt;
            player.HJScarlet().blackKeyDefenseBuff = .5f;

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
