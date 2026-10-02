using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Magic
{
    public class LivingBoomerangMagicPetal : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.MaxUpdates = 2;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            float maxTime = Projectile.MaxUpdates * 15;
            float progress = Clamp(Timer / maxTime, 0, 1);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.GetTargetSafe(out NPC target, true, 360, true))
            {
                Projectile.HomingTarget(target.Center, -1, 13f, Lerp(30f, 5f, progress));
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), RandVelTwoPi(0f, 1f), RandLerpColor(Color.HotPink, Color.LightPink), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .45f, .2f);
            if (Main.rand.NextBool(3))
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 4f, RandLerpColor(Color.DeepPink, Color.HotPink), 40, 1, Projectile.rotation, Main.rand.NextFloat(.9f, 1.1f) * .2f, 1);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 9; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 4.2f);
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.HotPink, Color.IndianRed, .50f), Color.LightPink), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                if (Main.rand.NextBool())
                {
                    vel = Projectile.velocity.ToSafeNormalize() * Main.rand.NextFloat(-9f, 3f);
                    new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.IndianRed, Color.LightPink, 0.75f), Color.HotPink), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                }
            }
            for (int j = 0; j < 12; j++)
            {
                Vector2 dir = Projectile.SafeDir();
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.IndianRed, Color.HotPink), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
            }
            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6f);
                Vector2 vel = RandVelTwoPi(1f, 4.9f);
                new HRShinyOrb(pos, vel, RandLerpColor((Color.Lerp(Color.HotPink, Color.IndianRed, 0.5f)), Color.IndianRed), 40, 0.12f).Spawn();
                new HRShinyOrb(pos, vel, Color.White, 40, 0.12f * 0.5f).Spawn();
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float overallScale = Projectile.scale;
            int length = Projectile.oldPos.Length - 6;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .15f, (progress));
                float yMult = Lerp(1f, .85f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.DeepPink, Color.Lerp(Color.White, Color.HotPink, .93f), EaseInOutQuad(progress));
                Color glowColor = Color.Lerp(Color.HotPink, Color.DeepPink, EaseInOutQuad(progress));
                float opac = Lerp(1f, .96f, EaseInOutExpo(progress));
                SB.FastDraw(tex, oldPos, glowColor.ToAddColor(10) * opac * 0.821f, oldRot, tex.Size() / 2f, scale * 1.05f, 0);
                SB.FastDraw(tex, oldPos, c.ToAddColor(100) * opac * 1.1f, oldRot, tex.Size() / 2f, scale, 0);
            }
            Vector2 petalScale = new Vector2(0.95f, 1.15f);
            Texture2D blur = HJScarletTexture.Texture_FireBall.Value;
            Vector2 blurPetal = new Vector2(1.1f, 0.95f);
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, pos + (TwoPi / 8f * i).ToRotationVector2() * 2f, Color.White.ToAddColor(), Projectile.rotation + PiOver2, tex.Size() / 2f, overallScale * petalScale, 0);
            SB.FastDraw(tex, pos, Color.White.ToAddColor(200), Projectile.rotation + PiOver2, tex.Size() / 2f, overallScale * petalScale, 0);
            DeepGlow.SubmitCustomGlow(() =>
            {
                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.HotPink, 1.2f);
                SB.EndShaderArea();
            });
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DeepPink, 1f, .78f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.White, 0.85f, .78f);
            SB.EndShaderArea();

            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 170f * offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 5) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 30 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }

    }
}
