using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ThePunishmentStar : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;
        public NPC TargetNPC = null;
        public AnimationStruct Helper = new(3);
        public bool CanFadeAway = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(18, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.scale = 0f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 25;

            InitParticles();
        }
        public void InitParticles()
        {
            Vector2 pos = Projectile.SafeDir() * 30f;
            for (int i = 0; i < 8; i++)
            {
                new TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(6f) + pos, 1f, RandLerpColor(Color.PaleGoldenrod, Color.Goldenrod), 30, 0.38f, RandRotTwoPi).Spawn();
                if (Main.rand.NextBool(8))
                    new KiraStar(Projectile.Center.ToRandCirclePosEdge(12f) + pos, Vector2.Zero, RandLerpColor(Color.PaleGoldenrod, Color.Goldenrod), 30, 0, 1, 0.18f).Spawn();
            }
            for (int j = 0; j < 16; j++)
            {
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(8f) + pos, Projectile.SafeDir().ToRandVelocity(ToRadians(10f), 2f, 8f), RandLerpColor(Color.PaleGoldenrod, Color.Goldenrod), 40, Main.rand.NextFloat(0.24f, 0.35f)).Spawn();
            }
            new ShinyCrossStar(Projectile.Center + pos, Vector2.Zero, RandLerpColor(Color.Orange, Color.Goldenrod), 30, 0, 1, 0.8f, false).Spawn();

        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateAttackAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.scale = EaseOutExpo(Helper.GetAniProgress(0));
                Projectile.velocity *= 0.926f;

            }
            if (CanFadeAway)
            {
                Projectile.scale = Lerp(Projectile.scale, 0f, 0.22f);
                Projectile.velocity *= 0.967f;
                if (Projectile.scale <= 0f)
                    Projectile.Kill();
            }
            else
            {
                if (TargetNPC != null && TargetNPC.CanBeChasedBy())
                    Projectile.HomingTarget(TargetNPC.Center, -1, 16f, 16f, 10f);
                else
                    CanFadeAway = true;
            }
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextFloat() < Projectile.scale && Projectile.FinalUpdateNextBool(3))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePosEdge(6f), Projectile.velocity / 6f, RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, Main.rand.NextFloat(0.24f, 0.35f) * Projectile.scale).Spawn();
            if (Main.rand.NextFloat() < Projectile.scale && Projectile.FinalUpdateNextBool(8))
                new KiraStar(Projectile.Center.ToRandCirclePosEdge(6f), Vector2.Zero, RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, 0, 1, Main.rand.NextFloat(0.10f, 0.135f) * Projectile.scale).Spawn();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = target.Equals(TargetNPC) && TargetNPC != null && Helper.IsDone[0];
            if (canHit)
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CanFadeAway = true;
            for (int i = 0; i < 4; i++)
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePosEdge(10), Projectile.velocity.ToRandVelocity(ToRadians(15f), 3f, 6f), RandLerpColor(Color.DarkOrange, Color.Goldenrod), 30, 0.38f).Spawn();

            base.OnHitNPC(target, hit, damageDone);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawCoreStar(sb);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkGoldenrod, 1.26f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Goldenrod, 0.8f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f);

            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawCoreStar(SpriteBatch sb)
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (float i = 0; i < 1f; i += 0.1f)
            {
                Vector2 starScale = GetScale(i) * Projectile.scale;
                float colorAlpha = GetAlphaFade(1 - i);
                Color drawColor = Color.Lerp(Color.DarkGoldenrod * colorAlpha, Color.LightGoldenrodYellow * colorAlpha, colorAlpha);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, drawColor, Projectile.rotation + PiOver2, star.Size() / 2, starScale, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.LightGoldenrodYellow * colorAlpha, Projectile.rotation, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
                sb.Draw(star, drawPos, null, Color.LightGoldenrodYellow * colorAlpha, Projectile.rotation + PiOver2, star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0);
            }
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(0.9f, 1.4f);
            Vector2 beginScale = new(0.1f, 0.2f);
            return Vector2.Lerp(beginScale, starScale, t) * 0.91f;
        }

        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 8 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
