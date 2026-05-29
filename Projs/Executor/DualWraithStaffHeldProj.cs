using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DualWraithStaffHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<DualWraithStaff>();
        public override string Texture => HJScarletItemProj.DualWraithStaff.Path;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public AnimationStruct Helper = new(3);
        public ref float Timer => ref Projectile.ai[0];
        public bool IsAFK = false;
        public bool OnExecutionNow = false;
        public Vector2 OrbPosition = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
            Projectile.width = Projectile.height = 3;
            Projectile.noEnchantmentVisuals = true;
            Projectile.Opacity = 0;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 1 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 20 * Projectile.MaxUpdates;
            OrbPosition = Projectile.Center;
        }
        public override void ProjAI()
        {
            if (CheckOwnerDead())
                return;
            UpdateAniStatement();
            UpdatePlayerState();
            Projectile.netUpdate = true;
        }

        public void UpdateAniStatement()
        {
            if (!Helper.IsDone[0])
                UpdateBeginAnimation();
            else if (!Helper.IsDone[1])
                UpdateMidAnimation();
            else
            {
                if (IsAFK)
                {
                    //一旦执行了afk动画进程，直到射弹完全确定死亡了，我们才允许玩家召唤下一把武器
                    UpdateAFKAnimation();
                    return;
                }
                if (Main.rand.NextBool(12))
                    if (!Owner.channel)
                        new SmokeParticle(Projectile.Center.ToRandCirclePos(5f) + Projectile.SafeDirByRot() * Main.rand.NextFloat(-10f, 80f), Projectile.SafeDirByRot() * Main.rand.NextFloat(), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.60f, 0.24f, Main.rand.NextBool()).Spawn();
                if (Owner.CheckExecution(OriginalItemID) && !Projectile.HJScarlet().ExecutionStrike)
                {
                    Projectile.HJScarlet().ExecutionStrike = true;
                    Owner.RemoveExecutionProgress(OriginalItemID);
                    Timer = 0;
                    OnExecutionNow = true;
                }
                if(OnExecutionNow)
                {
                    UpdateExecutionStrikeAni();
                    return;
                }

                if (Owner.channel)
                    UpdateAttack();
                else
                    UpdateHeldState();
            }
        }
        public void UpdateExecutionStrikeAni()
        {
            if (Timer == 0)
            {

                SoundEngine.PlaySound(HJScarletSounds.Misc_AirFlowAlt with { MaxInstances = 0, Pitch = .5f });
                int count = 50;
                for (int i = 0; i < count; i++)
                {
                    Vector2 spanwPos = Projectile.Center.ToRandCirclePos(10f);
                    new SnowCloud(spanwPos, RandVelTwoPi(1f, 10f), RandLerpColor(Color.WhiteSmoke, Color.White), 30, RandRotTwoPi, 0.25f, 0.08f, false).SpawnToPriority();
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 vel = Vector2.UnitY.ToRandVelocity(ToRadians(30f), 1f, 18);
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(5f), vel, RandLerpColor(Color.WhiteSmoke, Color.White), 30, RandRotTwoPi, 0.30f, 0.4f, true).Spawn();
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 vel = Vector2.UnitY.ToRandVelocity(ToRadians(10f), 1f, 12);
                    new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePos(100f), 3.2f, Color.WhiteSmoke, 100, 0.12f, RandRotTwoPi).Spawn();
                }

            }

            Timer++;
            float executeProgress = Clamp(Timer / (25f * Projectile.MaxUpdates), 0, 1f);
            if (executeProgress < .5f)
            {
                Oscillation = Projectile.ClampOscillation(Oscillation, 3f);
                //锤子应当朝向的位置
                float anchorPosX = Owner.MountedCenter.X - Owner.direction * 80f;
                float anchorPosY = Owner.MountedCenter.Y - (50f * MathF.Sin(Oscillation) / 9f) + 30f;
                //递增的值越大，锤子的摆动幅度越大
                //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
                Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
                //实际更新位置
                float scaleValue = Lerp(0.05f, 0f, Helper.GetAniProgress(1));
                float lerpValue = (0.55f + scaleValue) / Projectile.MaxUpdates;
                Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, lerpValue);
                float angleToWhat = (-Vector2.UnitY).ToRotation();
                Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.48f / Projectile.MaxUpdates);

            }
            else
            {
                if (Projectile.velocity.LengthSquared() < 4f)
                {
                    SoundEngine.PlaySound(HJScarletSounds.Air_HeavyFlow with { MaxInstances = 0, Pitch = 0.4f });
                    for (int i = 0; i < 30; i++)
                    {
                        new SmokeParticle(Projectile.Center + Vector2.UnitY * Main.rand.NextFloat(-280f, 11f), Vector2.UnitY.ToRandVelocity(ToRadians(10f), 1f, 12f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.50f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.9f, 1.1f) * 0.6f * Projectile.scale, Main.rand.NextBool()).SpawnToPriority();
                    }
                }
                Projectile.velocity = -Vector2.UnitY * 24f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Vector2 spawnPos = Projectile.Center + RandVelTwoPi(1f, 68f);
                Vector2 dir = Projectile.Center.GetNormalVector2(spawnPos);
                float scale = Projectile.scale * 0.6f;
                new SmokeParticle(spawnPos, dir * Main.rand.NextFloat(1f, 1.3f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.30f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.9f, 1.1f) * scale, Main.rand.NextBool()).SpawnToPriority();
                if (Main.rand.NextBool(8))
                    new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePos(100f), 3.2f, Color.WhiteSmoke, 100, 0.2f, RandRotTwoPi).Spawn();
            }
            
            if (Main.rand.NextFloat() < executeProgress && Main.rand.NextBool(8))
            {

                Vector2 pos = Projectile.Center.ToRandCirclePos(5f) + Projectile.SafeDirByRot() * Main.rand.NextFloat(-10f, 80f) + Projectile.SafeDirByRot().RotatedBy(PiOver2) * Main.rand.NextFloat(-20f, 21f);
                Vector2 dirMulter = Projectile.SafeDirByRot() * executeProgress;
                new SmokeParticle(pos, dirMulter * Main.rand.NextFloat(0.8f, 3.2f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.80f, 0.24f, Main.rand.NextBool()).Spawn();
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = pos;
                        p.Velocity = dirMulter * Main.rand.NextFloat(1.8f, 4.2f);
                        p.DrawColor = Color.White;
                        p.Scale = Main.rand.NextFloat(0.8f, 1.2f) * .1f;
                        p.Opacity = Main.rand.NextFloat(0.68f, 0.9f);
                        p.Lifetime = 40;
                        p.GlowCenterMult = 0.5f;
                    });
            }
            if (executeProgress == 1)
            {
                ((DualWraithStaff)Owner.HeldItem.ModItem).AlterVersion = true;
                for (int i = 0; i < 60; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(5f) + Projectile.SafeDirByRot() * Main.rand.NextFloat(-10f, 80f) + Projectile.SafeDirByRot().RotatedBy(PiOver2) * Main.rand.NextFloat(-20f, 21f);
                    Vector2 dirMulter = Projectile.SafeDirByRot() * executeProgress;
                    new SmokeParticle(pos, dirMulter * Main.rand.NextFloat(0.8f, 3.2f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.80f, 0.24f, Main.rand.NextBool()).Spawn();
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                        {
                            p.Position = pos;
                            p.Velocity = dirMulter * Main.rand.NextFloat(1.8f, 4.2f);
                            p.DrawColor = Color.White;
                            p.Scale = Main.rand.NextFloat(0.8f, 1.2f) * .1f;
                            p.Opacity = Main.rand.NextFloat(0.68f, 0.9f);
                            p.Lifetime = 40;
                            p.GlowCenterMult = 0.5f;
                        });

                }
                Projectile.Kill();
            }
        }


        public void UpdateAFKAnimation()
        {
            float maxAFKTime = Projectile.MaxUpdates * 65;
            if (Timer == 0)
            {
                SoundEngine.PlaySound(HJScarletSounds.Air_HeavyFlow with { MaxInstances = 0 });
                for (int i = 0; i < 30; i++)
                {
                    new SmokeParticle(Projectile.Center + Vector2.UnitY * Main.rand.NextFloat(-280f, 11f), Vector2.UnitY.ToRandVelocity(ToRadians(10f),1f, 12f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.50f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.9f, 1.1f) * 0.6f * Projectile.scale, Main.rand.NextBool()).SpawnToPriority();
                }
            }
            Timer++;
            float progress = Clamp(Timer / maxAFKTime, 0f, 1f);
            float eased = EaseInBack(progress);
            Projectile.velocity = Vector2.Lerp(-Vector2.UnitY * 12f, (-Vector2.UnitY* .02f), eased);
            Projectile.rotation = Projectile.velocity.ToRotation();
            {
                Vector2 spawnPos = Projectile.Center + RandVelTwoPi(1f, 68f);
                Vector2 dir = Projectile.Center.GetNormalVector2(spawnPos);
                float scale = Projectile.scale * 0.6f;
                new SmokeParticle(spawnPos, dir * Main.rand.NextFloat(1f, 1.3f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.30f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.9f, 1.1f) * scale, Main.rand.NextBool()).SpawnToPriority();
                if(Main.rand.NextBool(8))
                    new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePos(100f), 3.2f, Color.WhiteSmoke, 100, 0.2f, RandRotTwoPi).Spawn();
            }
            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, progress);
            if (Projectile.Opacity <= 0.02f)
                Projectile.Kill();
        }
        #region 第一段动画
        public void UpdateBeginAnimation()
        {
            Vector2 spawnPos = Projectile.Center + RandVelTwoPi(1f, 68f);
            Vector2 dir = Projectile.Center.GetNormalVector2(spawnPos);
            float scale = Projectile.scale * 0.6f;
            new SmokeParticle(spawnPos, dir * Main.rand.NextFloat(1f, 1.3f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 0.30f * Main.rand.NextFloat(0.5f, 1f), Main.rand.NextFloat(0.9f, 1.1f) * scale, Main.rand.NextBool()).SpawnToPriority();

            Helper.UpdateAniState(0);
            float angleToWhat = (-Vector2.UnitY).ToRotation();
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.48f / Projectile.MaxUpdates);
        }
        #endregion

        #region 第二段动画
        public void UpdateMidAnimation()
        {
            OnMidInit();
            if (Projectile.Opacity < Main.rand.NextFloat(0.1f, 1f))
            {
                Vector2 vel = Vector2.UnitY.ToRandVelocity(ToRadians(15f), 1f, 18);
                new SmokeParticle(Projectile.Center.ToRandCirclePos(5f) - Vector2.UnitY * Main.rand.NextFloat(8f, 120f), vel, RandLerpColor(Color.WhiteSmoke, Color.White), 30, RandRotTwoPi, 0.30f, 0.4f, true).Spawn();
            }
            Projectile.Opacity = Helper.GetAniProgress(1);
            UpdateHeldState();
            Helper.UpdateAniState(1);
        }
        public void OnMidInit()
        {
            if (Helper.OnAnimationBegin(1))
            {
                SoundEngine.PlaySound(HJScarletSounds.Misc_AirCharge with { MaxInstances = 0, Pitch = -0.5f });
                int count = 50;
                for (int i = 0; i < count; i++)
                {
                    Vector2 spanwPos = Projectile.Center.ToRandCirclePos(10f);
                    new SnowCloud(spanwPos, RandVelTwoPi(1f, 10f), RandLerpColor(Color.WhiteSmoke, Color.White), 30, RandRotTwoPi, 0.25f, 0.08f, false).SpawnToPriority();
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 vel = Vector2.UnitY.ToRandVelocity(ToRadians(30f), 1f, 18);
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(5f), vel, RandLerpColor(Color.WhiteSmoke, Color.White), 30, RandRotTwoPi, 0.30f, 0.4f, true).Spawn();
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 vel = Vector2.UnitY.ToRandVelocity(ToRadians(10f), 1f, 12);
                    new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePos(100f), 3.2f, Color.WhiteSmoke, 100, 0.12f, RandRotTwoPi).Spawn();
                }
            }

        }
        #endregion


        public float Oscillation = 0;
        public float OrbOscillation = 0;
        public void UpdateHeldState()
        {
            Oscillation += ToRadians(3f) / Projectile.MaxUpdates;
            if (Oscillation > ToRadians(360f))
                Oscillation = ToRadians(-360f);
            
            //锤子应当朝向的位置
            float anchorPosX = Owner.MountedCenter.X - Owner.direction * 80f;
            float anchorPosY = Owner.MountedCenter.Y - (50f * MathF.Sin(Oscillation) / 9f) + 30f;
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            float scaleValue = Lerp(0.05f, 0f, Helper.GetAniProgress(1));
            float lerpValue = (0.15f +scaleValue) / Projectile.MaxUpdates;
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, lerpValue);
            float angleToWhat = (-Vector2.UnitY).ToRotation();
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.48f/ Projectile.MaxUpdates);

        }

        public void UpdateAttack()
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter, 0.81f);
            Projectile.velocity = Owner.ToMouseVector2();
            Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.48f / Projectile.MaxUpdates);
            Owner.ControlPlayerArm(Projectile.rotation);
            Owner.ChangeDir(Projectile.direction);
            Owner.itemAnimation = Owner.itemTime = 2;

            Timer++;
            if (Timer < AttackSpeed / 3)
                return;
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeExpired with { MaxInstances = 0, Pitch = -0.4f, PitchVariance = 0.1f, Volume = 0.6f });
            Timer = 0;
            Vector2 spawnPos = Projectile.Center + Projectile.SafeDirByRot() * 70f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, Projectile.velocity.ToRandVelocity(ToRadians(0f), 12f, 18f), ProjectileType<DualWraithStaffGhost>(), Projectile.damage, 2f, Owner.whoAmI);
            proj.extraUpdates = 2;
            proj.HJScarlet().HasExecutionMechanic = true;
            for (int i = 0; i < 2; i++)
            {
                proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, Projectile.velocity.ToRandVelocity(ToRadians(10f), 12f, 18f), ProjectileType<DualWraithStaffGhost>(), Projectile.damage, 2f, Owner.whoAmI);
                proj.extraUpdates = 2;
                proj.HJScarlet().HasExecutionMechanic = true;
                for (int j = 0; j < 6; j++)
                    new SmokeParticle(spawnPos, proj.velocity.ToRandVelocity(ToRadians(30f), 1f, 12f), RandLerpColor(Color.WhiteSmoke, Color.SkyBlue), 40, RandRotTwoPi, 0.8f, 0.3f,true).Spawn();
            }
        }

        public bool CheckOwnerDead()
        {
            //分三种情况
            //第一种：如果处于第一动画进程，也就是仍然在中央位置，满足处死条件的情况下立刻处死
            bool ifDead = Owner.dead || Owner.HeldItem.type != OriginalItemID;
            bool directyDead = !Helper.IsDone[0] && ifDead;
            if (directyDead)
            {
                Projectile.Kill();
                return true;
            }
            //第二种：处于第二动画继进程，不要执行处死
            //只有两动画完成结束时，判断玩家手持情况，我们再执行afk动画
            bool doAFK = Helper.IsDone[0] && Helper.IsDone[1] && (ifDead || ((DualWraithStaff)Owner.HeldItem.ModItem).AlterVersion) && !IsAFK;
            if(doAFK)
            {
                //Timer会立刻被重置，这里的Timer已经没用了。只用于一个计时
                Timer = 0;
                IsAFK=true;
            }
            return false;
        }
        private void UpdatePlayerState()
        {
            Projectile.timeLeft = 2;
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.heldProj = Projectile.whoAmI;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool ShouldUpdatePosition() => IsAFK || OnExecutionNow;
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            //基础的一些数据
            Texture2D tex =Projectile.GetTexture(); 
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + ToRadians(60);
            Vector2 drawPoint = new Vector2(0, tex.Height);
            SpriteEffects se =  SpriteEffects.None;
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -12;
            Vector2 orbDrawPos = OrbPosition - Main.screenPosition;
            //绘制残影
            if (Owner.channel)
            {
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, realDrawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * Projectile.Opacity, drawRot, drawPoint, Projectile.scale, se, 0);
            }
            SB.Draw(tex, realDrawPos, null, Color.White * Projectile.Opacity, drawRot, drawPoint, Projectile.scale, se, 0);
            return false;
        }
    }
}
