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
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class JudgementStarExecution : HJScarletProj, IPixelatedRenderer
    {

        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;

        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(22, 2);
        }
        private enum DoType
        {
            IsShooted,
            IsHomingToTarget,
            IsFading
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private ref float InitRot => ref Projectile.localAI[0];
        private AnimationStruct Helper = new(2);
        public NPC TargetNPC = null;
        public override void ExSD()
        {
            Projectile.extraUpdates = 1;
            Projectile.height = Projectile.width = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 150;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (TargetNPC.Equals(target) && TargetNPC != null && AttackType == DoType.IsHomingToTarget)
                return null;
            return false;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 10;
            Helper.MaxProgress[1] = 30;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextFloat() < Projectile.scale && Projectile.FinalUpdateNextBool(3))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePosEdge(6f), Projectile.velocity / 6f, RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, Main.rand.NextFloat(0.24f, 0.35f) * Projectile.scale).Spawn();
            if (Main.rand.NextFloat() < Projectile.scale && Projectile.FinalUpdateNextBool(8))
                new KiraStar(Projectile.Center.ToRandCirclePosEdge(6f), Vector2.Zero, RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, 0, 1, Main.rand.NextFloat(0.10f, 0.135f) * Projectile.scale).Spawn();


        }

        public void UpdateAttackAI()
        {
            //下方会进行手动处死，这里需要全程保证射弹存活维持演出效果
            Projectile.timeLeft = 2;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.TooAwayFromOwner())
                Projectile.Kill();

            if (Projectile.damage == 0 && Projectile.penetrate == -1)
                AttackType = DoType.IsFading;

            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsHomingToTarget:
                    DoHomingToTarget();
                    break;
                case DoType.IsFading:
                    DoFading();
                    break;
            }
        }
        #region AI方法合集
        private void DoShooted()
        {
            AttackTimer += 1f;
            if (AttackTimer < 30f * Projectile.extraUpdates)
                return;

            Projectile.netUpdate = true;
            AttackTimer = 0f;
            AttackType = DoType.IsHomingToTarget;
            //临时注册一次
            InitRot = Projectile.rotation;
        }
        public void HandleHoming()
        {
            //允许造成伤害
            //正式执行追踪AI
            if(TargetNPC.CanBeChasedBy() && TargetNPC != null)
                Projectile.HomingTarget(TargetNPC.Center, -1f, 28f, 20f);
            //我草别直接杀死射弹了
            else
            {
                AttackType = DoType.IsFading;
                Projectile.netUpdate = true;
            }
        }
        public void DoHomingToTarget()
        {
            //除非整个过程完成，否则不执行追踪AI
            //潜伏状态下，神圣新星才会转圈
            if (Helper.IsDone[0])
                HandleHoming();
            else
            {
                float finalRotation = InitRot + Pi * Helper.GetAniProgress(0);
                //将转角实际转化为需要的速度
                Helper.UpdateAniState(0);
                Projectile.velocity = finalRotation.ToRotationVector2() * 4f;
            }
        }
        //这个Fading什么都不会做，因为射弹的处死本身由timeleft手动控制
        //好吧还是会做点。
        public void DoFading()
        {
            Projectile.scale = Lerp(Projectile.scale, 0f, 0.2f);
            if (Projectile.scale <= 0f)
                Projectile.Kill();
        }
        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        #region 绘制
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;

            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawCoreStar(sb);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture,Color.DarkGoldenrod , 1.26f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture,Color.Goldenrod, 0.8f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture,Color.White, 0.58f);

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
            return Vector2.Lerp(beginScale, starScale, t) * 1f;
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
        #endregion
    }
}
