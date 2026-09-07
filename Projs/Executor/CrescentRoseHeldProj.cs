using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrescentRoseHeldProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<CrescentRose>();
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
            ScarletProjIDSets.IsHeldProj[Type] = true;
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
                Projectile.SetupImmnuity(45);
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
            base.ProjAI();
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
            if (StopTiming > 0)
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
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.65f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120;
                OldAimPos.Add(slashPosFinal);

                if (easedProgress >= 0.98f)
                    return;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.991f, 1.01f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    Vector2 posOff = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * Projectile.spriteDirection) * i * 1.4f;
                    pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.67f, 0.85f));
                    dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.SmokeParticle(pos + posOff, vel * 9.3f, RandLerpColor(Color.DarkRed, Color.Black), Main.rand.Next(16, 41), RandRotTwoPi, 0.3670f * (easedProgress), Main.rand.NextFloat(.75f, 1.15f) * Lerp(0.395f, 0.595f, easedProgress), false, BlendState.AlphaBlend);
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
        #endregion
        #region 起手两挥砍
        public void UpdateHalfCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();

            }
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
                if (easedProgress >= 0.98f)
                    return;
                for (int i = 0; i <= 4; i += 2)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.67f, 0.85f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    if (Main.rand.NextBool(3))
                        ECSParticle.SmokeParticle(pos + vel * i * 1.5f, vel * 8.3f, RandLerpColor(Color.DarkRed, Color.Black), Main.rand.Next(16, 41), RandRotTwoPi, 0.970f * (1 - easedProgress), Main.rand.NextFloat(.75f, 1.15f) * Lerp(0.357f, 0.9f, easedProgress), false, BlendState.AlphaBlend);
                }
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 115, Main.rand.NextFloat(.41f, .95f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 0.45f, 0.15f, BlendState.Additive);
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
            float beginAngle = 185f * Flip.ToDirectionInt();
            float endAngle = 183f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .015f);
        }
        #endregion
        #region 处理处死
        public override void OnKill(int timeLeft)
        {
            HandleExecution();
            if (ThirdSwing)
            {
                Projectile.HJScarlet().ExecutionStrike = false;
            }
            if (Main.mouseLeft && Projectile.ai[0] == 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((CrescentRoseHeldProj)proj.ModProjectile).Flip = !Flip;
                ((CrescentRoseHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = Projectile.HJScarlet().ExecutionStrike;
            }
            else if (!ThirdSwing || Projectile.ai[0] != 0)
            {
                ScarletSound(HJScarletSounds.Misc_ManaClearUse, Owner.Center, 0.55f, 1, -0.84f, 0.2f);
                Owner.ScarletHeal(2);
                for (int i = 0; i < 92; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.78f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    BloodyMetaballAlt.SpawnParticle(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 60, RandVelTwoPi(1.4f, 2.1f), 0.145f, RandRotTwoPi, true);
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
                    BloodyMetaballAlt.SpawnParticle(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 15, dir * Main.rand.NextFloat(1.4f, 2.1f), 0.745f, dir.ToRotation(), false, true);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(1.41f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2);
                    BloodyMetaballAlt.SpawnParticle(pos + dir * Main.rand.NextFloat(-42 * (-Flip.ToDirectionInt() * Owner.direction), 1.1f) * 1.5f, dir * Main.rand.NextFloat(1.4f, 2.1f), 0.745f, dir.ToRotation(), false, true);
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
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
        #endregion
        public BlendState BlendState => BlendState.AlphaBlend;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkRed * 0.80f, 0.55f);
            DrawSlash(texture, Color.Red * 0.40f, 0.40f);
            DrawSlash(texture, Color.IndianRed * 0.140f, 0.350f);


            texture = HJScarletTexture.Texture_SwordSlash.Value;
            effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.21f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkRed * 0.55f, 0.95f);
            DrawSlash(texture, Color.Red * 0.40f, 0.50f);
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.05f);
            DrawSlash(texture, Color.Lerp(Color.Crimson, Color.White, 0.760f) * 0.75f, 0.85f, 1f);
            DrawSlash(texture, Color.Lerp(Color.IndianRed, Color.White, 0.790f) * 0.75f, 0.90f, 1f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(.1f, .1f, 0, 0), new Vector2(-Main.GlobalTimeWrappedHourly * 1.395f, 0), new Vector2(1, 2), Color.Crimson);
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.Red, 0.60f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White, 0.45f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * beginMult + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
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
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            Texture2D texture = HJScarletTexture.Texture_SwordSlashWhite.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(0.41f, 0.53f, 0.12f, 0.12f), Vector2.One, Vector2.One);
            DrawSlash(texture, Color.Black * 0.6f, 0.75f, 0.99f);
            DrawSlash(texture, Color.DarkRed * 0.150f, 0.60f, 0.99f);
            SB.EndShaderArea();
            return false;
        }
    }
}
