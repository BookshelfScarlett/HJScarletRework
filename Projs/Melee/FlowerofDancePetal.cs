using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class FlowerofDancePetal : HJScarletProj
    {
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(36);
        }
        public enum State
        {
            Idle,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public bool ActiveHomingAbility
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        public NPC CurTarget = null;
        public ref float Timer => ref Projectile.ai[0];
        public float OriginalSpeed = 0;
        public float RadiansValue = 0;
        public int Direction = 0;
        public bool ChangeDirection = false;
        public Vector2 OriginalPosition = Vector2.Zero;

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.SetupImmnuity(-1);
            Projectile.tileCollide = false;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.penetrate = 1;

        }
        public override void ProjAI()
        {
            if(Projectile.damage == 0 && Projectile.penetrate == -1)
            {
                if (Projectile.timeLeft > 51)
                    Projectile.timeLeft = 51;
                Projectile.velocity *= 0.55f;
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.21f);
                    return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer > 60f && !ActiveHomingAbility)
            {
                ActiveHomingAbility = true;
                Timer = 0;
            }
            if (CurTarget.IsLegal() && ActiveHomingAbility)
            {
                float time = Projectile.MaxUpdates * 35f;
                float ratios = Clamp(Timer / time, 0, 1);
                float speed = Lerp(1f, 14f, ratios);
                float angle = Lerp(0f, 30f, EaseOutBack(ratios));
                Projectile.HomingTarget(CurTarget.Center, -1, speed, 10, angle);
            }
            if (Projectile.Opacity < Main.rand.NextFloat())
                return;

            if (Main.rand.NextBool(12))
            {
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(6), Projectile.SafeDir(), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 30, 1f, Projectile.scale * 0.08f, 0.65f);
            }
            if (Main.rand.NextBool(16))
                ECSParticle.SnowCloud(Projectile.Center.ToRandCirclePos(6f), Projectile.velocity / 4f, RandLerpColor(Color.SkyBlue, Color.Aquamarine), 30, 0, 0.73f, 0.1f * 0.15f, BlendState.Additive);
        }
        public override void OnFirstFrame()
        {
            OriginalSpeed = Projectile.velocity.LengthSquared();
            OriginalPosition = Projectile.Center;
            for (int i = 0; i < 6; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 4.2f);
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(10f);
                ECSParticle.LightntingGlow(spawnpos, Projectile.SafeDir() * 7f, RandLerpColor(Color.SkyBlue, Color.Aquamarine), 40, 1, 0.64f);
            }
            for (int j = 0; j < 6; j++)
            {
                Vector2 dir = Projectile.SafeDir();
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(20) + dir * Main.rand.NextFloat(0, 6), dir * 12 * Main.rand.NextFloat(), RandLerpColor(Color.Aquamarine, Color.SkyBlue), 50, 1, 0.47f);
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            float time = Projectile.MaxUpdates * 15f;
            float ratios = Clamp(Timer / time, 0, 1);

            if (!ActiveHomingAbility && ratios >= 0.5f)
                return null;
            if (ActiveHomingAbility && CurTarget.IsLegal() && CurTarget.Equals(target))
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D tex, out Vector2 drawPos, out Vector2 ori);
            SpriteEffects se = Projectile.velocity.X > 0 ? SpriteEffects.FlipVertically : 0;
            float rot = Projectile.rotation + PiOver2 + ToRadians(360f / 5);
            float rotOffset = PiOver2 + Pi - ToRadians(30);
            rot = Projectile.rotation + rotOffset;
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int length = (int)((Projectile.oldPos.Length - 12) * 0.8f * rad);

            for (int i = 0; i < length; i++)
            {
                float rads = i / (float)length;
                Color color = Color.Lerp(Color.White, Color.Aquamarine, rads).ToAddColor(200) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 lerpPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float lerpRot = Projectile.oldRot[i] + rotOffset;
                float scale = Lerp(0.6f, 0.15f, rads);
                SB.Draw(tex, lerpPos, null, color, lerpRot, ori, Projectile.scale * scale, 0, 0);
            }
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DeepSkyBlue, 1.2f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.Black, 1.12f);
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.Aquamarine, 1f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.White, 0.85f, offsetHeight: 0f);
            SB.EndShaderArea();
            for(int i =0;i<8;i++)
            SB.Draw(tex, drawPos +(TwoPi / 8f * i).ToRotationVector2() * 1.5f, null, Color.White with { A = 0 } * Projectile.Opacity, rot, ori, Projectile.scale, 0, 0);
            SB.Draw(tex, drawPos, null, Color.LightSkyBlue with { A = 255 } * Projectile.Opacity, rot, ori, Projectile.scale, 0, 0);
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
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 170f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
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
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2) + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 40 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
