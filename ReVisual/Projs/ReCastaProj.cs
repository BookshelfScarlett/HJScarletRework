using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Terraria;
using Terraria.GameContent;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReCastaProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<Casta>();
        public override bool PreAI(Projectile projectile)
        {
            if (!VisualOwner.reCasta)
                return true;
            else
            {
                base.PreAI(projectile);
            }
            return true;
        }
        public override void AI(Projectile projectile)
        {
            if (!VisualOwner.reCasta)
            {

            }
            else
            {
                ECSParticle.ShinyCrossStarECS(projectile.Center.ToRandCirclePos(6), RandVelTwoPi(1), RandLerpColor(Color.Gold, Color.Violet), 40, 1, .2f, .2f);
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!VisualOwner.reCasta)
                return base.PreDraw(projectile, ref lightColor);
            else
            {
                Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
                Vector2 pos = projectile.Center - Main.screenPosition;
                Texture2D crossStar = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                SB.EnterShaderArea();
                SB.FastDraw(crossStar, pos.ToRandCirclePos(3.5f), Color.SkyBlue, 0, crossStar.Size() / 2f, projectile.scale * .25f, 0);
                SB.FastDraw(crossStar, pos, Color.White, 0, crossStar.Size() / 2f, projectile.scale * .23f, 0);
                SB.EndShaderArea();
                SB.EndShaderArea();

                for (int i = 0; i < 8; i++)
                    SB.FastDraw(tex, pos.ToRandCirclePos(3.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), projectile.rotation, tex.Size() / 2f, projectile.scale, 0);
                SB.FastDraw(tex, pos, Color.White, projectile.rotation, tex.Size() / 2f, projectile.scale, 0);

                return false;
            }
        }
    }
}
