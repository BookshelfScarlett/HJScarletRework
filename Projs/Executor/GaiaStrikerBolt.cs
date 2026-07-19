using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerBolt : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public NPC CurTarget = null;
        public enum State
        {
            Shoot,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;

        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Homing:
                    DoHoiming();
                    break;
            }
            if (Projectile.IsOutScreen())
                return;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > Projectile.MaxUpdates * 2)
            {
                Projectile.frameCounter = 0;
                for (int i = 0; i < 32; i++)
                {
                    float rotArgs = (TwoPi / 32f * i);
                    Vector2 rotVec = Projectile.SafeDir().RotatedBy(rotArgs);
                    BloodyMetaball.SpawnParticle(Projectile.Center, rotVec * 1 + Projectile.SafeDir() * Main.rand.NextFloat() * 2f, 0.137f, Projectile.rotation, true);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool())
                    BloodyMetaball.SpawnParticle(Projectile.Center, RandVelTwoPi(0, 2f), 0.137f, Projectile.rotation, true);
            }
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool())
                    BloodyMetaball.SpawnParticle(Projectile.Center, RandVelTwoPi(0, 1f), 0.137f, Projectile.rotation, true);
            }

            BloodyMetaball.SpawnParticle(Projectile.Center, Projectile.SafeDir(), .17f, Projectile.rotation, true);
            if (Main.rand.NextBool(8))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(5, 5);
                ECSParticle.SmokeParticle(pos, Projectile.velocity.ToSafeNormalize(), RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 0.55f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * 0.375f, Main.rand.NextBool(), BlendState.NonPremultiplied);
            }
        }

        public void DoShoot()
        {
            Timer++;
            if(Projectile.MeetMaxUpdatesFrame(Timer,2))
            {
                AttackState = State.Homing;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }
        public void DoHoiming()
        {
            if (Projectile.GetTargetSafe(out NPC target, canPassWall: true) || CurTarget.IsLegal())
            {
                Projectile.timeLeft = GetSeconds(5);
                if (CurTarget.IsLegal())
                    target = CurTarget;
                float speedValue = Projectile.velocity.Length();
                float rotation = Projectile.velocity.ToRotation();
                float angleTo = Projectile.AngleTo(target.Center);
                float dist = Projectile.Distance(target.Center);
                float r = dist * 0.40f / (float)Math.Abs(Math.Sin(rotation - angleTo));
                if (Vector2.Dot(Projectile.velocity, Projectile.DirectionTo(target.Center)) < 0)
                {
                    r = Clamp(r, 1, 180);
                }
                Projectile.velocity = Projectile.velocity.RotatedBy(-Math.Sign(WrapAngle(rotation - angleTo)) * speedValue / r);
                if (Projectile.velocity.LengthSquared() < 8f * 8f)
                    Projectile.velocity *= 1.1f;
                else
                    Projectile.velocity *= 0.9f;
            }
            else
                Projectile.Kill();
        }

        public override void OnFirstFrame()
        {
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f);
                BloodyMetaball.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.3f), 0, true);
            }
            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f);
                BloodyMetaball.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.3f), spawnVec.ToRotation(), false);
            }
            if (Projectile.ai[2]==1)
            {
                Vector2 vel = Projectile.SafeDir().RotatedByRandom(ToRadians(60)) * Main.rand.NextFloat(3f, 6f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center.ToRandCirclePos(6), vel, ProjectileType<GaiaStrikerBloodyBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.ai[2] = 1;
                proj.HJScarlet().HasExecutionMechanic = true;
            }
            if (HJScarletRework.CrossMod_UCA.TryFind("CarnageRay", out ModItem value))
            {
                if (Owner.HeldItem.type == value.Type)
                {
                    if (Owner.statMana < Owner.statManaMax2 * 0.95f)
                        Owner.statMana += 10;
                }
            }

        }
            
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
