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
    public class AmberYoyoProj : GemYoyoProj
    {
        protected override float YoyoLength => 16f * 32;
        protected override int YoyoLifeTime => GetSeconds(5);
        public ref float FloatingTimer => ref Projectile.ai[2];
        protected override int YoyoMaxUpdates => 2;
        protected override float YoyoTopSpeed => 36;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.ToTrailSetting(13);
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.ProjAI();
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            if (Projectile.velocity.LengthSquared() > 2.5f * 2.5f)
            {
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8f, RandLerpColor(Color.Orange, Color.OrangeRed), 40, 1, Projectile.velocity.ToRotation(), 0.16f);

            }
            else
            {

            }
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(4), RandVelTwoPi(1), Color.DarkOrange, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);

            FloatingTimer++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FloatingTimer > Projectile.MaxUpdates * 10)
            {

                Vector2 pos = Owner.Center.ToRandCirclePosEdge(5, 50) + Owner.ToMouseVector2() * -100f;
                for (int i = 0; i < 8; i++)
                {
                    Color color = RandLerpColor(Color.Orange, Color.DarkOrange);
                    ECSParticle.GlowSquare(pos.ToRandCirclePosEdge(6), RandVelTwoPi(3.5f), color, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
                }
                ECSParticle.HRShinyOrb(pos, Vector2.Zero, Color.Orange, 40, 1, 0.2f, 0.4f);

                Vector2 dir = pos.GetNormalVector2(Main.MouseWorld);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, dir * 10f, ProjectileType<AmberYoyoAmber>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (target.IsLegal())
                    proj.HJScarlet().CurStoredTarget = target;

                float rod = RandRotTwoPi;
                FloatingTimer = 0;
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int length = Projectile.oldPos.Length;
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_MegaBeam.Texture, Color.Orange, 1.9f, 1f, -50);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.Orange, 1f, 1f, 0);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.Brown, 1f, 1f, 0);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.White, 1f, 1f, 0);
            SB.EnterShaderArea();
            Texture2D glowTex = HJScarletTexture.Particle_CrossGlow.Value;
            SB.FastDraw(glowTex, drawPos.ToRandCirclePos(1.5f, 1.5f), Color.Orange, 0, glowTex.Size() / 2f, Projectile.scale * Main.rand.NextFloat(.9f, 1.01f) * .15f, 0);
            SB.EndShaderArea();
            SB.EndShaderArea();

            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.OrangeRed, Color.DarkOrange, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .3f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Brown, Color.Orange, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.Orange.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
        public void DrawString()
        {
            Asset<Texture2D> value = HJScarletTexture.Trail_Lightning3.Texture;
            float BeamLength = (Projectile.Center - Owner.MountedCenter).Length();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            //轨迹
            SB.EnterShaderArea();
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 60);
            shader.Parameters["uColor"].SetValue(Color.Orange.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.Orange, (Owner.MountedCenter - Projectile.Center).ToRotation(), orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), 0.15f * Projectile.scale), 0, 0);
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.White * 0.85f, (Owner.MountedCenter - Projectile.Center).ToRotation(), orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), 0.10f * Projectile.scale), 0, 0);
            SB.EndShaderArea();
            SB.EndShaderArea();
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
    }
}
