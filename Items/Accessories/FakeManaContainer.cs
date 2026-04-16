using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class FakeManaContainer : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public int GiveMana = 0;
        public override void ExSD()
        {
            Item.width = 30;
            Item.height = 30;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Green);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            GiveMana = (int)Clamp(Math.Min(150 - player.statManaMax, 150), 0, 150);
            player.HJScarlet().fakeManaContainer = GiveMana;
            player.statManaMax2 += GiveMana;
        }
    }
}
