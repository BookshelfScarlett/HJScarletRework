using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class PrunusMumePetal : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Typeless;
        public enum Style
        {
            NormalStrike,
            ExecutionStrike
        }
        public Style AttackStyle
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(36);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
        }
        public float OriginalSpeed = 0;
        public float RadiansValue = 0;
        public int Direction = 0;
        public bool ChangeDirection = false;
        public Vector2 OriginalPosition = Vector2.Zero;
        public override void OnFirstFrame()
        {
            OriginalSpeed = Projectile.velocity.LengthSquared();
            OriginalPosition = Projectile.Center;
            if(AttackStyle == Style.ExecutionStrike)
            {
                Projectile.penetrate = 3;
                Projectile.stopsDealingDamageAfterPenetrateHits = true;
            }
            else
            {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 4.2f);
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.HotPink, Color.IndianRed,.50f), Color.LightPink), 40, RandRotTwoPi, .61f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                if (Main.rand.NextBool())
                {
                    vel = Projectile.velocity.ToSafeNormalize() * Main.rand.NextFloat(-9f, 3f);
                    new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.IndianRed, Color.LightPink, 0.75f), Color.HotPink), 40, RandRotTwoPi, .61f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                }
            }
            for (int j = 0; j < 12; j++)
            {
                Vector2 dir = Projectile.SafeDir();
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.IndianRed, Color.HotPink), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
            }

            }

        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackStyle)
            {
                case Style.NormalStrike:
                    DoNormalStrike();
                    break;
                case Style.ExecutionStrike:
                    DoExecutionStrike();
                    break;
            }
            DrawParticle();
        }

        public void DoNormalStrike()
        {
            if (Projectile.velocity.LengthSquared() < OriginalSpeed * OriginalSpeed)
                Projectile.velocity *= 1.1f;
            else
                Projectile.velocity *= 0.9f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Direction > 0)
            {
                return true;
            }
            SoundEngine.PlaySound(SoundID.Item110 with { MaxInstances = 1 });
            Projectile.BounceOnTile(oldVelocity);
            for (int i = 0; i < 9; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 4.2f);
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.HotPink, Color.IndianRed,.50f), Color.LightPink), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
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

            Direction++;
            return false;
        }

        private void DrawParticle()
        {
            if (Main.rand.NextBool(5))
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(4);
                    p.Velocity = Projectile.velocity / 4f;
                    p.Scale = 0.105f * Main.rand.NextFloat(0.75f, 0.95f);
                    p.DrawColor = RandLerpColor(Color.IndianRed, Color.DarkRed);
                    p.Lifetime = 40;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.65f;
                });
            }
            if (Main.rand.NextBool())
            {
                new SnowCloud(Projectile.Center.ToRandCirclePos(6f), Projectile.velocity / 4f, RandLerpColor(Color.IndianRed, Color.HotPink), 40, 0, 0.73f, 0.1f * 0.5f, true).Spawn();
            }
        }

        private void DoExecutionStrike()
        {
            Projectile.tileCollide = false;
            if (RadiansValue >= ToRadians(360))
            {
                RadiansValue = 0;
                ChangeDirection = !ChangeDirection;
            }
            RadiansValue += ToRadians(2f);
            Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2f * Projectile.ai[2]) * ChangeDirection.ToDirectionInt());
            if (Projectile.velocity.LengthSquared() > OriginalSpeed)
                Projectile.velocity *= 0.9f;
            else
                Projectile.velocity *= 1.1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D tex, out Vector2 drawPos, out Vector2 ori);
            //ori = new Vector2(tex.Width * 0.95f, tex.Height * 0.05f);
            SpriteEffects se = Projectile.velocity.X > 0 ? SpriteEffects.FlipVertically : 0;
            //for (int i = 0; i < 5; i++)
            //{

            float rot = Projectile.rotation + PiOver2 + ToRadians(360f / 5);
            float rotOffset = PiOver2 + Pi - ToRadians(30);
            rot = Projectile.rotation + rotOffset;
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f;

            int length = (int)(Projectile.oldPos.Length * 0.8f * rad);

            for (int i = 0; i < length; i++)
            {
                float rads = i / (float)length;
                Color color = Color.Lerp(Color.White, Color.HotPink, rads).ToAddColor(200) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 lerpPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float lerpRot = Projectile.oldRot[i] + rotOffset;
                float scale = Lerp(0.6f, 0.15f, (rads));
                SB.Draw(tex, lerpPos, null, color, lerpRot, ori, Projectile.scale * scale, 0, 0);
            }
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DarkRed, 1f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.Red, .95f, 1f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.White, 0.85f, offsetHeight: 0f);
            SB.EndShaderArea();
            SB.Draw(tex, drawPos, null, Color.White, rot, ori, Projectile.scale, 0, 0);
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
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 70f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f;

            int posCount = (int)(Projectile.oldPos.Length * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2) + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 22 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
