using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerRocket : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.MiniNukeRocketII);
        public override ClassCategory Category => ClassCategory.Executor;
        public List<NPC> TargetList = [];
        public NPC CurTarget = null;
        public bool ShouldHome = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20);
        }

        public override void ExSD()
        {
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 4;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 600;
        }
        public override void OnFirstFrame()
        {
            if (ShouldHome)
                Projectile.damage /= 2;
        }
        public override void ProjAI()
        {
            if (Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height, Main.MouseWorld, 1, 1))
                Projectile.tileCollide = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (ShouldHome)
            {
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1000f) && CurTarget is null)
                {
                    CurTarget = target;
                }
            }
            CurTarget = CurTarget.IsLegal() ? CurTarget : null;
            if (Projectile.penetrate == -1 && Projectile.damage == 0)
            {
                Projectile.velocity *= 0.06f;
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0.03f)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                if (CurTarget.IsLegal() && ShouldHome)
                {
                    Projectile.timeLeft = GetSeconds(5);
                    float speedValue = Projectile.velocity.Length();
                    float rotation = Projectile.velocity.ToRotation();
                    float angleTo = Projectile.AngleTo(CurTarget.Center);
                    float dist = Projectile.Distance(CurTarget.Center);
                    float r = dist * 0.30f / (float)Math.Abs(Math.Sin(rotation - angleTo));
                    if (Vector2.Dot(Projectile.velocity, Projectile.DirectionTo(CurTarget.Center)) < 0)
                    {
                        r = Clamp(r, 1, 180);
                    }
                    float realAnlge = (rotation - angleTo);
                    Projectile.velocity = Projectile.velocity.RotatedBy(-Math.Sign(WrapAngle(realAnlge)) * speedValue / r);
                    if (Projectile.velocity.LengthSquared() < 16f * 16f)
                        Projectile.velocity *= 1.1f;
                    else
                        Projectile.velocity *= 0.9f;

                }
                else
                {
                    if (Projectile.velocity.LengthSquared() > 16f * 16f)
                        Projectile.velocity *= 0.9f;
                    else
                        Projectile.velocity *= 1.1f;
                }
            }
            PlayParticle();
        }

        public void PlayParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.numUpdates < Main.rand.Next(-1, 3))
                return;
            int[] dTypes = [DustID.Torch, DustID.OrangeTorch];
            Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(6), dTypes[Main.rand.Next(2)]);
            d.scale *= 1.19f;
            d.velocity = Projectile.velocity;
            d.noGravity = true;

            if (Main.rand.NextBool(4))
            {
                float GeneralScaleMul = 1.1f * RandZeroToOne;
                int GetLifeTime() => Main.rand.Next(8, 16);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f) + Projectile.SafeDir() * Main.rand.NextFloat(10f);
                ECSParticle.SmokeParticle(pos, Projectile.SafeDir().ToRandVelocity(ToRadians(10f), 0.8f, Projectile.velocity.Length()), RandLerpColor(Color.White, Color.OrangeRed), 40, Projectile.rotation + Main.rand.NextFloat(-PiOver2, PiOver2), Main.rand.NextFloat(.4f, .51f) * .33f, 0.30f, true,BlendState.NonPremultiplied);
                //烟雾除了需要更多，也要更黑。
                for (int i = 0; i <= 1; i++)
                {
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(8f) + Projectile.SafeDirByRot() * i * 10f, -Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.Black), GetLifeTime(), RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.1f * GeneralScaleMul,blendstate:BlendState.Additive);
                }
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(10f), 0.8f, 1.4f);
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f), vel, RandLerpColor(Color.DarkOrange, Color.OrangeRed), GetLifeTime(), RandRotTwoPi, 1f, 0.3f * GeneralScaleMul, ToRadians(10f)).Spawn();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override void OnKill(int timeLeft)
        {

            if (Projectile.ai[0] == 1)
            {
                SlotId slotId1 = SoundEngine.PlaySound(HJScarletSounds.Misc_Boom with { Variants = [2], MaxInstances = 0, Pitch = .20f, Volume = .35f }, Projectile.Center);

            }
            int dustCount = 10;
            for (int i = 0; i < dustCount; ++i)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f) + dir * Main.rand.NextFloat(10f);
                new ShinyCrossStar(pos, RandVelTwoPi(2f, 6f), RandLerpColor(Color.Red, Color.OrangeRed), 45, RandRotTwoPi, RandZeroToOne, Projectile.scale, false, 0.5f).Spawn();
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f) + dir * Main.rand.NextFloat(10f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, RandVelTwoPi(0.2f, 3.1f));
                d.scale *= 1.1f;
            }

            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.DarkOrange);
                //同样，这里需要提供速度
                new Fire(Projectile.Center + Projectile.SafeDirByRot() * Main.rand.NextFloat(10f), Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, Color.Red);
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, Color.DarkOrange, 0.75f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.OrangeRed, 0.25f);
            DrawTrails(HJScarletTexture.Trail_RvSlash.Texture, Color.OrangeRed, 0.25f);
            SB.EndShaderArea();
            //最顶端绘制一个star。
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            //绘制残影
            float oriScale = 0.94f;
            float scale = 1f;
            int length = (int)(12 * Projectile.Opacity);
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.DarkOrange, Color.Lerp(Color.Orange, Color.White, .5f), 1 - rads).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.05f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.Opacity, 0, 0);
                edgeColor = Color.Lerp(Color.Orange, Color.White, 1 - rads).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor * 0.25f, rot, ori, oriScale * scale * Projectile.Opacity * 0.75f, 0, 0);
            }

            Projectile.DrawProj(Color.White, drawTime: 1, useOldPos: true, rotFix: PiOver2);

            return false;
        }
        public void DrawTrailsAlt(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new(useTex.Value, SamplerState.LinearClamp);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Projectile.Opacity);
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 20 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }

        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 200;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -60.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();
            //做掉可能存在的零向量
            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Projectile.Opacity);
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 20 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
