using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Vanity.Arceca;
using HJScarletRework.Items.Vanity.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players.VanitySets
{
    public partial class ScarletVanityPlayer : ModPlayer
    {
        public List<string> YardName =
        [
            "特莉波卡",
            "tlipoca",
            "耶芙娜",
            "yavanna",
            "小叶子",
            "littleleaf",
            "little leaf",
            "霞露零",
        ];
        public List<string> ArcaceName =
        [
            "光",
            "hikari",
            "对立",
            "tairitsu",
            "arcaea",
        ];
        public List<string> LacrimosaName =
        [
            "安魂曲",
            "lacrimosa",
            "requim"
        ];
        public List<string> KajuName =
        [
            "笑面教授",
            "niyaniya"
        ];
        public List<string> KeiName =
        [
            "凯伊",
            "kei"
        ];
        public List<string> ArisuName =
        [
            "爱丽丝",
            "arisu",
            "AL-IS"
        ];


        public int accVanityID = -1;
        public bool arcaceVanity = false;
        public bool yardVanity = false;
        public bool lacrimosaVanity = false;
        public bool niyaniyakajuVanity = false;
        public bool keiVanity = false;
        public bool arisuVanity = false;
        public bool mollyVanity = false;
        public override void ResetEffects()
        {
            accVanityID = -1;
        }
        /// <summary>本 ModPlayer 中需要持久化的所有 bool 字段名。</summary>
        private static readonly string[] PersistedBoolFields =
        {
            nameof(arcaceVanity),
            nameof(yardVanity),
            nameof(niyaniyakajuVanity),
            nameof(lacrimosaVanity),
            nameof(arisuVanity),
            nameof(keiVanity),
            nameof(mollyVanity)
        };
        //这里的saveload采用了反射
        //由于进入退出游戏只会执行一次，开销问题不大
        public override void SaveData(TagCompound tag)
        {
            tag[nameof(accVanityID)] = accVanityID;

            Type type = GetType();
            foreach (string name in PersistedBoolFields)
                tag[name] = (bool)type.GetField(name).GetValue(this);
        }

        public override void LoadData(TagCompound tag)
        {
            accVanityID = tag.GetInt(nameof(accVanityID));

            Type type = GetType();
            foreach (string name in PersistedBoolFields)
                type.GetField(name).SetValue(this, tag.GetBool(name));
        }
        public override void OnEnterWorld()
        {
            void GetVanityQuick(ref bool value, int itemType, params string[] obj)
            {
                if (!value)
                {
                    string nameLow = Player.name.ToLower();
                    if (obj.Contains(nameLow))
                    {
                        Player.QuickSpawnItem(Player.GetSource_FromThis(), itemType);
                        value = true;
                    }
                }
            }
            GetVanityQuick(ref niyaniyakajuVanity, ItemType<KajuItem>(), [.. KajuName]);
            GetVanityQuick(ref lacrimosaVanity, ItemType<LacrimosaItem>(), [.. LacrimosaName]);
            GetVanityQuick(ref arcaceVanity, ItemType<ArcaeaPack>(), [.. ArcaceName]);
            GetVanityQuick(ref yardVanity, ItemType<YogsothothsYardPack>(), [.. YardName]);
            GetVanityQuick(ref keiVanity, ItemType<TendouKeiItem>(), [.. KeiName]);
            GetVanityQuick(ref arisuVanity, ItemType<TendouArisuItem>(), [.. ArisuName]);
            GetVanityQuick(ref mollyVanity, ItemType<MollyItem>(), "初镜巡弱杂骸音");
        }
        public override void UpdateDead()
        {
            InIdleStatement = false;
            IdleTimer = 0;
        }
        public override void FrameEffects()
        {
            if (accVanityID != -1)
            {
                UpdateVanityItem();
                UpdateVanityParticle();
                if (IdleTimer == 0)
                    InIdleStatement = false;
            }

        }

        public void UpdateVanityParticle()
        {
            bool isMoving = (Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y)) > 5;
            if (accVanityID == ItemType<TairitsuItem>())
            {
                DrawTairitsuItemParticle(isMoving);
            }
            if (accVanityID == ItemType<HikariItem>())
            {
                DrawHikariItemParticle(isMoving);
            }
        }

        private void DrawHikariItemParticle(bool isMoving)
        {
            if (isMoving)
            {
                if (Main.rand.NextBool(8))
                {
                    Vector2 posBase = Player.ToRandRec() - Player.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.8f, 1.3f);
                    new CrossGlow(posBase, RandLerpColor(Color.White, Color.IndianRed), 30, 1, 0.1f).Spawn();
                    new NoahButterfly(posBase, -Vector2.UnitY.RotatedByRandom(PiOver4), RandLerpColor(Color.IndianRed, Color.HotPink), Main.rand.Next(45, 85), 0.8f, 0.35f * Main.rand.NextFloat(0.5f, 0.7f), 1f, drawGlowingOrbParticle: true).Spawn();
                }
                if (Main.rand.NextBool(3))
                {
                    Vector2 posBase = Player.ToRandRec() - Player.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.8f, 1.3f);
                    ECSParticle.HRShinyOrb(posBase, Player.velocity.ToSafeNormalize().ToRandVelocity(ToRadians(5f), .8f, 1.1f), RandLerpColor(Color.IndianRed, Color.HotPink), Main.rand.Next(45, 85), 1, .05f * Main.rand.NextFloat(.8f, 1.1f), .85f);
                }

            }
            else
            {

                if (Player.miscCounter % 120 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 posBase = Player.Center + Vector2.UnitY * Player.height * Main.rand.NextFloat(.2f, .6f);
                        posBase.X += Main.rand.NextFloat(-1, 1f) * 10f;
                        new CrossGlow(posBase, RandLerpColor(Color.White, Color.IndianRed), 30, 1, 0.1f).Spawn();
                        new NoahButterfly(posBase, -Vector2.UnitY.RotatedByRandom(PiOver4), RandLerpColor(Color.IndianRed, Color.HotPink), Main.rand.Next(45, 85), 0.8f, 0.35f * Main.rand.NextFloat(0.5f, 0.7f), 1f, drawGlowingOrbParticle: true).Spawn();
                    }
                }
            }
        }

        public void DrawTairitsuItemParticle(bool isMoving)
        {
            if (isMoving)
            {
                Vector2 offset = Player.velocity / 8;
                if (Main.rand.NextBool(8))
                {
                    Vector2 posBase = Player.ToRandRec() - Player.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.8f, 1.3f) - offset;
                    new NoahButterfly(posBase, Player.velocity.ToSafeNormalize().RotatedByRandom(PiOver4), RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), Main.rand.Next(45, 85), 0.8f, 0.31f * Main.rand.NextFloat(0.5f, 0.7f), 1f, drawGlowingOrbParticle: true).Spawn();
                }
                if (Main.rand.NextBool(6))
                {
                    Vector2 posBase = Player.ToRandRec() - Player.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.8f, 1.3f) - offset;
                    new ShinyCrossStar(posBase, Vector2.Zero, RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), 40, 0, 1, 0.5f, false).Spawn();
                }
                if (Main.rand.NextBool(3))
                {
                    Vector2 posBase = Player.ToRandRec() - Player.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.8f, 1.3f) - offset;
                    ECSParticle.HRShinyOrb(posBase, Player.velocity.ToSafeNormalize().ToRandVelocity(ToRadians(5f), .8f, 1.1f), RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), Main.rand.Next(45, 85), 1, .05f * Main.rand.NextFloat(.8f, 1.1f), .85f);
                }

            }
            else
            {
                if (Player.miscCounter % 120 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 posBase = Player.Center + Vector2.UnitY * Player.height * Main.rand.NextFloat(.2f, .6f);
                        posBase.X += Main.rand.NextFloat(-1, 1f) * 10f;
                        new CrossGlow(posBase, RandLerpColor(Color.White, Color.RoyalBlue), 30, 1, 0.1f).Spawn();
                        new NoahButterfly(posBase, -Vector2.UnitY.RotatedByRandom(PiOver4), RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), Main.rand.Next(45, 85), 0.8f, 0.35f * Main.rand.NextFloat(0.5f, 0.7f), 1f, drawGlowingOrbParticle: true).Spawn();
                    }
                }
            }
        }

        public override void UpdateVisibleVanityAccessories()
        {
            bool isPausingGame = Main.gamePaused || Main.autoPause;
            if (accVanityID != -1 && isPausingGame)
                UpdateVanityItem();
        }
        public void UpdateVanityItem()
        {
            string name = HJScarletList.VanityItemDictionary[accVanityID];
            //怎么都是特殊情况。
            if (name == nameof(TairitsuItem) || name == nameof(TendouKeiItem) || name == nameof(TendouArisuItem))
                Player.back = EquipLoader.GetEquipSlot(Mod, name, EquipType.Back);
            Player.legs = EquipLoader.GetEquipSlot(Mod, name, EquipType.Legs);
            Player.body = EquipLoader.GetEquipSlot(Mod, name, EquipType.Body);
            Player.head = EquipLoader.GetEquipSlot(Mod, name, EquipType.Head);
        }
    }
}
