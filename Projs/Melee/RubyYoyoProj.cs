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
    public class RubyYoyoProj : GemYoyoProj
    {
        protected override float YoyoLength => 16f * 32;
        protected override int YoyoLifeTime => GetSeconds(5);
        public ref float FloatingTimer => ref Projectile.ai[2];
        protected override int YoyoMaxUpdates => 2;
        protected override float YoyoTopSpeed => 27;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.ToTrailSetting(8);
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
            Lighting.AddLight(Projectile.Center, TorchID.Red);
            if (Projectile.velocity.LengthSquared() > 2.5f * 2.5f)
            {
                ECSParticle.Stain(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8f, RandLerpColor(Color.Red, Color.Crimson), 40, 1, Projectile.velocity.ToRotation(), 0.16f);

            }
            else
            {

            }
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(4), RandVelTwoPi(1), Color.Red, 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);

            FloatingTimer++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FloatingTimer > Projectile.MaxUpdates * 60)
            {
                Vector2 spawnPos = Projectile.Center;
                for (int i = 0; i < 16; i++)
                {
                    Color color = RandLerpColor(Color.Red, Color.Crimson);
                    ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(6f), color, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);

                }
                for (int i = 0; i < 15; i++)
                    ECSParticle.TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(30), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.Crimson, Color.Red), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
                ScarletSound(HJScarletSounds.Misc_Spell, Projectile.Center, 0.45f);

                float rod = RandRotTwoPi;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = (rod + TwoPi / 3 * i).ToRotationVector2();
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 6f, ProjectileType<RubyYoyoRuby>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                }
                FloatingTimer = 0;
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.Red, 1f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.Crimson, 0.9f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.White, 1f, 1f, 1.1f);
            SB.EnterShaderArea();
            Texture2D glowTex = HJScarletTexture.Particle_CrossGlow.Value;
            SB.FastDraw(glowTex, drawPos.ToRandCirclePos(1.5f, 1.5f), Color.Red, 0, glowTex.Size() / 2f, Projectile.scale * Main.rand.NextFloat(.9f, 1.01f) * .15f, 0);
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
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.DarkRed, Color.Red, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .3f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Red, Color.Crimson, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.AliceBlue.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -0f);
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
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 6 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
