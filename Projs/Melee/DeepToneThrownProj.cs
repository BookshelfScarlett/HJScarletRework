using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<DeepTone>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(24, 2);
        public enum Style
        {
            Shoot,
            Stuck
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public List<Vector2> PortalPosList = [];
        public ref float AttackTimer => ref Projectile.ai[1];
        public Vector2 ImpactPos = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.extraUpdates = 4;
            Projectile.height = Projectile.width = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = 4;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {

                Vector2 mouthPos = Owner.Center - Vector2.UnitY * Main.rand.NextFloat(150f, 214f);
                Vector2 dir = (Projectile.SafeDir() - (mouthPos - Main.MouseWorld).RotatedBy(ToRadians(10f * Math.Sign(Projectile.velocity.X)))).ToSafeNormalize();
                for (int i = -1; i < 2; i += 1)
                {
                    Vector2 vel = dir.RotatedBy(ToRadians(Main.rand.NextFloat(10, 12)) * i) * Main.rand.NextFloat(12f, 16f);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), mouthPos, vel, ProjectileType<DeepToneMouth>(), Projectile.damage / 2, 1f, Owner.whoAmI);
                }
                for (int i = 0; i < 18; i++)
                {
                    new SmokeParticle(mouthPos.ToRandCirclePos(12f), RandVelTwoPi(0f, 9f), RandLerpColor(Color.DarkGreen, Color.Black), 40, RandRotTwoPi, 1, 0.7f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
                }
                new CrossGlow(mouthPos, Color.DarkSeaGreen, 40, 1, 0.34f, true).Spawn();
                new CrossGlow(mouthPos, Color.SeaGreen, 40, 1, 0.31f, true).Spawn();
                new CrossGlow(mouthPos, Color.White, 40, 1, 0.28f, true).Spawn();
            }
            switch (AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.Stuck:
                    DoStuck();
                    break;
            }
        }

        public void DoShoot()
        {
            AttackTimer += 1;
            if (AttackTimer > 30f)
            {
                //SpawnTenctacle(-1);
                AttackTimer = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            //粒子
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            if (Main.rand.NextBool(4))
            {
                Vector2 offset2 = Projectile.SafeDir() * 100;
                Vector2 offset = Projectile.SafeDir() * 90f + Projectile.SafeDir().RotatedBy(PiOver2) * 2.1f;
                Vector2 mountedPos = Projectile.Center + offset - offset2;
                Dust d = Dust.NewDustPerfect(mountedPos + Main.rand.NextVector2Circular(6f, 6f), DustID.JungleTorch);
                d.noGravity = true;
            }
            if (Main.rand.NextBool(4))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(20f), -Vector2.UnitY * 1.1f, RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 40, 0, 1, 0.6f * Main.rand.NextFloat(0.5f, 0.8f), false).Spawn();
            //除非超出这个最大点位数，不然添加传送门了
        }
        public void DoStuck()
        {
            Projectile.velocity *= 0.01f;
            NPC target = Projectile.ToHJScarletNPC();
            if (target != null && target.CanBeChasedBy())
            {
                Projectile.Center = target.Center + ImpactPos;
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override bool PreKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Projectile.oldVelocity.ToSafeNormalize() * Main.rand.NextFloat(0f, 8f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.DarkGreen, Color.DarkOliveGreen, 0.50f), Color.DarkOliveGreen), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                if (Main.rand.NextBool())

                    new SmokeParticle(spawnpos, Projectile.oldVelocity.ToSafeNormalize().RotatedByRandom(Pi) * Main.rand.NextFloat(0.2f, 8f), RandLerpColor(Color.Lerp(Color.DarkOliveGreen, Color.DarkGreen, 0.75f), Color.DarkSeaGreen), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
            }
            for (int j = 0; j < 30; j++)
            {
                for (int i = -1; i <= 2; i += 2)
                {
                    Vector2 dir = -Projectile.SafeDirByRot() * i;
                    new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
                }
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6f);
                Vector2 vel = RandVelTwoPi(1f, 4.9f);
                new HRShinyOrb(pos, vel, RandLerpColor((Color.Lerp(Color.DarkSeaGreen, Color.DarkGreen, 0.5f)), Color.DarkGreen), 40, 0.12f).Spawn();
                new HRShinyOrb(pos, vel, Color.White, 40, 0.12f * 0.5f).Spawn();
            }

            SpawnTenctacle(-1);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 dir = RandDirTwoPi;
            for (int i = -1; i < 2; i += 1)
            {
                Vector2 vel = dir.RotatedBy(ToRadians(i * 120f)) * 12f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, vel, ProjectileType<DeepToneHealOrb>(), Projectile.damage / 3, 0, Owner.whoAmI);
                ((DeepToneHealOrb)proj.ModProjectile).UseHeal = false;
            }
            for (int i = 0; i < 30; i++)
            {
                Vector2 rot = Vector2.UnitX.RotatedBy(ToRadians(360f / 30 * i));
                new SmokeParticle(target.Center + rot * 10f, rot * Main.rand.NextFloat(5.2f, 6.9f), RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 40, RandRotTwoPi, 1, 0.47f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 15; i++)
            {
                new SmokeParticle(target.Center, RandVelTwoPi(0.72f, 4.9f), RandLerpColor(Color.DarkGreen, Color.Black), 40, RandRotTwoPi, 1, 0.47f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            }
            new CrossGlow(target.Center, Color.DarkSeaGreen, 40, 1, 0.34f * 0.4f, true).Spawn();
            new CrossGlow(target.Center, Color.SeaGreen, 40, 1, 0.31f * 0.4f, true).Spawn();
            new CrossGlow(target.Center, Color.White, 40, 1, 0.28f * 0.4f, true).Spawn();
        }

        private void DoShoot_OnHit(NPC target)
        {
        }
        private void SpawnTenctacle(int targetIndex)
        {
            if (!Projectile.IsMe())
                return;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 offset2 = Projectile.SafeDir() * 100;
            Vector2 offset = Projectile.SafeDir() * 90f + Projectile.SafeDir().RotatedBy(PiOver2) * 2.1f - offset2;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.DarkOliveGreen, Color.DarkSeaGreen, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                SB.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter() + offset, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1f, 1.5f), 0, 0);
            }
            for (int k = 0; k < 6; k++)
            {
                float rads2 = (float)k / 6;
                Color drawColor = (Color.Lerp(Color.DeepPink, Color.LightPink, rads2) with { A = 0 }) * 0.5f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads2);
                Vector2 offsetPos = Projectile.SafeDir() * (110f - 10f * k) + Projectile.SafeDir().RotatedBy(PiOver2) * 1.5f - offset2;
                SB.Draw(star, Projectile.Center - Main.screenPosition + offsetPos, null, drawColor, Projectile.rotation - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1f, 1.2f), 0, 0);
            }
            //最顶端位置，绘制一个小的辉光 
            Projectile.DrawGlowEdge(Color.White, rotFix: ToRadians(135), drawPosOffset: offset2);
            Projectile.DrawProj(Color.White, drawTime: 4, rotFix: ToRadians(135), drawPosOffset: offset2);
            return false;
        }
    }
}
