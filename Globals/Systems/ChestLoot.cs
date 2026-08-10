using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using HJScarletRework.Items.Weapons.Executor.Thrown;
using HJScarletRework.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace HJScarletRework.Globals.Systems
{
    public partial class HJScarletGeneralSystem : ModSystem
    {
        public void PlacePurePrism()
        {
            // Place some additional items in Frozen Chests:
            // These are the 3 new items we will place.
            int[] itemsToPlaceInFrozenChests = [ItemType<PurePrismFate>()];
            // This variable will help cycle through the items so that different Frozen Chests get different items
            // Rather than place items in each chest, we'll place up to 6 items (2 of each).
            // Loop over all the chests
            bool forceSpawn = false;
            int chestIndex = 0;
            if (!forceSpawn)
            {
                for (; chestIndex < Main.maxChests; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];
                    if (chest == null)
                    {
                        continue;
                    }
                    Tile chestTile = Main.tile[chest.x, chest.y];
                    if (chestTile.TileType == TileID.Containers)
                    {
                        if (WorldGen.genRand.NextBool(4))
                            continue;
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {
                                chest.item[inventoryIndex].SetDefaults(ItemType<PurePrismFate>());
                                chest.item[inventoryIndex].stack = Main.rand.Next(5, 14);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void PlaceDungeonBreaker()
        {
            // Place some additional items in Frozen Chests:
            // These are the 3 new items we will place.
            int[] itemsToPlaceInFrozenChests = [ItemType<DungeonBreaker>()];
            // This variable will help cycle through the items so that different Frozen Chests get different items
            int itemsToPlaceInFrozenChestsChoice = 0;
            // Rather than place items in each chest, we'll place up to 6 items (2 of each).
            int itemsPlaced = 0;
            int maxItems = 6;
            // Loop over all the chests
            bool forceSpawn = false;
            int chestIndex = 0;
            if (!forceSpawn)
            {
                for (; chestIndex < Main.maxChests; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];
                    if (chest == null)
                    {
                        continue;
                    }
                    Tile chestTile = Main.tile[chest.x, chest.y];
                    if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 2 * 36)
                    {
                        // We have found a Frozen Chest
                        // If we don't want to add one of the items to every Frozen Chest, we can randomly skip this chest with a 33% chance.
                        if (WorldGen.genRand.NextBool(3))
                            continue;
                        // Next we need to find the first empty slot for our item
                        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {
                                // Place the item
                                chest.item[inventoryIndex].SetDefaults(itemsToPlaceInFrozenChests[itemsToPlaceInFrozenChestsChoice]);
                                // Decide on the next item that will be placed.
                                itemsPlaced++;
                                break;
                            }
                        }
                    }
                    // Once we've placed as many items as we wanted, break out of the loop
                    if (itemsPlaced >= maxItems)
                    {
                        break;
                    }
                }
            }
        }

        public void PlaceIceSpear()
        {
            // Place some additional items in Frozen Chests:
            // These are the 3 new items we will place.
            int[] itemsToPlaceInFrozenChests = [ItemType<AzureFrostmark>()];
            // This variable will help cycle through the items so that different Frozen Chests get different items
            int itemsToPlaceInFrozenChestsChoice = 0;
            // Rather than place items in each chest, we'll place up to 6 items (2 of each).
            int itemsPlaced = 0;
            int maxItems = 6;
            // Loop over all the chests
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                {
                    continue;
                }
                Tile chestTile = Main.tile[chest.x, chest.y];
                // We need to check if the current chest is the Frozen Chest. We need to check that it exists and has the TileType and TileFrameX values corresponding to the Frozen Chest.
                // If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Frozen Chest.
                // Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding.
                // An alternate approach is to check the wiki and looking for the "Internal Tile ID" section in the infobox: https://terraria.wiki.gg/wiki/Frozen_Chest
                if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 11 * 36)
                {
                    // We have found a Frozen Chest
                    // If we don't want to add one of the items to every Frozen Chest, we can randomly skip this chest with a 33% chance.
                    if (WorldGen.genRand.NextBool(3))
                        continue;
                    // Next we need to find the first empty slot for our item
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            // Place the item
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInFrozenChests[itemsToPlaceInFrozenChestsChoice]);
                            // Decide on the next item that will be placed.
                            itemsPlaced++;
                            break;
                        }
                    }
                }
                // Once we've placed as many items as we wanted, break out of the loop
                if (itemsPlaced >= maxItems)
                {
                    break;
                }
            }
        }
        public void ModifyGoldenChestLoost()
        {
            int[] itemsToPlaceInFrozenChests =
                [
                    ItemID.ManaCrystal,
                    ItemID.Compass,
                    ItemID.DepthMeter,
                    ItemID.MetalDetector,
                    ItemType<ManaSavingsJar>()
                ];
            PlaceItemsInSpecificChests(1, itemsToPlaceInFrozenChests, 120, .25f);
            /*
             * int itemsToPlaceInFrozenChestsChoice = 0;
            //int itemsPlaced = 0;
            //int maxItems = 120;
            //// Loop over all the chests
            //for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            //{
            //    Chest chest = Main.chest[chestIndex];
            //    if (chest == null)
            //    {
            //        continue;
            //    }
            //    Tile chestTile = Main.tile[chest.x, chest.y];
            //    if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 1 * 36)
            //    {
            //        if (WorldGen.genRand.NextBool(3))
            //            continue;
            //        // Next we need to find the first empty slot for our item
            //        for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
            //        {
            //            if (chest.item[inventoryIndex].type == ItemID.None)
            //            {
            //                // Place the item
            //                chest.item[inventoryIndex].SetDefaults(itemsToPlaceInFrozenChests[itemsToPlaceInFrozenChestsChoice]);
            //                // Decide on the next item that will be placed.
            //                itemsToPlaceInFrozenChestsChoice = (itemsToPlaceInFrozenChestsChoice + 1) % itemsToPlaceInFrozenChests.Length;
            //                //当前的choice如果不是0（即魔力水晶），则以1/3的概率替换为魔力水晶本身
            //                if (itemsToPlaceInFrozenChestsChoice != 0 && Main.rand.NextBool(4))
            //                    itemsToPlaceInFrozenChestsChoice = 0;
            //                // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
            //                itemsPlaced++;
            //                break;
            //            }
            //        }
            //    }
            //    if (itemsPlaced >= maxItems)
            //    {
            //        break;
            //    }
            //}*/
        }
        public void ModifyFrozenChestLoot()
        {
            int[] list = [ItemType<AzureFrostmark>()];
            PlaceItemsInSpecificChests(11, list, 24);
        }
        public void ModifySkyChestLoot()
        {
            int[] list = [ItemType<StarofHope>()];
            PlaceItemsInSpecificChests(21, list, 4);
        }
        public void ModifyDungeonChestLoot()
        {
            int[] list = [ItemType<DungeonBreaker>(), ItemType<DungeonKnife>()];
            PlaceItemsInSpecificChests(2, list, 18);
        }
        public void ModifyDesertChestLoost()
        {
            int[] list = [ItemType<DesertKnife>()];
            PlaceItemsInSpecificChests(10, list, 18, .25f);
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            base.ModifyWorldGenTasks(tasks, ref totalWeight);
        }
        public override void PostWorldGen()
        {
            ModifyGoldenChestLoost();
            ModifySkyChestLoot();
            ModifyFrozenChestLoot();
            ModifyDungeonChestLoot();
            ModifyDesertChestLoost();
            PlacePurePrism();
        }
        public override void PostUpdateWorld()
        {
        }
        /// <summary>
        /// 在指定类型的箱子中放置物品（常用于世界生成时向特定箱子添加内容）
        /// <br>让AI进行了封装</br>
        /// </summary>
        /// <param name="chestTileFrameX">箱子的 TileFrameX 值（如 11 * 36 对应冰冻箱）</param>
        /// <param name="itemsToPlace">要放置的物品 type 数组</param>
        /// <param name="maxItemsToPlace">总共最多放置多少件物品</param>
        /// <param name="skipChance">跳过某个箱子的概率（0~1，0 表示每个箱子都放）</param>
        /// <returns>实际放置的物品数量</returns>
        public static int PlaceItemsInSpecificChests(int chestTileFrameX, int[] itemsToPlace, int maxItemsToPlace, float skipChance = 0.33f)
        {
            if (itemsToPlace == null || itemsToPlace.Length == 0)
                return 0;

            int itemsPlaced = 0;
            int nextItemIndex = 0; // 循环选择物品

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                    continue;

                Tile chestTile = Main.tile[chest.x, chest.y];
                // 检查是否为目标箱子
                if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == chestTileFrameX * 36)
                {
                    // 随机跳过（如果 skipChance > 0）
                    if (skipChance > 0f && WorldGen.genRand.NextFloat() < skipChance)
                        continue;

                    // 寻找空栏位
                    for (int slot = 0; slot < Chest.maxItems; slot++)
                    {
                        if (chest.item[slot].type == ItemID.None)
                        {
                            chest.item[slot].SetDefaults(itemsToPlace[nextItemIndex]);
                            nextItemIndex = (nextItemIndex + 1) % itemsToPlace.Length; // 循环使用物品列表
                            itemsPlaced++;
                            break; // 一个箱子只放一件
                        }
                    }

                    if (itemsPlaced >= maxItemsToPlace)
                        break;
                }
            }

            return itemsPlaced;
        }
    }
}
