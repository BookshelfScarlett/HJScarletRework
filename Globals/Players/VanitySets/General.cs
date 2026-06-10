using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Vanity.Arceca;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players.VanitySets
{
    public partial class ScarletVanityPlayer : ModPlayer
    {
        public int accVanityID = -1;
        public bool arcaceVanity = false;
        public override void ResetEffects()
        {
            accVanityID = -1;
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(accVanityID), accVanityID);
            tag.Add(nameof(arcaceVanity), arcaceVanity);
        }
        public override void LoadData(TagCompound tag)
        {
            accVanityID = tag.GetInt(nameof(accVanityID));
            arcaceVanity = tag.GetBool(nameof(arcaceVanity));
        }
        public override void OnEnterWorld()
        {
            if (arcaceVanity)
            {
                string nameLow = Player.name.ToLower();
                if (nameLow.Contains('光') || nameLow.Contains("对立") || nameLow.Contains("arcaea") || nameLow.Contains("hikari") || nameLow.Contains("tairitsu"))
                {
                    Player.QuickSpawnItem(Player.GetSource_FromThis(), ItemType<ArcaeaPack>());
                    arcaceVanity = true;
                }
            }
        }
        public override void FrameEffects()
        {
            if (accVanityID != -1)
            {
                UpdateVanityItem();
                UpdateVanityParticle();
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
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = posBase;
                        p.Velocity = Player.velocity.ToSafeNormalize().ToRandVelocity(ToRadians(5f), 0.8f, 1.1f);
                        p.DrawColor = RandLerpColor(Color.IndianRed, Color.HotPink);
                        p.Lifetime = Main.rand.Next(45, 85);
                        p.Scale = .051f * Main.rand.NextFloat(0.8f, 1.1f);
                        p.Opacity = 1;
                        p.GlowCenterMult = 0.85f;
                    });
                }

            }
            else
            {

                if (Player.miscCounter % 120 == 0)
                {
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
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = posBase;
                        p.Velocity = Player.velocity.ToSafeNormalize().ToRandVelocity(ToRadians(5f), 0.8f, 1.1f);
                        p.DrawColor = RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue);
                        p.Lifetime = Main.rand.Next(45, 85);
                        p.Scale = .05f * Main.rand.NextFloat(0.8f, 1.1f);
                        p.Opacity = 1;
                        p.GlowCenterMult = 0.85f;
                    });
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
            if (name == nameof(TairitsuItem))
                Player.back = EquipLoader.GetEquipSlot(Mod, name, EquipType.Back);
            Player.legs = EquipLoader.GetEquipSlot(Mod, name, EquipType.Legs);
            Player.body = EquipLoader.GetEquipSlot(Mod, name, EquipType.Body);
            Player.head = EquipLoader.GetEquipSlot(Mod, name, EquipType.Head);
        }
    }
}
