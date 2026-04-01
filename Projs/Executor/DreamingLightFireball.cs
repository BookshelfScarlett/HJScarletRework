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
    public class DreamingLightFireball:HJScarletProj, IPixelatedRenderer
    {
        public override string Texture =>HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;
        public NPC TargetNPC = null;
        public AnimationStruct Helper = new(2);
        public float Speed = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetupImmnuity(30);
            Projectile.extraUpdates = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 600;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 30;
            Speed = Projectile.velocity.LengthSquared();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            UpdateParticles();
            if(Projectile.damage == 0 && Projectile.penetrate == -1)
            {
                UpdateIsHit();
            }
            else
                UpdateNotHit();
        }

        private void UpdateIsHit()
        {
            Projectile.scale = Lerp(Projectile.scale, 0f, 0.45f);
            if (Projectile.scale <= 0f)
                Projectile.Kill();

        }

        private void UpdateNotHit()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(5f * Helper.GetAniProgress(0) - 2.5f));
                if (Projectile.velocity.LengthSquared() < Speed)
                    Projectile.velocity *= 1.1f;
                else
                    Projectile.velocity *= 0.9f;
            }
            else
            {
                if (TargetNPC != null && TargetNPC.CanBeChasedBy())
                {
                    Projectile.HomingTarget(TargetNPC.Center, -1, 18f, 12f, 10f);
                }
                else if (Projectile.GetTargetSafe(out NPC target))
                {
                    TargetNPC = target;
                }
            }

        }

        private void UpdateParticles()
        {
            if (Projectile.IsOutScreen() && Main.rand.NextFloat() > Projectile.scale)
                return;
            if(Main.rand.NextBool(6))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(12f), Projectile.velocity / 2f, RandLerpColor(Color.LawnGreen, Color.LimeGreen), 30, 0.54f * Projectile.scale).Spawn();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (TargetNPC != null && TargetNPC.Equals(target) && Helper.IsDone[0])
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8 - PerformanceMode.ToInt() * 4; i++)
                new SmokeParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(1.4f, 4.9f), RandLerpColor(Color.LimeGreen, Color.Black), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.4f, 0.7f) * 0.5f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            for (int i = 0; i < 16 - PerformanceMode.ToInt() * 8; i++)
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(1.2f, 3.6f), RandLerpColor(Color.LawnGreen, Color.Lime), 40, Main.rand.NextFromList(0.5f, 0.8f)).Spawn();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawCoreStar(sb);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkSeaGreen, 1.48f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.LawnGreen, 1.26f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Lime, 0.88f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.54f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public void DrawCoreStar(SpriteBatch sb)
        {
            //绘制残影
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Texture2D fuseball = HJScarletTexture.Particle_FusableBall.Value;
            Texture2D projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            float oriScale = 0.72f;
            float fuseballScale = oriScale * 0.13f;
            float projTexScale = oriScale * 0.72f;
            //往后拖尾一个。
            QuickDraw(sb, fuseball, Color.LawnGreen, new Vector2(1.5f, 0.5f), fuseballScale, 0f);
            //在最底部打一层棱形。尽量模拟火焰的尖角效果
            //尽管如此，这个棱形很大概率会被覆盖
            QuickDraw(sb, fuseball, Color.LawnGreen, new Vector2(1.2f, 0.8f), fuseballScale, 0);
            QuickDraw(sb, projTex, Color.White, new Vector2(1.5f, 0.5f), projTexScale, 0f);
            QuickDraw(sb, star, Color.LawnGreen, new Vector2(1.27f, 0.5f), 0.72f, 0f, PiOver2);
            QuickDraw(sb, projTex, Color.White, new Vector2(1.2f, 0.8f), projTexScale);
        }
        public void QuickDraw(SpriteBatch sb, Texture2D tex, Color color, Vector2 shape, float scaleMul, float posOffset = 0f,float rotFixer = 0f)
        {
            Vector2 projPos = Projectile.Center - Main.screenPosition;
            sb.Draw(tex, projPos - Projectile.SafeDir() * posOffset, null, color, Projectile.rotation + rotFixer, tex.ToOrigin(), Projectile.scale * shape * scaleMul, 0, 0);
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
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(1.2f, 0.8f);
            Vector2 beginScale = new(0.5f, 0.2f);
            return Vector2.Lerp(beginScale, starScale, t) * 0.921f;
        }
    }
}
