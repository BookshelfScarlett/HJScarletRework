using HJScarletRework.Core;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class ShadowCast : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.master = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ApplyDash(ScarletContent.DashType<ShadowCastDash>());
        }
    }
}
