using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class MoonfireBullet : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(22);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.penetrate = 3;
            Projectile.SetupImmnuity(-1);
            Projectile.MaxUpdates = 4;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.Green.ToVector3()*2);
            if (Projectile.IsOutScreen())
                return;
            if(Main.rand.NextBool(3))
            {
                float scale = Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * 1f;
                Vector2 pos = Projectile.Center.ToRandCirclePos(8);
                ECSParticle.GlowSquare(pos, Projectile.velocity / 6f, RandLerpColor(Color.LimeGreen, Color.Lime), 45, 1, RandRotTwoPi, scale, 0, Main.rand.NextFloat(-.05f,.05f), 0.9f);
            }
            if(Main.rand.NextBool(3))
            {
                float scale = Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .3f;
                Vector2 pos = Projectile.Center.ToRandCirclePos(8);
                ECSParticle.ShinyCrossStarECS(pos, Projectile.velocity / 6f, RandLerpColor(Color.LightGreen, Color.LimeGreen), 40, 1, scale, 0.2f);
            }

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.penetrate == 0)
                return;
            ScarletSound(HJScarletSounds.Lightning_Quick, Projectile.Center, .75f, 1, .3f, pitchVariance: .1f);
            Vector2 pos = Projectile.Center;
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(pos, RandVelTwoPi(.2f, 8f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(35, 45), 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 1f, 0, Main.rand.NextFloat(-.08f, 0.09f), 0.9f);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 8f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * .88f, .2f);
            }
            float glowScale = .3f;
            ECSParticle.CrossGlow(pos, Color.DarkGreen, 45, 1, glowScale, .3f);
            ECSParticle.CrossGlow(pos, Color.LimeGreen, 45, 1, glowScale * .95f, .3f);
            ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate<Moonfire>();
            ScarletSound(HJScarletSounds.Lightning_Quick, Projectile.Center, .75f, 1, .3f, pitchVariance: .1f);
            Vector2 pos = Projectile.Center;
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(pos, RandVelTwoPi(.2f, 8f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(35, 45), 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 1f, 0, Main.rand.NextFloat(-.08f, 0.09f), 0.9f);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 8f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * .88f, .2f);
            }
            float glowScale = .3f;
            ECSParticle.CrossGlow(pos, Color.DarkGreen, 45, 1, glowScale, .3f);
            ECSParticle.CrossGlow(pos, Color.LimeGreen, 45, 1, glowScale * .95f, .3f);
            ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            DeepGlow.SubmitCustomGlow(() =>
            {
                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.LimeGreen, 1f,1f,0.78f);
            });
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Noise_HeavyAura.Texture, Color.LimeGreen, 0.25f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.15f, offsetHeight: 1.1f);
            SB.EnterShaderArea();
            //绘制残影
            float oriScale = .8f;
            Vector2 scale = new Vector2(0.51f,1.4f);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            SB.Draw(tex, pos, null, Color.Green, Projectile.rotation, ori, oriScale*.3f, 0, 0);
            Projectile.SetCrossStar(1.2f, Projectile.rotation, Color.Green);
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
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 170f*offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 8) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2) + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 14 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
