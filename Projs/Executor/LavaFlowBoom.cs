using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class LavaFlowBoom : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(13, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = GetSeconds(3);
        }
        public override void ProjAI()
        {
            Timer++;
            Projectile.AffactedByGrav(velMult: 1f, yAdd: 0.32f, maxGravSpeed: 12f);
            if (Projectile.IsOutScreen())
                return;
            for (int i = 0; i < 2; i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(8f), -Projectile.velocity / 8f, RandLerpColor(Color.DarkOrange, Color.DarkGray), Main.rand.Next(0, 41), RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.15f).SpawnToPriorityNonPreMult();
            }

            ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePosEdge(4f), -Projectile.velocity / 8f, RandLerpColor(Color.Orange, Color.OrangeRed), Main.rand.Next(0, 31), 1f, Main.rand.NextFloat(.12f, .14f) * .65f, .48f);
            Vector2 vel = -Projectile.velocity / 7f * Main.rand.NextFloat(.9f, 1.1f);
            ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4f), vel, RandLerpColor(Color.DarkOrange, Color.OrangeRed), Main.rand.Next(0, 45), 1f, Projectile.scale * Main.rand.NextFloat(.3f, .5f) * 1f, 0.12f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return Timer > 10f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            //绘制残影
            float oriScale = 0.64f;
            float scale = .9f;
            int length = 4;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.95f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.Orange, Color.Lerp(Color.Orange, Color.OrangeRed, rads * 0.7f), (1 - rads)).ToAddColor(80) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.Orange.ToAddColor(80), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.65f, 0, 0);
            return false;
        }
    }
}
