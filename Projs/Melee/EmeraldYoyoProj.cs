using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class EmeraldYoyoProj : GemYoyoProj
    {
        public ref float BoosterTime => ref Projectile.ai[2];
        protected override float YoyoLength => 16f * 15;
        protected override int YoyoLifeTime => GetSeconds(3);
        protected override int YoyoMaxUpdates => 1;
        protected override float YoyoTopSpeed => 16;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Green);
            if (Main.rand.NextBool(2))
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(4), RandVelTwoPi(1), Color.DarkGreen, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            }
            if (Projectile.velocity.LengthSquared() > 2.5f * 2.5f)
            {
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8f, RandLerpColor(Color.Green, Color.LimeGreen), 40, 1, Projectile.velocity.ToRotation(), 0.16f);
            }
            BoosterTime++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (BoosterTime > 90 * Projectile.MaxUpdates)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(6, 8f), ProjectileType<EmeraldYoyoEmerald>(), 0, 0, Projectile.owner);
                ScarletSound(SoundID.DD2_DarkMageAttack, Projectile.Center);
                BoosterTime = 0;
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
            if (BoosterTime > 0)
            {
                SB.EnterShaderArea();
                DrawTrails(HJScarletTexture.Trail_Lightning2.Texture, Color.LimeGreen, 1f, 1f, -105f);
                DrawTrails(HJScarletTexture.Trail_Lightning3.Texture, Color.DarkGreen, 0.9f, 1f, -111f);
                DrawTrails(HJScarletTexture.Trail_Lightning4.Texture, Color.White, 1f, 1f, -100f);
                SB.EndShaderArea();
                SB.EndShaderArea();

            }
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.Green, Color.Lime, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .59f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.DarkGreen, Color.LimeGreen, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(5, 5), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.Lime.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * offsetHeight);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * .85f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.05f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = Main.rand.NextVector2Circular(1.5f, 1.5f);
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 10 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

        public static void DrawSmear(Projectile proj, Color outerColor, Color innerColor)
        {
            Main.spriteBatch.EnterShaderArea();
            Texture2D glowTex = HJScarletTexture.Particle_Smear.Value;
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Main.spriteBatch.FastDraw(glowTex, drawPos.ToRandCirclePos(1.3f), outerColor, proj.rotation, glowTex.Size() / 2f, .23f, 0);
            Main.spriteBatch.FastDraw(glowTex, drawPos, innerColor, proj.rotation, glowTex.Size() / 2f, .21f, 0);
            Main.spriteBatch.EndShaderArea();
            Main.spriteBatch.EndShaderArea();
        }

    }
}
