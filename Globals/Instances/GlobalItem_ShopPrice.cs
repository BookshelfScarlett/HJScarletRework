using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    /// <summary>
    /// 是的你没有看错这里是接入到modsystem的
    /// </summary>
    public static class HJScarletShopPrice
    {
        public static int RarityPriceWhite => Item.buyPrice(0, 0, 50, 0); //白色
        public static int RarityPriceBlue => Item.buyPrice(0, 1, 0, 0); //蓝色
        public static int RarityPriceGreen => Item.buyPrice(0, 2, 0, 0); //绿色
        public static int RarityPriceOrange => Item.buyPrice(0, 4, 0, 0); //橙色
        public static int RarityPriceLightRed => Item.buyPrice(0, 12, 0, 0); //淡红色
        public static int RarityPricePink => Item.buyPrice(0, 24, 0, 0); //粉色
        public static int RarityPriceLightPurple => Item.buyPrice(0, 36, 0, 0); //淡紫色
        public static int RarityPriceLime => Item.buyPrice(0, 48, 0, 0); //淡绿色
        public static int RarityPriceYellow => Item.buyPrice(0, 60, 0, 0); //黄色
        public static int RarityPriceCyan =>Item.buyPrice(0, 80, 0, 0); //淡蓝色
        public static int RarityPriceRed => Item.buyPrice(1, 0, 0, 0); //红色
        public static int RarityPricePurple => Item.buyPrice(1, 20, 0, 0); //紫色
        public static Dictionary<int, int> _RarityMap = new()
        {
            {ItemRarityID.White, RarityPriceWhite },
            {ItemRarityID.Blue, RarityPriceBlue },
            {ItemRarityID.Green, RarityPriceGreen },
            {ItemRarityID.Orange, RarityPriceOrange },
            {ItemRarityID.LightRed, RarityPriceLightRed },
            {ItemRarityID.Lime, RarityPriceLime },
            {ItemRarityID.Pink, RarityPricePink },
            {ItemRarityID.LightPurple, RarityPriceLightPurple },
            {ItemRarityID.Yellow, RarityPriceYellow },
            {ItemRarityID.Cyan, RarityPriceCyan },
            {ItemRarityID.Red, RarityPriceRed},
            {ItemRarityID.Purple, RarityPricePurple },
        };
        public static int ConvertedToValue(int key)
        {
            if(_RarityMap.TryGetValue(key, out var value)) 
                return value;
            return RarityPriceGreen;
        }
    }
}
