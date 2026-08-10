using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DesertKnifeProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<DesertKnife>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            //new SmokeParticle(proj.Center.ToRandCirclePosEdge(5f) - proj.SafeDir() * 40f, RandVelTwoPi(3f), RandLerpColor(Color.Brown, Color.Orange), 40, RandRotTwoPi, 0.75f, Main.rand.NextFloat(0.4f, 0.6f) * 0.32f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Main.rand.NextBool(4))
            {
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePosEdge(5), Projectile.velocity / 4, RandLerpColor(Color.Brown, Color.Orange), 40, 1, Projectile.scale * Main.rand.NextFloat(.9f, 1.10f) * .1f, .4f);
            }
            if (Main.rand.NextBool(4))
            {
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(5), Projectile.velocity / 4f, RandLerpColor(Color.Brown, Color.Orange), 40, 1, Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .4f);
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.HasProj<DesertKnifeMark>())
                Projectile.AddExecutionTimeImmediate<DesertKnife>();
            ScarletSound(HJScarletSounds.TheSevenStar_Hit, Projectile.Center, .65f, pitch: -.2f);
            for (int i = 0; i < 14; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center, RandVelTwoPi(0.2f, 4.2f), Color.Brown, 40, 1, 0.6f);
            }
            for (int i = 0; i < 6; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center, RandVelTwoPi(0.2f, 6.2f), RandLerpColor(Color.Brown, Color.Orange), 40, 1, 0.9f, 0.31f, blendstate: BlendState.AlphaBlend);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawProj(Vector2.Zero);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public void DrawProj(Vector2 offset)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color c = Color.Lerp(Color.Brown, Color.DarkOrange, ratios).ToAddColor(10);
                float scale = Lerp(.14f, 1f, ratios);
                float opa = Lerp(.31f, 1f, ratios);
                Vector2 sharpScale = new Vector2(0.63f, 1.4f);
                Vector2 sharpPos = pos - new Vector2(20, 0).RotatedBy(Projectile.oldRot[i]);
                SB.Draw(HJScarletTexture.Particle_SharpTear, sharpPos, null, c.ToAddColor(50) * opa, Projectile.oldRot[i] + PiOver2, HJScarletTexture.Particle_SharpTear.Size() / 2f, sharpScale * scale, 0, 0);
                SB.Draw(HJScarletTexture.Particle_SharpTear, sharpPos, null, Color.White.ToAddColor(0) * opa, Projectile.oldRot[i] + PiOver2, HJScarletTexture.Particle_SharpTear.Size() / 2f, sharpScale * scale * .65f, 0, 0);
                c = Color.Lerp(Color.Gold, Color.White, ratios).ToAddColor(200);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale * scale, se);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 2.5f, Color.Orange.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);

            SB.EnterShaderArea();
            Texture2D glow = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Vector2 glowPos = drawPosition - new Vector2(5, 0).RotatedBy(Projectile.rotation);
            Vector2 glowScale = new Vector2(0.24f, 1f) * Projectile.scale * .31f;
            float glowRot = Projectile.rotation + PiOver2;
            Vector2 glowOri = glow.Size() / 2f;
            SB.FastDraw(glow, glowPos, Color.Red * .75f, glowRot, glowOri, glowScale, se);
            SB.FastDraw(glow, glowPos, Color.White * .75f, glowRot, glowOri, glowScale * .85f, se);
            SB.EndShaderArea();

        }
    }
}
