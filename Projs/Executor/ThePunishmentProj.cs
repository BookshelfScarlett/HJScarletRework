using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ThePunishmentProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<ThePunishment>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8, 2);
        }
        public enum State
        {
            Shoot,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 32;
            Projectile.penetrate = 6;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.1f;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
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
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, -1, 21f, 16f);
            Projectile.tileCollide = false;
            Projectile.rotation += 0.2f;
            if (Projectile.IntersectOwnerByDistance(75))
            {
                if (Projectile.HJScarlet().AddFocusHit)
                    Owner.HJScarlet().ExecutionTime += 1;
                if (Projectile.HJScarlet().ExecutionStrike)
                    SpawnExecution();
                Projectile.Kill();
            }
        }

        public void SpawnExecution()
        {
            SoundEngine.PlaySound(HJScarletSounds.HymnFireball_Release with { Pitch = 0.4f}, Projectile.Center);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 1.5f, ProjectileType<ThePunishmentExecution>(), Projectile.damage, 1f, Owner.whoAmI);
            for (int i = 0; i < 30; i++)
            {
                if (Main.rand.NextBool())
                    new ShinyCrossStar(Projectile.Center.ToRandCirclePos(6f), Projectile.velocity.ToRandVelocity(ToRadians(10f), 1f, 18f), RandLerpColor(Color.Orange, Color.Goldenrod), 60, RandRotTwoPi, 1f, 0.45f, false).Spawn();
                else
                    new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(6f), Projectile.velocity.ToRandVelocity(ToRadians(10f), 1f, 18f), RandLerpColor(Color.Orange, Color.Goldenrod), 60, 0.45f).Spawn();
            }
        }

        public void DoShoot()
        {
            Projectile.rotation += .2f;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 7))
            {
                Projectile.netUpdate = true;
                AttackState = State.Return;
                Timer = 0;
            }
        }


        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(10), DustID.GemDiamond, RandVelTwoPi(2) + Projectile.velocity / 10f);
                dust.noGravity = true;
                dust.scale = 0.8f;
            }
            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(12), DustID.GoldCoin, RandVelTwoPi(4f) + Projectile.velocity / 10f);
                dust.noGravity = true;
                dust.scale = 1.8f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity, 0.24f);
            SoundEngine.PlaySound(HJScarletSounds.Hammer_LightHit with { Pitch = 0.25f, PitchVariance = 0.1f }, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(16f), RandVelTwoPi(1.3f, 5f), RandLerpColor(Color.Goldenrod, Color.Orange), 120, RandRotTwoPi, 1f, 0.48f, false).Spawn();
            }

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HJScarletSounds.Hammer_LightHit with { MaxInstances = 0,Pitch = 0.25f, PitchVariance = 0.1f }, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                new ShinyCrossStar(target.Center.ToRandCirclePos(16f), RandVelTwoPi(1.3f, 5f), RandLerpColor(Color.Goldenrod, Color.Orange), 120, RandRotTwoPi, 1f, 0.48f, false).Spawn();
            }
            int reverse = Main.rand.NextBool().ToDirectionInt();
            Vector2 spawnPos = Owner.Center + Projectile.SafeDir().RotatedBy(PiOver2) * reverse * 25f - Owner.ToMouseVector2() * 100f;
            Vector2 vel = Owner.ToMouseVector2().RotatedBy(ToRadians(15f) * reverse) * 28f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<ThePunishmentStar>(), Projectile.damage, 1f, Owner.whoAmI);
            ((ThePunishmentStar)proj.ModProjectile).TargetNPC = target;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
