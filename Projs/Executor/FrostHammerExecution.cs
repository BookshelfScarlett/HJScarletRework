using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class FrostHammerExecution : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, 917);
        public override Vector2 TileHitbox => new Vector2(16, 16);
        public NPC CurTarget = null;
        public bool SetHoming = false;
        public int SearchCooldown = 0;
        public bool Attacking = false;
        public AnimationStruct Helper = new(3);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.scale *= 0.9f;
            Projectile.timeLeft = GetSeconds(30);
            Projectile.noEnchantmentVisuals = true;
        }

        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 15 * Projectile.MaxUpdates;
                    SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { MaxInstances = 0 });
                        for (int i = 0; i < 32; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16);
                Vector2 vel = Projectile.velocity.ToRandVelocity(0,4f,10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .45f, 0.28f * 0.35f, Main.rand.NextBool()).Spawn();
            }
            for (int i = 0; i < 32; i++)
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(10);
                    p.Velocity = Projectile.velocity.ToRandVelocity(0, 0.4f, 10f);
                    p.Scale = Main.rand.NextFloat(0.9f, 1.1f) * 0.1f;
                    p.Opacity = 1f;
                    p.DrawColor = RandLerpColor(Color.AliceBlue, Color.RoyalBlue);
                    p.Lifetime = 40;
                    p.GlowCenterMult = 0.5f;
                });
            }

        }
        public override void ProjAI()
        {
            ResetTimer();
            if (Helper.IsDone[0])
            {
                UpdateAttack();
                ChaseEnemy();
            }
            else
            {
                Helper.UpdateAniState(0);
                HandleBeginAnimation();
            }
            if (Projectile.IsOutScreen() || Main.rand.NextBool())
                return;
            
            if (Main.rand.NextBool(4))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(30), Vector2.UnitY * Main.rand.NextFloat(1.8f, 5.4f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 0, 1, 0.68f * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();
            if (!Attacking)
            {
                if (Main.rand.NextBool(9))
                {
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = Projectile.Center.ToRandCirclePos(10);
                        p.Velocity = Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 4.2f);
                        p.DrawColor = RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue);
                        p.Lifetime = 40;
                        p.Scale = Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * .1f;
                        p.GlowCenterMult = 0.75f;
                    });
                }
        if (Main.rand.NextBool(8))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.25f, Main.rand.NextBool()).Spawn();
            if (Main.rand.NextBool(5))
                new SnowCloud(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.56f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.07f, true).SpawnToPriority();
            }

            Attacking = false;

        }

        public void HandleBeginAnimation()
        {
            Projectile.velocity *= 0.90f;
            Projectile.rotation = Projectile.SpeedAffectRotation() * Math.Abs(Projectile.velocity.X);
        }

        public void UpdateAttack()
        {
            if (!SetHoming)
                return;
            foreach (var target in Main.ActiveProjectiles)
            {
                if (target.owner != Projectile.owner)
                    continue;
                if (Projectile.damage < 5)
                    continue;
                if (target.type != ProjectileType<FrostHammerIceSpike>())
                    continue;
                ((FrostHammerIceSpike)target.ModProjectile).ActiveHoming = true;
                if (CurTarget.IsLegal())
                    ((FrostHammerIceSpike)target.ModProjectile).CurTarget = CurTarget;
            }
        }

        public void ResetTimer()
        {
            if (SearchCooldown > 0)
                SearchCooldown--;
        }
        
        public void ChaseEnemy()
        {
            if(Main.mouseRight && Main.mouseRightRelease && SearchCooldown == 0)
            {
                NPC target = Main.MouseWorld.FindClosestTarget(200, ignoreTiles: false);
                if (target.IsLegal())
                {
                    Vector2 chargeVel = Projectile.Center.GetNormalVector2(target.Center) * 23f;
                    Projectile.velocity = chargeVel;
                    ChargeParticle();
                    CurTarget = target;
                    SpawnParticle(target);
                    SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { MaxInstances = 0 });
                }
                SearchCooldown = 60;
            }
            if (CurTarget == null)
            {
                Projectile.velocity *= .2f;
                IdleStatement();
                Projectile.rotation += 0.05f / Projectile.MaxUpdates;
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1000))
                {
                    SoundEngine.PlaySound(HJScarletSounds.Frostwave_LightRelease with { MaxInstances = 0 ,Pitch = -.5f});
                    CurTarget = target;
                    Vector2 chargeVel = Projectile.Center.GetNormalVector2(target.Center) * 23f;
                    Projectile.velocity = chargeVel;
                    ChargeParticle();
                }
            }
            else
            {
                Projectile.rotation += 0.2f / Projectile.MaxUpdates;
                if (CurTarget.IsLegal())
                    Projectile.HomingTarget(CurTarget.Center, -1, 16, 20);
                else
                    CurTarget = null;
            }
        }

        private void SpawnParticle(NPC target)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = RandVelTwoPi(1.2f,3.6f);
                float scale = Main.rand.NextFloat(0.4f, 0.9f) * .12f;
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = pos;
                    p.Velocity = vel;
                    p.DrawColor = RandLerpColor(Color.LightBlue, Color.RoyalBlue);
                    p.Lifetime = 45;
                    p.Scale = scale;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.75f;
                });
            }

            for (int i = 0; i < 30; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = target.Center + RandVelTwoPi(10f, 30f);
                Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(1f, 10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .25f, 0.28f, Main.rand.NextBool()).Spawn();
            }

        }

        public void ChargeParticle()
        {
            for (int i = 0; i < 32; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16);
                Vector2 vel = Projectile.velocity.ToRandVelocity(0,-14f,10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .45f, 0.28f * 0.35f, Main.rand.NextBool()).Spawn();
            }
            for (int i = 0; i < 32; i++)
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(10);
                    p.Velocity = -Projectile.velocity.ToRandVelocity(0, 0.4f, 10f);
                    p.Scale = Main.rand.NextFloat(0.9f, 1.1f) * 0.1f;
                    p.Opacity = 1f;
                    p.DrawColor = RandLerpColor(Color.AliceBlue, Color.RoyalBlue);
                    p.Lifetime = 40;
                    p.GlowCenterMult = 0.5f;
                });
            }
        }

        public float Oscillation = 0;
        public void IdleStatement()
        {
            Oscillation = Projectile.ClampOscillation(Oscillation, 1.5f);
            //锤子应当朝向的位置
            Vector2 mountedDirection = Vector2.UnitX.RotatedBy(Oscillation); 
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = Owner.Center + mountedDirection * 120;
            //实际更新位置
            float lerpValue = (0.20f) / Projectile.MaxUpdates;
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, lerpValue);
        }
        public override bool ShouldUpdatePosition()
        {
            return CurTarget != null || !Helper.IsDone[0];
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.Equals(CurTarget) && CurTarget != null)
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits > 1)
            {
                Attacking = true;
                CurTarget = target;
                SetHoming = true;
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = RandVelTwoPi(1.2f,3.6f);
                float scale = Main.rand.NextFloat(0.4f, 0.9f) * .12f;
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = pos;
                    p.Velocity = vel;
                    p.DrawColor = RandLerpColor(Color.LightBlue, Color.RoyalBlue);
                    p.Lifetime = 45;
                    p.Scale = scale;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.75f;
                });
            }

            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = target.Center + RandVelTwoPi(10f, 30f);
                Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(1f, 10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .25f, 0.28f, Main.rand.NextBool()).Spawn();
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White, useOldPos: true);
            return false;
        }
    }
}
