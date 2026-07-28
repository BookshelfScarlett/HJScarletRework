using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheHeldProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override int OriginalItemID => ItemType<CrimsonScythe>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1.25f;
        public float Width = 1.25f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;
        public List<Vector2> OldAimPos = [];
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }

        public override void ExSD()
        {
            Projectile.SetUpHeldProj(10);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

        }
        public override void OnFirstFrame()
        {
            ThirdSwing = SwingTime > 2;
            if (ThirdSwing)
            {
                ScarletSound(HJScarletSounds.Tlipoca_Swing, Projectile.Center, 0.75f, 1, 0.14f, 0.1f, 1);
                Helper.MaxProgress[0] = (int)(AttackSpeed * .65f);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .3f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * .95f);
            }
            else
            {
                ScarletSound(HJScarletSounds.Tlipoca_Swing, Projectile.Center, 0.75f, 1, 0.1f + 0.14f * SwingTime, 0.1f, 2);
                Helper.MaxProgress[0] = (int)(AttackSpeed * .65f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * .95f);
                Width = Height *= Lerp(1f, 1.12f, SwingTime / 2f);
            }
            BeginTargetRotation = Owner.Center.ToMouseVector2().ToRotation();
            TargetRotation = BeginTargetRotation;
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            if (OldAimPos.Count > 5 * Projectile.MaxUpdates)
                OldAimPos.RemoveAt(0);
        }
        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ControlPlayerArm(Projectile.rotation);

        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            if (Helper.Progress[2] <= 0)
            {
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;
            }
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }

        public void UpdateAnimation()
        {
            if(StopTiming>0)
            {
                StopTiming--;
                return;
            }
            if (!ThirdSwing)
            {
                UpdateHalfCircleSwingAnimation();
            }
            else
            {
                UpdateFullCircleSwingAnimation();
            }
        }
        #region 全向的第三挥砍
        public void UpdateFullCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdtaeFullCircleBegin();
            }
            else if (!Helper.IsDone[1])
            {
                UpdtaeFullCircleEnd();
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);
            }
            else
            {
                SwingTime = -1;
                Projectile.Kill();
            }

        }
        public void UpdtaeFullCircleEnd()
        {
            Helper.UpdateAniState(1);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(1));
            float beginAngle = 415f * Flip.ToDirectionInt();
            float endAngle = 420 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width + .24f, Height + .24f, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.65f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        public void UpdtaeFullCircleBegin()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -210f * Flip.ToDirectionInt();
            float endAngle = 415f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width + .24f, Height + .24f, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.65f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width + .24f, Height + .24f, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.65f *heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120;
                OldAimPos.Add(slashPosFinal);
                    

                if (easedProgress < 0.96f)
                {
                    float lerpVal = Lerp(1f, 1.8f, (SwingTime / 3f));
                    int total = ThirdSwing ? 8 : (int)(4 * lerpVal);
                    total = (int)(total* heldScale);
                    for (int i = 0; i < total+1; i++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.71f, 0.95f));
                        Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                        Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(1.2f, 6.5f);
                        BloodyMetaball.SpawnParticle(pos + Projectile.Center.GetNormalVector2(pos).RotatedBy(PiOver2*Projectile.spriteDirection) * i * 7.7f, vel, 0.14f, dir.ToRotation() + (PiOver2), true);
                    }

                    if (Main.rand.NextBool(1))
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 115, Main.rand.NextFloat(.31f, .8f));
                        Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                        Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                        ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 0.45f, 0.15f, BlendState.Additive);
                    }

                }
            }
        }
        #endregion
        #region 起手两挥砍
        public void UpdateHalfCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();

            }
            //else if (!Helper.IsDone[1])
            //{
            //    UpdateEndAnimation();
            //}
            else if (!Helper.IsDone[2] && !Main.mouseLeft)
            {
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);

                if (Main.mouseLeft || Owner.HeldItem.type != OriginalItemID)
                {
                    Projectile.Kill();
                }
                UpdateFinalAnimation();
            }
            else
                Projectile.Kill();

        }
        public void UpdateBeginAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(0));
            float beginAngle = -195f * Flip.ToDirectionInt();
            float endAngle = 185f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.5f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120;
                OldAimPos.Add(slashPosFinal);
                if (easedProgress < 0.96f)
                {
                    float lerpVal = Lerp(1f, 1.8f, (SwingTime / 3f));
                    int total = ThirdSwing ? 8 : (int)(4 * lerpVal);
                    total = (int)(total* heldScale);
                    for (int i = 0; i < total; i++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.71f, 0.95f));
                        Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                        Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(1.2f, 6.5f);
                        BloodyMetaball.SpawnParticle(pos + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2*Projectile.spriteDirection) * i * 7.7f, vel, 0.14f * Main.rand.NextFloat(.95f,1.13f), dir.ToRotation() + (PiOver2), true);
                    }
                    if (Main.rand.NextBool(6))
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 115, Main.rand.NextFloat(.71f, .95f));
                        Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                        Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                        ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 0.45f, 0.15f, BlendState.Additive);
                    }

                }
            }
        }
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(1);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            float beginAngle = 185f * Flip.ToDirectionInt();
            float endAngle = 195 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }
        /// <summary>
        /// 收尾动画，在开始收尾的时候这里就不会占用玩家的itemTime了
        /// <br>在这个动画下按下左键会强制进行下一次的攻击</br>
        /// </summary>
        public void UpdateFinalAnimation()
        {
            Helper.UpdateAniState(2);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(2));
            float beginAngle = 195f * Flip.ToDirectionInt();
            float endAngle = 185f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .015f);
        }
        #endregion

        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((CrimsonScytheHeldProj)proj.ModProjectile).Flip = !Flip;
                ((CrimsonScytheHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
            }
            else if (!ThirdSwing)
            {
                ScarletSound(HJScarletSounds.Misc_ManaClearUse, Owner.Center, 0.55f, 1, -0.84f, 0.2f);
                for (int i = 0; i < 92; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.78f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    BloodyMetaball.SpawnParticle(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 60, RandVelTwoPi(1.4f, 2.1f), 0.145f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    ECSParticle.SnowCloud(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 75, RandVelTwoPi(0.7f, 1.2f), Color.Red, 45, 1, 0.45f, 0.2f);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    BloodyMetaball.SpawnParticle(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 15, dir * Main.rand.NextFloat(1.4f, 2.1f), 0.945f, dir.ToRotation(), false);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(1.41f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2);
                    BloodyMetaball.SpawnParticle(pos + dir * Main.rand.NextFloat(-42 * (-Flip.ToDirectionInt() * Owner.direction), 1.1f) * 1.5f, dir * Main.rand.NextFloat(1.4f, 2.1f), 0.945f, dir.ToRotation(), false);

                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
            {
                float pitch = ThirdSwing ? 0f : -0.4f + SwingTime * .2f;
                int t = ThirdSwing ? 2 : 1;
                ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, instances: 0, pitch: pitch, variantType: t);
                if (!ThirdSwing)
                    ScreenShakeSystem.AddScreenShakes(target.Center, 10, 25, Projectile.rotation + PiOver2 * Projectile.spriteDirection, 0, easingFunc: EaseOutExpo);
                else
                    ScreenShakeSystem.AddScreenShakes(target.Center, 20, 30, Projectile.rotation + PiOver2 * Projectile.spriteDirection, 0, easingFunc: EaseOutExpo);

                StopTiming = 35;
            }
            if(Projectile.numHits < 12)
                HitSparkle(target, hit, damageDone);
        }

        public void HitSparkle(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int reverse = Projectile.spriteDirection;
            Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * reverse);
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(target.Center, dir.ToRandVelocity(ToRadians(35), 1.2f, 22.5f), RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .52f, blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Red, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .28f, Main.rand.NextBool(), blendstate: BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.LiliesFire(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Black, Color.Red), 40, RandRotTwoPi, 1, 0.2f * Main.rand.NextFloat(0.45f, 1.3f));
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.DarkRed, Color.Red), 40, 1, 0.92f * Main.rand.NextFloat(0.95f, 1.3f), blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(25), 1.9f, 19f);
                BloodyMetaball.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.392f, vel.ToRotation(), true);
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(15), 12f, 28f);
                BloodyMetaball.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.92f, vel.ToRotation());
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 128;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        /// <summary>
        /// ToDo:当务之急为给这个刀光处理一些黑色底图增强白天效果
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.AlphaBlend);
            Texture2D texture = HJScarletTexture.Metaball_Bloody.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.White * 0.90f, 0.95f);
            DrawSlash(texture, Color.White * 0.60f, 0.55f);
            DrawSlash(texture, Color.White * 0.40f, 0.40f);

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            texture = HJScarletTexture.Metaball_Bloody.Value;
            effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.IndianRed * 0.90f, 0.95f);
            DrawSlash(texture, Color.DarkRed * 0.60f, 0.55f);
            DrawSlash(texture, Color.Red * 0.40f, 0.40f);

            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(0.42f);
            effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect2.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * .935f, 0));
            effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
            effect2.Parameters["OverlayColor"].SetValue(Color.Red.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.Red * .95f, 0.90f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.DarkRed * .80f, 0.55f);
            DrawSlash(texture, Color.Crimson * 0.40f, 0.40f);

            texture = HJScarletTexture.Texture_SwordSlash.Value;
            effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.41f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.Red * 0.95f, 0.95f);
            DrawSlash(texture, Color.DarkRed * 0.60f, 0.50f);
            DrawSlash(texture, Color.Crimson * 0.40f, 0.40f);
            DrawSlash(texture, Color.Red * 0.40f, 0.40f);

            HJScarletMethods.EndShaderAreaPixel();


        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            if (ThirdSwing)
            {
                Color c = Color.White;
                float endPro = EaseInCubic(Helper.GetAniProgress(1));
                float endPro2 = EaseInBack(Helper.GetAniProgress(1));
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, Color.Red.ToAddColor() * (1 - endPro2), drawRotation, rotationPoint, Projectile.scale * (1 - endPro), flipSprite, 0);
                SB.Draw(tex, drawPosition, null, c * (1 - endPro2), drawRotation, rotationPoint, Projectile.scale * (1 - endPro), flipSprite, 0);
                SB.EnterShaderArea();
                Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
                Vector2 pos = drawPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * 95f * Projectile.scale * (1 - endPro);
                float glowScale = Projectile.scale * .15f * (1 - endPro);
                SB.Draw(glow, pos, null, Color.DarkRed, drawRotation, glow.Size() / 2, glowScale, flipSprite, 0);
                SB.Draw(glow, pos, null, Color.Red, drawRotation, glow.Size() / 2, glowScale * .95f, flipSprite, 0);
                SB.Draw(glow, pos, null, Color.White, drawRotation, glow.Size() / 2, glowScale * .92f, flipSprite, 0);
                SB.EndShaderArea();
            }
            else
            {
                float time = SwingTime / 3f;
                Color c = Color.Lerp(Color.White, Color.Black, Helper.GetAniProgress(2));
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f * time, null, Color.Red.ToAddColor(), drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
                SB.Draw(tex, drawPosition, null, c, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            }
            return false;
        }
    }
}
