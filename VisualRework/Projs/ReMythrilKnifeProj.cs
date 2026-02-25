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
using Terraria.ID;

namespace HJScarletRework.VisualRework.Projs
{
    public class ReMythrilKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<MythrilKnife>();
        public float Timer = 0;
        public float ExtraTimer = 0;
        public float SpinDirection = 0;
        public List<float> RotList2 = []; 
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualMythrilKnife;
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

            if (SpinDirection == 0)
                proj.rotation = proj.velocity.ToRotation();
            Timer++;
            if (Timer == 30f)
                SpinDirection = Math.Sign(proj.velocity.X);
            
            Vector2 speedOffset = proj.velocity / 4;
            Vector2 dir = proj.SafeDir();
            Vector2 mountedPos = proj.Center;
            //总体在底下绘制一些别的粒子，这里用的是树叶
            ExtraTimer += ToRadians(8f);
            for (int j = -1; j < 2; j += 2)
            {
                for (float i = 0; i < 4; i++)
                {
                    Vector2 spawnPos = mountedPos + dir.RotatedBy(PiOver2 * j) * MathF.Sin(ExtraTimer - ToRadians(i * 2f)) * 13f;
                    new ShinyOrbParticle(spawnPos - speedOffset * i, dir * 1.2f, RandLerpColor(Color.DeepSkyBlue, Color.SeaGreen), 15, 0.28f).Spawn();
                    if (i == 2 && j == -1)
                    {
                        if (Main.rand.NextBool())
                            new TurbulenceGlowOrb(proj.Center.ToRandCirclePos(1.2f * MathF.Sin(ExtraTimer)), 0.5f, RandLerpColor(Color.DeepSkyBlue, Color.SeaGreen), 80, Main.rand.NextFloat(0.1f, 0.14f), RandRotTwoPi).Spawn();
                    }
                    
                }
            }
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && PosList.Count < 1)
                return;
            //最顶端绘制一个star。
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D tex = proj.GetTexture();

            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 scale = new Vector2(0.7f, 1.2f) * 0.7f;

            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = (1f - i / (float)PosList.Count);
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = (Color.Lerp(Color.DeepSkyBlue, Color.SeaGreen, rads).ToAddColor(100)) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList2[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
            }
            for (int i = 0; i < RotList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = (1f - i / (float)PosList.Count);
                Color drawColor = (Color.Lerp(Color.DeepSkyBlue, Color.SeaGreen, rads) with { A =0 }) * 0.9f * proj.Opacity * (1 - rads);
                if (RotList.Count > 0)
                    SB.Draw(starShape, PosList[i] - Main.screenPosition, null, drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
                scale *= 0.97f;
            }
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.velocity.ToRotation() + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, null, Color.White, proj.velocity.ToRotation() + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
        }
    }
}
