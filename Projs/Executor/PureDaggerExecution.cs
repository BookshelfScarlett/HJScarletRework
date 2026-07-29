using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class PureDaggerExecution : ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<PureDagger>();
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override string Texture => GetInstance<PureDagger>().Texture;
        public AnimationStruct Helper = new AnimationStruct(3);
        public float SwordLength = 60;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public int CurAttackTime = 0;
        public int MaxAttackTime = 15;
        public List<Vector2> OldAimPos = [];

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
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
            if (CurAttackTime == 0)
            {

            }
            Projectile.HJScarlet().ExecutionStrike = true;
            ScarletSound(HJScarletSounds.Misc_KnifeTossAlt, Projectile.Center, 0.5f, 1, 0.4f, 0.1f, 1);
            Projectile.originalDamage = Projectile.damage;
            Helper.MaxProgress[0] = (int)((AttackSpeed * .45f) * .45f);
            Helper.MaxProgress[1] = (int)((AttackSpeed * .45f) * .55f);
            BeginTargetRotation = Owner.Center.ToMouseVector2().ToRotation();
            TargetRotation = BeginTargetRotation;
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            if (OldAimPos.Count > 15)
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
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public void UpdateAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();

            }
            else if (!Helper.IsDone[1])
            {
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);

                UpdateEndAnimation();
            }
            else
                Projectile.Kill();
        }
        public void UpdateBeginAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -210f * Flip.ToDirectionInt();
            float endAngle = 115f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.4f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(1f, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.4f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 80;
                OldAimPos.Add(slashPosFinal);
                if (Main.rand.NextBool(8))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 85, Main.rand.NextFloat(0.51f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(1.2f, 1.5f);
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.White, Color.DarkGray), 40, 1f, .04f * Projectile.scale * Main.rand.NextFloat(.8f, 1.1f), glowMult: .51f);
                }
                if (Main.rand.NextBool(8))

                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 85, Main.rand.NextFloat(.51f, .98f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.DarkGray), 30, 1f, Main.rand.NextFloat(.7f, 1.01f) * Projectile.scale * .45f, 0.2f);
                }
            }
        }
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(1);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseInOutExpo(Helper.GetAniProgress(1));
            float beginAngle = 115f * Flip.ToDirectionInt();
            float endAngle = 125 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.4f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DefenseEffectiveness *= 0;
            ScreenShakeSystem.AddScreenShakes(target.Center, 4, 12, Projectile.rotation, 0, easingFunc: EaseOutCubic);
            ECSParticle.LightntingGlow(target.Center, (Projectile.rotation + PiOver2).ToRotationVector2() * .1f, Color.White, 40, 1, .8f);
            ECSParticle.LightntingGlow(target.Center, (Projectile.rotation + PiOver2 + PiOver4).ToRotationVector2() * .1f, Color.White, 40, 1, .8f);
            for (int i = 0; i < 10; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 6.6f), Color.White, 45, 1, 0.64f);
            }
            ScarletSound(HJScarletSounds.Misc_SwordHit, target.Center, 0.7f, 1, -0.14f, 0.1f);
            modifiers.Knockback *= 1.72f;
            if (Projectile.IsMe() && Projectile.numHits < 1)
            {
                Vector2 dir = Owner.Center.GetNormalVector2(target.Center);
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, dir.ToRandVelocity(ToRadians(30), 14f, 18f), ProjectileType<PureDaggerStar>(), Projectile.originalDamage / 3, Projectile.knockBack, Owner.whoAmI);
            }

        }
        public override void OnKill(int timeLeft)
        {
            if (CurAttackTime > MaxAttackTime)
            {
                Owner.RemoveExecutionProgress(OriginalItemID);
                Owner.CheckExecution(OriginalItemID);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<PureDaggerProj>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.HJScarlet().HasExecutionMechanic = true;
                ((PureDaggerProj)proj.ModProjectile).Flip = !Flip;
                ((PureDaggerProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;

            }
            else
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.HJScarlet().HasExecutionMechanic = true;
                ((PureDaggerExecution)proj.ModProjectile).Flip = !Flip;
                ((PureDaggerExecution)proj.ModProjectile).BeginTargetRotation = TargetRotation;
                ((PureDaggerExecution)proj.ModProjectile).CurAttackTime = CurAttackTime + 1;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.HJScarlet().defenseBuff = PureDagger.DefenseAdd + 2;
            Owner.HJScarlet().defenseBuffTimer = GetSeconds(1);
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
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 98;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {

            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < .01f)
                return;
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 topPos = drawPosition + (Vector2.UnitX).RotatedBy(Projectile.rotation) * 80f * Projectile.scale;
            Texture2D glow = HJScarletTexture.Particle_KiraStarGlow.Value;
            float scale = Projectile.scale * .12f * (1 - EaseOutExpo(Helper.GetAniProgress(1)));
            SB.Draw(glow, topPos, null, Color.White, PiOver4, glow.Size() / 2, scale, 0, 0);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkGray * 0.90f, 0.95f);
            DrawSlash(texture, Color.White * 0.60f, 0.55f);

            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(0.42f);
            effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect2.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * .935f, 0));
            effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
            effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.Silver * .95f, 0.90f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White * .80f, 0.55f);

            texture = HJScarletTexture.Texture_SwordSlash.Value;
            effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.41f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkGray * 0.95f, 0.95f);
            DrawSlash(texture, Color.White * 0.60f, 0.50f);

            HJScarletMethods.EndShaderAreaPixel();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            PixelatedRenderManager.BeginDrawProj = true;
            Color c = Color.White;
            float progress = (1 - Helper.GetAniProgress(1));
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 1.5f * EaseInCubic(progress), null, c.ToAddColor(), drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            SB.Draw(tex, drawPosition, null, c, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            return false;
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
    }
}
