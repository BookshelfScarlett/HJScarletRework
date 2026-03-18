using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class FakeManaContainer : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public int GiveMana = 0;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(GiveMana);
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            GiveMana = (int)Clamp(Math.Min(150 - player.statManaMax, 150), 0, 150);
            player.HJScarlet().fakeManaContainer = GiveMana;
            player.statManaMax2 += GiveMana;
        }
    }
}
