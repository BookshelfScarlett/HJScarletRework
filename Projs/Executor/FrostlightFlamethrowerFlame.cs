using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class FrostlightFlamethrowerFlame : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public override Vector2 TileHitbox => base.TileHitbox;
        public ref float Timer => ref Projectile.ai[0];
        public int LifeTime = 100;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 6;
            Projectile.timeLeft = LifeTime;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }
        public Vector2 OriginalSpeedVector = Vector2.Zero;
        public float OriginalSpeed = 0;
        public override void OnFirstFrame()
        {
            OriginalSpeedVector = Projectile.velocity;
            OriginalSpeed = Projectile.velocity.Length();
        }
        public override void ProjAI()
        {
            DrawShinyParticle();
            DrawShinyFire();

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDir() * OriginalSpeed * .30f, 0.03f);
            Timer++;
        }

        public void DrawShinyParticle()
        {
            if (Main.rand.NextBool(3))
            {
                float lifeTimeRatios = Timer / LifeTime;
                Vector2 pos = Projectile.Center + Projectile.SafeDir().RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt()) + Projectile.SafeDir() * 5f;
                pos.Y += Main.rand.NextFloat(-15f, 15f);
                Vector2 offsetvec = (pos - Projectile.Center).ToSafeNormalize();
                Vector2 vel = offsetvec * OriginalSpeed * Lerp(.15f, .20f, lifeTimeRatios) * 1f + Projectile.SafeDir() * (Lerp(1f, 10f, lifeTimeRatios));
                Vector2 spawnPos = Projectile.Center + offsetvec * Main.rand.NextFloat(lifeTimeRatios * 100);
                spawnPos -= Projectile.SafeDir() * lifeTimeRatios * 100f;
                ECSParticle.ShinyCrossStarECS(spawnPos, vel, RandLerpColor(RandLerpColor(Color.RoyalBlue, Color.LightBlue), Color.SkyBlue), 40, 1, .8f, 0.2f);
            }
        }
        private void DrawShinyFire()
        {
            float lifetimeInterpolant = Timer / LifeTime;
            float particleScale = Lerp(0.10f, 1.4f, (float)Math.Pow(lifetimeInterpolant, 0.9f)) * 1f;
            particleScale *= (1f + lifetimeInterpolant * Main.rand.NextFloat(0.3f, .5f));
            float opacity = Utils.GetLerpValue(0.9f, 0.57f, lifetimeInterpolant, true);
            float fadeToBlack = Utils.GetLerpValue(0.67f, 0.89f, lifetimeInterpolant, true);
            Color fireColor = Color.Lerp(Color.RoyalBlue, Color.SkyBlue, Main.rand.NextFloat(0.2f, 0.8f));
            fireColor = Color.Lerp(fireColor, Color.LightBlue, fadeToBlack);
            Color brightColor = RandLerpColor(RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), Color.WhiteSmoke);
            brightColor = Color.Lerp(brightColor, Color.LightSkyBlue, fadeToBlack);

            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = Projectile.velocity * .1f * i + Projectile.velocity.ToSafeNormalize() * lifetimeInterpolant * Main.rand.NextFloat(.5f, 4f);
                if (Main.rand.NextBool(3))
                {
                    vel.X *= 10f;
                    vel.Y *= 20f;
                }
                Vector2 brighterVel = vel + Main.rand.NextVector2Circular(.8f, 2f);
                Vector2 darkerVel = vel + Main.rand.NextVector2Circular(1f, 3f);
                float brigherScale = particleScale * .90f;
                if (Main.rand.NextBool(3))
                    brigherScale *= Main.rand.NextFloat(1.1f, 1.3f);
                new SmokeParticle(Projectile.Center, brighterVel, brightColor, 12, RandRotTwoPi, opacity * .75f, brigherScale * .95f, false).Spawn();
                new SmokeParticle(Projectile.Center, darkerVel, fireColor, 12, RandRotTwoPi, opacity, particleScale * 1.1f, true).SpawnToPriorityNonPreMult();
            }

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<Frostlight>(), Main.rand.Next(2,5));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
