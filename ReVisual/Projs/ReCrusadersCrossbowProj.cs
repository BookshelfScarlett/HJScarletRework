using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReCrusadersCrossbowProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<CrusadersCrossbow>();
        protected override int TrailLength => 4;
        public override bool PreAI(Projectile projectile)
        {

            if (TrailLength > 0)
            {
                projectile.rotation = projectile.velocity.ToRotation();
                OldRotationList.Add(projectile.rotation);
                OldPositionList.Add(projectile.Center);
                if (OldRotationList.Count > TrailLength)
                    OldRotationList.RemoveAt(0);
                if (OldPositionList.Count > TrailLength)
                    OldPositionList.RemoveAt(0);
            }
            ECSParticle.ShinyCrossStarSmall(projectile.Center.ToRandCirclePos(6), projectile.velocity / 4f, RandLerpColor(Color.Goldenrod, Color.LightGoldenrodYellow),
                40, 1, Main.rand.NextFloat(.9f, 1.1f) * .24f, Main.rand.NextFloat(-.05f, .05f));
            if (Main.rand.NextBool())
                ECSParticle.LightntingGlow(projectile.Center.ToRandCirclePosEdge(6), projectile.velocity / 4f, RandLerpColor(Color.LightGoldenrodYellow, Color.Goldenrod),
                    40, 1, 0.34f);
            //AddList(projectile);
            return base.PreAI(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            ReVisualPlayer reVisualPlayer = projectile.GetReVisualPlayer();
            if (reVisualPlayer.reVisualCrusadersCrossbow)
            {
                int count = OldPositionList.Count;
                Texture2D tex = HJScarletTexture.Particle_SharpTear;
                Vector2 ori = tex.Size() / 2f;
                for (int i = 0; i < count; i++)
                {
                    float progress = 1 - (float)i / (count);
                    Vector2 oldPos = OldPositionList[i] - Main.screenPosition;
                    float oldRot = OldRotationList[i] + PiOver2;
                    float opac = Lerp(1f, .5f, progress);
                    Color c = Color.Lerp(Color.White, Color.DarkGoldenrod, progress) * opac;
                    Color edgeColor = Color.Lerp(Color.White, Color.Goldenrod, progress) * opac * 0.85f;
                    float scale2 = Lerp(1, .64f, progress);
                    Vector2 scale = new Vector2(0.33f, 1.1f) * scale2;
                    SB.FastDraw(tex, oldPos, edgeColor.ToAddColor(), oldRot, ori, scale * 1.2f, SpriteEffects.None);
                    SB.FastDraw(tex, oldPos, c.ToAddColor(150), oldRot, ori, scale, SpriteEffects.None);
                }
                Vector2 pos = projectile.Center - Main.screenPosition;
                float rot = projectile.rotation + PiOver2;
                Vector2 sharpScale = new Vector2(.4f, 1.3f);
                for (int i = 0; i < 3; i++)
                    SB.FastDraw(tex, pos + projectile.SafeDir() * 3.5f * i, Color.Gold.ToAddColor(), rot, tex.Size() / 2f, projectile.scale * sharpScale, SpriteEffects.None);
                tex = projectile.GetTexture();

                SB.FastDraw(tex, pos, Color.White, rot, tex.Size() / 2f, projectile.scale, SpriteEffects.None);
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
