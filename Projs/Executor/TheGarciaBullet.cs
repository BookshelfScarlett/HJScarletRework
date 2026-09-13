using ContinentOfJourney.Dusts;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheGarciaBullet : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(22);
        }

        public override void ExSD()
        {
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300 * 3;
            Projectile.SetupImmnuity(30);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            //ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(3)+Projectile.velocity.ToSafeNormalize()*Main.rand.NextFloat(0f,1.1f)*40, Projectile.velocity / 8f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), Main.rand.Next(35, 46), 1, Main.rand.NextFloat(.9f, 1.1f) * .2f, .2f);
            if(Main.rand.NextBool(3))
            for(int i =0;i<3;i++)
            ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(6)+Projectile.velocity/i, 0.62f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), Main.rand.Next(35, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .1f,glowMult:.25f);
        }
        public override void OnKill(int timeLeft)
        {
            ECSParticle.ShinyCrossStarSmall(Projectile.Center, Projectile.SafeDir()*.1f, Color.LightGoldenrodYellow, 40, 1, 1f, 0);
            for (int i = 0; i < 12;i++)
            {
            ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(12), 0.62f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), Main.rand.Next(35, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .1f,glowMult:.25f);

            }
                base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            DeepGlow.SubmitCustomGlow(() =>
            {
                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DarkGoldenrod, 1f,1f,0.78f);
            });
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Noise_HeavyAura.Texture, Color.Goldenrod, 0.25f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.15f, offsetHeight: 1.1f);
            SB.EnterShaderArea();
            //绘制残影
            Texture2D orb = HJScarletTexture.Texture_BloodStain.Value;
            Vector2 orbScale = new Vector2(1f,1.25f)*.5f*Projectile.scale;
            float rot = Projectile.rotation;
            SB.FastDraw(orb, Projectile.Center - Main.screenPosition, Color.DarkGoldenrod, rot, orb.Size()/ 2f, orbScale, 0, 0);
            SB.FastDraw(orb, Projectile.Center - Main.screenPosition, Color.White, rot, orb.Size()/ 2f, orbScale*.75f, 0, 0);
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
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 270f*offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 10) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 10 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
