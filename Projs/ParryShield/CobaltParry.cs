using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.ParryShield
{
    public class CobaltParryShield :BaseParryShield
    {
        public override string Texture => GetInstance<CobaltParry>().Texture;
        protected override int TrailCounts => 14;
        protected override float ParryShieldScale => 1.5f;
        protected override void OnActuallyGoingParry(float progress, Vector2 tarPos, Vector2 offset)
        {
            if (Main.rand.NextFloat()<progress)
                return;
            float maxRange = ApplyParryShieldHorizonalRangeScale() * 90;
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + offset + tarPos.RotatedBy(TargetRotation) * maxRange, Main.rand.NextFloat(0.31f, .6f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                //ECSParticle.ShrinkParticle(pos, vel, Color.White, 40, 1, vel.ToRotation(), 0.12f, 1);
                ECSParticle.HRShinyOrb(pos, vel, Color.White, 40, 1, 0.05f);
            }
            if (Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + offset + tarPos.RotatedBy(TargetRotation) * maxRange, Main.rand.NextFloat(0.41f, .50f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, 1, Main.rand.NextFloat(.9f, 1.15f) * .2f, .2f);
            }
        }
        protected override void PostOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone, Vector2 parryDirection)
        {
            for (int i = 0; i < 26; i++)
            {
                Vector2 vel = parryDirection.ToRandVelocity(ToRadians(35f), .1f, 19f);
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * 10f, RandVelTwoPi(.3f, 14f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 20; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6) + parryDirection* 4f, RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                Dust d = Dust.NewDustPerfect(target.Center, DustID.GemDiamond);
                d.velocity = RandVelTwoPi(1.2f, 6.2f) + parryDirection* 3f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.2f, 1.61f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            SB.EnterShaderArea();
            Texture2D tex2 = HJScarletTexture.Particle_Smear.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(ParryRange * ApplyParryShieldHorizonalRangeScale(), 0f).RotatedBy(Projectile.rotation);
            float rot = Projectile.rotation + PiOver4 + (Projectile.direction < 0).ToInt() * -Pi;
            float scale = Projectile.scale * .3f;
            SB.FastDraw(tex2, pos, Color.RoyalBlue * ApplyParryShieldHorizonalRangeScale() * 1.5f, rot, tex2.Size() / 2f, scale, 0);
            SB.FastDraw(tex2, pos, Color.CornflowerBlue* ApplyParryShieldHorizonalRangeScale()*1.5f, rot, tex2.Size() / 2f, scale*.75f, 0);
            SB.FastDraw(tex2, pos, Color.AliceBlue* ApplyParryShieldHorizonalRangeScale()*1.5f, rot, tex2.Size() / 2f, scale*.5f, 0);
            SB.EndShaderArea();
            //RenderPixelated(Main.spriteBatch);
            return false;
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            SB.EnterShaderArea();
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
            SB.EndShaderArea();
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldParryShieldPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldParryShieldPos.Count; i++)
            {
                float progress = (float)i / OldParryShieldPos.Count;
                Vector2 DrawPos_Head = OldParryShieldPos[i] * beginMult + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldParryShieldPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }

    }
    /// <summary>
    /// 复用竹刀的代码
    /// </summary>
    public class CobaltParry :HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.CobaltShield);
        public AnimationStruct Helper = new AnimationStruct(2);
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float Width = 1f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;
        public int AttackSpeed => 30 *Projectile.MaxUpdates;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.SetUpHeldProj(5);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 3;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            ScarletSound(HJScarletSounds.TheSevenStar_Swing, Projectile.Center, 0.75f, 1, -0.1f + 0.14f * SwingTime, 0.1f);
            Helper.MaxProgress[0] = (int)(AttackSpeed);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .95f);
            TargetRotation = Owner.Center.ToMouseVector2().ToRotation();
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
        }
        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            //Owner.itemTime = 2;
            //Owner.itemAnimation = 2;
            //Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            //Owner.ChangeDir(Projectile.direction);
            //Owner.ControlPlayerArm(Projectile.rotation);
        }

        public void UpdateAnimation()
        {
            if (StopTiming > 0)
            {
                StopTiming--;
                Projectile.position += Main.rand.NextVector2Circular(5, 5);
                return;
            }
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();
            }
            else
                Projectile.Kill();

        }
        public void UpdateBeginAnimation()
        {
            float heldScale = 1.5f;
            Helper.UpdateAniState(0);
            float easedProgress = EaseInOutExpo(EaseOutCubic(Helper.GetAniProgress(0)));
            float beginAngle = -185f * Flip.ToDirectionInt();
            float endAngle = 185f * Flip.ToDirectionInt();
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
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.1f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 90;
            float lerp = 1f;
            float curLerp = .5f;
            if(easedProgress< curLerp)
            {
                lerp = Lerp(0f, 1f, EaseOutCubic(Utils.GetLerpValue(0f,curLerp,easedProgress,true)));
            }
            else
            {
                lerp = Lerp(1f, 0.2f, Utils.GetLerpValue(curLerp,1f,easedProgress,true));
            }
            Vector2 offset = new Vector2(120*lerp, 0).RotatedBy(Projectile.rotation);

                if (easedProgress >= 0.95f)
                    return;
                if (Main.rand.NextBool(4))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center +offset + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.31f, .6f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.ShrinkParticle(pos, vel, Color.White, 40, 1, vel.ToRotation(), 0.2f, 1);
                }
                if (Main.rand.NextBool())
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + offset + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .50f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1, 0.75f, 0.23f, blendstate: BlendState.NonPremultiplied);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseInOutExpo(EaseOutCubic(Helper.GetAniProgress(0)));
            if (easedProgress < 0.01f)
                return false;
            float curLerp = .5f;
            float lerp;
            if(easedProgress< curLerp)
            {
                lerp = Lerp(0f, 1f, EaseOutCubic(Utils.GetLerpValue(0f,curLerp,easedProgress,true)));
            }
            else
            {
                lerp = Lerp(1f, 0.2f, Utils.GetLerpValue(curLerp,1f,easedProgress,true));
            }

            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 120 * lerp;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 34f, ref _);
            return c;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
            {
                target.AddBuff(BuffType<ParrySpin>(), GetSeconds(3));
                Vector2 finalDir = Owner.Center.GetNormalVector2(target.Center)-Vector2.UnitY*15f;
                StopTiming = 5*Projectile.MaxUpdates;
                target.PunchTarget(finalDir, 15);
                ScreenShakeSystem.AddScreenShakes(target.Center, 30,30, RandRotTwoPi, RandRotTwoPi);
                ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, pitch: .24f, pitchVariance: .1f, variantType: 2);
                for (int i = 0; i < 26; i++)
                {
                    Vector2 vel = finalDir.ToRandVelocity(ToRadians(35f), .1f, 19f);
                    ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * 10f, RandVelTwoPi(.3f, 14f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
                }
                for (int i = 0; i < 20; i++)
                {
                    ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6) + finalDir * 4f, RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                    Dust d = Dust.NewDustPerfect(target.Center, DustID.GemDiamond);
                    d.velocity = RandVelTwoPi(1.2f, 6.2f) + finalDir * 3f;
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1.2f, 1.61f);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        //你也要画刀光吗？
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            int length = Projectile.oldPos.Length-10;
            float easedProgress = EaseInOutExpo(EaseOutCubic(Helper.GetAniProgress(0)));
            float pro = easedProgress;
            float lerp = 1f;
            float curLerp = .5f;
            if(pro < curLerp)
            {
                lerp = Lerp(0f, 1f, EaseOutCubic(Utils.GetLerpValue(0f,curLerp,pro,true)));
            }
            else
            {
                lerp = Lerp(1f, 0f, EaseInCubic(Utils.GetLerpValue(curLerp,1f,pro,true)));
            }
            Vector2 offset = new Vector2(120*lerp, 0).RotatedBy(Projectile.rotation);
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1 - i / (float)length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i] + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
                float opac = Lerp(0.05f, 1f, ratios) * .30f;
                Color c = Color.Lerp(Color.WhiteSmoke, Color.White, ratios).ToAddColor(25);
                SB.FastDraw(tex, pos + offset, c * opac*lerp*lerp, rot, rotationPoint, Projectile.scale, flipSprite);
            }

            for(int i =0;i<8;i++)
            SB.FastDraw(tex, drawPosition+(TwoPi/8f*i).ToRotationVector2()*1.1f + offset , Color.White.ToAddColor()*lerp*lerp, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            SB.FastDraw(tex, drawPosition + offset , Color.White*lerp*lerp, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}
