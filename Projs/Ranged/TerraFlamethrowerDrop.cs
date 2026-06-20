using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class TerraFlamethrowerDrop : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Ranged;
        public NPC OriginalTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(30);
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 3;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (Projectile.velocity.Y < 7f)
                Projectile.velocity.Y += .05f;
            ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(5), -Projectile.velocity / 8f, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 16, RandRotTwoPi, .85f, 0.135f * Main.rand.NextFloat(.8f,1.0f), true,BlendState.Additive);
            if (Main.rand.NextBool(3))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(5), -Projectile.velocity / 8f, RandLerpColor(Color.LimeGreen, Color.LightGreen), 16, 1f, .40f * Main.rand.NextFloat(0.8f,1.1f));
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (OriginalTarget.IsLegal() && target.Equals(OriginalTarget))
            {
                return false;
            }
            return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            Color baseColor = Color.DarkSeaGreen;
            Color targetColor = Color.LimeGreen;
            //绘制残影
            float oriScale = 0.45f;
            float scale = 1f;
            int length = 16;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(baseColor, targetColor, (1 - rads)).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.080f);
                scale = Lerp(1f, 0.3f, rads);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }

            SB.Draw(projTex, projPos, null, Color.DarkGreen.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.75f, 0, 0);
            return false;
        }
    }
}
