using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReLonginusProj: ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<Longinus>();
        public override int TotalListCount => 5;
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualLonginus;
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            return base.PreKill(projectile, timeLeft);
        }
        public override void RevisualUpdate(Projectile proj)
        {
            AddOldPosRotList(proj, oldRot: proj.velocity.ToRotation());

        }
        public void DrawCirlceGlow(Projectile proj, Vector2 projDir, Vector2 drawPos)
        {
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            float starDrawTime = 18f;
            for (float i = 0; i < starDrawTime; i++)
            {
                Vector2 argDir = projDir.RotatedBy(ToRadians(360f / starDrawTime * i)) * 12f;
                Vector2 starPos = drawPos + argDir - projDir * 25f;
                Vector2 scale = proj.scale * new Vector2(0.2f, 0.3f) * 0.8f * proj.Opacity;
                SB.Draw(starShape, starPos, null, Color.Lerp(Color.Gold, Color.White, i / starDrawTime).ToAddColor(150), argDir.ToRotation(), starShape.ToOrigin(), scale, 0, 0);
            }
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && RotList.Count < 1)
                return;
            //最顶端绘制一个star。
            Vector2 drawPos = proj.Center - Main.screenPosition;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Texture2D tex = proj.GetTexture();
            Vector2 scale = new Vector2(.6f, 1.4f);
            Vector2 ori = tex.Size()/2 - Vector2.One.RotatedBy(PiOver2) * 24f ;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.Gold, Color.LightYellow, rads) with { A = 50 }) * 0.9f * proj.Opacity * (1 - rads);
                Vector2 shapeScale = scale * Clamp(i / ((float)PosList.Count - 4f), 0f, 1f);
                Vector2 lerpPos = PosList[i] - Main.screenPosition - proj.SafeDir() * 12f;
                if (shapeScale.X > 0.1f && shapeScale.Y > 0.2f)
                {
                    SB.Draw(starShape, lerpPos, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos - proj.SafeDir() * 10f, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                    SB.Draw(starShape, lerpPos - proj.SafeDir() * 10f, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                }
                Color drawColor2 = (Color.Lerp(Color.White, Color.Yellow, rads) with { A = 150 }) * 1f * proj.Opacity * (1 - rads);
                SB.Draw(tex, lerpPos, null, drawColor2, RotList[i] + PiOver4, ori, proj.scale * (1f - rads), 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.velocity.ToRotation() + PiOver4, ori, proj.scale, 0, 0);
            SB.Draw(tex, drawPos, null, Color.Gold.ToAddColor(200), proj.velocity.ToRotation() + PiOver4, ori, proj.scale, 0, 0);
            DrawCirlceGlow(proj, proj.SafeDir(), drawPos);
        }
    }

}
