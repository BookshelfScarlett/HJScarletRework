using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Tiles
{
    public class SmeltList
    {
        public static List<int> OreType = new(150);
        public static List<int> BarType = new(150);
        public static ushort[] VanillaOres =
        [
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Gold,
            TileID.Platinum,
            TileID.Meteorite,
            TileID.Demonite,
            TileID.Crimtane,
            TileID.Hellstone,
            TileID.Cobalt,
            TileID.Palladium,
            TileID.Mythril,
            TileID.Orichalcum,
            TileID.Adamantite,
            TileID.Titanium,
            TileID.Chlorophyte,
            TileID.LunarOre
        ];
        public static short[] VanillaBars =
        [
            ItemID.CopperBar,
            ItemID.TinBar,
            ItemID.IronBar,
            ItemID.LeadBar,
            ItemID.SilverBar,
            ItemID.TungstenBar,
            ItemID.GoldBar,
            ItemID.PlatinumBar,
            ItemID.MeteoriteBar,
            ItemID.DemoniteBar,
            ItemID.CrimtaneBar,
            ItemID.HellstoneBar,
            ItemID.CobaltBar,
            ItemID.PalladiumBar,
            ItemID.MythrilBar,
            ItemID.OrichalcumBar,
            ItemID.AdamantiteBar,
            ItemID.TitaniumBar,
            ItemID.ChlorophyteBar,
            ItemID.LunarBar
        ];
        //原版矿统一5换1
        /// <summary>
        /// 将模组的矿（与对应的锭）打表
        /// </summary>
        /// <param name="oreType">矿物物块种类</param>
        /// <param name="barType">锭的物品种类</param>
        public static void AddOres(int oreType, int barType)
        {
            OreType.Add(oreType);
            BarType.Add(barType);
        }
        /// <summary>
        /// 刷新矿的表单
        /// </summary>
        public static void ReloadOreList()
        {
            ClearOreList();
            //重加载一次
            GetInstance<HJScarletRework>().PostSetupContent();
        }
        /// <summary>
        /// 清理矿锭表单，用于卸载模组时用
        /// </summary>
        public static void ClearOreList()
        {
            OreType.Clear();
            BarType.Clear();
        }
    }
}

