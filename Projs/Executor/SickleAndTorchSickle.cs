using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SickleAndTorchSickle : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<SickleAndTorch>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public float SickleLength = 60;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public bool IsBehind
        {
            get => (Projectile.ai[2] == 1f);
            set => Projectile.ai[2] = value ? 1 : 0;

        }
        public List<Vector2> OldAimPos = [];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(5);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(6);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

        }

        public override void OnKill(int timeLeft)
        {
        }
        public override void OnFirstFrame()
        {
            ScarletSound(HJScarletSounds.Misc_KnifeTossAlt, Projectile.Center, 0.5f, 1, 0.4f, 0.1f, 2);
            Projectile.originalDamage = Projectile.damage;
            Helper.MaxProgress[0] = (int)(AttackSpeed * .45f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .55f);
            BeginTargetRotation = Owner.Center.ToMouseVector2().ToRotation();
            TargetRotation = BeginTargetRotation;
            SickleLength = 60;

        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            UpdateExecution();
            if (OldAimPos.Count > 5 * Projectile.MaxUpdates)
                OldAimPos.RemoveAt(0);
        }

        public void UpdateExecution()
        {
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
            if (!IsBehind)
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
                UpdateEndAnimation();
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);
                if (IsBehind)
                    Projectile.scale *= (1 - EaseInCubic(Helper.GetAniProgress(1)));
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
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.4f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else if (!IsBehind)
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(1.4f, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.2f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 35;
                OldAimPos.Add(slashPosFinal);
                if (Main.rand.NextBool(12))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 45, Main.rand.NextFloat(0.51f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(1.2f, 1.5f);
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.White, Color.DarkGray), 40, 1f, .04f * Projectile.scale * Main.rand.NextFloat(.8f, 1.1f), glowMult: .51f);
                }
                if (Main.rand.NextBool(12))

                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 45, Main.rand.NextFloat(.51f, .98f));
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
            float beginAngle = 125f * Flip.ToDirectionInt();
            float endAngle = 130 * Flip.ToDirectionInt();

            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.4f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                modifiers.DefenseEffectiveness *= 0;
                ScreenShakeSystem.AddScreenShakes(target.Center, 4, 12, Projectile.rotation, 0, easingFunc: EaseOutCubic);
                ECSParticle.LightntingGlow(target.Center, (Projectile.rotation + PiOver2).ToRotationVector2() * .1f, Color.White, 40, 1, .8f);
                ScarletSound(HJScarletSounds.TheMars_Hit, target.Center, 0.7f, 1, -0.4f, 0.1f);
                modifiers.Knockback *= 1.72f;
                target.AddBuff(BuffID.Oiled, GetSeconds(2));
            }
            else
            {
                ScreenShakeSystem.AddScreenShakes(target.Center, 4, 12, Projectile.rotation, 0, easingFunc: EaseOutCubic);
                ECSParticle.LightntingGlow(target.Center, (Projectile.rotation + PiOver2).ToRotationVector2() * .1f, Color.White, 40, 1, .8f);
                ScarletSound(HJScarletSounds.TheMars_Hit, target.Center, 0.7f, 1, -0.4f, 0.1f);
                modifiers.Knockback *= 1.2f;

                Projectile.AddExecutionTimeImmediate(OriginalItemID);
            }
            modifiers.HitDirectionOverride = ((target.Center.X - Owner.Center.X) > 0).ToDirectionInt();
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
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 38;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            float lerp = IsBehind ? 1f : 0.75f;
            Color c = IsBehind ? Color.Lerp(Color.White, Color.Black, 0.705f) with { A = 255 } : Color.White;
            if (Projectile.HJScarlet().ExecutionStrike && !IsBehind)
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 1.5f, null, c.ToAddColor(), drawRotation, rotationPoint, Projectile.scale * lerp, flipSprite, 0);
            SB.Draw(tex, drawPosition, null, c, drawRotation, rotationPoint, Projectile.scale * lerp, flipSprite, 0);
            if (IsBehind)
                return false;
            SB.EnterShaderArea();
            Vector2 topPos = drawPosition + (Vector2.UnitX).RotatedBy(Projectile.rotation) * 35f * Projectile.scale;
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            float scale = Projectile.scale * .072f * (1 - EaseOutExpo(Helper.GetAniProgress(1)));
            SB.Draw(glow, topPos, null, Color.White, 0, glow.Size() / 2, scale, 0, 0);
            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(0.2f);
            effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect2.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * .35f, 0));
            effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
            effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.Silver * .85f, 0.90f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White * .90f, 0.85f);

            Texture2D texture = HJScarletTexture.Texture_SwordSlash.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkGray * 0.90f, 0.95f);
            DrawSlash(texture, Color.White * 0.40f, 0.80f);

            SB.EndShaderArea();
            SB.EndShaderArea();
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
