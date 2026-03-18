using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheHeldProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<CrimsonScythe>().Texture;
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
        public AnimationStruct ScytheAnimation = new(3);
        public SwingState SwingType
        {
            get => (SwingState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
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
            Projectile.localNPCHitCooldown = 3;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            ScytheAnimation.MaxProgress[AniState.Begin] = 25;
            ScytheAnimation.MaxProgress[AniState.Mid] = 15;
            ScytheAnimation.MaxProgress[AniState.End] = 10;
            Projectile.direction = Projectile.spriteDirection = ((SwingType != SwingState.RightHalf).ToDirectionInt());
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
        
        private float SwingProgress;
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
            float easedProgress = EaseInOutExpo(ScytheAnimation.GetAniProgress(AniID.Begin));
            float curRotation = ScytheAnimation.UpdateAngle(-125, -115, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }

        private void FullCircleAniHandlerMid()
        {
            float easedProgress = EaseInOutExpo(ScytheAnimation.GetAniProgress(AniID.Mid));
            float curRotation = ScytheAnimation.UpdateAngle(-115, 260, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }

        private void FullCircleAniHandlerEnd()
        {
            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.End));
            float curRotation = ScytheAnimation.UpdateAngle(260, 270, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

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
            float easedProgress = EaseInOutExpo(ScytheAnimation.GetAniProgress(AniID.Begin));
            float curRotation = ScytheAnimation.UpdateAngle(-110, -125, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }

        private void RightHalftAniHandlerMid()
        {
            float easedProgress = EaseInOutExpo(ScytheAnimation.GetAniProgress(AniID.Mid));
            float curRotation = ScytheAnimation.UpdateAngle(-125, 210, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }
        private void RightHalftAniHandlerEnd()
        {
            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.End));
            float curRotation = ScytheAnimation.UpdateAngle(210, 220, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

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
            float easedProgress = EaseInOutExpo(ScytheAnimation.GetAniProgress(AniID.Begin));
            float curRotation = ScytheAnimation.UpdateAngle(-110, -125, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
        }

        private void LeftHalftAniHandlerMid()
        {
            float easedProgress = EaseInOutExpo(ScytheAnimation.GetAniProgress(AniID.Mid));
            float curRotation = ScytheAnimation.UpdateAngle(-125, 170, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
        }

        private void LeftHalftAniHandlerEnd()
        {
            float easedProgress = EaseInCubic(ScytheAnimation.GetAniProgress(AniID.End));
            float curRotation = ScytheAnimation.UpdateAngle(170, 180, Projectile.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRotation) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * 1.2f;
            HeldPos = InitVector.RotatedBy(curRotation).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
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
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Vector2 rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(projTex.Width, projTex.Height) : new Vector2(0, projTex.Height);
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
            Vector2 origin = new Vector2(projTex.Width / 2 - 50, projTex.Height / 2 + 50);
            SB.Draw(projTex, drawPos, null, Color.White, drawRotation, rotationPoint, Projectile.scale * 1.4f, flipSprite, 0);
            return false;
        }
    }
}
