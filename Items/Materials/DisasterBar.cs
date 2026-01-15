using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Materials
{
    public class DisasterBar : HJScarletItems
    {
        public override ItemCategory ItemCate => ItemCategory.Material;
        public override string Texture => HJScarletItemProj.Material_DisasterBar.Path;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.SoulofNight, 1).
                AddIngredient(ItemID.BeetleHusk, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
