using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.VisualRework.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.VisualRework.Projs
{
    internal class ReHellfireProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<Hellfire>();
        public override int TotalListCount => 7;
        public float Timer = 0;
        public float SpinDirection = 0;
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp) => proj.IsMe() && vp.reVisualHellfire;
        public override void RevisualUpdate(Projectile projectile)
        {
            AddListOnNeed(projectile);
            if (SpinDirection == 0)
                projectile.rotation = projectile.velocity.ToRotation();
            Timer++;
            if (Timer == 30f)
                SpinDirection = Math.Sign(projectile.velocity.X);
            if (Main.rand.NextBool())
            {
                new SmokeParticle(projectile.Center.ToRandCirclePosEdge(3f), -projectile.velocity.ToSafeNormalize() * 1.2f, RandLerpColor(Color.Orange, Color.OrangeRed), 40, RandRotTwoPi, 1, 0.18f).Spawn();
                int i = 0;
                while (i < 2)
                {
                    new TurbulenceGlowOrb(projectile.Center.ToRandCirclePosEdge(4f), 0.8f, RandLerpColor(Color.OrangeRed, Color.Orange), 40, 0.12f, RandRotTwoPi).Spawn();
                    i++;
                }
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            
            bool isEnable = IsMyPlayer(projectile, out ReVisualPlayer player) && player.reVisualHellfire;
            if (!isEnable)
            {
                return base.PreDraw(projectile, ref lightColor);
            }
            if (PosList.Count < 1 && PosList.Count < 1)
                return false;
            //最顶端绘制一个star。
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            Texture2D tex = projectile.GetTexture();
            Vector2 scale = new Vector2(0.7f, 1.2f) * 0.7f;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = (1f - i / (float)PosList.Count);
                float colorRads = 0.9f * projectile.Opacity * (1 - rads);
                Color drawColor2 = (Color.Lerp(Color.Orange, Color.White, rads).ToAddColor(150)) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList[i] + PiOver4, tex.ToOrigin(), projectile.scale * (1f - rads) * 0.99f, 0, 0);
                for (int j = 0; j < 8; j++)
                {
                    SB.Draw(tex, PosList[i] + ToRadians(60f * j).ToRotationVector2() * 2.5f, null, Color.White.ToAddColor(), RotList[i] + PiOver4, tex.ToOrigin(), projectile.scale * (1f - rads) * 0.99f, 0, 0);
                }

                scale *= 0.97f;
            }
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), projectile.rotation + PiOver4, tex.ToOrigin(), projectile.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, null, Color.White, projectile.rotation + PiOver4, tex.ToOrigin(), projectile.scale, 0, 0);
            return false;
        }
    }
}
