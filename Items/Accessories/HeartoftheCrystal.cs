using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheCrystal: HJScarletItems
    {
        public override ItemCategory ItemCate => ItemCategory.Accessories;
        public override string Texture => HJScarletItemProj.Equip_HeartoftheCrystal.Path;
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
