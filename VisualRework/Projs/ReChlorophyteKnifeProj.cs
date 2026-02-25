using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.VisualRework.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.VisualRework.Projs
{
    public class ReChlorophyteKnifeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<ChlorophyteKnife>();
        public override int TotalListCount => 7;
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(projectile.position, 32, 32, 128);
            }
            return true;
        }
        public override void RevisualUpdate(Projectile projectile)
        {
            if (!projectile.HJScarlet().FirstFrame)
            {
                PosList.Clear();
                RotList.Clear();
                for (int i = 0; i < TotalListCount; i++)
                {
                    PosList.Add(Vector2.Zero);
                    RotList.Add(0);
                }
            }
            PosList.Add(projectile.Center);
            RotList.Add(projectile.velocity.ToRotation());
            if (RotList.Count > TotalListCount)
                RotList.RemoveAt(0);
            if (PosList.Count > TotalListCount)
                PosList.RemoveAt(0);
            if (Main.rand.NextBool(3))
            {
                new ShinyCrossStar(projectile.Center.ToRandCirclePosEdge(3f), projectile.velocity.ToRandVelocity(ToRadians(3f), 1.4f), RandLerpColor(Color.Green, Color.Lime), 40, RandRotTwoPi, 1f, 0.3f, 0.2f).Spawn();
            }
            if (Main.rand.NextBool())
                new ShinyOrbParticle(projectile.Center.ToRandCirclePosEdge(4f), projectile.velocity.ToRandVelocity(ToRadians(6f), 1.8f), RandLerpColor(Color.Green, Color.LimeGreen), 30, Main.rand.NextFloat(.25f, .45f)).Spawn();
        }
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualChlorophyteKnife;
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            bool isEnable = IsMyPlayer(projectile, out ReVisualPlayer player) && player.reVisualChlorophyteKnife;
            if(!isEnable)
                return base.PreDraw(projectile, ref lightColor);
            if (PosList.Count < 1 && RotList.Count < 1)
                return false;
            //最顶端绘制一个star。
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            Vector2 scale3 = new Vector2(1.1f, 1.3f) * 0.7f;
            Vector2 glowPos = drawPos + projectile.velocity.ToRotation().ToRotationVector2() * 5f;
            SB.Draw(starShape, (glowPos) * projectile.scale, null, Color.Green.ToAddColor(0), projectile.velocity.ToRotation() + PiOver2, starShape.ToOrigin(), scale3, 0, 0);
            Texture2D tex = projectile.GetTexture();
            Vector2 scale = new Vector2(0.7f, 1.2f) * 0.7f;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = (1f - i / (float)PosList.Count);
                Color drawColor = (Color.Lerp(Color.Green, Color.Lime, rads) with { A = 50 }) * 0.7f * projectile.Opacity * (1 - rads);
                SB.Draw(starShape, PosList[i] - Main.screenPosition, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), scale, 0, 0);
                Color drawColor2 = (Color.Lerp(Color.White, Color.Lime, rads) with { A = 150 }) * 0.9f * projectile.Opacity * (1 - rads);
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList[i] + PiOver4, tex.ToOrigin(), projectile.scale * (1f - rads) * 0.95f, 0, 0);
                scale *= 0.97f;
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
