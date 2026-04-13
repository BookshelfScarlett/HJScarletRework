using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class MantleLayerExecution : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<MantleLayerProj>().Texture;
        public enum State
        {
            Idle,
            Attack,
            Return,
            Strike
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public int HangingHitTime = 0;
        public int TotalHitTime = 10;
        public bool IsHit = false;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.SetupImmnuity(15);
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        private void UpdateAttackAI()
        {

            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            switch(AttackType)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Return:
                    DoReturn();
                    break;
                case State.Strike:
                    DoStrike();
                    break;
            }
        }


        private void DoIdle()
        {
            Projectile.rotation = Projectile.SpeedAffectRotation();
            Projectile.velocity *= 0.858f;
            Timer++;
            if(Projectile.MeetMaxUpdatesFrame(Timer, 15))
                UpdateNextAttack(State.Attack);
        }

        private void DoAttack()
        {
            Projectile.rotation += 0.2f;
            if (Projectile.GetTargetSafe(out NPC target, false))
                Projectile.HomingTarget(target.Center, -1, 24, 20);
            else
                UpdateNextAttack(State.Return);
        }
        private void DoReturn()
        {
            Timer++;
            Projectile.scale = Lerp(Projectile.scale, 1f, 0.1f);
            Projectile.rotation += 0.2f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            if (!Projectile.MeetMaxUpdatesFrame(Timer, 10f))
            {
                Projectile.velocity *= 0.86f;
                return;
            }
            Projectile.HomingTarget(Owner.Center, -1, 20, 20);
            
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                Projectile.Kill();
        }
        private void DoStrike()
        {
            Projectile.tileCollide = false;
            Timer++;
            if (Projectile.GetTargetSafe(out NPC CurTarget, false))
            {

                if (!Projectile.MeetMaxUpdatesFrame(Timer, 27))
                {
                    Projectile.position.X = Lerp(Projectile.position.X, CurTarget.position.X, 0.12f);
                    Projectile.rotation = Projectile.SpeedAffectRotation();
                    Projectile.velocity *= 0.92f;
                    Projectile.velocity.Y *= 1f;
                    if (Projectile.velocity.Y < 30f)
                        Projectile.velocity.Y += 0.37f;
                    return;
                }
                StrikeAI(CurTarget);
            }
            else
                UpdateNextAttack(State.Return);
        }
        private void StrikeAI(NPC CurTarget)
        {
            //距离足够进直接修正这个速度
            if (Vector2.DistanceSquared(Projectile.Center, CurTarget.Center) < 70f * 70f)
            {
                Projectile.velocity = (CurTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (30f);
                Projectile.rotation = Projectile.velocity.ToRotation() - PiOver4;
            }
            else
            {
                float angleOffset = WrapAngle(Projectile.AngleTo(CurTarget.Center) - Projectile.velocity.ToRotation());
                angleOffset = Clamp(angleOffset, -0.2f, 0.2f);
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset);
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() - PiOver4, 0.12f);
                Projectile.scale = Lerp(Projectile.scale, 1.20f, 0.25f);
                //控速
                if (Projectile.velocity.Length() < 30f)
                    Projectile.velocity *= 1.45f;
                else
                    Projectile.velocity *= 0.9f;
            }
        }
        private void UpdateParticle()
        {
            if (Projectile.IsOutScreen() || IsHit)
                return;
            if(Projectile.FinalUpdateNextBool())
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f,1.8f), RandLerpColor(Color.OrangeRed, Color.DarkGray), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.4f, 0.8f) * 0.42f, Main.rand.NextBool()).SpawnToNonPreMult();
            if(Projectile.FinalUpdateNextBool())
                new SmokeParticle(Projectile.Center.ToRandCirclePos(15), -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f,1.8f), RandLerpColor(Color.OrangeRed, Color.DarkGray), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.4f, 0.8f) * 0.32f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Projectile.FinalUpdateNextBool(3))
                new EmptyRing(Projectile.Center.ToRandCirclePos(15), -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 1.8f), RandLerpColor(Color.OrangeRed, Color.DarkOrange), 40, Main.rand.NextFloat(0.5f, 0.8f) * 0.30f, 1f, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
        }

        private void UpdateNextAttack(State id)
        {
            Projectile.netUpdate = true;
            Timer = 0;
            AttackType = id;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            IsHit = AttackType == State.Attack;
            if (AttackType == State.Attack)
            {
                SoundEngine.PlaySound(SoundID.Item69 with { MaxInstances = 2}, Projectile.Center);
                UpdateAttackOnHit(target);
                UpdateAttackOnHitParticle(target);

            }
            if (AttackType == State.Strike && Projectile.MeetMaxUpdatesFrame(Timer, 20))
            {
                SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Pitch = 0.2f}, Projectile.Center);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 40f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
                UpdateMiscHitParticle(Projectile.Center);
                UpdateNextAttack(State.Return);
            }
        }

        public void UpdateAttackOnHitParticle(NPC target)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16f);
                new ShinyCrossStar(spawnPos, RandVelTwoPi(8f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, 0.75f, 0.2f).Spawn();
            }
            for (int i = 0; i < 7; i++)
                new SmokeParticle(Projectile.Center.ToRandCirclePos(16f), RandVelTwoPi(4f, 10f), RandLerpColor(Color.OrangeRed, Color.DarkOrange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.4f, 0.7f) * 0.5f, Main.rand.NextBool()).SpawnToNonPreMult();
            for (int i = 0; i < 8; i++)
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePos(12f), 1.1f, RandLerpColor(Color.OrangeRed, Color.Orange), 60, 0.1f, RandRotTwoPi).Spawn();
        }
        private void UpdateMiscHitParticle(Vector2 target)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 vec = Projectile.velocity.ToRandVelocity(ToRadians(30f), 1.8f, 16.8f);
                Dust d = Dust.NewDustPerfect(target, DustID.Ash);
                d.velocity = vec * 1.2f;
                d.scale *= Main.rand.NextFloat(1f, 1.3f);
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
                new ShinyCrossStar(target.ToRandCirclePos(8f), Projectile.velocity.ToRandVelocity(ToRadians(25f), 4.8f, 14f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), 0.2f).Spawn();
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(target.ToRandCirclePos(8f), Main.rand.NextBool() ? DustID.Torch : DustID.YellowTorch);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(25f), 4.8f, 16f);
                d.scale *= Main.rand.NextFloat(1.2f, 1.8f) * 2f;
                d.noGravity = true;
            }
            for (int i = 0; i < 20; i++)
                new SmokeParticle(target.ToRandCirclePos(10f), Projectile.velocity.ToRandVelocity(ToRadians(20f), 4.7f, 18f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
        }
        public void UpdateAttackOnHit(NPC target)
        {
            HangingHitTime++;
            if (HangingHitTime > TotalHitTime)
            {
                Projectile.velocity = -Vector2.UnitY * 34f + target.velocity.ToSafeNormalize() * Clamp(target.velocity.Length(), 0f, 12f);
                UpdateMiscHitParticle(target.Center);
                UpdateNextAttack(State.Strike);
                SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy, Projectile.Center);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.HJScarlet().GlobalTargetIndex == -1)
                return null;
            NPC curTar = Projectile.ToHJScarletNPC();
            if (curTar.Equals(target))
                return null;
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            OnTileCollideParticle(oldVelocity);
            return false;
        }
        public void OnTileCollideParticle(Vector2 velo)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
            Vector2 vel = velo.ToSafeNormalize();
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 8.8f) * 1.2f;
                new SmokeParticle(pos, dVel, RandLerpColor(Color.DarkRed, Color.DarkGray), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 8; i++)
            {

                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 4.8f) * 1.2f;
                new ShinyOrbHard(pos, dVel, RandLerpColor(Color.OrangeRed, Color.Orange), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float ratios = ((float)i / (Projectile.oldPos.Length - 1));
                Color lerpColor = Color.Lerp(Color.White, Color.Orange, ratios);
                SB.Draw(tex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, lerpColor * (1 - ratios), Projectile.oldRot[i], tex.ToOrigin(), Projectile.scale, se, 0);
            }
            return false;
        }
    }
}
