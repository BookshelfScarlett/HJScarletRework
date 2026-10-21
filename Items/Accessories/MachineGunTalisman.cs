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
        public int HealAmt = 16;
        public float DamageReduce = .5f;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.accessory = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealAmt, ConceptualBlackKey.BuffTime);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().blackKeyHeal = HealAmt;
            player.HJScarlet().blackKeyDefenseBuff = DamageReduce;
            player.HJScarlet().arcBowLevel = 2;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArcBow>().
                AddIngredient(ItemID.ShroomiteBar, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
