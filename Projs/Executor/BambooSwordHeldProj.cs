using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class BambooSwordHeldProj : ExecutorHeldProj
    {
        public override string Texture => GetInstance<BambooSword>().Texture;
        public override int OriginalItemID => ItemType<BambooSword>();
        public AnimationStruct Helper = new AnimationStruct(2);
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float Width = 1.2f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 3;
            Projectile.ownerHitCheck = true;
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
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        public void UpdateAnimation()
        {
            if (StopTiming > 0)
            {
                StopTiming--;
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
                if (easedProgress >= 0.95f)
                    return;
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .81f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    Dust d = Dust.NewDustPerfect(pos, DustID.JungleSpore);
                    d.velocity = vel;
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(.9f, 1.1f);
                    //ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.3f);
                }
                if (Main.rand.NextBool(7))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .8f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.LightGreen, Color.LawnGreen), 40, 1, 0.75f, 0.23f, blendstate: BlendState.NonPremultiplied);
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
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 70;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 34f, ref _);
            return c;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 dir = Owner.Center.GetNormalVector2(target.Center);
            //下方找了AI

            // 计算鼠标相对于敌人中心的高度差（鼠标在敌人上方时为正）
            float mouseHeightOffset = target.Center.Y - Main.MouseWorld.Y;

            // 完全挑飞所需的最大高度差（可根据手感调整）
            float maxHeightOffset = 200f;

            // 插值因子：鼠标越高，越偏向垂直向上
            float t = Math.Clamp(mouseHeightOffset / maxHeightOffset, 0f, 1f);

            // 垂直向上方向（Terraria 中 Y 轴向下，所以向上是 -Vector2.UnitY）
            Vector2 upDir = -Vector2.UnitY;

            // 最终击退方向
            Vector2 finalDir = Vector2.Lerp(dir, upDir, t).ToSafeNormalize();

            // 击退速度：鼠标越高，力度越大（从 10 到 18）
            float speed = Lerp(10f, 18f, t);
            //if (!target.boss)
            {
                if (Helper.GetAniProgress(0) < .98f)
                {
                    if (target.type == NPCID.DungeonGuardian)
                        target.HJScarlet().PostSpeed = finalDir * speed * 50f;
                    else
                        target.HJScarlet().PostSpeed = finalDir * speed;
                }
                target.HJScarlet().StopNpcTime = 10;
            }
            if (Projectile.numHits < 1)
            {
                StopTiming = 10;
                ScreenShakeSystem.AddScreenShakes(target.Center, 12, 12, Owner.Center.GetNormalVector2(target.Center).ToRotation(), 0, easingFunc: EaseOutBack);
                ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, pitch: -.64f, pitchVariance: .1f, variantType: 1);
                for (int i = 0; i < 26; i++)
                {
                    Vector2 vel = finalDir.ToRandVelocity(ToRadians(35f), .1f, 19f);
                    ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * 10f, RandVelTwoPi(.3f, 14f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
                }
                for (int i = 0; i < 20; i++)
                {
                    ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6) + finalDir * 4f, RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                    Dust d = Dust.NewDustPerfect(target.Center, DustID.JungleTorch);
                    d.velocity = RandVelTwoPi(1.2f, 6.2f) + finalDir * 3f;
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1.2f, 1.61f);
                }
            }
            if (!Owner.HasProj<BambooSwordSpin>())
                Projectile.AddExecutionTimeImmediate(OriginalItemID);
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
            int length = Projectile.oldPos.Length - 2;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1 - i / (float)length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i] + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
                float opac = Lerp(0.05f, 1f, ratios) * .30f;
                Color c = Color.Lerp(Color.LimeGreen, Color.White, ratios).ToAddColor(75);
                SB.FastDraw(tex, pos, c * opac, rot, rotationPoint, Projectile.scale, flipSprite);
            }
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
}
