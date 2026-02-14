using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class SpearofEscapeThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<SpearOfEscape>().Texture;
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(4, 2);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            Vector2 dir = Projectile.SafeDirByRot();
            Vector2 offset = dir * -80;
            for (int i = 0; i < 2; i++)
            {
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(4f) + offset, 0.5f, RandLerpColor(Color.Orange, Color.OrangeRed), 30, Main.rand.NextFloat(0.1f, 0.12f), RandRotTwoPi).Spawn();
                new SmokeParticle(Projectile.Center.ToRandCirclePos(8f)+offset, -Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.DarkGray), 30, RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.1f).SpawnToPriorityNonPreMult();
            }
            Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(10f), 0.8f, 1.4f);
            new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f)+ offset, vel, RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.3f, ToRadians(10f)).Spawn();
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float rotFix = ToRadians(135);
            Projectile.DrawProj(Color.White, rotFix:rotFix);
            return false;
        }
    }
}
