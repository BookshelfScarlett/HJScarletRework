using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ShadowCast : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
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
