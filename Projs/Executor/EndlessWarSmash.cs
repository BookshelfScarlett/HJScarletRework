using ContinentOfJourney.Tiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using System;
using System.Collections.Generic;
using System.CommandLine.Help;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    internal class EndlessWarSmasher : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public AnimationStruct Helper = new AnimationStruct(6);
        public float SwordLength = 60;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Width = 1f;
        public float Height = 1f;
        public float SwordScale = 1.40f;
        public List<Vector2> OldAimPos = [];
        /// <summary>
        /// SwingTime == 0，圆
        /// <br>SwingTime == 1，椭圆从</br>
        /// <br>SwingTime == 2，圆</br>
        /// <br>超出第二次后，使其重新死亡变为仆从模式</br>
        /// </summary>
        public int SwingTime = 0;
        /// <summary>
        /// 挥砍总共的次数，三轮。
        /// </summary>
        public int TotalSwingTime = 3;

        //一些Get和Set
        public bool FinalSwing => SwingTime == 2;
        public bool SecondSwing => SwingTime == 1;
        public bool FirstSwing => SwingTime == 0;
        public float SlashOpacity = 1f;
        public int StopTimer = 0;

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.SetUpHeldProj(15);
            Projectile.penetrate = -1;
        }
        public Vector2 ProjCenterLerp = Vector2.Zero;
        public override void OnFirstFrame()
        {
            TargetRotation = BeginTargetRotation;
            if (SwingTime < 3)
            {
                Helper.MaxProgress[0] = 0 * Projectile.MaxUpdates;
                Helper.MaxProgress[1] = 10 * Projectile.MaxUpdates;
                Helper.MaxProgress[2] = 25 * Projectile.MaxUpdates;
                Helper.MaxProgress[3] = 0 * Projectile.MaxUpdates;
                Width = 1f;
            }
            else
            {
                if (SwingTime == 3)
                    ScarletSound(HJScarletSounds.TheSevenStar_Charge, Projectile.Center);
                Helper.MaxProgress[0] = 0 * Projectile.MaxUpdates;
                Helper.MaxProgress[1] = 8 * Projectile.MaxUpdates;
                Helper.MaxProgress[2] = 20 * Projectile.MaxUpdates;
                Helper.MaxProgress[3] = 0 * Projectile.MaxUpdates;
                SwordScale = 1.60f;
                Width = 1f;
            }
        }

        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAttackAnimation();
            UpdatePlayerStatement();
        }

        public void UpdateAttackAnimation()
        {
            if (StopTimer > 0)
            {
                StopTimer--;
                return;
            }
            UpdateFirstSwingTotalAnimation();
        }
        #region 第一挥砍动画：大半圆
        public void UpdateFirstSwingTotalAnimation()
        {
            if (!Helper.IsDone[1])
            {
                Helper.UpdateAniState(1);
                UpdateFirstSwingSecondAnimation();
            }
            else if (!Helper.IsDone[2])
            {
                if (Helper.OnAnimationBegin(2))
                {
                    ScarletSound(HJScarletSounds.EndlessWar_SmashSwing, Projectile.Center,volume:.65f,instances:0,pitch:-.24f,pitchVariance:.2f);
                }
                Helper.UpdateAniState(2);
                UpdateFirstSwingThirdAnimation();
            }
            else
            { 
                Projectile.Kill();
            }
        }
        public int slashWidth = 220;
        public void UpdateFirstWingFirstAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(0));
            float beginAngle = 195f * Flip.ToDirectionInt();
            float endAngle = 201 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) *SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        private void UpdateFirstSwingSecondAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(1));
            float beginAngle = 175f * Flip.ToDirectionInt();
            float endAngle = 180 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        private void UpdateFirstSwingThirdAnimation()
        {
            float heldScale = 1;
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(2));
            float beginAngle = 180f * Flip.ToDirectionInt();
            float endAngle = -175f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if ((easedProgress) < Main.rand.NextFloat(.8f, 1f))
                GeneralParticleInsert(tarPos);
        }
        private void UpdateFirstSwingFinalAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(3));
            float beginAngle = -145f * Flip.ToDirectionInt();
            float endAngle = -155 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) *SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }


        #endregion
        #region 第二挥砍动画，第二个大半圆
        public void UpdateSecondSwingTotalAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateSecondSwingFirstAnimation();
                Helper.UpdateAniState(0, 1 * Projectile.MaxUpdates);
            }
            else if (!Helper.IsDone[1])
            {
                if (Helper.OnAnimationBegin(1))
                {
                    ScarletSound(HJScarletSounds.EndlessWar_SmashReady, Projectile.Center, pitch: .1f);
                    Vector2 offset = Projectile.rotation.ToRotationVector2() * 480f;
                    Vector2 mountedPos = Projectile.Center + offset;
                    float centerGlowScale = .92f;
                    ECSParticle.CrossGlow(mountedPos, Color.Purple, 25, 1, centerGlowScale);
                    ECSParticle.CrossGlow(mountedPos, Color.Violet, 25, 1, centerGlowScale * .98f);
                    ECSParticle.CrossGlow(mountedPos, Color.White, 25, 1, centerGlowScale * .96f);

                    for (int i = 0; i < 6; i++)
                    {
                        Color color = RandLerpColor(Color.Violet, Color.Purple);
                        new NoiseShockRing(mountedPos, Vector2.Zero, color, 25, 1f, .1f + i * 0.32f, Projectile.whoAmI, offset, false).Spawn();
                    }
                    for (int i = 0; i < 180; i++)
                        ECSParticle.TurbulenceShinyOrb(mountedPos.ToRandCirclePos(160), Main.rand.NextFloat(1.2f, 2.4f) * 1.4f, RandLerpColor(Color.Purple, Color.Violet), 60, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
                }
                //逐渐变化为椭圆
                UpdateSecondSwingSecondAnimation();
                Helper.UpdateAniState(1);
            }
            else if (!Helper.IsDone[2])
            {
                if (Helper.OnAnimationBegin(2))
                {
                    ScarletSound(HJScarletSounds.EndlessWar_SmashSwing, Projectile.Center, pitch: .1f);
                }
                UpdateSecondSwingThirdAnimation();
                Helper.UpdateAniState(2, 0 * Projectile.MaxUpdates);
                if (OldAimPos.Count > 100)
                    OldAimPos.RemoveAt(0);
            }
            else if (!Helper.IsDone[3])
            {
                Helper.UpdateAniState(3);
                UpdateSecondSwingFinalAnimation();
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);
                SlashOpacity = Lerp(SlashOpacity, 0, 0.1f / Projectile.MaxUpdates);
                if (SlashOpacity <= .1f)
                    SlashOpacity = 0;

            }
            else
            {
                Projectile.Kill();
            }
        }
        public void UpdateSecondSwingFirstAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseInOutExpo(Helper.GetAniProgress(0));
            float beginAngle = -175f * Flip.ToDirectionInt();
            float endAngle = -175 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) *SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        private void UpdateSecondSwingSecondAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            float beginAngle = -175f * Flip.ToDirectionInt();
            float endAngle = -185 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        private void UpdateSecondSwingThirdAnimation()
        {
            float heldScale = 1;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(2));
            float beginAngle =-185f * Flip.ToDirectionInt();
            float endAngle = 165f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * SwordScale * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 380;
                OldAimPos.Add(slashPosFinal);
                if (easedProgress <= .02f)
                    return;
                GeneralParticleInsert(tarPos);
                
            }
        }
        private void UpdateSecondSwingFinalAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(3));
            float beginAngle = 165f * Flip.ToDirectionInt();
            float endAngle = 185 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) *SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }


        #endregion
        #region 第三挥砍动画，一个完整圆
        public void UpdateThirdSwingTotalAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateThirdSwingFirstAnimation();
                Helper.UpdateAniState(0, 1 * Projectile.MaxUpdates);
            }
            else if (!Helper.IsDone[1])
            {
                if (Helper.OnAnimationBegin(1))
                {
                    ScarletSound(HJScarletSounds.EndlessWar_SmashReady, Projectile.Center,pitch:.2f);
                    Vector2 offset = Projectile.rotation.ToRotationVector2() * 480f;
                    Vector2 mountedPos = Projectile.Center + offset;
                    float centerGlowScale = .92f;
                    ECSParticle.CrossGlow(mountedPos, Color.Purple, 35, 1, centerGlowScale);
                    ECSParticle.CrossGlow(mountedPos, Color.Violet, 35, 1, centerGlowScale * .98f);
                    ECSParticle.CrossGlow(mountedPos, Color.White, 35, 1, centerGlowScale * .96f);

                    for (int i = 0; i < 6; i++)
                    {
                        Color color = RandLerpColor(Color.Violet, Color.Purple);
                        new NoiseShockRing(mountedPos, Vector2.Zero, color, 35, 1f, .1f + i * 0.32f, Projectile.whoAmI, offset, false).Spawn();
                    }
                    for (int i = 0; i < 180; i++)
                        ECSParticle.TurbulenceShinyOrb(mountedPos.ToRandCirclePos(160), Main.rand.NextFloat(1.2f, 2.4f) * 1.4f, RandLerpColor(Color.Purple, Color.Violet), 80, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
                }
                UpdateThirdSwingSecondAnimation();
                Helper.UpdateAniState(1);
            }
            else if (!Helper.IsDone[2])
            {
                if (Helper.OnAnimationBegin(2))
                {
                    ScarletSound(HJScarletSounds.EndlessWar_SmashSwing, Projectile.Center,pitch:.2f);
                }
                UpdateThirdSwingThirdAnimation();
                Helper.UpdateAniState(2,0*Projectile.MaxUpdates);
                if (OldAimPos.Count > 100)
                    OldAimPos.RemoveAt(0);

            }
            else if (!Helper.IsDone[3])
            {
                Helper.UpdateAniState(3);
                UpdateThirdSwingFinalAnimation();
                if(OldAimPos.Count>0)
                    OldAimPos.RemoveAt(0);
                SlashOpacity = Lerp(SlashOpacity, 0, 0.1f / Projectile.MaxUpdates);
                if (SlashOpacity <= .1f)
                    SlashOpacity = 0;

            }
            else
            {
                Projectile.Kill();
            }
        }

        public void UpdateThirdSwingFirstAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseInOutExpo(Helper.GetAniProgress(0));
            float beginAngle = 185f * Flip.ToDirectionInt();
            float endAngle = 185 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) *SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        private void UpdateThirdSwingSecondAnimation()
        {
            float heldScale = 1f;
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            float beginAngle = 185f * Flip.ToDirectionInt();
            float endAngle = 205 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        private void UpdateThirdSwingThirdAnimation()
        {
            float heldScale = 1;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(2));
            float beginAngle =205f * Flip.ToDirectionInt();
            float endAngle = -195f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * SwordScale * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 380;
                OldAimPos.Add(slashPosFinal);
                if (easedProgress <= .02f)
                    return;
                GeneralParticleInsert(tarPos);

            }
        }
        public void UpdateThirdSwingFinalAnimation()
        {
            float heldScale = 1;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(3));
            float beginAngle =-195f * Flip.ToDirectionInt();
            float endAngle = -215f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;

        }

        #endregion
        public void GeneralParticleInsert(Vector2 tarPos)
        {
            for (int i = 1; i <= 2; i++)
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * slashWidth, Main.rand.NextFloat(.40f, .90f));
                float scale = .64f * Main.rand.NextFloat(0.8f, 1.3f);
                Vector2 dir = (pos - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Vector2 vel = Owner.velocity * Main.rand.NextFloat(0.1f, 1.5f) + dir * Main.rand.NextFloat(0.1f, 44f);
                ECSParticle.SnowCloud(pos, vel * .05f, RandLerpColor(Color.Lerp(Color.SkyBlue, Color.WhiteSmoke, 0.5f), Color.RoyalBlue), 20, RandRotTwoPi, .10f + 0.050f * i, scale * (0.50f + i * 0.12f));
            }
            if(Main.rand.NextBool())
            for(int i =0;i<2;i++)
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * slashWidth, Main.rand.NextFloat(.61f, .95f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 * Projectile.direction + ToRadians(10))) * Main.rand.NextFloat(12f, 20.5f);
                ECSParticle.TurbulenceShinyOrb(pos+dir*i*1.5f, 1.4f, RandLerpColor(Color.White, Color.SkyBlue), 60, 1, 0.11f, glowMult: .4f);
                //ECSParticle.ShinyCrossStarSmall(pos + dir * i * 1.5f, Vector2.Zero, RandLerpColor(Color.White, Color.SkyBlue), 60, 1, 0.34f, 0.1f);
            }
            if(Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * slashWidth, Main.rand.NextFloat(.75f, .9f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(5f, 9f);
                ECSParticle.CrossGlow(pos, vel*.01f, RandLerpColor(Color.White, Color.SkyBlue), 45,1,0, Projectile.scale * Main.rand.NextFloat(.8f, 1.1f) * .12f,.2f);
                //ECSParticle.HighResolutionThunder(pos, vel * 0f, RandLerpColor(Color.White, Color.RoyalBlue), 40, 1f, vel.ToRotation(), Projectile.scale * Main.rand.NextFloat(.8f, 1.1f) * .1f, 2);
            }
        }
        public void UpdatePlayerStatement()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            //Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ControlPlayerArm(Projectile.rotation);

            //这里只更新锤子的死亡状态，不要占用其他功能
            Projectile.Center = Owner.MountedCenter;
            Owner.itemAnimation = Owner.itemTime = 2;
            if (Owner.dead)
                Projectile.Kill();
            else
            {
                //全过程锁定生命值。我们会在需要的时候手动处理
                Projectile.timeLeft = 2;
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(2));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 200;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override void OnKill(int timeLeft)
        {
            if(Main.mouseLeft)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((EndlessWarSmasher)proj.ModProjectile).BeginTargetRotation = TargetRotation; 
                ((EndlessWarSmasher)proj.ModProjectile).Flip = !Flip; 
                ((EndlessWarSmasher)proj.ModProjectile).SwingTime= SwingTime+1; 
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ScarletSound(HJScarletSounds.Smash_GroundHeavy, target.Center,pitch:.25f,pitchVariance:.2f);
            if (Projectile.numHits < 1)
            {
                if (SwingTime > 3)
                {
                    StopTimer = 6 * Projectile.MaxUpdates;
                    ScreenShakeSystem.AddScreenShakes(target.Center, 60f, 40, Projectile.rotation, 0, easingFunc: EaseOutBack);
                    //ScreenDarknessSystem.AddScreenDarkness(0.95f, 2,0, 32,easeIn:EaseOutCubic,easeOut:EaseInCubic);
                }
                else
                {
                    StopTimer = 4 * Projectile.MaxUpdates;
                    ScreenShakeSystem.AddScreenShakes(target.Center, 40f, 40, Projectile.rotation, 0, easingFunc: EaseOutBack);
                    //ScreenDarknessSystem.AddScreenDarkness(0.45f, 2,2,16,easeOut:EaseInCubic);
                }
            }
            for(int i =0;i<4;i++)
            {
                Vector2 baseDir= Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 *Flip.ToDirectionInt());
                for (int j = 0; j < 32; j++)
                {
                    Vector2 dir = baseDir.ToRandVelocity(ToRadians(36f),.51f,38f);
                    Vector2 pos = target.Center.ToRandCirclePosEdge(12);
                    ECSParticle.HRShinyOrb(pos, dir, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 50, 1, 0.1f, 0.5f);
                }
                for (int j = 0; j < 32; j++)
                {
                    Vector2 dir = baseDir.ToRandVelocity(ToRadians(36f),.51f,38f);
                    Vector2 pos = target.Center.ToRandCirclePosEdge(12);
                    ECSParticle.SmokeParticle(pos, dir, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, RandRotTwoPi, 1, 0.3f, blendstate: BlendState.AlphaBlend);
                }
                for (int j = 0; j < 32; j++)
                {
                    Vector2 dir = baseDir.ToRandVelocity(ToRadians(36f),.5f,38f);
                    Vector2 pos = target.Center.ToRandCirclePosEdge(12);
                    ECSParticle.ShinyCrossStarSmall(pos, dir, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1, 0.4f, 0.15f);
                }

            }
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存

        public float HammerScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects spriteEffects);
            FirstSwingDraw(tex, drawPosition, drawRotation, rotationPoint, spriteEffects);
            return false;
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        //public object UpdateThirdSwingFinalAnimation { get; private set; }

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
        }

        public void FirstSwingDraw(Texture2D tex, Vector2 drawPosition, float drawRotation, Vector2 rotationPoint, SpriteEffects spriteEffects)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            Vector2 offset = new Vector2(60, 0).RotatedBy(Projectile.rotation);
            if(SwingTime>3)
                for(int i =0;i<8;i++)
            SB.Draw(tex, drawPosition + (TwoPi/8f*i).ToRotationVector2() * 2f + offset, null, Color.White.ToAddColor(), drawRotation, rotationPoint, Projectile.scale * HammerScale, spriteEffects, 0);
            SB.Draw(tex, drawPosition + offset, null, Color.White, drawRotation, rotationPoint, Projectile.scale * HammerScale, spriteEffects, 0);
        }
        public void SecondSwingDraw(Texture2D tex, Vector2 drawPosition, float drawRotation, Vector2 rotationPoint, SpriteEffects spriteEffects)
        {
            Vector2 offset = new Vector2(200, 0).RotatedBy(Projectile.rotation);
            SB.Draw(tex, drawPosition + offset, null, Color.White, drawRotation, rotationPoint, Projectile.scale * HammerScale, spriteEffects, 0);
            SB.EnterShaderArea();
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            //绘制主要底图
            HJScarletMethods.ApplyAlphaCut(new Vector4(.1f, .21f, 0, 0.31f), new Vector2(-Main.GlobalTimeWrappedHourly * .4f, 0f), new Vector2(1));
            DrawSlash(texture, Color.SkyBlue* 0.6f, 0.6f);
            DrawSlash(texture, Color.DeepSkyBlue* 0.40f, .40f);
            HJScarletMethods.ApplyAlphaCut(new Vector4(.2f, .21f, .02f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * .2f, 0f), new Vector2(1));
            DrawSlash(texture, Color.LightSkyBlue* 0.50f, 0.7f);
            DrawSlash(texture, Color.AliceBlue * 0.95f, 0.94f);
            //绘制大面积的，小型的流光，渲染一层
            HJScarletMethods.ApplyAlphaCut(new Vector4(.13f, .2f, 0f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * .25f, 0f), new Vector2(.31f, 1f), Color.SkyBlue);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.LightSkyBlue* .95f, 0.8f);
            DrawSlash(texture2, Color.White* .95f, 0.7f);
            //绘制复杂的，多层的流光。渲染两层
            HJScarletMethods.ApplyAlphaCut(new Vector4(.23f, .2f, 0f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * .15f, 0f), new Vector2(0.2f, 0.75f), Color.SkyBlue);
            texture2 = HJScarletTexture.Noise_Smoke.Value;
            DrawSlash(texture2, Color.SkyBlue* .95f, 0.9f);
            DrawSlash(texture2, Color.White* .5f, 0.7f);

            SB.EndShaderArea();

        }

        public void FinalSwingDraw(Texture2D tex, Vector2 drawPosition, float drawRotation, Vector2 rotationPoint, SpriteEffects spriteEffects)
        {
            float progress = EaseInCubic(Helper.GetAniProgress(3));
            //HJScarletMethods.ApplyMeltShader(tex, Color.RoyalBlue, (progress));
            Vector2 offset = new Vector2(200, 0).RotatedBy(Projectile.rotation);
            SB.Draw(tex, drawPosition + offset, null, Color.White * (1 - progress), drawRotation, rotationPoint, Projectile.scale * HammerScale, spriteEffects, 0);
            SB.EnterShaderArea();
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            //绘制主要底图
            HJScarletMethods.ApplyAlphaCut(new Vector4(.1f, .21f, 0, 0.31f), new Vector2(-Main.GlobalTimeWrappedHourly * .4f, 0f), new Vector2(1));
            DrawSlash(texture, Color.SkyBlue* 0.6f, 0.6f);
            DrawSlash(texture, Color.DeepSkyBlue* 0.40f, .40f);
            HJScarletMethods.ApplyAlphaCut(new Vector4(.2f, .21f, .02f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * .2f, 0f), new Vector2(1));
            DrawSlash(texture, Color.LightSkyBlue* 0.50f, 0.7f);
            DrawSlash(texture, Color.AliceBlue * 0.95f, 0.94f);
            //绘制大面积的，小型的流光，渲染一层
            HJScarletMethods.ApplyAlphaCut(new Vector4(.13f, .2f, 0f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * .25f, 0f), new Vector2(.31f, 1f), Color.SkyBlue);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.LightSkyBlue* .95f, 0.8f);
            DrawSlash(texture2, Color.White* .95f, 0.7f);
            //绘制复杂的，多层的流光。渲染两层
            HJScarletMethods.ApplyAlphaCut(new Vector4(.23f, .2f, 0f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * .15f, 0f), new Vector2(0.2f, 0.75f), Color.SkyBlue);
            texture2 = HJScarletTexture.Noise_Smoke.Value;
            DrawSlash(texture2, Color.SkyBlue* .95f, 0.9f);
            DrawSlash(texture2, Color.White* .5f, 0.7f);


            SB.EndShaderArea();

        }

        public void EndDraw(Texture2D tex, Vector2 drawPosition, float drawRotation, Vector2 rotationPoint, SpriteEffects spriteEffects)
        {
        }
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] *beginMult + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor*SlashOpacity, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor*SlashOpacity, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }

    }
}
