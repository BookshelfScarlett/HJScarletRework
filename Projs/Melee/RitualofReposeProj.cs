using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class RitualofReposeProj : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override string Texture => GetInstance<RitualofRepose>().Texture;
        public bool BeginDisapper = false;
        public bool BeginAppear = false;
        public bool JustStartAttacked = false;
        public float AppearRatios = 0;
        public float ReadyAttackFrame = 2000;
        public enum State
        {
            Idle,
            Shoot
        }
        public ref float IdleTimer => ref Projectile.ai[2];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 3;
            Projectile.SetupImmnuity(30);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Shoot:
                    DoShoot();
                    break;
            }
        }

        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(9))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(28), Projectile.velocity / 8f, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 45, 1, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .42f, 0);
            if (Main.rand.NextBool(7))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 8f, RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 45, 1, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .6f);
        }
        public float Osci = 0;
        public void DoIdle()
        {

            //渐变出现
            if (!BeginAppear && !BeginDisapper)
            {
                if (AppearRatios == 0)
                    ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center);
                Projectile.timeLeft = 10;
                if (Projectile.FinalUpdate())
                    AppearRatios += .1f;
                if (AppearRatios > 1f)
                {
                    BeginAppear = true;
                    AppearRatios = 1f;
                }
            }
            //渐变死亡
            if (BeginDisapper)
            {
                if (AppearRatios >= 0)
                {
                    Projectile.timeLeft = 10;
                    if (Projectile.FinalUpdate())
                        AppearRatios -= 0.05f;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            //只有完全出现的时候，才开始更新玩家的状态
            if (BeginAppear)
            {
                if (Owner.CanUseHoldout(ItemType<RitualofRepose>()))
                {
                    IdleTimer = GetSeconds(5) * Projectile.MaxUpdates;
                    Projectile.timeLeft = 10;
                }
                else
                {
                    IdleTimer--;
                    if (IdleTimer <= 0)
                    {
                        //死亡的时候给一个特效
                        ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center, pitch: -.4f);
                        BeginAppear = false;
                        BeginDisapper = true;
                    }
                }
            }
            //挂载
            Osci += ToRadians(0.5f);
            float yMult = (30f * MathF.Sin(Osci) / 9f) + 30f;
            float xPos = Owner.MountedCenter.X - 40f * Owner.direction;
            float yPos = Owner.MountedCenter.Y - yMult;
            Vector2 mountedPos = new(xPos, yPos);
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, .1f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = ToRadians(-105f);
            if (Owner.direction > 0)
                angleToWhat = ToRadians(-75f);
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);

            //下方为粒子特效
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePos(45 * Projectile.scale, 110 * Projectile.scale), Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(.2f, 1f) * 4f, RandLerpColor(Color.White, Color.Gold), Main.rand.Next(10, 46), 1, Main.rand.NextFloat(.65f, 1.1f) * .15f, 0f);

            //待一切尘埃落定，我们会给这个世界献上带来安息
            if (Owner.CanUseHoldout(ItemType<RitualofRepose>()) && Owner.JustPressLeftClick() && !JustStartAttacked)
            {
                JustStartAttacked = true;
                InitAttack();
            }
            if (JustStartAttacked)
            {

                //更新动画进程，这里控制的，其实是手臂的动画
                if (!Helper.IsDone[0])
                {
                    Helper.UpdateAniState(0);
                    float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
                    if (LockDirection > 0)
                    {
                        float curRot = Helper.ToCurAnimationRot(-120, -150, Owner.direction, true, easedProgress);
                        Vector2 tarPos = curRot.ToTargetPosByMartix(1, 1, 1);
                        ArmRotation = tarPos.ToRotation() + TargetRotation;
                    }
                    else
                    {
                        float curRot = Helper.ToCurAnimationRot(-120, -150, Owner.direction, true, easedProgress);
                        Vector2 tarPos = curRot.ToTargetPosByMartix(1, 1, 1);
                        ArmRotation = tarPos.ToRotation() + TargetRotation;

                    }
                }
                else if (!Helper.IsDone[1])
                {
                    Helper.UpdateAniState(1);
                    float easedProgress = EaseOutCubic(Helper.GetAniProgress(1));
                    if (LockDirection > 0)
                    {
                        float curRot = Helper.ToCurAnimationRot(-150, 100, Owner.direction, true, easedProgress);
                        Vector2 tarPos = curRot.ToTargetPosByMartix(1, 1, 1);
                        ArmRotation = tarPos.ToRotation() + TargetRotation;
                    }
                    else
                    {
                        float curRot = Helper.ToCurAnimationRot(-150, 100, Owner.direction, true, easedProgress);
                        Vector2 tarPos = curRot.ToTargetPosByMartix(1, 1, 1);
                        ArmRotation = tarPos.ToRotation() + TargetRotation;
                    }
                }
                else
                {
                    Projectile.MaxUpdates = 4;
                    Projectile.timeLeft = GetSeconds(5) * Projectile.MaxUpdates;
                    //准备完毕，向上。
                    ChargeReady();
                    AttackState = State.Shoot;
                }
                //占用玩家攻击
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.ChangeDir(LockDirection);
                Owner.ControlPlayerArm(ArmRotation);
            }
        }

        public void ChargeReady()
        {

        }

        public AnimationStruct Helper = new AnimationStruct(2);
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(GetInstance<RitualofRepose>().Item, GetInstance<RitualofRepose>().Item.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public float TargetRotation = 0;
        public float ArmRotation = 0;
        public int LockDirection = 0;
        public void InitAttack()
        {
            Helper.MaxProgress[0] = (int)(AttackSpeed * .75f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .25f);
            LockDirection = ((Owner.LocalMouseWorld().X - Owner.MountedCenter.X) > 0).ToDirectionInt();
            TargetRotation = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
        }
        public BlendState BlendState => BlendState.Additive;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int length = Projectile.oldPos.Length;
            Texture2D trailTex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Rectangle curSrc = trailTex.Bounds;
            curSrc.Width = (int)(curSrc.Width * 0.8f);
            Vector2 cutSrcOri = new Vector2(curSrc.Width / 2f, curSrc.Height / 2f);
            Vector2 sharpScale = new Vector2(0.7f, 0.60f);
            float glowSpreadMult = Clamp(Math.Abs(MathF.Sin((float)Main.timeForVisualEffects / 100f)), 0.35f, 1f);
            float generalGlow = AppearRatios;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1f - i / (float)length;
                Vector2 pos = Projectile.Center - Main.screenPosition;
                Color trailColor = Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, ratios).ToAddColor(100);
                float scale = Lerp(.44f, 1f, ratios);
                float opacity = Lerp(.51f, .7f, ratios);
                Vector2 sharpPos = pos - new Vector2(15, 2).RotatedBy(Projectile.rotation);
                float trailRot = Projectile.rotation;
                SB.Draw(trailTex, sharpPos, curSrc, trailColor * opacity * glowSpreadMult * generalGlow, trailRot, cutSrcOri, sharpScale * scale, 0, 0);
            }
            //十字光晕
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 glowPos = drawPos + Projectile.rotation.ToRotationVector2() * 25f * Projectile.scale;
            Vector2 glowScale = Projectile.scale * .48f * new Vector2(.75f, 1.35f);

            SB.FastDraw(glow, glowPos, Color.DarkGoldenrod * .65f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale, 0);
            SB.FastDraw(glow, glowPos, Color.Gold * .35f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale * .98f, 0);
            SB.FastDraw(glow, glowPos, Color.White * .45f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale * .95f, 0);

            //环状光晕
            glow = HJScarletTexture.Particle_RingShiny.Value;
            glowScale = Projectile.scale * .16f * Vector2.One;

            SB.FastDraw(glow, glowPos, Color.DarkGoldenrod * .45f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale, 0);
            SB.FastDraw(glow, glowPos, Color.Gold * .4f * generalGlow, Projectile.rotation + Pi, glow.Size() / 2f, glowScale * .98f, 0);
            SB.FastDraw(glow, glowPos, Color.White * .5f * generalGlow, Projectile.rotation + PiOver2, glow.Size() / 2f, glowScale * .95f, 0);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GD.Textures[0] = projTex;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            GD.Textures[1] = HJScarletTexture.Noise_Misc.Value;
            GD.SamplerStates[1] = SamplerState.PointClamp;
            //应用这个shader，我们正式开始画这把“喷火器”
            Effect shader = HJScarletShader.EdgeMeltsShader;
            shader.Parameters["progress"].SetValue((1 - AppearRatios));
            shader.Parameters["InPutTextureSize"].SetValue(projTex.Size());
            shader.Parameters["EdgeColor"].SetValue(Color.Gold.ToVector4());
            shader.Parameters["EdgeWidth"].SetValue(.01f);
            shader.CurrentTechnique.Passes[0].Apply();

            float rotFixer = PiOver4;
            float spearRotation = Projectile.rotation + rotFixer;
            SB.FastDraw(projTex, drawPos, Color.White, spearRotation, ori, Projectile.scale, SpriteEffects.None);
            SB.EndShaderArea();
            return false;
        }

    }
}