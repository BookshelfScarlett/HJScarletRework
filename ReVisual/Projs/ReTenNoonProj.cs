using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReTenNoonProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<TenNoons>();
        protected override int TrailLength => 12;
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.GetReVisualPlayer().reTenNoon)
            {
                if (projectile.localAI[1] > 1f)
                {
                    projectile.penetrate = 0;
                    return false;
                }

                Lighting.AddLight((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f), 0f, 0f, 0.2f);
                Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 6);
                dust.noLightEmittence = true;
                dust.velocity *= 0f;
                dust.scale = Main.rand.NextFloat(1f, 3f);
                dust.noGravity = Main.rand.Next(3) <= 2;
                if (dust.scale >= 1.66f)
                {
                    dust.noGravity = true;
                }

                if (projectile.numHits > 0)
                {
                    projectile.localAI[1] = 5f;
                }
                else if (projectile2.active && projectile2.friendly && (float)projectile2.type == projectile.ai[1])
                {
                    projectile.Center = projectile2.Center - projectile2.velocity;
                    projectile.velocity = projectile2.velocity;
                    projectile.width = projectile2.width;
                    projectile.height = projectile2.height;
                }
                else
                {
                    projectile.localAI[1] = 5f;
                }
            }
            return base.PreAI(projectile);
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            return base.PreKill(projectile, timeLeft);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.GetReVisualPlayer().reTenNoon)
            {
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
