using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Particles;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReAdamantiteKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<AdamantiteKnife>();
        public float Timer = 0;
        public float SpinDirection = 0;
        public List<float> RotList2 = [];
        public List<Vector2> PosList2 = [];
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualAdamantiteKnife;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            AddListOnNeed(proj);
            AddListForAda(proj);
            Timer++;
            if (Timer == 30f)
                SpinDirection = Math.Sign(proj.velocity.X);

            for (int i = 0; i < 2; i++)
                new TurbulenceShinyCube(proj.Center, proj.SafeDir().ToRandVelocity(ToRadians(5f), 4f), RandLerpColor(Color.Crimson, Color.Silver), 10, proj.velocity.ToRotation(), RandZeroToOne, 0.2f, randPosMoveValue: 4).SpawnToNonPreMult();
            //new TrailGlowBall(proj.Center, proj.SafeDir() * 1.5f, RandLerpColor(Color.Crimson, Color.Silver), 10, 0.15f).Spawn();
        }
        public void AddListForAda(Projectile proj)
        {
            if (!proj.HJScarlet().FirstFrame)
            {
                RotList2.Clear();
                PosList2.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    RotList2.Add(0);
                    PosList2.Add(Vector2.Zero);
                }
            }
            RotList2.Add(proj.velocity.ToRotation());
            PosList2.Add(proj.position + proj.SafeDir() * 10f);
            if (RotList2.Count > TotalListCount + 3)
            {
                RotList2.RemoveAt(0);
                PosList2.RemoveAt(0);
            }

        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && PosList.Count < 1 && PosList2.Count < 1)
                return;

            DrawTrailStarShape(proj);
            DrawProj(proj);
            DrawCirlceGlow(proj, proj.SafeDir(), proj.Center - Main.screenPosition);
        }
        public void DrawProj(Projectile proj)
        {
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D tex = Request<Texture2D>(GetInstance<ContinentOfJourney.Items.AdamantiteKnife>().Texture).Value;

            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = Color.Lerp(Color.Crimson, Color.White, rads).ToAddColor(150) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList2[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
            }

            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.velocity.ToRotation() + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, null, Color.White, proj.velocity.ToRotation() + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);

        }
        public void DrawTrailStarShape(Projectile proj)
        {
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 scale = new Vector2(0.5f, 1.2f) * 0.9f;
            for (int j = -1; j < 2; j += 2)
            {
                for (int i = 0; i < PosList2.Count; i++)
                {
                    if (PosList2[i] == Vector2.Zero)
                        continue;
                    float rads = 1f - i / (float)PosList.Count;
                    float ratiosColor = 1f * proj.Opacity * (1 - rads);
                    Color drawColor = Color.Lerp(Color.Crimson, Color.IndianRed, rads).ToAddColor(150) * ratiosColor;
                    Color drawColor2 = Color.Lerp(Color.Silver, Color.White, rads).ToAddColor(150) * ratiosColor;
                    Vector2 posOffset = proj.Size / 2 + proj.SafeDir().RotatedBy(PiOver2 * j) * 7f - Main.screenPosition;
                    if (RotList2.Count > 0)
                    {
                        SB.Draw(starShape, PosList2[i] + posOffset, null, j == -1 ? drawColor2 : drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
                    }
                }
            }
        }

        public void DrawCirlceGlow(Projectile proj, Vector2 projDir, Vector2 drawPos)
        {
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            float starDrawTime = 18f;
            for (float i = 0; i < starDrawTime; i++)
            {
                Vector2 argDir = projDir.RotatedBy(ToRadians(360f / starDrawTime * i)) * 12f;
                Vector2 starPos = drawPos + argDir + projDir * 5f;
                Vector2 scale = proj.scale * new Vector2(0.2f, 0.3f) * 0.8f * proj.Opacity;
                SB.Draw(starShape, starPos, null, Color.Lerp( Color.Red, Color.White, i / starDrawTime).ToAddColor(150), argDir.ToRotation(), starShape.ToOrigin(), scale, 0, 0);
            }
        }

    }
}
