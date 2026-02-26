using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReBackstabberProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<Backstabber>();
        public override int TotalListCount => 4;
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualBackStabber;
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            return base.PreKill(projectile, timeLeft);
        }
        public List<Vector2> TrailPos = [];
        public List<float> TrailRot = [];
        public float Timer = 0;
        public float SpinDirection = 0;

        public override void RevisualUpdate(Projectile proj)
        {
            AddListOnNeed(proj);
            if (!proj.HJScarlet().FirstFrame)
            {
                for (int i = 0; i < TotalListCount; i++)
                {
                    TrailPos.Add(Vector2.Zero);
                    TrailRot.Add(0);
                }
            }
            TrailPos.Add(proj.Center);
            TrailRot.Add(proj.velocity.ToRotation());
            if (TrailPos.Count > TotalListCount)
                TrailPos.RemoveAt(0);
            if (TrailRot.Count > TotalListCount)
                TrailRot.RemoveAt(0);
            Timer++;
            if (SpinDirection == 0)
                proj.rotation = proj.velocity.ToRotation();
            if (Timer == 30f)
                SpinDirection = Math.Sign(proj.velocity.X);
            if (Main.rand.NextBool())
                new ShinyOrbParticle(proj.Center.ToRandCirclePosEdge(8f), proj.velocity.ToRandVelocity(ToRadians(5f), 2f), RandLerpColor(Color.DarkBlue, Color.DarkViolet), 40, 0.4f).Spawn();

            base.RevisualUpdate(proj);
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && RotList.Count < 1)
                return;
            Texture2D tex = proj.GetTexture();
            Vector2 drawPos = proj.Center - Main.screenPosition;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = Color.Lerp(Color.DarkGray, Color.White, rads).ToAddColor(150) * colorRads;
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList[i] + PiOver4, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
            }
            float speedRot = proj.rotation;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(0), speedRot + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
            SB.Draw(tex, drawPos, null, lightColor, speedRot + PiOver4, tex.ToOrigin(), proj.scale, 0, 0);
        }
    }

}
