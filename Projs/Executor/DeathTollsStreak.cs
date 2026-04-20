using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DeathTollsStreak : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public int HitCount = 0;
        public bool FirstFrameInit = false;
        public override void ExSD()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 50;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }
        public override bool PreDraw(ref Color lightColor) => false;
        public override void AI()
        {
            FirstFrame();
            Projectile.rotation = Projectile.velocity.ToRotation();
            GenDust();
        }
        public void GenDust()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center, 0.3f))
                return;
            Vector2 offset = Main.rand.NextVector2Circular(12, 12) + new Vector2(-48, 0).RotatedBy(Projectile.rotation);
            Color color = RandLerpColor(Color.Purple, Color.BlueViolet);
            Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Color Firecolor = RandLerpColor(Color.Black, Color.DarkViolet);
            new Fire(Projectile.Center, fireVelocity * 4.5f, Firecolor, 60, Main.rand.NextFloat(TwoPi), 1f, 0.24f).SpawnToPriorityNonPreMult();
            if (Projectile.numUpdates % 2 == 0)
            {
                if (Main.rand.NextBool())
                {
                    new TrailGlowBall(Projectile.Center + offset, fireVelocity * 4.5f, color, 70, 0.1f, true).Spawn();
                    new TrailGlowBall(Projectile.Center + offset, fireVelocity * 4.5f, Color.White, 70, 0.1f * 0.5f, true).Spawn();
                }
                Vector2 VecOffset = Projectile.velocity / 4f;
                new LightningGlow(Projectile.Center, fireVelocity * 0.5f, Color.DarkViolet, 50, 0.70f).Spawn();
                new LightningGlow(Projectile.Center, fireVelocity * 0.5f, Color.Violet, 50, 0.70f).Spawn();
            }
        }
        public void FirstFrame()
        {
            if (FirstFrameInit)
                return;
            new CrossGlow(Projectile.Center, Color.DarkViolet, 30, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center, Color.Violet, 30, 1f, 0.3f).Spawn();
            new CrossGlow(Projectile.Center, Color.White, 30, 1f, 0.2f).Spawn();
            for (int i = 0; i < 10; i++)
            {
                Color Firecolor = RandLerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
            FirstFrameInit = true;

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HitCount > 12)
                HitCount = 12;
            modifiers.SourceDamage *= 1 + 0.15f * HitCount;
            HitCount++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = oldVelocity.ToSafeNormalize();
                float rotvalue = ToRadians(360f / 8f * i) * 1f;
                float scale = (i % 2 == 0) ? 0.5f : 0.35f;
                for (int j = 0; j < 3; j++)
                {
                    new LightningGlow(Projectile.Center + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.DarkViolet, 50, scale).Spawn();
                    new LightningGlow(Projectile.Center + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.Violet, 50, scale).Spawn();
                }
            }
            for (int i =0;i<12;i++)
            {
                new Fire(Projectile.Center + RandVelTwoPi(2f,12.4f), RandVelTwoPi(0.8f, 5.1f), RandLerpColor(Color.Black, Color.DarkViolet), 60, RandRotTwoPi, 1f, 0.24f).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(12f);
                Vector2 vel = RandVelTwoPi(1f, 4f);
                new KiraStar(pos, vel, RandLerpColor(Color.DarkViolet, Color.Purple), 50, 0, 1, 0.070f * Main.rand.NextFloat(0.85f,1.01f), useAlt: true).Spawn();
                new KiraStar(pos, vel, Color.White, 50, 0, 1, 0.08f * 0.5f, useAlt: true).Spawn();
            }


            return true;
        }

        public override void OnKill(int timeLeft)
        {
        }
    }
}
