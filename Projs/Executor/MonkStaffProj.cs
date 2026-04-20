using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class MonkStaffProj : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        private float TargetRotation = 0;
        private Vector2 HeldPos;
        private Vector2 InitVector;
        public AnimationStruct Helper = new(3);
        public bool ProjType
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public float OpacityGlow = 0f;
        public float ScaleGlow = 0f;
        public Vector2 GlowPosCenter = Vector2.Zero;
        public float GlowPosOffset => ProjType ? 70f : 120f;
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 40;
            Helper.MaxProgress[1] = 50;
            Helper.MaxProgress[2] = 15;
            InitVector = Owner.direction  > 0 ? PiOver4.ToRotationVector2() : (PiOver4 + PiOver2).ToRotationVector2();
            ProjType = Main.rand.NextBool();
            TargetRotation = InitVector.ToRotation();
            Projectile.rotation = TargetRotation;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void ProjAI()
        {
            if (CheckOwnerDead())
            {
                Projectile.Kill();
                return;
            }
            UpdateProj();
            UpdatePlayerState();

        }

        public void UpdateProj()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                UpdateBeginAni();
            }
            else if (!Helper.IsDone[1])
            {
                if (Helper.GetAniProgress(1) == 0)
                {
                    SetUpInitParticle();
                }
                Helper.UpdateAniState(1);
                UpdateMidAni();

            }
            else if (!Helper.IsDone[2])
            {
                if(Helper.GetAniProgress(2) == 0)
                {
                    Rectangle hitbox = Utils.CenteredRectangle(Owner.Center + Vector2.UnitY * 20f, new Vector2(5, 5));
                    CombatText.NewText(hitbox, Color.Lime, $"治疗总量：{healAmt}", true);
                }
                Helper.UpdateAniState(2);
                UpdateEndAni();
            }
            else
                Projectile.Kill();
        }

        public void SetUpInitParticle()
        {
            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset;
            for (int i = 0; i < 40; i++)
            {
                new ShinyCrossStar(mountedPos, RandVelTwoPi(0.8f,9.7f), RandLerpColor(Color.DarkGreen, Color.Green), 40, 0, 1, 0.75f, false).Spawn();
            }
            for (int i = 0; i < 32; i++)
            {
                new StarShape(mountedPos.ToRandCirclePosEdge(4f), RandVelTwoPi(1.2f, 9.6f), Color.Lime, 0.84f, 60).Spawn();
            }
            for (int i = 0; i < 18; i++)
            {
                new SmokeParticle(mountedPos.ToRandCirclePosEdge(20f), RandVelTwoPi(5.2f, 6.4f), RandLerpColor(Color.LawnGreen, Color.LimeGreen), 60, RandRotTwoPi, 1f, 0.37f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            }
            new CrossGlow(mountedPos, Color.DarkGreen, 40, 1, 0.38f).Spawn();
            new CrossGlow(mountedPos, Color.Green, 40, 1, 0.35f).Spawn();
            new CrossGlow(mountedPos, Color.White, 40, 1, 0.30f).Spawn();
        }

        public void UpdateEndAni()
        {
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(2));
            OpacityGlow = ScaleGlow = (1 - easedProgress);
            float curRot = Helper.UpdateAngle(220, 230, Owner.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRot) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix) * (1 -easedProgress);
            HeldPos = InitVector.RotatedBy(curRot).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();

        }
        int healAmt = 0;
        public void UpdateHealParticle()
        {
            Vector2 pos = Owner.Center + Vector2.UnitY * (Owner.height * 0.5f);
            int curHeal = Main.rand.Next(1, 4);
            Owner.Heal(curHeal);
            healAmt += curHeal;
            if (Main.rand.NextBool())
            {
                pos.X += Main.rand.NextFloat(-1f, 1.1f) * Owner.width;
                pos.Y -= Main.rand.NextFloat(0f, 1f) * Owner.height;
                new StarShape(pos, -Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.4f), Color.Lime, 0.4f, 40).Spawn();
            }
            if (Main.rand.NextBool())
            {
                pos = Owner.Center + Vector2.UnitY * (Owner.height * 0.5f);
                pos.X += Main.rand.NextFloat(-1f, 1.1f) * Owner.width;
                pos.Y -= Main.rand.NextFloat(0f, 1f) * Owner.height;
                new ShinyCrossStar(pos, -Vector2.UnitY * Main.rand.NextFloat(0.1f, .4f), RandLerpColor(Color.Lime, Color.LimeGreen), 40, 0, 1, 0.4f, false).Spawn();
            }
        }

        public void UpdateMidAni()
        {
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(1));
            float curRot = Helper.UpdateAngle(-140, 220, Owner.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRot) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix);
            HeldPos = InitVector.RotatedBy(curRot).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            OpacityGlow = ScaleGlow = Lerp(OpacityGlow,0f, 0.01f);
            UpdateMidParticle();
            UpdateHealParticle();

        }

        public void UpdateMidParticle()
        {
            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset;
            Vector2 vel = RandVelTwoPi(1.2f, 2.4f);
                new ShinyCrossStar(mountedPos, vel, RandLerpColor(Color.DarkGreen, Color.Green), 40, 0, 1, 0.5f, false).Spawn();
            mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset;
            vel = RandVelTwoPi(1.2f, 3.4f);
            new EmptyRing(mountedPos, vel, RandLerpColor(Color.Lime,Color.DarkGreen), 40, 0.14f, 1f, altRing:Main.rand.NextBool()).Spawn();


        }

        public void UpdateBeginAni()
        {
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            OpacityGlow = ScaleGlow = Lerp(OpacityGlow,1.1f,0.2f);
            float curRot = Helper.UpdateAngle(-110, -140, Owner.direction, easedProgress);
            Matrix matrix = Matrix.CreateRotationZ(curRot) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, matrix);
            HeldPos = InitVector.RotatedBy(curRot).RotatedBy(TargetRotation);
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Projectile.scale = tarPos.Length();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            Vector2 mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset;
            Vector2 pos = mountedPos + RandVelTwoPi(100f, 190f);
            Vector2 vel = (mountedPos - pos).ToSafeNormalize() * Main.rand.NextFloat(2.2f, 8.4f);
            if(Main.rand.NextBool())
            new EmptyRing(pos, vel, Color.Lime, 40, 0.19f, 1f, altRing: true).Spawn();
            mountedPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * GlowPosOffset;
            pos = mountedPos + RandVelTwoPi(100f, 190f);
            vel = (mountedPos - pos).ToSafeNormalize() * Main.rand.NextFloat(2.2f, 8.4f);
            if(Main.rand.NextBool())
            new ShinyCrossStar(pos, vel, RandLerpColor(Color.DarkGreen,Color.Green), 40, 0, 1, 0.5f, false).Spawn();

        }

        public bool CheckOwnerDead()
        {
            if (Owner.dead || !Owner.active || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return true;
            }
            return false;
        }

        private void UpdatePlayerState()
        {
            Projectile.Center = Owner.Center + HeldPos;
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.direction =  Projectile.spriteDirection = Owner.direction;
            Owner.ControlPlayerArm(Projectile.rotation);
            GlowPosCenter = HeldPos;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = ProjType ? TextureAssets.Projectile[ProjectileID.MonkStaffT1].Value : TextureAssets.Projectile[ProjectileID.MonkStaffT3].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 rotationPoint = Projectile.spriteDirection == -1 ? new Vector2(projTex.Width - 20, projTex.Height - 20) : new Vector2(20, projTex.Height - 20);
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
            SB.Draw(projTex, drawPos, null, Color.White, drawRotation, rotationPoint, Projectile.scale * 1.4f, flipSprite, 0);
            Vector2 pos = Projectile.Center  + Projectile.rotation.ToRotationVector2() * (GlowPosOffset* Projectile.scale)- Main.screenPosition;
            SB.EnterShaderArea();
            Texture2D kiraStar = HJScarletTexture.Particle_CrossGlow.Value;
            Texture2D ring = HJScarletTexture.Particle_RingHard.Value;
            //Texture2D orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            float opValue = Projectile.Opacity * OpacityGlow;
            float scaleValue = Projectile.scale * ScaleGlow;
            SB.Draw(kiraStar, pos, null, Color.DarkGreen* opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.55f, 0, 0);
            SB.Draw(kiraStar, pos, null, Color.Green* opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.35f, 0, 0);
            SB.Draw(kiraStar, pos, null, Color.GreenYellow* opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.20f, 0, 0);
            SB.EndShaderArea();


            return false;
        }
    }
}
