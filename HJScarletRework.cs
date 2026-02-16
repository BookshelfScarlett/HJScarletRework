global using static HJScarletRework.Globals.Handlers.EasingHandler;
global using static HJScarletRework.Globals.Handlers.RandHandler;
global using static Microsoft.Xna.Framework.MathHelper;
global using static Terraria.ModLoader.ModContent;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Tiles;
using System.Formats.Asn1;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static HJScarletRework.Globals.Instances.HJScarletGlobalTiles;

namespace HJScarletRework
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class HJScarletRework : Mod
	{
		public static HJScarletRework Instance;
        public static Mod CrossMod_Calamity;
        public static Mod CrossMod_HomewardJourney;
        public static Mod CrossMod_UCA;
        public static Mod CrossMod_FuckEmma = null;
        public override void Load()
        {
            Instance = this;
            ModLoader.TryGetMod(HJScarletMethods.CalamityMod, out CrossMod_Calamity);
            ModLoader.TryGetMod(HJScarletMethods.HomewardJourney, out CrossMod_HomewardJourney);
            ModLoader.TryGetMod(HJScarletMethods.HomewardJourney, out CrossMod_UCA);
            ModLoader.TryGetMod("Sounds_SakurabaEma", out CrossMod_FuckEmma);
        }
        public override void Unload()
        {
            Instance = null;
            CrossMod_Calamity = null;
            CrossMod_HomewardJourney = null;
            CrossMod_Calamity = null;
            CrossMod_FuckEmma = null;
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModPacket packet = GetPacket();

            if (Main.netMode == NetmodeID.Server)
            {
                ushort x = reader.ReadUInt16(), y = reader.ReadUInt16();
                ushort chance = reader.ReadUInt16();
                ushort targetType = reader.ReadUInt16();
                packet.Write(x);
                packet.Write(y);
                packet.Write(chance);
                packet.Send(-1, whoAmI);
                SoulGlobalTile autoSmelt = ModContent.GetInstance<SoulGlobalTile>();
                autoSmelt.SmeltOres(x, y, chance, targetType);
            }
            else
            {
                ushort x = reader.ReadUInt16(), y = reader.ReadUInt16();
                ushort chance = reader.ReadUInt16();
                ushort targetType = reader.ReadUInt16();
                SoulGlobalTile autoSmelt = ModContent.GetInstance<SoulGlobalTile>();
                autoSmelt.SmeltOres(x, y, chance, targetType);
            }
        }
        public override void PostSetupContent()
        {
            //首先刷新一次表单
            SmeltList.ClearOreList();
            //其次开始添加矿石与对应的矿锭
            if (SmeltList.OreType != null && SmeltList.OreType.Count == 0 && SmeltList.BarType != null && SmeltList.BarType.Count == 0)
            {
                //初始化原版矿物表单
                //这个写法其实有点风险的，如果表单不能一一对应就出事
                //后面在考虑改写，先实现效果
                for (int i = 0; i < SmeltList.VanillaOres.Length; i++)
                {
                    SmeltList.AddOres(SmeltList.VanillaOres[i], SmeltList.VanillaBars[i]);
                }
                //旅人矿物的打表
                //我草，谁几把让物块名跟物品名是同一个的
                SmeltList.AddOres(TileType<ContinentOfJourney.Tiles.EternalOre>(), ItemType<EternalBar>());
                SmeltList.AddOres(TileType<ContinentOfJourney.Tiles.LivingOre>(), ItemType<LivingBar>());
                SmeltList.AddOres(TileType<ContinentOfJourney.Tiles.CubistOre>(), ItemType<CubistBar>());
            }
        }
    }
}
