using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.VisualRework.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.VisualRework.Projs
{
    public class ReTitaniumKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<TitaniumKnife>();
        public float Timer = 0;
        public float SpinDirection = 0;
        public List<float> RotList2 = []; 
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualTitaniumKnife;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            AddListOnNeed(proj);
            if (!proj.HJScarlet().FirstFrame)
            {
                RotList.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    RotList2.Add(0);
                }
            }
            RotList2.Add(proj.velocity.ToRotation());
            if (RotList2.Count > TotalListCount)
                RotList2.RemoveAt(0);

            if (SpinDirection == 0)
                proj.rotation = proj.velocity.ToRotation();
            Timer++;
            if (Timer == 40f)
                SpinDirection = Math.Sign(proj.velocity.X);
            if (SpinDirection == 0)
            {
                proj.rotation = proj.velocity.ToRotation();
                if (Main.rand.NextBool(3))
                    new StarShape(proj.Center.ToRandCirclePosEdge(6f), proj.velocity.ToSafeNormalize() * Main.rand.NextFloat(0.1f, 1.4f), RandLerpColor(Color.DarkGray, Color.Silver), 0.6f, 40).Spawn();
            }
            if (Main.rand.NextBool())
                new ShinyOrbParticle(proj.Center.ToRandCirclePosEdge(8f), proj.velocity.ToRandVelocity(ToRadians(5f), 2f), RandLerpColor(Color.DarkGray, Color.Silver), 40, 0.4f).Spawn();

        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && PosList.Count < 1)
                return;
            //最顶端绘制一个star。
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D tex = proj.GetTexture();
            Vector2 scale = new Vector2(0.7f, 1.2f) * 0.7f;
            if (SpinDirection == 0)
            {
                for (int i = 0; i < RotList.Count; i++)
                {
                    if (PosList[i] == Vector2.Zero)
                        continue;
                    float rads = (1f - i / (float)PosList.Count);
                    Color drawColor = (Color.Lerp(Color.DarkGray, Color.Silver, rads) with { A = 50 }) * 0.9f * proj.Opacity * (1 - rads);
                    if (RotList.Count > 0)
                        SB.Draw(starShape, PosList[i] - Main.screenPosition, null, drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
                }
            }

            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = (1f - i / (float)PosList.Count);
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = (Color.Lerp(Color.DarkGray, Color.White, rads).ToAddColor(150)) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
                for (int j = 0; j < 8; j++)
                {
                    SB.Draw(tex, PosList[i] + ToRadians(60f * j).ToRotationVector2() * 2.5f, null, Color.White.ToAddColor(), RotList[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
                }

                scale *= 0.97f;
            }
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.rotation + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, null, lightColor, proj.rotation + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
        }
    }
}
