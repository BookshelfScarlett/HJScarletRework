using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class CursedGoldenKey : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.GoldenKey);
        public override void ExSD()
        {
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GoldenKey, 5).
                AddTile(TileID.CrystalBall).
                DisableDecraft().
                Register();
        }
    }
    public class GoldenKeyPlayer : ModPlayer
    {
        private int lastChestIndex = -1;
        public override void PostUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                //Player.chest == -1的时候为关闭状态
                //Player.chest > 0 的时候基本上就是玩家打开的箱子在世界范围内的索引
                if (lastChestIndex != -1 && Player.chest == -1)
                {
                    OnChestClosed(lastChestIndex);
                }
                //延后存箱子的索引
                lastChestIndex = Player.chest;
            }
        }

        private void OnChestClosed(int lastChestIndex)
        {
            if (lastChestIndex < 0 || lastChestIndex >= Main.chest.Length)
                return;
            Chest c = Main.chest[lastChestIndex];
            if (c == null)
                return;
            bool hasKey = false;
            int keySlot = -1;
            //遍历是否有钥匙
            for (int i = 0; i < Chest.maxItems; i++)
            {
                Item item = c.item[i];
                if (item != null && !item.IsAir && item.type == ItemType<CursedGoldenKey>())
                {
                    hasKey = true;
                    //slot仍然需要
                    keySlot = i;
                    break;

                }
            }
            if (!hasKey)
                return;

            bool isEmpty = true;
            //遍历是否为空箱子
            for (int i = 0; i < Chest.maxItems; i++)
            {
                //跳过钥匙索引
                if (i == keySlot)
                    continue;
                if (c.item[i] != null && !c.item[i].IsAir)
                {
                    isEmpty = false;
                    break;
                }
            }
            if (!isEmpty)
                return;

            //箱子坐标
            int x = c.x;
            int y = c.y;
            Tile tile = Main.tile[x, y];
            //basicchest用来保证你这个Tile不会是虚空袋那种特殊玩意。
            if (!TileID.Sets.BasicChest[tile.TileType])
                return;
            if (tile.TileFrameX % 36 != 0)
                x--;
            if (tile.TileFrameY % 36 != 0)
                y--;
            Chest.DestroyChest(x, y);
            for (int j = x; j <= x + 1; j++)
            {
                for (int k = y; k <= y + 1; k++)
                {
                    //是的就是这么暴力。
                    Main.tile[j, k].ClearTile();
                }
            }
            //下面这些叽里咕噜啥的我也看不懂，我是抄的
            //反正看起来没问题
            NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 1, x, y, 0, lastChestIndex, 0, 0);
            NetMessage.SendTileSquare(-1, x, y, -1);
            Vector2 worldCenter = new Vector2(x * 16 + 8, y * 16 + 8);
            int npcType = NPCID.Mimic;
            if (Player.ZoneSnow)
                npcType = NPCID.IceMimic;
            int npcID = NPC.NewNPC(Player.GetSource_FromThis(), (int)worldCenter.X, (int)worldCenter.Y, npcType);
            SoundEngine.PlaySound(SoundID.Dig);
            //原版是有这个方法的，莫名其妙
            Main.npc[npcID].BigMimicSpawnSmoke();
        }
    }
}
