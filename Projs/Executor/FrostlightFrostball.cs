using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
            base.OnFirstFrame();
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
            if(Projectile.MeetMaxUpdatesFrame(Timer,40))
            {
                Projectile.AffactedByGrav(yAdd: .08f, maxGravSpeed: 18f);
                Projectile.timeLeft = InitLifeTime;
                if (Math.Abs(Projectile.velocity.Y) < 5f)
                    Timer = 40 * Projectile.MaxUpdates;
                return;
            }
            if(CurTarget is null)
            {
                NPC target = Main.MouseWorld.FindClosestTarget(600);
                if (target.IsLegal())
                    CurTarget = target;
            }
            float speedValue = Clamp((12f + (Timer - 30f) / 5f) / 2f, 0, 20f);
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Projectile.SafeDir().RotatedBy(ToRadians(5f) * Main.rand.NextBool().ToDirectionInt()) * speed;
            if(CurTarget.IsLegal())
            {
                Projectile.HomingTarget(CurTarget.Center, -1f, speedValue, 15f, MaxAngleChange);
                Projectile.timeLeft = GetSeconds(3);
            }
            else
            {
                Projectile.HomingTarget(Main.MouseWorld, -1f, speedValue, 15f, MaxAngleChange);
                CurTarget = null;
            }

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
            int length = 3;
            for(int i =0;i<length;i++)
            {
                mult *= .975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.RoyalBlue, Color.LightBlue, 1 - rads).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * mult * Projectile.scale, 0, 0);
            }
            SB.EnterShaderArea();
            SB.EndShaderArea();
            return false;
        }
        public void DrawTrails(Asset<Texture2D> tex, Color color, float mult = 1f, float alpha = 1f, float offsetHeight = 0f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float length = 150;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(length, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -1.2f);
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
                date.Add(new(listPos, Color.White, new(0, offsetHeight * Clamp(1 - ratios, 0, 1)), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), set);
        }
    }
}
