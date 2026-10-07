using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Vanity;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class StarterBag : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.FateWhite);
            ItemID.Sets.OpenableBag[Type] = true;
        }
        public override void ExSD()
        {
            Item.rare = RarityType<VanityEffectClass>();
            Item.consumable = true;

        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.AddLootSimple(ItemType<DescriptionPaper>());
            itemLoot.AddLootSimple(ItemType<FishingPaper>());
            //Luiafk
            if (!ModLoader.TryGetMod("miningcracks_take_on_luiafk", out Mod Luiafk))
                return;
            //Fargo突变
            if (!ModLoader.TryGetMod("Fargowiltas", out Mod FargoMutant))
                return;
            //魔法存储
            if (!ModLoader.TryGetMod("MagicStorage", out Mod MagicStorage))
                return;
            //更好的体验
            if (!ModLoader.TryGetMod("ImproveGame", out Mod QoT))
                return;
            //anpc
            if (!ModLoader.TryGetMod("AlchemistNPCLite", out Mod ANpc))
                return;
            List<(int, int)> itemList =
            [
                (ItemID.CellPhone, 1),
                (ItemID.TerrasparkBoots, 1),
                (ItemID.CloudinaBottle, 1),
                (ItemID.DiamondHook, 1),
                (ItemID.MoneyTrough,1),
                (ItemID.Safe,1),
                (GetSoftReferrenceItemID(QoT, "SpaceWand"), 1),
                (GetSoftReferrenceItemID(QoT, "MagickWand"), 1),
                (GetSoftReferrenceItemID(QoT, "WallPlace"), 1),
                (GetSoftReferrenceItemID(QoT, "PotionBag"), 1),
                (GetSoftReferrenceItemID(QoT, "BannerChest"), 1),
                (GetSoftReferrenceItemID(QoT, "ShellShipInBottle"), 1),
                (GetSoftReferrenceItemID(QoT, "WeatherBook"), 1),
                (GetSoftReferrenceItemID(QoT, "HiveGlobe"), 1),
                (GetSoftReferrenceItemID(QoT, "EnchantedSwordGlobe"), 1),
                (GetSoftReferrenceItemID(QoT, "FloatingIslandGlobe"), 1),
                (GetSoftReferrenceItemID(QoT, "PyramidGlobe"), 1),
                (GetSoftReferrenceItemID(QoT, "TempleGlobe"), 1),
                (GetSoftReferrenceItemID(QoT, "DungeonGlobe"), 1),
                (GetSoftReferrenceItemID(QoT, "AetherGlobe"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "StorageHeart"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "EnvironmentAccess"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "CraftingAccess"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "CombinedStations1Item"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "PortableCratingAccessPreHM"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "PortableAccessPreHM"), 1),
                (GetSoftReferrenceItemID(MagicStorage, "StorageUnitDemonite"), 64),
                (GetSoftReferrenceItemID(FargoMutant, "Instavator"), 1),
                (GetSoftReferrenceItemID(FargoMutant, "BattleCry"), 1),
                (GetSoftReferrenceItemID(FargoMutant, "GraveBuster"), 9999),
                (GetSoftReferrenceItemID(FargoMutant, "AutoHouse"), 16),
                (GetSoftReferrenceItemID(Luiafk, "ScrollCrafter"), 1),
                (GetSoftReferrenceItemID(Luiafk, "DeepsDummy"), 1),
                (GetSoftReferrenceItemID(Luiafk, "FasterMining"), 1),
                (GetSoftReferrenceItemID(Luiafk, "ArenaBuilder"), 1),
                (GetSoftReferrenceItemID(Luiafk, "LootMagnet"), 1),
                (GetSoftReferrenceItemID(Luiafk, "UnlimitedExplorer"), 1),
                (GetSoftReferrenceItemID(Luiafk, "Paper"), 9999),

            ];
            for (int i = 0; i < itemList.Count; i++)
            {
                QuickAdd(ref itemLoot, itemList[i].Item1, itemList[i].Item2);
            }
            void QuickAdd(ref ItemLoot itemLoot, int id, int stack)
            {
                if (id != -1)
                {
                    itemLoot.AddLootSimple(id, minQuantity: stack, maxQuantity: stack);
                }
            }
            int GetSoftReferrenceItemID(Mod mod, string name)
            {
                int itemID = -1;
                if (mod.TryFind(name, out ModItem value))
                {
                    return value.Type;
                }
                return itemID;
            }
        }
    }
}
