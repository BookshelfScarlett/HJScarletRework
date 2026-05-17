using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerFireball : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public NPC CurTarget = null;
        public int BounceTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(26);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.SetupImmnuity(60);
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 1;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1200f) && CurTarget is null)
            {
                CurTarget = target;
            }
            CurTarget = CurTarget.IsLegal() ? CurTarget : null;

            Projectile.rotation = Projectile.velocity.ToRotation();

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
                Timer++;
                if (Timer < 15f)
                    return;
                if (CurTarget.IsLegal())
                {
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
                    if (Projectile.velocity.LengthSquared() < 8f * 8f)
                        Projectile.velocity *= 1.1f;
                    else
                        Projectile.velocity *= 0.9f;

                }
                else
                {
                    if (Projectile.velocity.LengthSquared() > 8f * 8f)
                        Projectile.velocity *= 0.9f;
                    else
                        Projectile.velocity *= 1.1f;
                }
            }
            if (Projectile.IsOutScreen() && Projectile.Opacity < Main.rand.NextFloat())
                return;
            if (Main.rand.NextBool(9))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f), Projectile.velocity / 8f, RandLerpColor(Color.Lerp(Color.DarkOrange, Color.Red, 0.64f), Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.53f * Projectile.Opacity, false, ToRadians(1f)).Spawn();

            if (Main.rand.NextBool(12))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity / 8f, RandLerpColor(Color.DarkOrange, Color.OrangeRed), 20, RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 0.86f * Projectile.Opacity, true).SpawnToPriority();

        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 9; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 4.2f);
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel + dir * Main.rand.NextFloat(1f, 8f), RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange), 40, RandRotTwoPi, .35f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                if (Main.rand.NextBool())
                {
                    vel = Projectile.oldVelocity.ToSafeNormalize() * Main.rand.NextFloat(-3f, 12f);
                    new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.75f), Color.OrangeRed), 40, RandRotTwoPi, .35f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                }
            }
            for (int j = 0; j < 10; j++)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.Orange, Color.OrangeRed), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
            }
            if (BounceTime > 2)
            {
                Projectile.damage = 0;
                Projectile.penetrate = -1;

            }    

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(!Owner.HasProj<SundownerRocket>())
            SoundEngine.PlaySound(SoundID.Item45 with { MaxInstances = 1, Pitch = 0.3f }, Projectile.Center);
            Projectile.AddExecutionTimeImmediate(ItemType<Sundowner>());
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(!Owner.HasProj<SundownerRocket>())
            SoundEngine.PlaySound(SoundID.Item45 with { MaxInstances = 1, Pitch = 0.3f }, Projectile.Center);
            for (int i = 0; i < 9; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 4.2f);
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                if (Main.rand.NextBool())
                {
                    vel = Projectile.oldVelocity.ToSafeNormalize() * Main.rand.NextFloat(-9f, 3f);
                    new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.75f), Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                }
            }
            for (int j = 0; j < 10; j++)
            {
                Vector2 dir = -Projectile.SafeDirByRot();
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.Orange, Color.OrangeRed), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
            }
            if (BounceTime > 2)
            {
                Projectile.damage = 0;
                Projectile.penetrate = -1;

            }    
            Projectile.BounceOnTile(oldVelocity);
            BounceTime++;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = (int)(12 * Projectile.Opacity);
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.DarkOrange, Color.OrangeRed, 1 - rads).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.05f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.Opacity, 0, 0);
                edgeColor = Color.Lerp(Color.Orange, Color.White, 1 - rads).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor * 0.25f, rot, ori, oriScale * scale * Projectile.Opacity * 0.75f, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.DarkOrange.ToAddColor(75) * Projectile.Opacity, Projectile.rotation, ori, oriScale , 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0) * Projectile.Opacity, Projectile.rotation, ori, oriScale * 0.75f, 0, 0);

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.Red);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DarkOrange, 0.75f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.OrangeRed, 0.25f);
            HJScarletMethods.EndShaderAreaPixel();
        }

        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -24.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Texture_Spirite.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Projectile.Opacity);
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 35 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
