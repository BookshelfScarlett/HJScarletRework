using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
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
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class StarofHopeStar : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.FallenStar);
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public int RandFrame = 0;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            RandFrame = Main.rand.Next(0, 8);
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.penetrate == -1 && Projectile.damage == 0)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.2f);
                if (Projectile.Opacity <= 0.1f)
                {
                    Projectile.Kill();
                    return;
                }
            }
            //if (Main.rand.NextBool(2))
            //{
            //    Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(12f), DustID.YellowStarDust);
            //    d.velocity = Projectile.velocity / 2f;
            //    d.scale *= Main.rand.NextFloat(0.6f, 1f) ;
            //}
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextFloat() < Projectile.Opacity)
            {
                if (Main.rand.NextBool(2))
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(4), Projectile.velocity / 2f, RandLerpColor(Color.DarkGoldenrod, Color.WhiteSmoke), Main.rand.Next(20, 40), RandRotTwoPi, 1, Main.rand.NextFloat(0.5f, 0.7f) * 0.45f, true, BlendState.Additive);
                if (Main.rand.NextBool(2))
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(10), Projectile.velocity / 2f + RandVelTwoPi(0.82f, 1.4f), RandLerpColor(Color.PaleGoldenrod, Color.Gold), 40, 1, 0.8f, 0.2f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustCount = 25;
            for (int i = 0; i < dustCount; ++i)
            {
                float randX = Main.rand.NextFloat(-10f, 10f);
                float randY = Main.rand.NextFloat(-10f, 10f);
                float randVelocity = Main.rand.NextFloat(5f, 10f);
                float speed = (float)Math.Sqrt((double)(randX * randX + randY * randY));
                speed = randVelocity / speed;
                randX *= speed;
                randY *= speed;
                Vector2 vel = new Vector2(randX, randY) * Main.rand.NextFloat(0.1f, 1.3f) * .6f;
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 30, 1f, 0.1f * 0.45f * Main.rand.NextFloat(0.9f, 1.1f), 0.75f);
            }
            //SoundEngine.PlaySound(HJScarletSounds.Frosthammer_SnowCharge with { Pitch = .9f, Volume = .5f });
            Projectile.velocity *= 0.01f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = Projectile.GetTexture();
            Rectangle frame = projTex.Frame(1, 8, 0, 2);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 16; i++)
                SB.Draw(projTex, pos + (TwoPi / 16f * i).ToRotationVector2() * 2, frame, Color.White.ToAddColor() * Projectile.Opacity, Projectile.rotation - PiOver2, ori, Projectile.scale, 0, 0);
            SB.Draw(projTex, pos, frame, Color.White * Projectile.Opacity, Projectile.rotation - PiOver2, ori, Projectile.scale, 0, 0);
            float oriScale = 1f;
            float scale = 1f;
            int length = 7;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.LightSkyBlue, Color.Gold, (1 - rads)).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) - PiOver2;
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), frame, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
                scale *= 0.985f;
            }


            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawNebulaTrail(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DarkGoldenrod, 15f);
            SB.EnterShaderArea();
            DrawNebulaTrail(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.Gold, 10.2f);
            DrawNebulaTrail(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.White, 7.5f);
            SB.EndShaderArea();

            return false;
        }
        public void DrawNebulaTrail(Asset<Texture2D> tex, Color trailColor, float height)
        {
            float laserLength = 150;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -150);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4());
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.071f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting sets = new(tex.Value);
            List<TrailDrawDate> date = [];
            List<TrailDrawDate> date2 = [];
            int length = (int)(Projectile.oldPos.Length * Projectile.Opacity);
            for (int i = 0; i < length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2;
                float ratios = i / (float)Projectile.oldPos.Length;
                float rot = Projectile.oldRot[i];
                date.Add(new(listPos, Color.White, new(0, height * 5), rot));
                //date2.Add(new(listPos + rot.ToRotationVector2().RotatedBy(PiOver2) * 10, Color.White, new(0, height * 5), rot));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
            //TrailRender.DrawTrail(date2.ToArray(), sets);
        }
    }
}
