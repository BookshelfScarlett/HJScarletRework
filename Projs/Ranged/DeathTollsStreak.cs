using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class DeathTollsStreak : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Ranged;
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
            Color color = Color.Purple.RandLerpTo(Color.BlueViolet);
            Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Color Firecolor = Color.Black.RandLerpTo(Color.DarkViolet);
            new Fire(Projectile.Center, fireVelocity * 4.5f, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.1f).SpawnToPriorityNonPreMult();
            new TrailGlowBall(Projectile.Center + offset, fireVelocity * 4.5f, color, 90, 0.1f, true).Spawn();
            for (int i = 0; i < 3; i++)
            {
                Vector2 VecOffset = Projectile.velocity / 3f;
                new ShinyOrbParticle(Projectile.Center + VecOffset * i, fireVelocity * 0.5f, Color.BlueViolet, 60, 0.8f).Spawn();
            }
        }
        public void FirstFrame()
        {
            if (FirstFrameInit)
                return;
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.Violet, 30, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkViolet, 30, 1f, 0.4f).Spawn();
            for (int i = 0; i < 10; i++)
            {
                Color Firecolor = Color.Black.RandLerpTo(Color.DarkViolet);
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
        public override void OnKill(int timeLeft)
        {
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.Violet, 30, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkViolet, 30, 1f, 0.4f).Spawn();
        }
    }
}
