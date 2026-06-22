using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class FrostlightFrostball : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public NPC CurTarget = null;
        public int InitLifeTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public ref float MaxAngleChange => ref Projectile.ai[2];
        public float SeedValue;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 1;
            Projectile.height = Projectile.width = 30;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
        }
        public override void OnFirstFrame()
        {
            SeedValue = Main.rand.NextFloat(0.4f, 1.1f);
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 dir = Projectile.SafeDirByRot();
            for (int i = 0; i < 9; i++)
            {
                Vector2 randDir = RandVelTwoPi(0.1f, 4.2f);
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                Color color = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.SkyBlue, .5f), Color.LightBlue, Main.rand.NextFloat());
                Vector2 vel = randDir + dir * Main.rand.NextFloat(1f, 8f);
                ECSParticle.SmokeParticle(spawnpos, vel, color, Main.rand.Next(30, 40), RandRotTwoPi, .35f, .3f * Main.rand.NextFloat(.75f, 1.1f), true, BlendState.Additive);
                if (Main.rand.NextBool())
                {
                    vel = Projectile.oldVelocity.ToSafeNormalize() * Main.rand.NextFloat(-3f, 12f);
                    ECSParticle.SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.RoyalBlue, Color.SkyBlue, .75f), Color.LightBlue), Main.rand.Next(30, 45), RandRotTwoPi, .35f, .30f * Main.rand.NextFloat(.75f, 1.1f) * Projectile.scale, true, BlendState.Additive);
                }
            }
            for (int j = 0; j < 10; j++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f);
                ECSParticle.ShinyCrossStarECS(spawnPos, dir * Main.rand.NextFloat() * 12f, RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 45), 1, .7f, .2f);
            }
        }
        public override void ProjAI()
        {
            if (!Owner.HasProj<FrostlightHeldProjAlt>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (CurTarget is null)
            {
                NPC target = Main.MouseWorld.FindClosestTarget(240);
                if (target.IsLegal())
                    CurTarget = target;
            }
            float speedValue = Clamp((12f + (Timer - 30f) / 10f) / 2f, 0, 15f);
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Projectile.SafeDir().RotatedBy(ToRadians(5f) * Main.rand.NextBool().ToDirectionInt()) * speed;
            if (CurTarget.IsLegal())
            {
                Projectile.HomingTarget(CurTarget.Center, -1f, speedValue, 15f, MaxAngleChange);
                Projectile.timeLeft = InitLifeTime;
            }
            else
            {
                Projectile.HomingTarget(Main.MouseWorld, -1f, speedValue, 15f * SeedValue, MaxAngleChange *SeedValue);
                CurTarget = null;
            }

            //粒子效果
            if (Main.rand.NextBool(8))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(8), Projectile.SafeDir().ToRandVelocity(ToRadians(10), 1.2f, 6.4f), RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 40), Main.rand.NextFloat(.7f, 1), Projectile.scale * Main.rand.NextFloat(.7f, .9f) * .70f, .2f);
            if (Main.rand.NextBool(8))
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(8), Projectile.SafeDir().ToRandVelocity(ToRadians(10), 1.2f, 4.4f), RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 40), Main.rand.NextFloat(.7f, 1) * .85f, Projectile.scale * Main.rand.NextFloat(.7f, .9f) * .08f, .5f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            Vector2 ori = projTex.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float oriScale = .64f;
            float mult = 1f;
            int length = 8;
            for (int i = 0; i < length; i++)
            {
                mult *= .975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.RoyalBlue, Color.LightBlue, 1 - rads).ToAddColor(30) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * mult * Projectile.scale, 0, 0);
                edgeColor = Color.Lerp(Color.SkyBlue, Color.LightBlue, 1 - rads).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * mult * Projectile.scale, 0, 0);
            }

            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawTrails2(HJScarletTexture.Trail_RvSlash.Texture, Color.DarkBlue.ToAddColor(10), 20);
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, Color.RoyalBlue, 20);
            DrawTrails(HJScarletTexture.Trail_Lightning4.Texture, Color.LightBlue, 18f);
            DrawTrails(HJScarletTexture.Trail_Lightning3.Texture, Color.SkyBlue, 16f);
            SB.EndShaderArea();
            SB.Draw(projTex, drawPos, null, Color.RoyalBlue.ToAddColor(70), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, drawPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.75f, 0, 0);
            return false;
        }
        public void DrawTrails2(Asset<Texture2D> tex, Color color, float mult = 1f, float alpha = 1f, float offsetHeight = 0f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            Texture2D texture = tex.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.6f);
            effect.Parameters["uFadeinTopLength"].SetValue(0.11f);
            effect.Parameters["uFadeinBottomLength"].SetValue(0.11f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
                      DrawSetting set = new DrawSetting(texture, true);
            List<TrailDrawDate> date = [];
            for(int i =0;i<Projectile.oldPos.Length;i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2;
                float ratios = i / (float)Projectile.oldPos.Length;
                date.Add(new(listPos, color, new(0, mult), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), set);
        }

        public void DrawTrails(Asset<Texture2D> tex, Color color, float mult = 1f, float alpha = 1f, float offsetHeight = 0f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float length = 150;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(length, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -80.2f);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * alpha);
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting set = new DrawSetting(tex.Value, true);
            List<TrailDrawDate> date = [];
            for(int i =0;i<Projectile.oldPos.Length;i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2;
                float ratios = i / (float)Projectile.oldPos.Length;
                date.Add(new(listPos, Color.White, new(0, mult), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), set);
        }
    }
}
