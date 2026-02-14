using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Materials
{
    public class DisasterEssence : HJScarletItems
    {
        public override ItemCategory LocalCategory => ItemCategory.Material;
        public override string Texture => GetAsset(AssetCategory.Material);
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
    }
}
