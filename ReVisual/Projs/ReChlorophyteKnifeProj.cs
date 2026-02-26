using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReChlorophyteKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<ChlorophyteKnife>();
        public override int TotalListCount => 6;
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            for (int i = 0; i < 6; i++)
                new ShinyOrbParticle(projectile.Center, -projectile.SafeDir().ToRandVelocity(ToRadians(15f), 3.2f, 7.7f), RandLerpColor(Color.Green, Color.DarkGreen), 40, 0.75f).Spawn();
            return false;
        }

        public override void RevisualUpdate(Projectile projectile)
        {
            Lighting.AddLight(projectile.Center, TorchID.Jungle);
            if (!projectile.HJScarlet().FirstFrame)
            {
                PosList.Clear();
                RotList.Clear();
                for (int i = 0; i < TotalListCount + 2; i++)
                {
                    PosList.Add(Vector2.Zero);
                    RotList.Add(0);
                }
            }
            PosList.Add(projectile.Center);
            RotList.Add(projectile.velocity.ToRotation());
            if (RotList.Count > TotalListCount + 2)
                RotList.RemoveAt(0);
            if (PosList.Count > TotalListCount + 2)
                PosList.RemoveAt(0);
            if (Main.rand.NextBool())
            {
                new ShinyOrbParticle(projectile.Center.ToRandCirclePosEdge(4f), projectile.velocity.ToRandVelocity(ToRadians(6f), 1.8f), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 30, Main.rand.NextFloat(.55f, .65f) * 1f).Spawn();
            }
        }
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualChlorophyteKnife;
        }
        public void TopStarShape(Vector2 drawPos, Texture2D starShape, Projectile projectile)
        {
            Vector2 scale3 = new Vector2(1.1f, 1.3f) * 0.7f;
            Vector2 glowPos = drawPos + projectile.velocity.ToRotation().ToRotationVector2() * 5f;
            SB.Draw(starShape, glowPos * projectile.scale, null, Color.Green.ToAddColor(0), projectile.velocity.ToRotation() + PiOver2, starShape.ToOrigin(), scale3, 0, 0);

        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            bool isEnable = IsMyPlayer(projectile, out ReVisualPlayer player) && player.reVisualChlorophyteKnife;
            if(!isEnable)
                return base.PreDraw(projectile, ref lightColor);
            if (PosList.Count < 1 && RotList.Count < 1)
                return false;
            //最顶端绘制一个star。
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Texture2D tex = projectile.GetTexture();
            Vector2 scale = new Vector2(.6f, 1.4f);
            Vector2 ori = tex.Size()/2;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.Green, Color.Lime, rads) with { A = 50 }) * 0.9f * projectile.Opacity * (1 - rads);
                Vector2 shapeScale = scale * Clamp(i / ((float)PosList.Count - 4f), 0f, 1f);
                Vector2 lerpPos = PosList[i] - Main.screenPosition;
                if (shapeScale.X > 0.1f && shapeScale.Y > 0.2f)
                {
                    SB.Draw(starShape, lerpPos, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos - projectile.SafeDir() * 10f, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                    SB.Draw(starShape, lerpPos - projectile.SafeDir() * 10f, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                }
                Color drawColor2 = (Color.Lerp(Color.White, Color.Lime, rads) with { A = 150 }) * 1f * projectile.Opacity * (1 - rads);
                SB.Draw(tex, lerpPos, null, drawColor2, RotList[i] + PiOver4, ori, projectile.scale * (1f - rads), 0, 0);
            }
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), projectile.velocity.ToRotation() + PiOver4, tex.ToOrigin(), projectile.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, null, Color.White, projectile.velocity.ToRotation() + PiOver4, tex.ToOrigin(), projectile.scale, 0, 0);
            
;
            return false;
        }
    }
}
