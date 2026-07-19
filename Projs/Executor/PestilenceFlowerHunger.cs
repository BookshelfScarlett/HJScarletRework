using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class PestilenceFlowerHunger : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => $"Terraria/Images/NPC_{NPCID.PlanterasTentacle}";
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12);
        }
        public override Vector2 TileHitbox => new Vector2(12);
        public ref float DrawScale => ref Projectile.localAI[0];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 2;
            Projectile.scale = 0;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.SetupImmnuity(-1);
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            Projectile.localAI[1] = Main.rand.Next(100, 300);
            Projectile.localAI[2] = Main.rand.NextFloat(.9f, 1.2f);
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.AddFrames(8, 4);
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 9))
            {
                if (Projectile.penetrate == -1 && Projectile.damage == 0)
                {
                    Projectile.Opacity = Lerp(Projectile.scale, 0f, 0.05f);
                    if (Projectile.Opacity <= 0.02f)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
            }
            else
            {
                float scaleLerp = Utils.GetLerpValue(0, 6f * Projectile.MaxUpdates, Timer, true);
                Projectile.scale = Lerp(0f, 0.8f, EaseOutBack(scaleLerp));
                if (scaleLerp >= 1f)
                {
                    float lerpValue = Utils.GetLerpValue(0f * Projectile.MaxUpdates, 9 * Projectile.MaxUpdates, Timer, true);
                    Timer++;
                    Projectile.scale = 0.85f;
                    DrawScale = EaseOutBack(lerpValue);
                }
            }
            DrawDust();
        }

        public void DrawDust()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextFloat(0f, 0.5f) > Projectile.scale)
                return;
            if (Main.rand.NextBool(10))
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(7), Projectile.velocity / 8f, RandLerpColor(Color.Pink, Color.LightPink), 45, 1, Main.rand.NextFloat(.75f, 1.1f) * Projectile.scale * .55f, 0.2f);
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(6);
                float scale = 0.27f * Main.rand.NextFloat(0.9f, 1.1f) * Projectile.scale;
                ECSParticle.LightntingGlow(pos, Projectile.velocity / 7f, RandLerpColor(Color.DarkGreen, Color.Green), 35, 1, scale, 6, BlendState.Additive);
                ECSParticle.LightntingGlow(pos, Projectile.velocity / 7f, RandLerpColor(Color.WhiteSmoke, Color.White), 35, 1, scale * .450f, 6, BlendState.Additive);
            }

            if (Main.rand.NextBool(22))
            {
                new PetalNoCollision(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 8f, RandLerpColor(Color.LimeGreen, Color.LightGreen), 45, RandRotTwoPi, 1, 0.10f, 0.4f, true).Spawn();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dVel = Projectile.oldVelocity.ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 18f);
                new SmokeParticle(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, dVel, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 dVel = Projectile.oldVelocity.ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 18f);
                new ShinyOrbHard(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, dVel, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 dVel = Projectile.SafeDir().ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 12f);
                new SmokeParticle(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, dVel, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 dVel = Projectile.SafeDir().ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 12f);
                new ShinyOrbHard(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, dVel, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, 4, 0, Projectile.frame);
            Vector2 frameOri = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float scaleLerp = Utils.GetLerpValue(0, 6f * Projectile.MaxUpdates, Timer, true);
            //绘制残影
            int length = Projectile.oldPos.Length;
            //for (int i = 0; i < length; i++)
            //{
            //    float rads = (float)i / length;
            //    Color edgeColor = Color.Lerp(Color.White, Color.DarkGreen, (1 - rads)).ToAddColor((byte)(int)Clamp(rads * 250f, 100, 250)) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
            //    float rot = Projectile.oldRot[i];
            //    SB.Draw(tex, Projectile.oldPos[i] + Projectile.PosToCenter(), frame, edgeColor, rot, frameOri, DrawScale * scale * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            //    scale *= 0.980f;
            //}
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(tex, pos + (TwoPi / 16f * i).ToRotationVector2() * 1.5f * DrawScale, frame, Color.DarkGreen.ToAddColor() * scaleLerp, Projectile.rotation, frameOri, Projectile.scale * DrawScale, SpriteEffects.FlipHorizontally, 0);
            }
            SB.Draw(tex, pos, frame, Color.White * scaleLerp, Projectile.rotation, frameOri, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.DarkGreen, 32);
            SB.EnterShaderArea();
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.Green, 30);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.Lime, 28);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.White, 27);
            SB.EndShaderArea();
            return false;
        }
        public void ApplyTrailAlt(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = (int)Projectile.localAI[1];
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -100 * Projectile.localAI[2]);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.13f);
            shader.Parameters["uFadeinLength"].SetValue(0.052f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.SafeDir() * 10f + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight * 2.1f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
    }
}
