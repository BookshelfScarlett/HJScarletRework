using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class FloatingPlants : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Typeless;
        public static int[] PlantArrat =
            [
            ItemID.Daybloom,
            ItemID.Moonglow,
            ItemID.Blinkroot,
            ItemID.Waterleaf,
            ItemID.Deathweed,
            ItemID.Shiverthorn,
            ItemID.Fireblossom,
            ];

        public ref float Osci => ref Projectile.ai[0];
        public int PlantType
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public enum State
        {
            Floating,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public AnimationStruct Helper = new(3);
        public bool IsIntersect = false;
        public int CurLifeTime = -1;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16, 2);
        }
        public override void ExSD()
        {
            //故意放大，因为这个是要人捡起来的
            Projectile.width = Projectile.height = 100;
            Projectile.scale = 0f;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void ProjAI()
        {
            if (Projectile.timeLeft < 50)
            {
                Projectile.velocity = -Vector2.UnitY;
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.2f);
                if (Projectile.Opacity < 0.02f)
                    Projectile.Opacity = 0;
                return;
            }
            else
                DoHoming();
            UpdateParticle();
        }

        private void UpdateParticle()
        {
            if (Projectile.IsOutScreen() || Projectile.scale < Main.rand.NextFloat() || Projectile.Opacity < Main.rand.NextFloat())
                return;
            //每个草药都是有自己独立的一个视觉粒子的。
            Vector2 spawnPos;
            Vector2 dir;
            //满足条件时。启用另外一套粒子。
            if (Helper.IsDone[0])
            {
                dir = Projectile.velocity.ToSafeNormalize();
                switch (PlantType)
                {
                    //太阳花
                    default:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.LawnGreen, Color.Lime), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.LawnGreen, Color.Lime), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();
                        break;

                    //月光花
                    case 1:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new TurbulenceGlowOrb(spawnPos, 0.2f, RandLerpColor(Color.DarkSlateBlue, Color.SkyBlue), 40, 0.1f, RandRotTwoPi).Spawn();
                        if (Main.rand.NextBool(4))
                            new KiraStar(spawnPos, Vector2.Zero, RandLerpColor(Color.DarkSlateBlue, Color.SkyBlue), 40, 0, 1, 0.140f * 0.25f, useAlt: true).Spawn();

                        break;

                    //闪耀根
                    case 2:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();


                        break;
                    //水业草
                    case 3:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();
                        break;
                    //死亡草
                    case 4:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos + dir * 40f, dir * -Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos + dir * 30f, dir * -Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 50f, dir * -Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();

                        break;
                    //寒蝉棘
                    case 5:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new ShinyCrossStar(spawnPos, dir * Main.rand.NextFloat(0.7f, 1.28f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, dir.ToRotation(), 0.5f, false).Spawn();
                        if (Main.rand.NextBool())
                            new HRShinyOrb(spawnPos, dir.RotatedByRandom(Pi) * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, 1f, 0.140f * 0.40f).Spawn();
                        if (Main.rand.NextBool())
                            new TurbulenceGlowOrb(spawnPos, 0.62f, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0.14f, RandRotTwoPi).Spawn();

                        break;

                    //火焰花
                    case 6:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new HRShinyOrb(spawnPos + dir * 15f, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0, 1, 0.08f).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 4.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.18f).SpawnToNonPreMult();
                        break;
                }
            }
            else
            {
                if (Helper.GetAniProgress(0) > Main.rand.NextFloat())
                    return;
                switch (PlantType)
                {
                    //太阳花
                    default:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 68f);
                        dir = (Projectile.Center - spawnPos).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.LawnGreen, Color.Lime), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.LawnGreen, Color.Lime), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();
                        break;

                    //月光花
                    case 1:
                        spawnPos = Projectile.Center + RandVelTwoPi(5f, 48f);
                        dir = (Projectile.Center - spawnPos).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new TurbulenceGlowOrb(spawnPos, 0.2f, RandLerpColor(Color.DarkSlateBlue, Color.SkyBlue), 40, 0.1f, RandRotTwoPi).Spawn();
                        if (Main.rand.NextBool(4))
                            new KiraStar(spawnPos, Vector2.Zero, RandLerpColor(Color.DarkSlateBlue, Color.SkyBlue), 40, 0, 1, 0.140f * 0.25f, useAlt: true).Spawn();

                        break;

                    //闪耀根
                    case 2:
                        spawnPos = Projectile.Center + RandVelTwoPi(5f, 28f);
                        dir = Main.rand.NextBool() ? (Vector2.UnitY * Main.rand.NextBool().ToDirectionInt()).ToSafeNormalize() : (Vector2.UnitX * Main.rand.NextBool().ToDirectionInt()).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();


                        break;
                    //水业草
                    case 3:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 48f);
                        dir = (-Vector2.UnitY).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();

                        break;
                    //死亡草
                    case 4:
                        spawnPos = Projectile.Center + RandVelTwoPi(40f, 68f);
                        dir = (Projectile.Center - spawnPos).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos + dir * 40f, dir * -Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new StarShape(spawnPos + dir * 30f, dir * -Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.35f, 40).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 50f, dir * -Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();

                        break;
                    //寒蝉棘
                    case 5:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 38f);
                        dir = (Projectile.Center - spawnPos).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new ShinyCrossStar(spawnPos, dir * Main.rand.NextFloat(0.7f, 1.28f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, dir.ToRotation(), 0.5f, false).Spawn();
                        if (Main.rand.NextBool())
                            new HRShinyOrb(spawnPos, dir.RotatedByRandom(Pi) * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, 1f, 0.140f * 0.40f).Spawn();
                        if (Main.rand.NextBool())
                            new TurbulenceGlowOrb(spawnPos, 0.62f, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0.14f, RandRotTwoPi).Spawn();

                        break;

                    //火焰花
                    case 6:
                        spawnPos = Projectile.Center + RandVelTwoPi(10f, 28f);
                        dir = (-(Projectile.Center - spawnPos)).RotatedBy(PiOver2).ToSafeNormalize();
                        if (Main.rand.NextBool())
                            new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.35f).Spawn();
                        if (Main.rand.NextBool())
                            new HRShinyOrb(spawnPos + dir * 15f, dir * Main.rand.NextFloat(0.7f, 2.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0, 1, 0.08f).Spawn();
                        if (Main.rand.NextBool())
                            new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 4.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.18f).SpawnToNonPreMult();

                        break;
                }
            }
        }

        public void DoHoming()
        {
            float distance = (Projectile.Center - Owner.Center).LengthSquared();
            float searchdist = 330f;
            switch (AttackState)
            {
                case State.Floating:
                    Helper.Progress[0] = (int)Lerp((float)Helper.Progress[0], 0f, 0.2f);
                    if (Helper.GetAniProgress(0) < 0.02f)
                        Helper.Progress[0] = 0;
                    Projectile.scale = Lerp(Projectile.scale, 1.01f, 0.2f);
                    Projectile.velocity *= 0.86f;
                    Osci += ToRadians(2.5f);
                    Vector2 floatingPosition = Projectile.Center + Vector2.UnitY * (int)(5f * Math.Sin(Osci));
                    Projectile.Center = Vector2.Lerp(Projectile.Center, floatingPosition, 0.08f);
                    CurLifeTime = Projectile.timeLeft;
                    if (distance < searchdist * searchdist && Projectile.scale > 1f)
                        AttackState = State.Homing;

                    break;
                case State.Homing:
                    Projectile.timeLeft = CurLifeTime;
                    if (distance > (searchdist + 100f) * (searchdist + 100f))
                    {
                        Helper.IsDone[0] = false;
                        AttackState = State.Floating;
                        return;
                    }
                    if (Helper.IsDone[0])
                    {

                        Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.Center - Owner.Center).ToRotation(), 0.2f);
                        Projectile.HomingTarget(Owner.Center, -1, 11f, 10f);
                        if (Projectile.IntersectOwnerByDistance(30))
                        {
                            if (!IsIntersect)
                                HandleApplyBuff();
                            IsIntersect = true;

                        }
                        if (IsIntersect)
                        {

                            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.2f);
                            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.2f);
                            if (Projectile.Opacity <= 0.12f)
                                Projectile.Kill();
                        }
                    }
                    else
                    {
                        Osci += ToRadians(7.5f);
                        Vector2 floatingPosition2 = Projectile.Center + Vector2.UnitY * (int)(10f * Math.Sin(Osci));
                        Projectile.Center = Vector2.Lerp(Projectile.Center, floatingPosition2, 0.08f);

                        Helper.UpdateAniState(0);
                        Projectile.velocity = -Vector2.UnitY * (16f * (1 - (0.21f + Helper.GetAniProgress(0))));
                    }
                    break;

            }
        }

        public void HandleApplyBuff()
        {
            if (!Owner.HasBuff<HerbBagBuff>())
            {
                SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 0 }, Projectile.Center);
                Owner.HJScarlet().protectorPlantID = PlantArrat[PlantType];
                Owner.AddBuff(BuffType<HerbBagBuff>(), GetSeconds(8));
            }
            else
            {
                int[] list = Owner.HJScarlet().protectorHerbTimerList;
                float pitch = 0f;

                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] > 0)
                        pitch += 0.05f;
                }
                SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 0, Pitch = pitch }, Projectile.Center);
                Owner.HJScarlet().protectorHerbTimerList[PlantType] = GetSeconds(4);
                Owner.Heal(Main.rand.Next(1,5));
            }
        }

        public void DoFloating()
        {
        }

        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 15;
            DrawFirstFrameParticle();
        }

        private void DrawFirstFrameParticle()
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(4f);
                Vector2 dir = RandVelTwoPi(1.0f);

                switch (PlantType)
                {
                    //太阳花
                    default:
                        new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.LawnGreen, Color.Lime), 40, 0.35f).Spawn();
                        new StarShape(spawnPos, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.LawnGreen, Color.Lime), 0.35f, 40).Spawn();
                        new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();
                        break;

                    //月光花
                    case 1:
                        new TurbulenceGlowOrb(spawnPos, 0.2f, RandLerpColor(Color.DarkSlateBlue, Color.SkyBlue), 40, 0.1f, RandRotTwoPi).Spawn();
                        new KiraStar(spawnPos, Vector2.Zero, RandLerpColor(Color.DarkSlateBlue, Color.SkyBlue), 40, 0, 1, 0.140f * 0.25f, useAlt: true).Spawn();

                        break;

                    //闪耀根
                    case 2:
                        new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 40, 0.35f).Spawn();
                        new StarShape(spawnPos, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 0.35f, 40).Spawn();
                        new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();


                        break;
                    //水业草
                    case 3:
                        new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 7.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, 0.35f).Spawn();
                        new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 7.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 0.35f, 40).Spawn();
                        new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 7.24f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();

                        break;
                    //死亡草
                    case 4:
                        new StarShape(spawnPos + dir * 40f, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.35f, 40).Spawn();
                        new StarShape(spawnPos + dir * 30f, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.35f, 40).Spawn();
                        new SmokeParticle(spawnPos + dir * 50f, dir * -Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.14f).SpawnToNonPreMult();

                        break;
                    //寒蝉棘
                    case 5:
                        new ShinyCrossStar(spawnPos, dir * Main.rand.NextFloat(0.7f, 7.28f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, dir.ToRotation(), 0.5f, false).Spawn();
                        new HRShinyOrb(spawnPos, dir.RotatedByRandom(Pi) * Main.rand.NextFloat(0.7f, 7.24f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, 1f, 0.140f * 0.40f).Spawn();
                        new TurbulenceGlowOrb(spawnPos, 0.62f, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0.14f, RandRotTwoPi).Spawn();

                        break;

                    //火焰花
                    case 6:
                        new ShinyOrbParticle(spawnPos, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.35f).Spawn();
                        new HRShinyOrb(spawnPos + dir * 15f, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0, 1, 0.08f).Spawn();
                        new SmokeParticle(spawnPos + dir * 10f, dir * Main.rand.NextFloat(0.7f, 8.4f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.18f).SpawnToNonPreMult();

                        break;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            if (Projectile.oldPos.Length < 2)
                return false;

            int clamp = (int)Clamp(PlantType, 0, 6);
            Texture2D plant = GetVanillaAsset(VanillaAsset.Item, PlantArrat[clamp]);
            SB.EnterShaderArea();
            DrawFlowersGlow();
            SB.EndShaderArea();
            Projectile.DrawGlowEdge(plant, GetTrailColor() * Projectile.Opacity, posMove: 1.2f * Projectile.Opacity);
            Projectile.DrawProj(plant, Color.White * Projectile.Opacity);
            return false;
        }
        public void DrawPlants(Texture2D plant, Vector2 posOffset, Color color, int i)
        {
            float ratios = i / (float)Projectile.oldPos.Length;
            Color drawColor = Color.Lerp(color, Color.Transparent, ratios) * Clamp(Projectile.velocity.Length(), 0, 1);
            SB.Draw(plant, Projectile.oldPos[i] + Projectile.PosToCenter() + posOffset, null, drawColor * Projectile.Opacity, Projectile.oldRot[i], plant.ToOrigin(), Projectile.scale * (1 - ratios), 0, 0);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawFlowersGlow()
        {
            float ratiosScale = Helper.GetAniProgress(0);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            switch (PlantType)
            {
                default:
                    Texture2D kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    Texture2D ring = HJScarletTexture.Particle_RingHard.Value;
                    Texture2D orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, GetTrailColor() * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(kiraStar, pos, null, GetTrailColor() * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);
                    break;

                case 1:
                    kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    ring = HJScarletTexture.Particle_RingHard.Value;
                    orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, GetTrailColor() * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.25f, 0, 0);
                    SB.Draw(kiraStar, pos, null, GetTrailColor() * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);

                    break;

                case 2:
                    kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    ring = HJScarletTexture.Particle_RingHard.Value;
                    orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, GetTrailColor() * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.25f, 0, 0);
                    SB.Draw(kiraStar, pos, null, GetTrailColor() * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);

                    break;

                case 3:
                    kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    ring = HJScarletTexture.Particle_RingHard.Value;
                    orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, GetTrailColor() * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(kiraStar, pos, null, GetTrailColor() * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);

                    break;

                case 4:
                    kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    ring = HJScarletTexture.Particle_RingHard.Value;
                    orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, Color.Violet * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, Color.Lerp(Color.DarkViolet, Color.Violet, 0.5f) * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.35f, 0, 0);
                    SB.Draw(orbs, pos, null, Color.Lerp(Color.DarkViolet, Color.Violet, 0.5f) * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.25f, 0, 0);
                    SB.Draw(kiraStar, pos, null, Color.Violet * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);

                    break;

                case 5:
                    kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    ring = HJScarletTexture.Particle_RingHard.Value;
                    orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, GetTrailColor() * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.35f, 0, 0);
                    SB.Draw(kiraStar, pos, null, GetTrailColor() * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);

                    break;

                case 6:
                    kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
                    ring = HJScarletTexture.Particle_RingHard.Value;
                    orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                    SB.Draw(ring, pos, null, GetTrailColor() * Projectile.Opacity, 0, ring.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(orbs, pos, null, GetTrailColor() * Projectile.Opacity, 0, orbs.ToOrigin(), Projectile.scale * 0.45f, 0, 0);
                    SB.Draw(kiraStar, pos, null, GetTrailColor() * Projectile.Opacity, 0, kiraStar.ToOrigin(), Projectile.scale * 0.15f * (1 - ratiosScale), 0, 0);

                    break;
            }
        }

        public Color GetTrailColor()
        {
            return PlantType switch
            {
                //月光花
                1 => Color.DarkSlateBlue,
                //闪耀根
                2 => Color.SaddleBrown,
                //水业草
                3 => Color.DarkSeaGreen,
                //死亡草
                4 => Color.DarkViolet,
                //寒蝉棘
                5 => Color.RoyalBlue,
                //火焰花
                6 => Color.OrangeRed,
                //太阳花（默认）
                _ => Color.DarkOliveGreen,
            };
        }

        public void GetTrailColor(out Color outerColor, out Color midColor, out Color innerColor)
        {
            switch (PlantType)
            {
                //月光花
                case 1:
                    outerColor = Color.DeepSkyBlue;
                    midColor = Color.LawnGreen;
                    innerColor = Color.Lime;
                    break;
                //闪耀根
                case 2:
                    outerColor = Color.DarkSeaGreen;
                    midColor = Color.LawnGreen;
                    innerColor = Color.Lime;
                    break;
                //水业草
                case 3:
                    outerColor = Color.DarkSeaGreen;
                    midColor = Color.SeaGreen;
                    innerColor = Color.Lime;
                    break;
                //死亡草
                case 4:
                    outerColor = Color.DarkViolet;
                    midColor = Color.Purple;
                    innerColor = Color.Violet;
                    break;
                //寒蝉棘
                case 5:
                    outerColor = Color.RoyalBlue;
                    midColor = Color.DeepSkyBlue;
                    innerColor = Color.SkyBlue;
                    break;
                //火焰花
                case 6:
                    outerColor = Color.Red;
                    midColor = Color.OrangeRed;
                    innerColor = Color.Orange;
                    break;
                //太阳花（默认）
                default:
                    outerColor = Color.DarkSeaGreen;
                    midColor = Color.LawnGreen;
                    innerColor = Color.Lime;

                    break;
            }
        }
    }
}
