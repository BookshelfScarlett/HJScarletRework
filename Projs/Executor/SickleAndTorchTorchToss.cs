using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
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
    /// <summary>
    /// 这个射弹是一个隐形射弹，实际上只管手部的动作
    /// 火把的掷出其实在第一帧就已经扔出去了
    /// </summary>
    public class SickleAndTorchTorchToss : ExecutorHeldProj
    {
        public override string Texture => GetInstance<SickleAndTorchSickle>().Texture;
        public override int OriginalItemID => ItemType<SickleAndTorch>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public List<Vector2> OldAimPos = [];
        public float SickleLength = 60;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(5);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
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
            ScarletSound(SoundID.DD2_BetsyFireballShot, Projectile.Center, 0.95f, 1, 0.25f);
            ScarletSound(SoundID.Item64, Projectile.Center, 0.95f, 1, 0.55f);

            Projectile.originalDamage = Projectile.damage;
            Helper.MaxProgress[0] = (int)(AttackSpeed * .35f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .95f);
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
        }

        public void UpdateExecution()
        {
        }

        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation);

        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
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
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(1.4f, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.12f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 21;
                OldAimPos.Add(slashPosFinal);
                if (Main.rand.NextBool(8))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 22, Main.rand.NextFloat(.51f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize();
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10))) * Owner.direction * (Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.OrangeRed, Color.Orange), 40, 1f, .07f * Projectile.scale * Main.rand.NextFloat(.8f, 1.1f), glowMult: .51f);
                }
                if (Main.rand.NextBool(8))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 22, Main.rand.NextFloat(.51f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize();
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10))) * Owner.direction * (Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                    Dust d = Dust.NewDustPerfect(pos, DustID.Torch, vel);
                    d.noGravity = true;
                    d.scale = 1.1f;

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
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.4f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            //SB.Draw(tex, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            SB.EnterShaderArea();
            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(0.2f);
            effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect2.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * .35f, 0));
            effect2.Parameters["UVMult"].SetValue(new Vector2(2f, 2f));
            effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.OrangeRed * .85f, 0.90f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.Orange * .90f, 0.70f);

            Texture2D texture = HJScarletTexture.Texture_SwordSlash.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.OrangeRed * 0.90f, 0.95f);
            DrawSlash(texture, Color.White * 0.30f, 0.70f);

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
