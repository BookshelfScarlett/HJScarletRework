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
    public class ReOrichalcumKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<OrichalcumKnife>();
        public float Timer = 0;
        public float SpinDirection = 0;
        public List<float> RotList2 = [];
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualOrihalcumKnife;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            AddListOnNeed(proj);
            if (!proj.HJScarlet().FirstFrame)
            {
                RotList2.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    RotList2.Add(0);
                }
            }
            RotList2.Add(proj.velocity.ToRotation());
            if (RotList2.Count > TotalListCount)
                RotList2.RemoveAt(0);

            Timer++;
            if (Timer == 30f)
                SpinDirection = Math.Sign(proj.velocity.X);
            if (Main.rand.NextBool(3))
                new StarShape(proj.Center.ToRandCirclePosEdge(6f), proj.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.1f, 1.4f), RandLerpColor(Color.HotPink, Color.Pink), 0.6f, 40).Spawn();
            if (Main.rand.NextBool())
                new ShinyOrbParticle(proj.Center.ToRandCirclePosEdge(8f), proj.velocity.ToRandVelocity(ToRadians(5f), 2f), RandLerpColor(Color.HotPink, Color.Silver), 40, 0.4f).Spawn();
        }
        public void DrawStarTrail(Projectile proj)
        {
            Vector2 scale = new Vector2(0.7f, 1.2f) * 0.7f;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            for (int i = 0; i < RotList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.HotPink, Color.Silver, rads) with { A = 50 }) * 0.9f * proj.Opacity * (1 - rads);
                if (RotList.Count > 0)
                    SB.Draw(starShape, PosList[i] - Main.screenPosition, null, drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
            }
        }
        public void DrawProj(Projectile proj)
        {
            //最顶端绘制一个star。
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D tex = proj.GetTexture();
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = Color.Lerp(Color.HotPink, Color.Silver, rads).ToAddColor(150) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList2[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
            }
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.velocity.ToRotation() + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, null, Color.White, proj.velocity.ToRotation() + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && PosList.Count < 1)
                return;
            DrawStarTrail(proj);
            DrawProj(proj);
        }
    }
}
