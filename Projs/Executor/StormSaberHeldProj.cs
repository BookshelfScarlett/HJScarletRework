using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class StormSaberHeldProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<StormSaber>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public float SwordLength = 60;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float Width = 1f;
        public float SwingScale = 1.21f;
        public float SwordScale = 1f;
        public List<Vector2> OldAimPos = [];
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float SlashOpacity = 1f;


        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
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
            ScarletSound(HJScarletSounds.TheSevenStar_Swing, Projectile.Center, 0.75f, 1, -0.1f + 0.14f * SwingTime, 0.1f);
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                SwingScale = 1.30f;
                Helper.MaxProgress[0] = (int)(AttackSpeed * .30f);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .05f);
            }
            else
            {
                Helper.MaxProgress[0] = (int)(AttackSpeed * .85f);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .15f);
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
            UpdateHalfCircleSwingAnimation();
        }
        public void UpdateHalfCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                if (Helper.OnAnimationBegin(0))
                {
                    Vector2 fireVel = (Main.MouseWorld - Owner.Center).ToSafeNormalize() * 40;
                    Vector2 pos = Owner.MountedCenter - fireVel.ToSafeNormalize() * 300;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, fireVel, ProjectileType<StormSaberSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.HJScarlet().HasExecutionMechanic = !Projectile.HJScarlet().ExecutionStrike;
                    if (Projectile.HJScarlet().ExecutionStrike)
                    {
                        proj.extraUpdates += 1;
                        ScarletSound(HJScarletSounds.TheSevenStar_Charge, Projectile.Center, .3f, 0, 0.10f, .1f);
                    }
                    else
                    {
                        ScarletSound(HJScarletSounds.TheSevenStar_Charge, Projectile.Center, .6f, 0, .05f, .1f);
                    }
                }
                UpdateBeginAnimation();
                if (OldAimPos.Count > 100)
                    OldAimPos.RemoveAt(0);
            }
            else if (!Helper.IsDone[1])
            {
                UpdateEndAnimation();
                if (Projectile.numUpdates % 2 == 0)
                    OldAimPos.RemoveAt(0);

                SlashOpacity = Lerp(SlashOpacity, 0f, .1f);
                if (SlashOpacity < .2f)
                    SlashOpacity = 0;
            }
            else
                Projectile.Kill();

        }

        private void UpdateEndAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(1);
            float easedProgress = EaseInOutExpo(Helper.GetAniProgress(1));
            float beginAngle = 195f * Flip.ToDirectionInt();
            float endAngle = 195f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwingScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .01f);
        }

        public void UpdateBeginAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(0));
            float beginAngle = -195f * Flip.ToDirectionInt();
            float endAngle = 195f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwingScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.2f * SwingScale * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 110;
                OldAimPos.Add(slashPosFinal);
                if (easedProgress >= 0.95f)
                    return;
                if (Main.rand.NextBool(4))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 130, Main.rand.NextFloat(0.41f, 1f));
                        Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                        Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                        Vector2 orbVel = vel * Main.rand.NextFloat(.5f, 1.1f) * 10f;
                        ECSParticle.ShinyCrossStarECS(pos + orbVel, vel, RandLerpColor(Color.WhiteSmoke, Color.White), 40, 1f, Main.rand.NextFloat(.95f, 1.10f) * .13f);
                    }
                }
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 110, Main.rand.NextFloat(0.45f, 1f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection) * Main.rand.NextFloat(.1f, 1.2f) * 5f;
                    for (int i = 0; i < 3; i++)
                    {
                        ECSParticle.LiliesFire(pos + dir * 5 * i, vel, RandLerpColor(Color.White, Color.WhiteSmoke), 45, RandRotTwoPi, .85f, Main.rand.NextFloat(.95f, 1.10f) * Projectile.scale * .21f, true, BlendState.Additive);
                    }
                }

            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft && !Owner.dead)
            {
                if (SwingTime >= 8 && !Projectile.HJScarlet().ExecutionStrike)
                {
                    Owner.RemoveExecutionProgress(OriginalItemID);
                    SwingTime = 0;
                }
                if (!Projectile.HJScarlet().ExecutionStrike)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    ((StormSaberHeldProj)proj.ModProjectile).Flip = !Flip;
                    proj.HJScarlet().HasExecutionMechanic = true;
                    proj.HJScarlet().ExecutionStrike = Owner.GetExecutionSrike();
                }
                else
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    ((StormSaberHeldProj)proj.ModProjectile).Flip = !Flip;
                    ((StormSaberHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
                    proj.HJScarlet().HasExecutionMechanic = true;
                    proj.HJScarlet().ExecutionStrike = SwingTime < 6;
                }
            }
            else
            {
                if (Projectile.HJScarlet().ExecutionStrike)
                    Owner.RemoveExecutionProgress(OriginalItemID);

            }

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch sb)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            HJScarletMethods.ApplyAlphaCut(new Vector4(.41f + .6f * (1 - SlashOpacity), .13f + .6f * (1 - SlashOpacity), .01f, .1f), Vector2.Zero, Vector2.One);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            DrawSlash(texture, Color.White * 0.95f, 0.94f);
            DrawSlash(texture, Color.White * 0.40f, 0.20f);
            DrawSlash(texture, Color.White * 0.60f, 0.40f);
            DrawSlash(texture, Color.White * 0.20f, 0.350f);


            texture = HJScarletTexture.Texture_SwordSlash.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.21f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(.15f, 1.2f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.WhiteSmoke * 0.55f, 0.95f);
            DrawSlash(texture, Color.White * 0.40f, 0.60f);
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.05f);
            DrawSlash(texture, Color.White * 0.55f, 0.85f, 1f);
            DrawSlash(texture, Color.White * 0.55f, 0.90f, 1f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(.2f, .2f, .1f, 0.1f), new Vector2(-Main.GlobalTimeWrappedHourly * 1.05f, 0), new Vector2(1, 1.5f), Color.WhiteSmoke);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.WhiteSmoke * .65f, 0.70f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White * .65f, 0.65f);
            HJScarletMethods.ApplyAlphaCut(new Vector4(.2f, .2f, .2f, .2f), new Vector2(-Main.GlobalTimeWrappedHourly * 0.485f, 0), new Vector2(0.42f, 1.2f), Color.WhiteSmoke);
            texture2 = HJScarletTexture.Noise_Misc2.Value;
            DrawSlash(texture2, Color.WhiteSmoke * .5f, 0.60f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White * .5f, 0.60f);

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
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
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
            float endPro = EaseOutExpo(Helper.GetAniProgress(0));
            float pro = 0;
            if (endPro < .5f)
                pro = endPro / .5f;
            else
                pro = (endPro - .5f) / .5f;
            Color c = Color.White;
            float offset = (30);
            Vector2 ori = Projectile.spriteDirection == -1 ? new Vector2(tex.Width, tex.Height) - new Vector2(offset) : new Vector2(offset, tex.Height - offset);
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 1.5f * (1 - pro), null, Color.White.ToAddColor(), drawRotation, ori, Projectile.scale, flipSprite, 0);
            SB.Draw(tex, drawPosition, null, c, drawRotation, ori, Projectile.scale, flipSprite, 0);
            SB.EnterShaderArea();
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 pos = drawPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * 130f * Projectile.scale * (1 - endPro);
            float glowScale = Projectile.scale * .1f * (1 - endPro);
            SB.Draw(glow, pos, null, Color.White, drawRotation, glow.Size() / 2, glowScale, flipSprite, 0);
            SB.Draw(glow, pos, null, Color.White, drawRotation, glow.Size() / 2, glowScale * .95f, flipSprite, 0);
            SB.Draw(glow, pos, null, Color.White, drawRotation, glow.Size() / 2, glowScale * .92f, flipSprite, 0);
            SB.EndShaderArea();
            SB.EndShaderArea();
            return false;
        }
    }
}
