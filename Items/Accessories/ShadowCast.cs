using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ShadowCast : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 100;
            player.HJScarlet().ShadowCastAcc = true;
            base.UpdateAccessory(player, hideVisual);
        }
    }
}
