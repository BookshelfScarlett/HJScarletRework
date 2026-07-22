using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace HJScarletRework.Projs.Executor
{
    public class ASMDBullet : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 5;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float rotFixer = PiOver2;
            float rot = Projectile.rotation + rotFixer;
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(projTex, drawPos + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, Color.RoyalBlue.ToAddColor(), rot, ori, Projectile.scale, 0, 0);
            }
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i > 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2;
                float oldrot = Projectile.oldRot[i] + rotFixer;
                float ratios = i / (float)length;
                Color c = Color.Lerp(Color.MediumAquamarine, Color.Transparent, ratios);
                float colorLerp = Lerp(15, 255, ratios);
                float opac = Lerp(.65f, 0.45f, ratios) * Clamp(Projectile.velocity.Length(), 0, 1);
                float oldScale = Lerp(Projectile.scale* .55f, Projectile.scale* .10f, ratios);
                c *= opac;
                for (int j = 0; j < 4; j++)
                {
                    SB.Draw(projTex, oldPos + (TwoPi / 4f * j).ToRotationVector2() * 2, null, c.ToAddColor(), rot, ori, oldScale, 0, 0);
                }
                SB.Draw(projTex, oldPos, null, c, rot, ori, oldScale, 0, 0);
            }
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Blue, 0.3265f, 0.95f);
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.RoyalBlue, 1.5f, 0.95f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.CornflowerBlue, 1.25f, .95f,1.5f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White , 0.315f,.695f,1.25f);
            SB.EndShaderArea();
            SB.Draw(projTex, drawPos, null, Color.White, rot, ori, Projectile.scale, 0, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 210f * offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + Projectile.Size / 2 + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 35 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
