using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class BiomeKeyString : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void ExSD()
        {
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TempleKey, 5).
                AddTile(TileID.CrystalBall).
                DisableDecraft().
                Register();
        }
    }
    public class BiomeKeyPlayer : ModPlayer
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
                if (item != null && !item.IsAir && item.type == ItemType<BiomeKeyString>())
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
            //开始放物品。先清除钥匙本身
            c.item[keySlot].TurnToAir();
            List<int> Treasureitem = [ItemID.VampireKnives, ItemID.ScourgeoftheCorruptor, ItemID.PiranhaGun, ItemID.RainbowGun, ItemID.StormTigerStaff, ItemID.StaffoftheFrostHydra];
            List<int> MeleeWeapons =
            [
                ItemID.PaladinsHammer,
                ItemID.Keybrand,
                ItemID.ShadowJoustingLance,
                ItemID.Kraken,
            ];
            List<int> RangedWeapons =
            [
                ItemID.SniperRifle,
                ItemID.RocketLauncher,
                ItemID.TacticalShotgun,
            ];
            List<int> MagicWeapon =
            [
                ItemID.ShadowbeamStaff,
                ItemID.SpectreStaff,
                ItemID.InfernoFork,
                ItemID.MagnetSphere,
            ];
            List<int> RandomTreasure =
            [
                ItemID.BoneFeather,
                ItemID.BlackBelt,
                ItemID.Tabi,
                ItemID.PaladinsShield,
                ItemID.WispinaBottle,
            ];

            List<int> RandomMat =
            [
                ItemID.Ectoplasm,
                ItemID.BBQRibs
            ];
            int otherTreasureChoice = 0;
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (i < 5)
                {
                    c.item[i].SetDefaults(Treasureitem[i]);
                }
                otherTreasureChoice++;
                if (otherTreasureChoice == 5)
                {
                    c.item[otherTreasureChoice].SetDefaults(MeleeWeapons[Main.rand.Next(0, MeleeWeapons.Count)]);
                    continue;
                }
                if (otherTreasureChoice == 6)
                {
                    c.item[otherTreasureChoice].SetDefaults(RangedWeapons[Main.rand.Next(0, RangedWeapons.Count)]);
                    continue;
                }
                if (otherTreasureChoice == 7)
                {
                    int id = MagicWeapon[Main.rand.Next(0, MagicWeapon.Count)];
                    c.item[otherTreasureChoice].SetDefaults(id);
                    continue;
                }
                if (otherTreasureChoice == 8)
                {
                    c.item[otherTreasureChoice].SetDefaults(ItemID.MaceWhip);
                    continue;
                }
                if (otherTreasureChoice == 9 || otherTreasureChoice == 10 || otherTreasureChoice == 11)
                {
                    int id = RandomTreasure[Main.rand.Next(0, RandomTreasure.Count)];
                    c.item[otherTreasureChoice].SetDefaults(id);
                    continue;
                }
                if (otherTreasureChoice == 12)
                {
                    int id = RandomMat[Main.rand.Next(0, RandomMat.Count)];
                    c.item[otherTreasureChoice] = new Item(id, Main.rand.Next(49, 100));
                }
            }
            for (int j = 0; j < 22; j++)
            {
                new TurbulenceGlowOrb(Player.ToRandRec(), 1.4f, Color.WhiteSmoke, 120, 0.14f, RandRotTwoPi).Spawn();
            }
            SoundEngine.PlaySound(HJScarletSounds.Misc_Spell);

        }
    }
}
