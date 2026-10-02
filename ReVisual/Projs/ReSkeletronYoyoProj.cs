using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReSkeletronYoyoProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<SkeletronYoyo>();
        public override void AI(Projectile projectile)
        {
            if (!VisualOwner.reSkeletronYoyo)
            {
                base.AI(projectile);
                return;
            }
            if (projectile.velocity.LengthSquared() > 2.5f * 2.5f)
            {
                if (Main.rand.NextBool())
                    ECSParticle.Stain(projectile.Center.ToRandCirclePosEdge(10), projectile.velocity / 6f, Color.White, 40, 1, projectile.velocity.ToRotation(), projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .3f);
            }
            if (Main.rand.NextBool())
            {
                Vector2 rnd = RandDirTwoPi * projectile.scale;
                Vector2 vRnd = -rnd * .5f + rnd.RotatedBy(PiOver2) * 2.5f;
                for (int i = -1; i <= 1; i += 2)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center + rnd * 20f * i, DustID.PlatinumCoin, vRnd * i);
                    dust.noGravity = true;
                    dust.scale = 1.1f;
                }
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!VisualOwner.reSkeletronYoyo)
            {
                return true;
            }
            Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 pos = projectile.Center - Main.screenPosition;

            Texture2D crossStar = HJScarletTexture.Texture_BloodStain.Value;
            SB.EnterShaderArea();
            for (int i = 0; i < 8; i++)
            {
                float stainRot = projectile.rotation + PiOver4 * i;
                SB.FastDraw(crossStar, pos.ToRandCirclePos(1.5f), Color.White, stainRot, crossStar.Size() / 2f, projectile.scale * .35f, 0);
                SB.FastDraw(crossStar, pos, Color.White, stainRot, crossStar.Size() / 2f, projectile.scale * .33f, 0);
            }
            SB.EndShaderArea();
            SB.EndShaderArea();


            Vector2 vector = new Vector2(projectile.Center.X, projectile.Center.Y);
            //???
            for (int i = 0; i < 8; i++)
                Main.EntitySpriteDraw(sourceRectangle: new Rectangle(0, 28, 28, 28), texture: tex, position: vector.ToRandCirclePos(1.5f) - Main.screenPosition + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, color: Color.White.ToAddColor(), rotation: projectile.rotation, origin: new Vector2(14f, 14f), scale: 1f, effects: SpriteEffects.None);
            Main.EntitySpriteDraw(sourceRectangle: new Rectangle(0, 28, 28, 28), texture: tex, position: vector - Main.screenPosition, color: Color.White, rotation: projectile.rotation, origin: new Vector2(14f, 14f), scale: 1f, effects: SpriteEffects.None);

            return false;
        }
    }
}
