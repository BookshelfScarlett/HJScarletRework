using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class PurePrismFate : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public static List<int> _RefusedList =
            [
                ItemID.GoldenKey,
                ItemID.ShadowKey,
                ItemID.CorruptionKey,
                ItemID.CrimsonKey,
                ItemID.JungleKey,
                ItemID.DungeonDesertKey,
                ItemID.FrozenKey,
                ItemID.HallowedKey,
                ItemID.WormFood,
                ItemID.SlimeCrown,
                ItemID.DeerThing,
                ItemID.Abeemination,
                ItemID.BloodySpine,
                ItemID.SuspiciousLookingEye,
                ItemID.QueenSlimeCrystal,
                ItemID.MechanicalEye,
                ItemID.MechanicalWorm,
                ItemID.MechanicalSkull,
                ItemID.LihzahrdPowerCell,
                ItemID.TruffleWorm,
                ItemID.EmpressButterfly,
                ItemID.CelestialSigil,
                ItemID.PumpkinMoonMedallion,
                ItemID.NaughtyPresent,
                ItemType<MetalSpine>(),
                ItemType<UnstableGlobe>(),
                ItemType<CannedSoulofFlight>(),
                ItemType<MaliciousPacket>(),
                ItemType<PurpleFlareGun>()
            ];
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, RareItemRarity.RareType.White);
        }

        public override void ExSD()
        {
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().drawUseableItemIcon = Type;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.IsAir || item is null)
                    continue;
                //必须得是材料，必须得没有伤害
                //必须得不是饰品，必须得什么都不会发射
                //必须得没有Buff，必须堆叠数>1
                //必须不会制作任何墙体
                bool isMate2 = item.material && item.damage < 1 && !item.accessory && item.shoot == ProjectileID.None && item.buffType == 0 && item.maxStack > 1 && item.createWall == -1;
                //为所有带有“Ore”或者“Bar”的物品添加白名单
                bool whiteList2 = SmeltList.BarType.Contains(item.type)
                              || SmeltList.OreType.Contains(item.type)
                              || HJScarletList.BarsHashSet.Contains(item.type)
                              || HJScarletList.OresHashSet.Contains(item.type);
                //拒绝手动打表的物品，并排除掉所有“火把”，“钓鱼匣“和”荧光棒
                bool blackList2 = _RefusedList.Contains(item.type)
                              || ItemID.Sets.Torches[item.type]
                              || ItemID.Sets.IsFishingCrate[item.type]
                              || ItemID.Sets.IsFishingCrateHardmode[item.type]
                              || ItemID.Sets.Glowsticks[item.type];
                            
                
                bool blackList = false;
                //过滤所有容器
                if (item.createTile != -1)
                {
                    int tileID = item.createTile;
                    blackList = TileID.Sets.BasicChest[tileID] || TileID.Sets.BasicDresser[tileID] || TileID.Sets.IsAContainer[tileID];
                }

                bool legalTarget2 = (isMate2 || whiteList2) && !blackList2 && !blackList;
                if (!legalTarget2)
                    continue;

                item.HJScarlet().purePrismLegalTarget = true;
            }
            return;
        }
    }
}
