using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class DesterrennachtHealProj : HJScarletHealProj, IPixelatedRenderer
    {
        public override int HealAmt => 25;
        public override int TrailLength => 24;
        public override int ExtraUpdates => 2;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit with { MaxInstances = 1, Pitch = 0.8f }, Projectile.Center);
            float generalScale = 0.75f;
            new CrossGlow(Projectile.Center, Color.RoyalBlue, 50, 1f, 0.21f * generalScale).Spawn();
            new CrossGlow(Projectile.Center, Color.SkyBlue, 50, 1f, 0.15f*generalScale).Spawn();
            new CrossGlow(Projectile.Center, Color.AliceBlue, 50, 0.5f, 0.10f * generalScale).Spawn();
            new FusableBall(Projectile.Center, Vector2.Zero, Color.Lerp(Color.RoyalBlue, Color.AliceBlue, 0.35f), 55, 1f, Vector2.One * 0.5f * generalScale).SpawnToPriorityNonPreMult();
            for (int i = 0; i < 20; i++)
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(12f * generalScale), 1.82f, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 80, .1f * Main.rand.NextFloat(0.75f,1.2f) * generalScale, RandRotTwoPi).Spawn();
            for (int i = 0; i < 10; i++)
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(10f * generalScale), Projectile.velocity.ToRandVelocity(ToRadians(15f), 2.4f, 8.6f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 30, RandRotTwoPi, 1f, 0.65f * generalScale, false).Spawn();
        }
        public override void ProjAI()
        {
            UpdateParticles();
            if (IsIntersect)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.2f);
                Projectile.velocity *= 0.26f;
                if (Projectile.Opacity < 0.02f)
                    Projectile.Kill();
            }
            else
                DefaultHealProjAI();
        }

        private void UpdateParticles()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3) && Projectile.Opacity > Main.rand.NextFloat())
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePosEdge(6f), Projectile.velocity / 8f, RandLerpColor(Color.RoyalBlue, Color.AliceBlue) * Projectile.Opacity, 30, 0.48f).Spawn();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            SB.EnterShaderArea();
            DrawCoreStar(SB);
            DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.RoyalBlue, 1.10f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.MidnightBlue, 0.98f);
            DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.AliceBlue, 0.58f);
            SB.EndShaderArea();
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            //DrawCoreStar(sb);
            

            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawCoreStar(SpriteBatch sb)
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i) * Projectile.scale;
                float colorAlpha = GetAlphaFade(1 - i) * Projectile.Opacity;
                Color drawColor = Color.Lerp(Color.RoyalBlue * colorAlpha, Color.MidnightBlue* colorAlpha, colorAlpha);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation, star.Size() / 2, new Vector2(starScale.X, starScale.Y / 3f), SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation + PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.White * colorAlpha, Projectile.rotation, star.Size() / 2,new Vector2(starScale.X, starScale.Y / 3f) * 0.5f, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.White * colorAlpha, Projectile.rotation + PiOver2, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
                sb.Draw(HJScarletTexture.Texture_BloomShockwave.Value, drawPos, null, drawColor * colorAlpha, 0, HJScarletTexture.Texture_BloomShockwave.Value.ToOrigin(), 0.05f, 0, 0);
            }
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.02f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            if (Projectile.Opacity < 1f)
            {
                posCount = (int)(posCount * (Projectile.Opacity));
                if (posCount < 3)
                    posCount = 3;
            }
            
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * (offsetHeight -1f);
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 9 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(0.9f, 1.4f);
            Vector2 beginScale = new(0.1f, 0.2f);
            return Vector2.Lerp(beginScale, starScale, t) * 0.99f;
        }
    }
}
