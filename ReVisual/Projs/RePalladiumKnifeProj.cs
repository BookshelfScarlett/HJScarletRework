using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class RePalladiumKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<PalladiumKnife>();
        public float Timer = 0;
        public float Rotations = 0;
        public float SpinDirection = 0;
        public List<float> RotList2 = []; 
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualPalladiumKnife;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            if (!proj.HJScarlet().FirstFrame)
            {
                SpinDirection = Math.Sign(proj.velocity.X);
                PosList.Clear();
                RotList.Clear();
                RotList.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    PosList.Add(Vector2.Zero);
                    RotList.Add(0);
                    RotList2.Add(0);
                }
            }
            Rotations += SpinDirection * 0.3f;
            PosList.Add(proj.Center);
            RotList.Add(Rotations);
            RotList2.Add(proj.velocity.ToRotation());
            if (RotList.Count > TotalListCount)
                RotList.RemoveAt(0);
            if (PosList.Count > TotalListCount)
                PosList.RemoveAt(0);
            if (RotList2.Count > TotalListCount)
                RotList2.RemoveAt(0);
            if (Main.rand.NextBool(3))
                new StarShape(proj.Center.ToRandCirclePosEdge(6f), proj.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.1f, 1.4f), RandLerpColor(Color.OrangeRed, Color.Orange), 0.6f, 40).Spawn();
            if (Main.rand.NextBool())
                new ShinyOrbParticle(proj.Center.ToRandCirclePosEdge(8f), proj.velocity.ToRandVelocity(ToRadians(5f), 2f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.4f).Spawn();
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && PosList.Count < 1)
                return;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D tex = proj.GetTexture();
            Vector2 scale = new Vector2(0.7f, 1.2f) * 0.8f;
            if (SpinDirection == 0)
            {
                for (int i = 0; i < RotList.Count; i++)
                {
                    if (PosList[i] == Vector2.Zero)
                        continue;
                    float rads = 1f - i / (float)PosList.Count;
                    Color drawColor = (Color.Lerp(Color.OrangeRed, Color.Orange, rads) with { A = 50 }) * 0.9f * proj.Opacity * (1 - rads);
                    if (RotList.Count > 0)
                        SB.Draw(starShape, PosList[i] - Main.screenPosition, null, drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
                }
            }
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                float colorRads = 0.98f * proj.Opacity * (1 - rads);
                Color drawColor2 = Color.Lerp(Color.Orange, Color.White, rads).ToAddColor(150) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
                for (int j = 0; j < 8; j++)
                    SB.Draw(tex, PosList[i] + ToRadians(60f * j).ToRotationVector2() * 2.5f, null, Color.White.ToAddColor(), RotList[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);

                scale *= 0.97f;
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.rotation + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            SB.Draw(tex, drawPos, null, lightColor, proj.rotation + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
        }
    }
}
