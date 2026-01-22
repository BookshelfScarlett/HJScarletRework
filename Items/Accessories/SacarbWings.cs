using HJScarletRework.Globals.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class SacarbWings : HJScarletItems
    {
        public override ItemCategory ItemCate => ItemCategory.Accessories;
        public override AssetCategory GetAssetCategory => AssetCategory.Equip;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 50;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //直接调用光女的无限飞饰品
            player.empressBrooch = true;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 6.25f;
        }
    }
}
