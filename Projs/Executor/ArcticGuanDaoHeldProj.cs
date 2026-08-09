using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Misc;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ArcticGuanDaoHeldProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<ArcticGuanDao>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float Width = 1f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;
        public List<Vector2> OldAimPos = [];

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(8);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 3;
            Projectile.ownerHitCheck = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            ThirdSwing = SwingTime >= 2;
            if (ThirdSwing)
            {
                ScarletSound(HJScarletSounds.Tlipoca_Swing, Projectile.Center, 0.35f, 1, 0.34f, 0.1f, 1);
                Helper.MaxProgress[0] = (int)(AttackSpeed);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .5f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * .65f);
            }
            else
            {
                ScarletSound(HJScarletSounds.Tlipoca_Swing, Projectile.Center, 0.75f, 1, -0.1f + 0.14f * SwingTime, 0.1f, 2);
                Helper.MaxProgress[0] = (int)(AttackSpeed);
                Helper.MaxProgress[2] = (int)(AttackSpeed * .95f);
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
            HandleExecution();
            if (OldAimPos.Count > 5 * Projectile.MaxUpdates)
                OldAimPos.RemoveAt(0);
        }
        public override void OnExecution()
        {
            ScarletSound(HJScarletSounds.Misc_ManaClearUse, Owner.Center, 0.55f, 1, -0.84f, 0.2f);
            if (Projectile.IsMe() && Projectile.FinalUpdate())
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.ToSafeNormalize() * 19f, ProjectileType<ArcticGuanDaoCloudMoving>(), Projectile.originalDamage, 1f, Projectile.owner);
                proj.originalDamage = Projectile.originalDamage;
                ((ArcticGuanDaoCloudMoving)proj.ModProjectile).TargetVector2 = Main.MouseWorld;
            }
            Projectile.HJScarlet().ExecutionStrike = false;
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
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.15f * heldScale;
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
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.15f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.15f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 90;
                OldAimPos.Add(slashPosFinal);

                if (easedProgress >= 0.95f)
                    return;
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.61f, .91f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.3f);
                }
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .8f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.75f, 0.1f);
                }

            }
        }
        #endregion
        public void UpdateHalfCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();
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
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.1f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 90;
                OldAimPos.Add(slashPosFinal);
                if (easedProgress >= 0.95f)
                    return;
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.61f, .91f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.3f);
                }
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .8f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.75f, 0.1f);
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
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 90;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
                StopTiming = 2 * Projectile.extraUpdates;
            Projectile.AddExecutionTimeImmediate(OriginalItemID);
            for (int i = 0; i < 34; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 4.2f), Color.RoyalBlue, 40, 1, 0.6f);
            }
            for (int i = 0; i < 20; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 6.2f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.9f, 0.31f, blendstate: BlendState.AlphaBlend);
            }
            ScarletSound(HJScarletSounds.TheMars_Hit, target.Center, 0.64f, 1, -0.6f, pitchVariance: 0.2f);
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                ((ArcticGuanDaoHeldProj)proj.ModProjectile).Flip = !Flip;
                ((ArcticGuanDaoHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = Projectile.HJScarlet().ExecutionStrike;
            }
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.RoyalBlue * 0.80f, 0.55f);
            DrawSlash(texture, Color.DeepSkyBlue * 0.40f, 0.40f);
            DrawSlash(texture, Color.SkyBlue * 0.140f, 0.350f);


            texture = HJScarletTexture.Texture_SwordSlash.Value;
            effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.21f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.RoyalBlue * 0.55f, 0.95f);
            DrawSlash(texture, Color.SkyBlue * 0.40f, 0.50f);
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.05f);
            DrawSlash(texture, Color.Lerp(Color.RoyalBlue, Color.White, 0.760f) * 0.75f, 0.85f, 1f);
            DrawSlash(texture, Color.Lerp(Color.DeepSkyBlue, Color.White, 0.790f) * 0.75f, 0.90f, 1f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(.1f, .1f, 0, 0), new Vector2(-Main.GlobalTimeWrappedHourly * 1.395f, 0), new Vector2(1, 2), Color.SkyBlue);
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.RoyalBlue, 0.60f);
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

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            float endPro = EaseInCubic(Helper.GetAniProgress(1));
            float endPro2 = EaseInBack(Helper.GetAniProgress(1));

            if (ThirdSwing)
            {
                Color c = Color.White;
                float offset = (10);
                Vector2 ori = Projectile.spriteDirection == -1 ? new Vector2(tex.Width, tex.Height) - new Vector2(offset) : new Vector2(offset, tex.Height - offset);
                SB.Draw(tex, drawPosition, null, c * (1 - endPro2), drawRotation, ori, Projectile.scale * (1 - endPro), flipSprite, 0);
            }
            else
            {
                float time = SwingTime / 3f;
                Color c = Color.Lerp(Color.White, Color.Black, Helper.GetAniProgress(2));
                float offset = (10);
                Vector2 ori = Projectile.spriteDirection == -1 ? new Vector2(tex.Width, tex.Height) - new Vector2(offset) : new Vector2(offset, tex.Height - offset);
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f * time, null, Color.White.ToAddColor(), drawRotation, ori, Projectile.scale, flipSprite, 0);
                SB.Draw(tex, drawPosition, null, c, drawRotation, ori, Projectile.scale, flipSprite, 0);
            }
            SB.EnterShaderArea();
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 pos = drawPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * 85f * Projectile.scale * (1 - endPro);
            float glowScale = Projectile.scale * .1f * (1 - endPro);
            SB.Draw(glow, pos, null, Color.RoyalBlue, drawRotation, glow.Size() / 2, glowScale, flipSprite, 0);
            SB.Draw(glow, pos, null, Color.LightSkyBlue, drawRotation, glow.Size() / 2, glowScale * .95f, flipSprite, 0);
            SB.Draw(glow, pos, null, Color.White, drawRotation, glow.Size() / 2, glowScale * .92f, flipSprite, 0);
            SB.EndShaderArea();
            return false;

        }
    }
}
