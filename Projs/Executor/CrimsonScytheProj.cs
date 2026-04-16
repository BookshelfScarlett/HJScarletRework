using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheProj : HJScarletProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override ClassCategory Category => ClassCategory.Executor;
        /// <summary>
        /// 挥砍的目标位置
        /// </summary>
        private float TargetRotation = 0;
        private Vector2 HeldPos;
        private float SwingRotation;
        private Vector2 InitVector;
        public enum SwingState
        {
            LeftHalf,
            RightHalf,
            FullCircle
        }
        public List<Vector2> OldAimPos = [];
        public AnimationStruct ScytheAnimation = new(3);
        public SwingState SwingType
        {
            get => (SwingState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public bool PlaySound = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 9;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }
        public override void ExSD()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 10;
            Projectile.localNPCHitCooldown = 3;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            switch (SwingType)
            {

                case SwingState.RightHalf:
                    ScytheAnimation.MaxProgress[AniState.Begin] = 10 * Projectile.extraUpdates;
                    ScytheAnimation.MaxProgress[AniState.Mid] = 35 * Projectile.extraUpdates;
                    ScytheAnimation.MaxProgress[AniState.End] = 20 * Projectile.extraUpdates;
                    break;
                default:
                    ScytheAnimation.MaxProgress[AniState.Begin] = 8 * Projectile.extraUpdates;
                    ScytheAnimation.MaxProgress[AniState.Mid] = 15 * Projectile.extraUpdates;
                    ScytheAnimation.MaxProgress[AniState.End] = 15 * Projectile.extraUpdates;
                    break;
            }
            Projectile.direction = Projectile.spriteDirection = ((SwingType != SwingState.RightHalf).ToDirectionInt()) * Owner.direction;
            InitVector = Owner.ToMouseVector2();
            TargetRotation = Owner.ToMouseVector2().ToRotation();
            Projectile.rotation = TargetRotation;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void ProjAI()
        {
            if (CheckOwnerDead())
                return;
            UpdateScytheAnimation();
            UpdateScytheState();
            UpdatePlayerState();
            if (OldAimPos.Count > 50)
                OldAimPos.RemoveAt(0);
        }
        private void UpdatePlayerState()
        {
            Projectile.Center = Owner.Center + HeldPos;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        private void UpdateScytheState()
        {
        }

        //类似于状态机，但仍然有点差距。
        public void UpdateScytheAnimation()
        {
            switch (SwingType)
            {
                case SwingState.LeftHalf:
                    UpdateLeftHalf();
                    break;
                case SwingState.RightHalf:
                    UpdateRightHalf();
                    break;
                case SwingState.FullCircle:
                    UpdateFullCircle();
                    break;
            }
        }
        #region 第三挥砍

        private void UpdateFullCircle()
        {
            if (!ScytheAnimation.IsDone[AniState.Begin])
            {
                ScytheAnimation.UpdateAniState(AniState.Begin);
                FullCircleAniHandlerBegin();
            }
            else if (!ScytheAnimation.IsDone[AniState.Mid])
            {
                ScytheAnimation.UpdateAniState(AniState.Mid);
                FullCircleAniHandlerMid();
            }
            else if (!ScytheAnimation.IsDone[AniState.End])
            {
                ScytheAnimation.UpdateAniState(AniState.End);
                FullCircleAniHandlerEnd();
            }
            else
                Projectile.Kill();

        }

        private void FullCircleAniHandlerBegin()
        {
            float easedProgress = (ScytheAnimation.GetAniProgress(AniID.Begin));
            float curRotation = ScytheAnimation.UpdateAngle(-105, -115, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetScaleMult(easedProgress);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }

        private void FullCircleAniHandlerMid()
        {
            if (!PlaySound)
            {
                PlaySound = true;
                SoundEngine.PlaySound(HJScarletSounds.Hammer_Shoot[2], Owner.Center);
            }

            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.Mid));
            float curRotation = ScytheAnimation.UpdateAngle(-115, 200, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetScaleMult2(easedProgress);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OldAimPos.Add(tarPos * 180f);

        }

        private void FullCircleAniHandlerEnd()
        {
            float easedProgress = EaseOutCubic(ScytheAnimation.GetAniProgress(AniID.End));
            float curRotation = ScytheAnimation.UpdateAngle(200, 220, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetScaleMult3(easedProgress);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OldAimPos.Add(tarPos * 180f);

        }
        #endregion

        #region 第二挥砍
        private void UpdateRightHalf()
        {
            if (!ScytheAnimation.IsDone[AniState.Begin])
            {
                ScytheAnimation.UpdateAniState(AniState.Begin);
                RightHalftAniHandlerBegin();
            }
            else if (!ScytheAnimation.IsDone[AniState.Mid])
            {
                ScytheAnimation.UpdateAniState(AniState.Mid);
                RightHalftAniHandlerMid();
            }
            else if (!ScytheAnimation.IsDone[AniState.End])
            {
                ScytheAnimation.UpdateAniState(AniState.End);
                RightHalftAniHandlerEnd();
            }
            else
                Projectile.Kill();

        }


        private void RightHalftAniHandlerBegin()
        {
            float easedProgress = (ScytheAnimation.GetAniProgress(AniID.Begin));
            float curRotation = ScytheAnimation.UpdateAngle(200, 190, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetLerp_Mid(easedProgress, 0);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }

        private void RightHalftAniHandlerMid()

        {

            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.Mid));
            if (!PlaySound && easedProgress > 0.15f)
            {
                PlaySound = true;
                SoundEngine.PlaySound(HJScarletSounds.Hammer_Shoot[1], Owner.Center);
            }
            float curRotation = ScytheAnimation.UpdateAngle(190, 800, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetLerp_Mid(easedProgress, 1);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OldAimPos.Add(tarPos * 180f);
        }
        private void RightHalftAniHandlerEnd()
        {
            float easedProgress = EaseOutCubic(ScytheAnimation.GetAniProgress(AniID.End));
            float curRotation = ScytheAnimation.UpdateAngle(800, 820, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetLerp_Mid(easedProgress, 2);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OldAimPos.Add(tarPos * 180f);

        }
        public float GetLerp_Mid(float t, int id)
        {
            if (id == 0)
            {
                return Lerp(0.75f, 1.0f, t);
            }
            else if (id == 1)
            {
                if (t < 0.2f)
                    return Lerp(1.0f, 1.05f, (t) / 0.2f);
                else if (t < 0.8f)
                    return Lerp(1.05f, 1.15f, (t - 0.2f) / (0.8f - 0.2f));
                else
                    return Lerp(1.15f, 1.0f, (t - 0.8f) / (1f - 0.8f));
            }
            else
                return Lerp(1.0f, 0.75f, t);
        }

        #endregion

        #region 第一挥砍
        private void UpdateLeftHalf()
        {
            if (!ScytheAnimation.IsDone[AniState.Begin])
            {
                ScytheAnimation.UpdateAniState(AniState.Begin);
                LeftHalftAniHandlerBegin();
            }
            else if (!ScytheAnimation.IsDone[AniState.Mid])
            {
                ScytheAnimation.UpdateAniState(AniState.Mid);
                LeftHalftAniHandlerMid();
            }
            else if (!ScytheAnimation.IsDone[AniState.End])
            {
                ScytheAnimation.UpdateAniState(AniState.End);
                LeftHalftAniHandlerEnd();
            }
            else
                Projectile.Kill();
        }
        private void LeftHalftAniHandlerBegin()
        {
            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.Begin));
            float curRotation = ScytheAnimation.UpdateAngle(-150, -160, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetScaleMult(easedProgress);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
        }

        private void LeftHalftAniHandlerMid()
        {
            if (!PlaySound)
            {
                PlaySound = true;
                SoundEngine.PlaySound(HJScarletSounds.Hammer_Shoot[0], Owner.Center);
            }

            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.Mid));

            float curRotation = ScytheAnimation.UpdateAngle(-160, 140, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetScaleMult2(easedProgress);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OldAimPos.Add(tarPos * 180f);
        }

        private void LeftHalftAniHandlerEnd()
        {
            float easedProgress = EaseOutCubic(ScytheAnimation.GetAniProgress(AniID.End));
            float curRotation = ScytheAnimation.UpdateAngle(140, 150, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f * GetScaleMult3(easedProgress);
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OldAimPos.Add(tarPos * 180f);
        }

        #endregion
        #region lerp函数
        public float GetScaleMult(float t)
        {
            return Lerp(0.75f, 1.0f, t);
        }
        public float GetScaleMult2(float t)
        {
            if (t < 0.5f)
                return Lerp(1.0f, 1.15f, t * 2f);
            else
                return Lerp(1.15f, 1f, (t - 0.5f) * 2f);
        }
        public float GetScaleMult3(float t)
        {
            return Lerp(1.0f, 0.75f, t);
        }
        #endregion

        public bool CheckOwnerDead()
        {
            if (Owner.dead || !Owner.active || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Vector2 rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(projTex.Width, projTex.Height) : new Vector2(0, projTex.Height);
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
            Vector2 origin = new Vector2(projTex.Width / 2 - 50, projTex.Height / 2 + 50);
            SB.Draw(projTex, drawPos, null, Color.White, drawRotation, rotationPoint, Projectile.scale * 1.4f, flipSprite, 0);
            Texture2D slash = HJScarletTexture.Texture_Swirl.Value;
            rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(slash.Width, slash.Height) : new Vector2(0, slash.Height);
            SB.EnterShaderArea();

            SB.EndShaderArea();
            return false;
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkRed* 0.9f, 0.95f);
            DrawSlash(texture, Color.Crimson* 0.6f, 0.7f);
            DrawSlash(texture, Color.Crimson* 0.4f, 0.3f);
            DrawSlash(texture, Color.Crimson * 0.3f, 0.2f);

            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Owner.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Owner.Center - Main.screenPosition;
                Vertexlist.Add(new ScarletVertex(DrawPos_Head, drawcolor * 1, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new ScarletVertex(DrawPos_Source, drawcolor * 1, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
        public void DrawSlash2(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * 0.5f + Owner.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult * 0.5f + Owner.Center - Main.screenPosition;
                Vertexlist.Add(new ScarletVertex(DrawPos_Head, drawcolor * 1, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new ScarletVertex(DrawPos_Source, drawcolor * 1, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
