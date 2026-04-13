using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class DreamingLightProj : HJScarletProj
    {
        public override string Texture => GetInstance<DreamingLight>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7, 2);
        }
        public enum State
        {
            Shoot,
            Return

        }
        public AnimationStruct Helper = new AnimationStruct(5);
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public bool StopSpawnAdditionHammer = false;
        public NPC TargetNPC = null;
        public override void ExSD()
        {
            Projectile.penetrate = 10;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(45);
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 60;
            Helper.MaxProgress[1] = 60;
            Helper.MaxProgress[2] = 60;
            Helper.MaxProgress[3] = 60;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }
        #region 主锤的AI
        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoShoot()
        {
            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Timer++;
            Projectile.rotation += 0.2f * Projectile.spriteDirection;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 6))
            {
                Projectile.netUpdate = true;
                Timer = 0;
                AttackState = State.Return;
            }
        }

        public void DoReturn()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                UpdateAnistateZero();
            }
            else
            {
                if (Timer == 0f)
                {
                    InitReturn();
                    Timer += 1;
                }
                ReturnToOwner();
            }
        }

        public void PoweredUpHammer()
        {
        }

        public void InitReturn()
        {
            Projectile.velocity = (Projectile.Center - Owner.Center).ToSafeNormalize() * -12f;
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[2] with { MaxInstances = 0, Pitch = -0.20f }, Projectile.Center);
        }

        public void ReturnToOwner()
        {
            Projectile.rotation = (-Projectile.velocity).ToRotation();
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.IntersectOwnerByDistance(90))
            {

                if (!Owner.HasProj<DreamingLightMinion>())
                    Projectile.AddExecutionTime(ItemType<DreamingLight>());
                Projectile.Kill();
            }
        }

        public void UpdateAnistateZero()
        {
            Projectile.velocity *= 0.92f;
            Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.velocity).ToRotation(), .1f);
        }
        #endregion
        public void UpdateParticle()
        {
            bool canNotUpdate = Projectile.IsOutScreen() || (AttackState == State.Return && Main.rand.NextFloat() > Helper.GetAniProgress(0));
            if (canNotUpdate)
                return;
            if (PerformanceMode && Main.rand.NextBool(3))
                return;
            if (Projectile.FinalUpdate())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(25), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, RandRotTwoPi, 1, 0.5f, false, 0.2f).Spawn();
            if (Main.rand.NextBool(4))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.Green, Color.GreenYellow), 40, 0.8f * Main.rand.NextFloat(0.6f, 1.2f)).Spawn();
            if (Main.rand.NextBool(3))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(32), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.185f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Projectile.FinalUpdateNextBool())
                new EmptyRing(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, 0.26f * Main.rand.NextFloat(0.8f, 1.2f), 1, altRing: false).SpawnToNonPreMult();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8 - PerformanceMode.ToInt() * 4; i++)
                new ShinyCrossStar(target.Center.ToRandCirclePos(7f), RandVelTwoPi(0f, 8f), RandLerpColor(Color.Violet, Color.DarkViolet), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.44f, 0.70f), false).Spawn();
            for (int i = 0; i < 2 - PerformanceMode.ToInt(); i++)
                new KiraStar(target.Center.ToRandCirclePosEdge(4f), RandVelTwoPi(1f, 3f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, 0.34f).SpawnToNonPreMult();

            if (Projectile.HJScarlet().ExecutionStrike && !Owner.HasProj<DreamingLightMinion>(out int projID))
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, projID, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                proj.rotation = Projectile.rotation;
                for (int i = 0; i < 8-PerformanceMode.ToInt() * 4; i++)
                    new Fire(target.Center.ToRandCirclePos(6), RandVelTwoPi(0.1f, 8.8f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1, 0.25f).SpawnToNonPreMult();
            }
            if (Projectile.numHits < 1 && !StopSpawnAdditionHammer)
            {
                TargetNPC = target;
                if (Owner.HasProj<DreamingLightMinion>())
                    SpawnShadowNebulaBeam(target);
                else
                {
                }
                SpawnDreamingStriker();
            }
        }
        public void SpawnShadowNebulaBeam(NPC target)
        {
            SoundEngine.PlaySound(HJScarletSounds.Dream_Toss with { Pitch = 0f, Volume = 1.5f, MaxInstances = 0 }, Projectile.Center);
            new CrossGlow(target.Center, Color.DarkViolet, 40, 1, 0.38f).Spawn();
            new CrossGlow(target.Center, Color.Violet, 40, 1, 0.28f).Spawn();
            new CrossGlow(target.Center, Color.Violet, 40, 1, 0.18f).Spawn();

                int offset = PerformanceMode ? 1 : 0;
            for (int i = 0; i < 5; i++)
            {
                Vector2 rotArg = (Projectile.velocity.ToRotation() + ToRadians(360f / 5 * i)).ToRotationVector2();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, 4f * rotArg, ProjectileType<DreamingLightBeam>(), Projectile.damage, 1f, Owner.whoAmI);
                proj.timeLeft = 68;
                proj.usesIDStaticNPCImmunity = true;
                proj.usesLocalNPCImmunity = false;
                proj.idStaticNPCHitCooldown = 30;
                for (int j = 0; j < 3 - offset; j++)
                    ShadowNebulaAlt.SpawnCircle(target.Center, RandVelTwoPi(0.2f, 1.2f) + rotArg.ToSafeNormalize() * 1.1f, 0.15f);
            }

        }
        public void SpawnDreamingStriker()
        {
            if (!StopSpawnAdditionHammer && !Owner.HasProj<DreamingLightStriker>())
            {
                Vector2 dir = (-Projectile.velocity).ToSafeNormalize().RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt());
                Vector2 spawnPos = Owner.ToMouseVector2() * -Main.rand.NextFloat(200f, 280f) + Owner.MountedCenter;
                Vector2 vel = (dir.RotateRandom(PiOver4) * Main.rand.NextFloat(34f, 38f));
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<DreamingLightStriker>(), Projectile.damage, 4f, Owner.whoAmI);
                SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Toss with { MaxInstances = 0, Pitch = -0.5f }, spawnPos);
                ((DreamingLightStriker)proj.ModProjectile).TargetNPC = TargetNPC;

                int offset = PerformanceMode ? 30 : 0;
                //粒子效果们
                for (int i = 0; i < 60 - offset; i++)
                {
                    Vector2 nebulaVel = RandVelTwoPi(-2f, 4.6f);
                    Vector2 nebulaPos = spawnPos.ToRandCirclePos(0f);
                    ShadowNebula.SpawnParticle(nebulaPos, nebulaVel, Main.rand.NextFloat(0.1f, 0.15f), HJScarletTexture.Texture_WhiteCircle.Value);
                }
                for (int i = 0; i < 60; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 nebulaVel = vel.ToSafeNormalize().RotatedBy(PiOver2 * j) * (i * 0.18f - 2f);
                        Vector2 nebulaPos = spawnPos;
                            ShadowNebula.SpawnParticle(nebulaPos, nebulaVel, 0.180f * Lerp(2.0f, 0.5f, i / 60f), HJScarletTexture.Texture_WhiteCircle.Value);
                    }
                }
                offset = PerformanceMode ? 5 : 0;
                for (int i = 0; i < 10 - offset; i++)
                {
                    Vector2 pos = spawnPos.ToRandCirclePos(6f);
                    vel = RandVelTwoPi(1f, 4.8f);
                    float scale = 0.25f * Main.rand.NextFloat(0.85f, 1.1f);
                    new KiraStar(pos, vel, RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, scale).Spawn();
                    new KiraStar(pos, vel, RandLerpColor(Color.White, Color.Silver), 40, 0, 1, scale * 0.75f).Spawn();
                }
                new CrossGlow(spawnPos, RandLerpColor(Color.DarkViolet, Color.Violet), 40, 1, 0.20f).Spawn();
                new CrossGlow(spawnPos, Color.Violet, 40, 1, 0.13f).Spawn();
                new CrossGlow(spawnPos, Color.Violet, 40, 1, 0.10f).Spawn();

            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            //两个状态机都会查询。
            return null;
        }

        public bool CanHitAddition(NPC target)
        {
            bool hit = TargetNPC.Equals(target) && TargetNPC != null;
            bool state = Helper.GetAniProgress(1) < 1f && Helper.IsDone[0];
            return hit && state;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White, rotFix: PiOver4);
            return false;
        }
    }
}
