using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ASMDIceBlock : HJScarletProj, IPixelatedRenderer
    {
        #region 基础
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public enum State
        {
            Shoot,
            Idle,
            Strike
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int BlockType
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public NPC CurTarget = null;
        public float TotalStrikeTime = 25;
        #endregion
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;

        }
        public override void OnFirstFrame()
        {
            BlockType = Main.rand.Next(0, 6);
            Projectile.localAI[1] = Main.rand.NextFloat(0.3f, 1);
        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Idle:
                    DoIdle();
                    break;
                case State.Strike:
                    DoStrike();
                    break;
            }
        }

        public void DoShoot()
        {
            float maxTime = 45 * Projectile.MaxUpdates;
            Projectile.velocity *= .96f;
            float progress = Utils.GetLerpValue(0, maxTime, Timer, true);
            Timer++;
            Projectile.rotation += Lerp(ToRadians(10f), ToRadians(0.5f), EaseOutBack(progress));
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(22, 2), Projectile.SafeDir(), RandLerpColor(Color.DodgerBlue, Color.White), 45, 1, 0.55f * Main.rand.NextFloat(.85f, 1.15f), 0.2f);
            if (progress >= 1f)
            {
                AttackState = State.Idle;
                Timer = 0;
                Projectile.extraUpdates = 0;
                Projectile.velocity *= 0;
                Projectile.timeLeft = GetSeconds(15);
                Projectile.netUpdate = true;
                Projectile.penetrate = 1;
            }

        }

        public void DoIdle()
        {
            //Timer这里用于代表悬浮的漂移值
            Timer += ToRadians(5) * Projectile.localAI[1];
            //只修改Vec.uny
            Vector2 lerpVector = Vector2.UnitY * (float)Math.Sin(Timer) * 5;
            Vector2 lerpPos = Projectile.Center + lerpVector;
            Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPos, 0.1f);
            //这里一直卡着，直到下一个处决炮弹主动发起追踪索敌
            if (CurTarget.IsLegal())
            {
                AttackState = State.Strike;
                Timer = 0;
                Projectile.extraUpdates = 2;
                Projectile.timeLeft = GetSeconds(30);
                //给予初始速度
                Projectile.velocity = Projectile.Center.GetNormalVector2(CurTarget.Center) * 1f;
            }
            if (Projectile.IsOutScreen())
                return;
            //ECSParticle.KiraStar(Projectile.Center.ToRandCirclePos(22, 2), -Vector2.UnitY * Main.rand.NextFloat(0.8f, 1.4f), RandLerpColor(Color.DodgerBlue, Color.White), 45, 1f, 0, 0.175f * Main.rand.NextFloat(.85f, 1.15f), false, 0.3f);
            ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(22, 2), -Vector2.UnitY * Main.rand.NextFloat(.8f, 1.4f), RandLerpColor(Color.DodgerBlue, Color.White), 45, 1, 0.55f * Main.rand.NextFloat(.85f, 1.15f), 0.2f);
        }
        public void DoStrike()
        {
            if (CurTarget.IsLegal())
            {
                float maxTime = TotalStrikeTime;
                float progress = Utils.GetLerpValue(0, maxTime, Timer, true);
                Timer++;
                if (progress > 0.5f)
                {
                    float lerpValue = (progress - .5f) / .5f;
                    float homingSpeed = EaseOutExpo(lerpValue) * 21f;
                    Vector2 orb = Projectile.Center.GetNormalVector2(CurTarget.Center);
                    Projectile.rotation = Projectile.rotation.AngleTowards(orb.ToRotation(), .5f);
                    Projectile.HomingTarget(CurTarget.Center, -1, homingSpeed, 20);
                }
                else
                {
                    float lerpValue = (progress) / 0.5f;
                    float pro = EaseOutBack(lerpValue);
                    Projectile.velocity = -Projectile.Center.GetNormalVector2(CurTarget.Center) * 3.5f * pro;
                }
                if (Main.rand.NextBool(4))
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(22, 2), Projectile.SafeDir(), RandLerpColor(Color.DodgerBlue, Color.White), 45, 1, 0.55f * Main.rand.NextFloat(.85f, 1.15f), 0.2f);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            //挂机状态下干掉冰块的速度更新，我们用世界插值来精确控制他
            return AttackState != State.Idle;

        }
        public override bool? CanHitNPC(NPC target)
        {
            if (AttackState == State.Shoot)
                return null;
            if (AttackState == State.Strike && CurTarget.IsLegal() && CurTarget.Equals(target))
                return null;
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //1.4.5更新之后射弹的绘制顺序将会修改一下
            //这里是因为我像素化渲染的系统问题，绘制的图层是在proj上方的，然后我自己也没时间改（
            overWiresUI.Add(index);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Strike)
            {
                ScarletSound(HJScarletSounds.ASMD_IceBlockSplit, Projectile.Center, 0.45f, 1, 0.3f, 0.2f);
                Projectile.AddExecutionTimeDelayed(ItemType<ASMD>());
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(3), Projectile.SafeDir().ToRandVelocity(ToRadians(20), 1, 14f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 45, 1, 0.95f * Main.rand.NextFloat(0.85f, 1.15f), .2f);
                }
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(3), Projectile.SafeDir().ToRandVelocity(ToRadians(20), 1, 21), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 45, RandRotTwoPi, 1, 0.45f * Main.rand.NextFloat(.85f, 1.15f), false, BlendState.AlphaBlend);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D iceBlock = Projectile.GetTexture();
            Rectangle iceBlockFrame = iceBlock.Frame(1, 6, 0, BlockType);
            Vector2 posBase = Projectile.Center - Main.screenPosition;
            DrawIceBlock(iceBlock, iceBlockFrame, posBase);
            if (CurTarget.IsLegal())
            {
                SB.EnterShaderArea();
                Texture2D crossGlow = HJScarletTexture.Particle_CrossGlow.Value;
                Vector2 pos = CurTarget.Center - Main.screenPosition;
                SB.Draw(crossGlow, pos, null, Color.CornflowerBlue * (1), 0, crossGlow.Size() / 2, Projectile.scale * .24f, 0, 0);
                SB.Draw(crossGlow, pos, null, Color.White * (1), 0, crossGlow.Size() / 2, Projectile.scale * .24f * .90f, 0, 0);
                SB.EndShaderArea();

            }
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            if (!CurTarget.IsLegal())
                return;
            float maxTime = TotalStrikeTime;
            float progress = Utils.GetLerpValue(0, maxTime, Timer, true);
            float distance = (CurTarget.Center - Projectile.Center).Length();
            Vector2 normalVec = Projectile.Center.GetNormalVector2(CurTarget.Center);
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawBeam(sb, distance, normalVec, progress, Color.CornflowerBlue, HJScarletTexture.Trail_Lightning4.Texture, 1.2f, 1.0f);
            DrawBeam(sb, distance, normalVec, progress, Color.RoyalBlue, HJScarletTexture.Trail_Lightning4.Texture, 1.2f, 0.75f);
            DrawBeam(sb, distance, normalVec, progress, Color.White, HJScarletTexture.Trail_Lightning4.Texture, 1.2f, 0.50f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawBeam(SpriteBatch sb, float distance, Vector2 normalVec, float progress, Color c, Asset<Texture2D> trail, float timeMult = 1f, float heightMult = 1)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(distance, trail.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -90f * timeMult);
            shader.Parameters["uColor"].SetValue(c.ToVector4() * .85f * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(.02f);
            shader.Parameters["uFadeinLength"].SetValue(.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            Vector2 orig = new Vector2(0, trail.Height() / 2);
            float xScale = distance / trail.Width();
            sb.Draw(trail.Value, Projectile.Center - Main.screenPosition, null, c, normalVec.ToRotation(), orig, new Vector2(xScale * Projectile.scale, heightMult * progress * 0.40f), SpriteEffects.None, 0);
        }
        /// <summary>
        /// 冰块
        /// </summary>
        public void DrawIceBlock(Texture2D tex, Rectangle rec, Vector2 pos)
        {
            float scale2 = Projectile.scale * 1.13f;
            if (AttackState == State.Strike)
            {
                float maxTime = TotalStrikeTime;
                float progress = Utils.GetLerpValue(0, maxTime, Timer, true);

            }
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(tex, pos + (TwoPi / 16f * i).ToRotationVector2() * 2, rec, Color.White.ToAddColor(), Projectile.rotation, rec.Size() / 2, scale2, 0, 0);

            }

            SB.Draw(tex, pos, rec, Color.White, Projectile.rotation, rec.Size() / 2, scale2, 0, 0);
        }
    }
}
