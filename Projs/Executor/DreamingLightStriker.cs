using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
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
    public class DreamingLightStriker : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<DreamingLight>().Texture;
        public NPC TargetNPC = null;
        public enum State
        {
            Shoot,
            Attack,
            Strike,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;

        }
        public AnimationStruct Helper = new AnimationStruct(3);
        public int TotalAttackHangingTime = 6;
        public bool IsHit = false;
        public int HeavyStrike = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.SetupImmnuity(30);
            Projectile.extraUpdates = 2;
            Projectile.scale = 0f;
            Projectile.penetrate = -1;

        }
        public override bool? CanHitNPC(NPC target)
        {
            return CanHitStatement(target) ? null : false;
        }
        public bool IsLegalTarget
        {
            get
            {
                return TargetNPC != null && TargetNPC.CanBeChasedBy();
            }
        }
        public bool CanHitStatement(NPC target)
        {
            return IsLegalTarget && TargetNPC.Equals(target) && AttackState != State.Shoot;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 22;
            Helper.MaxProgress[1] = 18;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Strike:
                    DoStrike();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoShoot()
        {
            Timer++;
            Projectile.rotation = Projectile.SpeedAffectRotation();
            Projectile.velocity *= 0.90f;
            if (Main.rand.NextFloat() >= Projectile.scale)
                new SmokeParticle(Projectile.Center.ToRandCirclePos(6f), Projectile.velocity.ToRandVelocity(ToRadians(10f), 1f, 14f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, RandRotTwoPi, 1, Main.rand.NextFloat(0.24f, 0.35f), Main.rand.NextBool()).SpawnToNonPreMult();
            Projectile.scale = Lerp(Projectile.scale, 1f, 0.2f);


            if (Projectile.MeetMaxUpdatesFrame(Timer, 18))
            {
                Projectile.scale = 1;
                StateSwitchTo(IsLegalTarget ? State.Attack : State.Return);
            }
        }

        public void DoAttack()
        {
            Projectile.rotation += 0.2f;
            if (IsLegalTarget)
            {
                if (Timer == 0)
                {
                    Projectile.velocity = (TargetNPC.Center - Projectile.Center).ToSafeNormalize() * 27f;
                    Timer = 1;
                }
                Projectile.HomingTarget(TargetNPC.Center, -1, 21f, 12f);
            }
            else
            {
                Projectile.velocity = RandVelTwoPi(18f, 22f);
                StateSwitchTo(State.Return);
            }
        }

        public void DoStrike()
        {
            if(!IsLegalTarget)
            {
                StateSwitchTo(State.Return);
                return;
            }
            if (!Helper.IsDone[0])
            {
                if (Projectile.FinalUpdate())
                    Helper.UpdateAniState(0);
                if (HeavyStrike < 1)
                    Projectile.position.Y = Lerp(Projectile.position.Y, TargetNPC.position.Y, 0.12f);
                Projectile.rotation = Projectile.SpeedAffectRotation();
                Projectile.AffactedByGrav(0.93f, 1f, 0.17f, 30f);
            }
            else
                StrikeAI();
        }

        public void StrikeAI()
        {
            //距离足够进直接修正这个速度
            if (Vector2.DistanceSquared(Projectile.Center, TargetNPC.Center) < 70f * 70f)
            {
                Projectile.velocity = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (30f);
                Projectile.rotation = Projectile.velocity.ToRotation() - PiOver4;
            }
            else
            {
                float angleOffset = WrapAngle(Projectile.AngleTo(TargetNPC.Center) - Projectile.velocity.ToRotation());
                angleOffset = Clamp(angleOffset, -0.2f, 0.2f);
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset);
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() - PiOver4, 0.12f);
                Projectile.scale = Lerp(Projectile.scale, 1.10f, 0.25f);
                //控速
                if (Projectile.velocity.Length() < 30f)
                    Projectile.velocity *= 1.45f;
                else
                    Projectile.velocity *= 0.9f;
            }
        }

        public void DoReturn()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = Lerp(Projectile.scale, 1f, 0.1f);
            if (!Helper.IsDone[1])
            {
                if (Projectile.FinalUpdate())
                    Helper.UpdateAniState(1);
                Projectile.velocity *= 0.915f;
                Projectile.rotation = Projectile.SpeedAffectRotation();
            }
            else
            {
                if (Timer == 0f)
                {
                    SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Toss with { Pitch = -.30f }, Projectile.Center);
                    Projectile.velocity = (Owner.Center - Projectile.Center).ToSafeNormalize() * 27f;
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 spawnPos = Projectile.Center.ToRandCirclePos(6f) - (Projectile.velocity).ToRandVelocity(0f, 4f, 8.4f) + Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                        Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-1.4f, 1.9f) - Projectile.velocity.ToRandVelocity(0f, 0f, 4.8f);
                        Color drawColor = RandLerpColor(Color.DarkViolet, Color.Violet);
                        new ShinyCrossStar(spawnPos, vel, drawColor, 40, RandRotTwoPi, 1f, 0.5f, false, 0.2f).Spawn();
                    }
                    Timer = 1;
                }
                Projectile.rotation += 0.2f;
                Projectile.HomingTarget(Owner.Center, -1, 27, 20);
                if (Projectile.IntersectOwnerByDistance(60f))
                    Projectile.Kill();

            }
        }

        private void StateSwitchTo(State id)
        {
            Timer *= 0;
            Projectile.netUpdate = true;
            AttackState = id;
        }
        //这下面这个过于吓人了。但是总体来说还是能理的
        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            //第一步，检查是否正在进行挂载攻击
            //只有进行挂载攻击时，我们才会生成闪亮星星
            if (IsHit)
            {
                if (Projectile.FinalUpdateNextBool(4))
                    new KiraStar(Projectile.Center.ToRandCirclePos(6f), RandVelTwoPi(1f, 4.8f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, 0.25f * Main.rand.NextFloat(0.85f, 1.1f)).SpawnToNonPreMult();
                return;
            }
            if (Projectile.FinalUpdateNextBool(4))
            {
                //第二步，检查是否刚丢出，且不再挂载
                //如果不符合，则生成闪亮星星
                if (AttackState == State.Strike || AttackState == State.Return)
                    new KiraStar(Projectile.Center.ToRandCirclePos(6f), RandVelTwoPi(1f, 4.8f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, 0.25f * Main.rand.NextFloat(0.85f, 1.1f)).SpawnToNonPreMult();
                else
                    new EmptyRing(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, 0.21f * Main.rand.NextFloat(0.8f, 1.2f), 1, altRing: false).SpawnToNonPreMult();
            }
            if (Projectile.FinalUpdateNextBool())
                new SmokeParticle(Projectile.Center.ToRandCirclePos(32), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.125f * Projectile.scale, Main.rand.NextBool()).SpawnToNonPreMult();
            //第三步，下放的绿色粒子不会在锤子进行反复重击时生成
            //其原因为，在重击的过程中会召唤火球。火球本身就有绿色的粒子
            if (AttackState == State.Strike)
                return;
            if (Projectile.FinalUpdateNextBool())
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.Green, Color.GreenYellow), 40, 0.8f * Main.rand.NextFloat(0.6f, 1.2f)).Spawn();
            if (Projectile.FinalUpdate())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(25), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, RandRotTwoPi, 1, 0.5f, false, 0.2f).Spawn();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = AttackState == State.Strike ? Color.LimeGreen : Color.Transparent;
            Projectile.DrawGlowEdge(drawColor);
            Projectile.DrawProj(Color.White);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Attack && Projectile.numHits > TotalAttackHangingTime)
            {
                StateSwitchTo(State.Strike);
                Projectile.velocity = Vector2.UnitX * Main.rand.NextFloat(32f, 38f) * Math.Sign(Projectile.velocity.X - Owner.velocity.X);
                SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Pitch = -0.2f }, Projectile.Center);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
                SpawnFireball(target);
                UpdateMiscHitParticle(target.Center);
                IsHit = false;

            }
            else if (AttackState == State.Strike)
            {
                if (HeavyStrike < 1)
                {
                    Projectile.velocity = Projectile.SafeDir().RotatedBy(ToRadians(45f)) * Main.rand.NextFloat(32f, 38f);
                    Helper.IsDone[0] = false;
                    Helper.Progress[0] = 0;
                    HeavyStrike += 1;
                    SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Pitch = 0.2f }, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 40f, 40, Projectile.velocity.ToRotation(), ToRadians(20f));
                }
                else
                {
                    float speed = Projectile.velocity.Length();
                    Projectile.velocity = (Projectile.velocity).RotatedBy(ToRadians(20)).ToSafeNormalize() * speed;
                    StateSwitchTo(State.Return);
                    SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Pitch = 0.4f }, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 40f, 40, Projectile.velocity.ToRotation(), ToRadians(10f));
                }
                SpawnFireball(target);
                UpdateMiscHitParticle(target.Center);
            }
            else
            {
                IsHit = true;
                int offset = PerformanceMode ? 10 : 0;
                for (int i = 0; i < 16 - offset; i++)
                {
                    Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16f);
                    ShadowNebulaAlt.SpawnCircle(spawnPos, RandVelTwoPi(1, 4), Main.rand.NextFloat(0.4f, 0.7f) * 0.25f);
                }
                SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { Pitch = -0.2f }, Projectile.Center);

            }
        }
        private void UpdateMiscHitParticle(Vector2 target)
        {
            int offset = PerformanceMode ? 12 : 0;
            for (int i = 0; i < 24 - offset; i++)
            {
                Vector2 vel2 = Projectile.velocity.ToRandVelocity(ToRadians(15f), 4f, 12f);
                ShadowNebulaAlt.SpawnCircle(target + vel2.ToRandVelocity(0f, 3f, 4f), vel2, Main.rand.NextFloat(0.89f, 1.2f) * 0.16f, 60);
            }
            for (int i = 0; i < 32 - offset; i++)
            {
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(15f), 4f, 21f);
                ShadowNebulaAlt.SpawnSharpTearClean(target + vel.ToRandVelocity(0f, 3f, 4f), vel, Main.rand.NextFloat(0.89f, 1.2f) * 0.6f, 60);
            }
        }
        public void SpawnFireball(NPC target)
        {
            if(Owner.HasProj<DreamingLightMinion>())
            for (int i = 0; i < 6; i++)
            {
                Vector2 spawnPos = target.Center.ToRandCirclePos(0f);
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 18f, 24f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<DreamlessNightArrow>(), Projectile.damage / 3, 0f, Owner.whoAmI);
                ((DreamlessNightArrow)proj.ModProjectile).CurSpeed = 0;
            }
            for (int i = 0; i < 3; i++)
            {

                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 10f, 14f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, vel, ProjectileType<DreamingLightFireball>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                ((DreamingLightFireball)proj.ModProjectile).TargetNPC = target;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

    }
}
