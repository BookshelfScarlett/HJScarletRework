using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class FrostoftheStormExecution : HJScarletProj
    {
        public override string Texture => GetInstance<FrostoftheStormHeldProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public AnimationStruct Helper = new(3);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 0.95f;
        public bool IsWavingAttack = false;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.extraUpdates, 5 * Projectile.extraUpdates);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.SetupImmnuity(60);
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 10;
            Projectile.penetrate = 1;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = (int)(AttackSpeed * 0.25f);
            Helper.MaxProgress[1] = (int)(AttackSpeed);
            Helper.MaxProgress[2] = (int)(AttackSpeed);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            HandleHeldProjState();
            HandleAttackAnimation();
            HandlePlayerState();
            if (StarShapeLifeTime > 0)
                StarShapeLifeTime -= 1;
        }
        public void HandleHeldProjState()
        {
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public void HandlePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation - ToRadians(90));
        }

        public void HandleAttackAnimation()
        {
            //跳过末尾动画，如果玩家试图一直攻击的话
            if (!Helper.IsDone[0])
                UpdateBeginAnimation();
            else if (!Helper.IsDone[1])
                UpdateMidAnimation();
            else
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<FrostoftheStormChargeProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                //改向
                ((FrostoftheStormChargeProj)proj.ModProjectile).Flip = false;
                //存储当前挥舞角度
                ((FrostoftheStormChargeProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;
                ((FrostoftheStormChargeProj)proj.ModProjectile).CurTime += 0;
                Projectile.Kill();
            }
        }
        public int StarShapeLifeTime = 0;
        public void UpdateEndAnimation()
        {
            if (Helper.Progress[2] % (AttackSpeed / 3) == 0)
            {
                StarShapeLifeTime = 10 * Projectile.extraUpdates;
            }
            Helper.UpdateAniState(2);
        }
        public bool HalfWay = false;
        public void UpdateMidAnimation()
        {
            Vector2 crystalDir = Projectile.SafeDirByRot();
            if (Helper.OnAnimationBegin(1))
            {
                SoundEngine.PlaySound(HJScarletSounds.Frostwave_Release with { Variants = [1], MaxInstances = 0, Pitch = 0.2f });
                Vector2 initPos = crystalDir * Projectile.scale * (50f) + Projectile.Center;
                for (int i = 0; i < 30; i++)
                {
                    new SnowCloud(initPos, crystalDir * Main.rand.NextFloat(20f, 64f), RandLerpColor(Color.RoyalBlue, Color.LightBlue), 20, 0, 0.32f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * 0.3f).Spawn();
                }
            }
            Helper.UpdateAniState(1);
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            if (!HalfWay && easedProgress > 0.85f)
            {
                SoundEngine.PlaySound(HJScarletSounds.Frostwave_Release with { Variants = [1], MaxInstances = 0, Pitch = 0.4f });
                Vector2 initPos = crystalDir * Projectile.scale * (50f) + Projectile.Center;
                for (int i = 0; i < 30; i++)
                {
                    new SnowCloud(initPos, crystalDir * Main.rand.NextFloat(20f, 60f), RandLerpColor(Color.RoyalBlue, Color.LightBlue), 20, 0, 0.32f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * 0.3f).Spawn();
                }

                HalfWay = true;
            }
            float beginAngle = 175;
            float endAngle = 185;
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.2f, Height, 1f);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            Vector2 crystalPos = crystalDir * Projectile.scale * (Main.rand.NextFloat(0f, 100f)) + Projectile.Center;
            if (Main.rand.NextBool(8))
                new SnowCloud(crystalPos.ToRandCirclePos(30), crystalDir * Main.rand.NextFloat(10f, 40f), RandLerpColor(Color.WhiteSmoke, Color.RoyalBlue), 40, RandRotTwoPi, 0.35f, Main.rand.NextFloat(0.7f, 1.1f) * .35f).Spawn();
            crystalPos = crystalDir * Projectile.scale * (50f) + Projectile.Center;
            Vector2 posBase = crystalPos;
            Vector2 starShapeSpawnPos = posBase + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(120f, 190f);
            Vector2 starShapeDir = (posBase - starShapeSpawnPos).SafeNormalize(Vector2.UnitX);
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(7))
                {
                    Vector2 vel = starShapeDir * Main.rand.NextFloat(12f, 24);
                    float scale = Main.rand.NextFloat(0.7f, 0.91f) * Projectile.scale * 0.12f;
                    new HRShinyOrb(starShapeSpawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, scale, 0.8f).Spawn();
                    new HRShinyOrb(starShapeSpawnPos, vel, Color.White, 40, scale * 0.5f, 0.8f).Spawn();
                }
                if (Main.rand.NextBool(5))
                {
                    starShapeSpawnPos = posBase + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(120f, 190f);
                    starShapeDir = (posBase - starShapeSpawnPos).SafeNormalize(Vector2.UnitX);
                    new ShinyCrossStar(starShapeSpawnPos, starShapeDir * Main.rand.NextFloat(0.8f, 8f), Color.Lerp(Color.RoyalBlue, Color.SkyBlue, Main.rand.NextFloat()), 40, 0, 1, 0.8f * Main.rand.NextFloat(0.8f, 1.1f), false).Spawn();
                }
                if (Main.rand.NextBool(3))
                {
                    starShapeSpawnPos = posBase + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(100f, 160f);
                    starShapeDir = (posBase - starShapeSpawnPos).SafeNormalize(Vector2.UnitX);
                    new SnowCloud(starShapeSpawnPos, starShapeDir * Main.rand.NextFloat(2f, 12f), RandLerpColor(RandLerpColor(Color.RoyalBlue, Color.SkyBlue), Color.White), 45, RandRotTwoPi, 0.35f, Main.rand.NextFloat(0.4f, 0.7f) * 0.2f, true).SpawnToPriority();
                }
            }
        }


        public void UpdateBeginAnimation()
        {
            //这里挥砍动画一定程度上使用了矩阵变化。
            Helper.UpdateAniState(0);
            float easedProgress = EaseInCubic(Helper.GetAniProgress(0));
            //末尾角度，也是下一个动画进程的起始角度
            float endAngle = 175f;
            float beginAngle = 150f;
            //更新当前的转角
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            //将其投影到矩阵上，并进行形变
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1.2f, Height, 1f);
            //而后再转化为射弹的目标指向，这个tarPos同时拥有指向和武器模长的信息。而不是一个单位向量
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.2f;
            //这样，Scale就是tarPos的向量模长
            Projectile.scale = tarPos.Length();
            //武器的角度为（起始角度 + 目标角度）的值
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < 0.01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            DrawSword();
            return false;
        }
        public void DrawSword()
        {
            //基础的一些数据
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + PiOver2;
            Vector2 drawPoint = new Vector2(tex.Width * 0.5f, tex.Height * 0.85f);

            bool ignoreFlip = Owner.direction > 0;
            SpriteEffects se = Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (ignoreFlip)
            {
                se = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -12;
            Color edgeColor;
            edgeColor = Color.Lerp(Color.White, Color.Transparent, Helper.GetAniProgress(2));
            //绘制残影
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, realDrawPos + ToRadians(360f / 8 * i).ToRotationVector2() * 2f, null, edgeColor.ToAddColor(), drawRot, drawPoint, Projectile.scale, se, 0);
            SB.Draw(tex, realDrawPos, null, Color.White, drawRot, drawPoint, Projectile.scale, se, 0);
            SB.EnterShaderArea();
            tex = HJScarletTexture.Particle_CrossGlow.Value;
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            float progress = Clamp(easedProgress, 0f, .91f);
            SB.Draw(tex, realDrawPos + Projectile.SafeDirByRot() * 60f * Projectile.scale, null, Color.RoyalBlue, ToRadians(0f), tex.ToOrigin(), Projectile.scale * 0.31f * progress, se, 0);
            SB.Draw(tex, realDrawPos + Projectile.SafeDirByRot() * 60f * Projectile.scale, null, Color.White, ToRadians(0f), tex.ToOrigin(), Projectile.scale * 0.28f * progress, se, 0);
            SB.EndShaderArea();
            SB.EndShaderArea();
        }
        public override bool ShouldUpdatePosition() => false;
    }
}
