using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class HighTechBladeHeldProj : HJScarletHeldProj, IPixelatedRenderer
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override ModItem OriginalItem => GetInstance<HighTechBlade>();
        public override string Texture => OriginalItem.Texture;
        public override int ExtraUpdates => 10;
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public AnimationStruct Helper = new AnimationStruct(3);
        public float SwordLength = 60;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Width = 1.15f;
        public float Height = 1.05f;
        public float SwimgTime = 0;
        public int SwingTime = 0;
        public float SwordScale = 0;
        public float SlashOpacity = 1;
        public List<Vector2> OldAimPos = [];
        public List<Vector2> ThirdOldAimPos = [];

        public override void ExSD()
        {
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            ScarletSound(HJScarletSounds.Misc_KnifeTossAlt, Projectile.Center, 0.5f, 1, 0.4f, 0.1f, 2);
            Projectile.originalDamage = Projectile.damage;
            if (Projectile.ai[1] > 2)
                Projectile.ai[1] = 0;
            if (Projectile.ai[1] <= 1)
            {
                Width = 1.25f;
                Height = 1.15f;
                SwordScale = 1.8f;
                Helper.MaxProgress[0] = (int)(AttackSpeed * .75f);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .25f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * 1.20f);

            }
            else
            {
                Width = 1.41f;
                Height = 1.25f;
                Helper.MaxProgress[0] = (int)(AttackSpeed * .65f);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .35f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * 1.20f);
                SwordScale = 2.4f;
            }
            TargetRotation = Owner.Center.ToMouseVector2().ToRotation();
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            if (OldAimPos.Count > 1800)
                OldAimPos.RemoveAt(0);
        }
        public void UpdateAnimation()
        {
            if (Projectile.ai[1] == 0)
            {
                UpdateFirstSwingAnimation();
            }
            else if (Projectile.ai[1] == 1)
            {
                UpdateSecondSwingAnimation();
            }
            else
            {
                UpdateThirdSwingAnimation();

            }
        }


        #region 第一套动画：半圆挥砍
        public void UpdateFirstSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();

            }
            else if (!Helper.IsDone[1])
            {
                SlashOpacity = Lerp(SlashOpacity, 0, 0.1f);

                UpdateEndAnimation();
            }
            else
                Projectile.Kill();
        }


        public void UpdateBeginAnimation()
        {
            float heldScale = Owner.HeldItem.scale;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -185f * Flip.ToDirectionInt();
            float endAngle = 180f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120 * SwordScale;
                OldAimPos.Add(slashPosFinal);
                SetSwingParticle();
            }
        }
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(1);
            float heldScale = Owner.HeldItem.scale;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(1));
            float beginAngle = 180f * Flip.ToDirectionInt();
            float endAngle = 185f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }
        #endregion

        #region 第二套动画：向前方的椭圆挥砍
        public void UpdateSecondSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimationSecond();

            }
            else if (!Helper.IsDone[1])
            {
                UpdateEndAnimationSecond();
                SlashOpacity = Lerp(SlashOpacity, 0, 0.1f);


            }
            else
            {
                Projectile.Kill();
            }

        }
        public void UpdateBeginAnimationSecond()
        {
            float heldScale = Owner.HeldItem.scale;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -185f * Flip.ToDirectionInt();
            float endAngle = 180f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120 * SwordScale;
                OldAimPos.Add(slashPosFinal);
                SetSwingParticle();
            }
        }
        public void UpdateEndAnimationSecond()
        {
            Helper.UpdateAniState(1);
            float heldScale = Owner.HeldItem.scale;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(1));
            float beginAngle = 180f * Flip.ToDirectionInt();
            float endAngle = 185f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        #endregion
        #region 第三套动画：椭圆挥砍。但尽可能覆盖玩家身后
        public void UpdateThirdSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimationThird();

            }
            else if (!Helper.IsDone[1])
            {
                UpdateEndAnimationThird();
                SlashOpacity = Lerp(SlashOpacity, 0, 0.1f);


            }
            else
            {
                Projectile.Kill();
            }

        }
        public void UpdateBeginAnimationThird()
        {
            float heldScale = Owner.HeldItem.scale;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -210f * Flip.ToDirectionInt();
            float endAngle = 208f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120 * SwordScale;
                OldAimPos.Add(slashPosFinal);
                slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 80 * SwordScale;
                ThirdOldAimPos.Add(slashPosFinal);
                SetSwingParticle();
            }
        }
        public void UpdateEndAnimationThird()
        {
            Helper.UpdateAniState(1);
            float heldScale = Owner.HeldItem.scale;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(1));
            float beginAngle = 208f * Flip.ToDirectionInt();
            float endAngle = 210f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        #endregion
        public void SetSwingParticle()
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
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft && !Owner.dead)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.ai[1] = Projectile.ai[1] + 1;
                ((HighTechBladeHeldProj)proj.ModProjectile).Flip = !Flip;
                ((HighTechBladeHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
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
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 98;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            PixelatedRenderManager.BeginDrawProj = true;
            SB.Draw(tex, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            return false;
        }
        public BlendState BlendState => BlendState.Additive;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(.71f, .4f, .1f, .1f), Vector2.One, Vector2.One);
            DrawSlash(texture, Color.DarkRed * 0.80f, 0.45f);
            DrawSlash(texture, Color.OrangeRed * 0.40f, 0.30f);
            DrawSlash(texture, Color.Orange * 0.140f, 0.150f);
            if (Projectile.ai[1] == 2)
            {
                DrawSlash2(texture, Color.DarkRed * 0.80f, 0.45f);
                DrawSlash2(texture, Color.Crimson * 0.40f, 0.30f);
                DrawSlash2(texture, Color.Orange * 0.140f, 0.150f);
            }

            texture = HJScarletTexture.Texture_SwordSlash.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.71f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.4f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.LightGoldenrodYellow * 0.95f, 0.80f);
            DrawSlash(texture, Color.OrangeRed * 0.40f, 0.25f);
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.71f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.4f);
            DrawSlash(texture, Color.Lerp(Color.Crimson, Color.OrangeRed, 0.760f) * 0.95f, 0.85f, 1f);
            DrawSlash(texture, Color.Lerp(Color.IndianRed, Color.White, 0.790f) * 0.75f, 0.90f, 1f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(.71f, .4f, .1f, .1f), new Vector2(-Main.GlobalTimeWrappedHourly * 1.145f, 0), new Vector2(1.5f, 2f), Color.White);
            Texture2D texture2 = HJScarletTexture.Noise_Smoke.Value;
            DrawSlash(texture2, Color.Crimson * .75f, 0.70f, 0.9f);
            if (Projectile.ai[1] == 2)
            {
                DrawSlash2(texture2, Color.Red * .75f, 0.70f, 0.9f);
            }
            texture2 = HJScarletTexture.Noise_Misc.Value;

            DrawSlash(texture2, Color.OrangeRed, 0.45f);
            DrawSlash(texture2, Color.DarkOrange * .5f, 0.55f);
            if (Projectile.ai[1] == 2)
            {
                DrawSlash2(texture2, Color.OrangeRed, 0.45f);
                DrawSlash2(texture2, Color.Red * .5f, 0.55f);

            }
            HJScarletMethods.EndShaderAreaPixel();
        }

        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            drawcolor *= SlashOpacity;
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
        private List<ScarletVertex> _vertexCache2 = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash2(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache2.Clear();
            drawcolor *= SlashOpacity * .75f;
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * beginMult * .5f + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult * .65f + Projectile.Center - Main.screenPosition;
                _vertexCache2.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache2.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache2.ToArray(), 0, _vertexCache2.Count - 2);
        }

    }
}
