using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Core.PixelatedRender;
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
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FrostHammerIceSpike : HJScarletProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.NorthPoleSnowflake);
        public override ClassCategory Category => ClassCategory.Executor;
        public int RandFrame = 0;
        public override Vector2 TileHitbox => new(3);
        public ref float Timer => ref Projectile.ai[0];
        public bool ActiveHoming = false;
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.Opacity = 0;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            RandFrame = Main.rand.Next(1, 3);
        }
        public override void ProjAI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Lerp(0, 1, Timer / Projectile.MaxUpdates * 10);
            if (CurTarget.IsLegal() && Projectile.ai[1] > 0 && Projectile.Opacity > 0.98f && Timer > 12 * Projectile.MaxUpdates)
            {
                Projectile.extraUpdates = 1;
                Projectile.timeLeft = GetSeconds(5);
                float speedValue = Projectile.velocity.Length();
                float rotation = Projectile.velocity.ToRotation();
                float angleTo = Projectile.AngleTo(CurTarget.Center);
                float dist = Projectile.Distance(CurTarget.Center);
                float r = dist * 0.40f / (float)Math.Abs(Math.Sin(rotation - angleTo));
                if (Vector2.Dot(Projectile.velocity, Projectile.DirectionTo(CurTarget.Center)) < 0)
                {
                    r = Clamp(r, 1, 240);
                }
                Projectile.velocity = Projectile.velocity.RotatedBy(-Math.Sign(WrapAngle(rotation - angleTo)) * speedValue / r);
                if (Projectile.velocity.LengthSquared() < 13f * 13f)
                    Projectile.velocity *= 1.1f;
                else
                    Projectile.velocity *= 0.9f;
                Projectile.tileCollide = false;

            }
            else
            {
                Projectile.extraUpdates = 0;
                Projectile.AffactedByGrav(yAdd: 1.1f);
                Projectile.tileCollide = true;
                ActiveHoming = false;
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(8))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.25f, Main.rand.NextBool()).Spawn();
            if (Main.rand.NextBool(5))
                new SnowCloud(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.56f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.07f, true).SpawnToPriority();
            if (Main.rand.NextBool(4))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(10), Vector2.UnitY * Main.rand.NextFloat(1.8f, 5.4f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 0, 1, 0.68f * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();
            if (Main.rand.NextBool(9))
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(10);
                    p.Velocity = Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 4.2f);
                    p.DrawColor = RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue);
                    p.Lifetime = 40;
                    p.Scale = Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * .1f;
                    p.GlowCenterMult = 0.75f;
                });
            }
        }
        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 8; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = Projectile.Center + RandVelTwoPi(10f, 30f);
                Vector2 vel = (Projectile.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(1f, 10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .45f, 0.28f * 0.35f, Main.rand.NextBool()).Spawn();
            }
            for (int i = 0; i < 16; i++)
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(10);
                    p.Velocity = -Projectile.oldVelocity.ToRandVelocity(0, 0.4f, 10f);
                    p.Scale = Main.rand.NextFloat(0.9f, 1.1f) * 0.1f;
                    p.Opacity = 1f;
                    p.DrawColor = RandLerpColor(Color.AliceBlue, Color.RoyalBlue);
                    p.Lifetime = 40;
                    p.GlowCenterMult = 0.5f;
                });
            }
            SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 1, Volume = 0.25f, PitchVariance = 0.05f, Pitch = -0.3f });

            base.OnKill(timeLeft);
        }
        public override bool? CanDamage()
        {
            return Timer > 10 * Projectile.MaxUpdates;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D tex = Projectile.GetTexture();
            tex = GetVanillaAsset(VanillaAsset.Projectile, 520);
            Rectangle frame = tex.Frame(1, 6, 0, RandFrame);
            Vector2 ori = frame.Size() / 2;
            ori = tex.Size() / 2;

            //绘制残影
            float oriScale = 0.8f;
            float scale = 0.9f;
            int length = 7;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.985f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.RoyalBlue, Color.LightSkyBlue, (1 - rads)).ToAddColor(20) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.10f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                SB.Draw(tex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }

            Vector2 pos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, pos + ToRadians(60f * i).ToRotationVector2(), null, Color.White.ToAddColor(0), Projectile.rotation + PiOver2, ori, oriScale, 0, 0);
            SB.Draw(tex, pos, null, Color.White, Projectile.rotation + PiOver2, ori, oriScale, 0, 0);

            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.RoyalBlue, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DeepSkyBlue, .95f);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.White, 0.85f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 10;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.08f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Texture_Spirite.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;
            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Projectile.Opacity * 0.70f);
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 25 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
