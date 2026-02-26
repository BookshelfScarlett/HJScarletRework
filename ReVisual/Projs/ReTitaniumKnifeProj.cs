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
using Terraria.ID;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReTitaniumKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<TitaniumKnife>();
        public float Timer = 0;
        public float SpinDirection = 0;
        public List<float> RotList2 = [];
        public List<Vector2> PosList2 = [];
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualTitaniumKnife;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            AddListOnNeed(proj);
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
            PosList2.Add(proj.Center);
            if (PosList2.Count > TotalListCount + 2)
                PosList2.RemoveAt(0);
            if (RotList2.Count > TotalListCount + 2)
                RotList2.RemoveAt(0);
            Timer++;
            if (Timer == 40f)
                SpinDirection = Math.Sign(proj.velocity.X);
            if (Main.rand.NextBool())
                new ShinyOrbParticle(proj.Center.ToRandCirclePosEdge(8f), proj.velocity.ToRandVelocity(ToRadians(5f), 2f), RandLerpColor(Color.DarkGray, Color.Silver), 40, 0.4f).Spawn();
            Dust d = Dust.NewDustPerfect(proj.Center.ToRandCirclePosEdge(4f), DustID.SilverCoin, -proj.velocity.ToRandVelocity(ToRadians(5f), 2f));
            d.noGravity = true;
            d.scale *= 1.1f;

        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && PosList.Count < 1)
                return;
            //最顶端绘制一个star。
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D tex = proj.GetTexture();
            Vector2 scale = new Vector2(1.16f, 1.4f);
            for (int i = 0; i < RotList2.Count; i++)
            {
                if (PosList2[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList2.Count;
                Color drawColor = (Color.Lerp(Color.DarkGray, Color.Silver, rads).ToAddColor(100)) * 0.4f * proj.Opacity * (1 - rads);
                if (RotList.Count > 0)
                {
                    SB.Draw(starShape, PosList2[i] - proj.SafeDir() * 10f - Main.screenPosition, null, drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale * 1f, 0, 0);
                    SB.Draw(starShape, PosList2[i] - Main.screenPosition, null, drawColor, RotList2[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
                }
                scale *= new Vector2(0.97f, 0.99f);
            }
            for (int i = 0; i < PosList2.Count; i++)
            {
                if (PosList2[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList2.Count;
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = Color.Lerp(Color.DarkGray, Color.White, rads).ToAddColor(150) * colorRads;
                SB.Draw(tex, PosList2[i] - Main.screenPosition, null, drawColor2, RotList2[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
            }
            float speedRot = proj.velocity.ToRotation();
            Color silverColor = Color.Lerp(Color.White, Color.Silver, Clamp(Timer / 40f, 0f, 1f));
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.DarkGray.ToAddColor(0), speedRot + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            SB.Draw(tex, drawPos, null, silverColor.ToAddColor(200), speedRot + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
        }
    }
}
