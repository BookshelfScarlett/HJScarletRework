using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonBreakerExecution : HJScarletProj
    {
        public override string Texture => GetInstance<DungeonBreakerProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            SmashToWall,
            Return,
            BeforeReturn
        }
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public List<Vector2> StoredCenter = [];
        public List<Rectangle> StoredRec = [];
        public AnimationStruct Helper = new(2);
        public ref float Timer => ref Projectile.ai[0];
        public int BounceTime = 0;
        public int TotalBouceTime = 15;
        public float CurRotation = 0;
        public bool CanSmashDust = false;
        public Vector2 OldVec = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 32;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = 4;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.scale = 1.2f;
        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[2] with { Pitch = -0.5f }, Projectile.Center);
            GenerateBackDust(-1);
            StoredCenter.Add(Projectile.Center);
        }

        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }
        public void UpdateAttackAI()
        {
            switch (AttackType)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.SmashToWall:
                    DoSmashToWall();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }
        public void DoSmashToWall()
        {
            Timer++;
            Projectile.extraUpdates = 0;
            Projectile.rotation = CurRotation;
            if (Projectile.timeLeft < (GetSeconds(1) + 1))
            {
                UpdateAnimationBeforeReturn();
                return;
            }
            GeneralUpdateParticleOnWall(Timer);
            //再一定时间后，我们才正式准备释放需要的射弹
            if (Timer < 40)
                return;
            SpawnWaterbolt();
        }
        #region 一堆封装方法，砸墙后用的东西
        public void UpdateAnimationBeforeReturn()
        {
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
            //初始化
            if (CanSmashDust)
            {
                CanSmashDust = false;
                Vector2 velDir = (Owner.Center - Projectile.Center).ToSafeNormalize().RotatedBy(PiOver4 *Main.rand.NextBool().ToDirectionInt());
                Projectile.velocity = velDir * Main.rand.NextFloat(34, 48);
                Helper.MaxProgress[0] = 30;
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 10f, 12, Projectile.velocity.ToRotation(), ToRadians(20f));
                SoundEngine.PlaySound(SoundID.Item69 with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
                GenerateBackDust(1);
                //因为这里就给了速度，我们也要在这里给粒子

            }
            //完成动画进程后再跳转返程
            if (Helper.IsDone[0])
            {
                Vector2 velDir = (Owner.Center - Projectile.Center).ToSafeNormalize();
                Projectile.velocity = velDir * 8f;
                Projectile.timeLeft = GetSeconds(5);
                SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.15f }, Projectile.Center);
                Projectile.extraUpdates = 2;
                GenerateBackDust(-1);
                UpdateToNextAttack(State.Return);
            }
            else
            {
                Helper.UpdateAniState(0);
                Projectile.velocity *= 0.88f;
                Projectile.rotation = Projectile.SpeedAffectRotation(5, 5);
            }
        }
        public void GenerateBackDust(int reverse)
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(6);
                Vector2 dir = Projectile.velocity.ToRandVelocity(ToRadians(30f), 0f, 8f) * reverse;
                new ShinyOrbParticle(spawnPos, dir, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0.8f).Spawn();
            }
        }

        public void SpawnWaterbolt()
        {
            Vector2 projPos = Projectile.Center + OldVec * 10f;
            if (Projectile.timeLeft % GetSeconds(1) == 0)
            {
                new ShinyCrossStar(projPos, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0, 1, 2.4f,useLegacy:false).Spawn();
                SoundEngine.PlaySound(SoundID.Item60 with { MaxInstances = 0}, Projectile.Center);
                Vector2 oldDir = -OldVec;
                for (int i = -1; i < 2; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), projPos, oldDir.RotatedBy(i * ToRadians(10)) * 7.9f, ProjectileType<DungeonBreakerWaterbolt>(), Projectile.damage, 8f, Owner.whoAmI);
                    proj.timeLeft = GetSeconds(3);
                }
                for (int i = 0; i < 25; i++)
                {
                    Vector2 spawnPos = projPos.ToRandCirclePos(6);
                    Vector2 dir = -OldVec.ToRandVelocity(ToRadians(30f), 0f, 8f);
                    new ShinyOrbParticle(spawnPos, dir, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0.8f).Spawn();
                }
            }
        }
        public void GeneralUpdateParticleOnWall(float t)
        {
            //延后震开低敌怪以确保音效的重音能与这个地方匹配
            if (t == 2)
                SmashEnemyToAir();
            //震屏时的粒子
            if (t < 40)
                DrawSmashParticle();
            //完全静止时的粒子
            if (t % 6 == 0)
                DrawGoundIdleParticle();
        }

        private int InitPhase = 10;
        private int EarlyPhase = 20;

        public void DrawSmashParticle()
        {
            short HigherDust = DustID.GemSapphire;
            short BottemDust = DustID.UnusedWhiteBluePurple;
            Vector2 scale = GetScale;
            Vector2 initSpawnPos = Projectile.Center + OldVec * 30f;
            Vector2 altDir = OldVec.RotatedBy(PiOver2);
            //只在前20帧更新粒子运动
            if (Timer > EarlyPhase)
                return;

            for (int i = 0; i < 20; i++)
            {
                float height = Main.rand.NextFloat();
                height = Main.rand.NextFloat(height - 1, 1 - height);
                Dust flame = Dust.NewDustPerfect(initSpawnPos, Main.rand.NextFloat() >= height ? HigherDust : BottemDust);
                flame.noGravity = true;
                flame.position += altDir * height * scale.X;
                flame.velocity = -OldVec.ToRandVelocity(0, 1, 12);
                if (Main.rand.NextBool(4))
                {
                    flame.position += altDir * 10f;
                    flame.velocity.X += 0.02f;
                    flame.scale *= 1.262f;
                }
            }
        }
        /// <summary>
        /// 砸地时的粒子
        /// </summary>
        private void DrawSmashDust()
        {
        }
        private Vector2 GetScale
        {
            get
            {
                Vector2 scale;
                Vector2 start = new(10f, 10f);
                Vector2 middle = new(30f, 20f);
                Vector2 late = new(5f, 5f);
                if (Timer < InitPhase)
                    scale = Vector2.SmoothStep(start, middle, Timer / InitPhase);
                else
                    scale = Vector2.SmoothStep(middle, late, (Timer - InitPhase) / EarlyPhase);
                return scale;
            }
        }

        public void DrawGoundIdleParticle()
        {
            short HigherDust = DustID.GemSapphire;
            short BottemDust = DustID.UnusedWhiteBluePurple;
            Vector2 scale = GetScale;
            Vector2 initSpawnPos = Projectile.Center + OldVec * 30f;
            Vector2 altDir = OldVec.RotatedBy(PiOver2);

            for (int i = 0; i < 10; i++)
            {
                float height = Main.rand.NextFloat(-1f, 1f);
                Dust flame = Dust.NewDustPerfect(initSpawnPos, Main.rand.NextFloat() >= height ? HigherDust : BottemDust);
                flame.noGravity = true;
                flame.position = initSpawnPos + altDir * height * 30f - OldVec * Main.rand.NextFloat(0f, 70f);
                flame.velocity = -OldVec.ToRandVelocity(0, 0.51f, 1.3f);
                if (Main.rand.NextBool(4))
                {
                    flame.scale *= 1.262f;
                }
            }

        }

        public void SmashEnemyToAir()
        {
        }
        #endregion
        public void DoShoot()
        {
            StoredRec.Add(Projectile.Hitbox);
            StoredCenter.Add(Projectile.Center);
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void DoReturn()
        {
            if(Timer == 0)
            {
                Timer = 1;

            }
            Projectile.rotation += 0.2f;
            Projectile.ResetBoomerangReturn();
            Projectile.HomingTarget(Owner.Center, -1, 20f, 2f);
            if (Projectile.IntersectOwnerByDistance(20))
            {
                Projectile.Kill();
            }
        }
        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen() || CanSmashDust)
                return;
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(32), Projectile.velocity.ToRandVelocity(ToRadians(15f), 2.4f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, RandRotTwoPi, 1f, 0.5f, false, 0.2f).Spawn();
            if (Main.rand.NextBool())
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(32), Projectile.velocity.ToRandVelocity(ToRadians(15f), 2.4f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, .5f).Spawn();
            if (Projectile.FinalUpdateNextBool())
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple);
                d.scale *= Main.rand.NextFloat(0.75f, 1.2f);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;
            if (AttackType == State.Shoot)
            {
                UpdateOnTileParticle(oldVelocity);
                ResetToSmashWall(oldVelocity);
            }
            return false;
        }
        #region 接触物块后的工具方法
        private void ResetToSmashWall(Vector2 oldVel)
        {
            //转角问题到底要怎么办？
            Projectile.spriteDirection = Projectile.direction = Math.Sign(oldVel.X);
            //嵌入进去，或者砸入进去
            Projectile.position += oldVel.ToSafeNormalize() * 15f;
            //冲击波。
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<DungeonBreakerShockwave>(), Projectile.damage, 1f, Owner.whoAmI);
            OldVec = oldVel.ToSafeNormalize();
            base.OnFirstFrame();
            
            //何意味？
            float rotSe = Projectile.direction > 0 ? 0 : 0;
            CurRotation = oldVel.ToRotation() + rotSe;
            //震屏，然后重新设定零速度
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 30, oldVel.ToRotation(), 0.2f);
            Projectile.velocity = Vector2.Zero;
            //略微嵌入进去
            //这里刷新一遍射弹的生命值，我们会用这个生命值来总控一下射弹的进程
            Projectile.timeLeft = GetSeconds(12);
            CanSmashDust = true;

            //天顶世界下变成……钢管。
            //并且钢管会在玩家中心播放。
            //并且他会同时播放五次
            //改为2次了，我的耳膜！！！
            if (Main.zenithWorld)
            {
                for (int i = 0; i < 2; i++)
                    SoundEngine.PlaySound(HJScarletSounds.Pipes with { MaxInstances = 0 }, Owner.Center);
            }
            else
                SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { MaxInstances = 1, Pitch = -0.1f }, Projectile.Center);
            UpdateToNextAttack(State.SmashToWall);
        }

        public void UpdateOnTileParticle(Vector2 oldVelocity)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(6f) + oldVelocity.ToRandVelocity(0f, 4f, 8.4f) + Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-1.4f, 1.9f) + Projectile.velocity.ToRandVelocity(0f, 0f, 4.8f);
                Color drawColor = RandLerpColor(Color.MidnightBlue, Color.RoyalBlue);
                new ShinyCrossStar(spawnPos, vel, drawColor, 40, RandRotTwoPi, 1f, 0.5f, 0.2f).Spawn();
            }
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(AttackType == State.Shoot)
            {
                SoundEngine.PlaySound(HJScarletSounds.Smash_AirHeavy[0] with { MaxInstances = 1, Pitch = -0.4f }, Projectile.Center);
                UpdateOnTileParticle(Projectile.velocity);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<DungeonBreakerShockwave>(), Projectile.damage, 1f, Owner.whoAmI);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteEffects se = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotSe = Projectile.spriteDirection < 0 ? PiOver2 : 0;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, Projectile.Center + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.RoyalBlue.ToAddColor(), Projectile.rotation + PiOver4 + rotSe, tex.ToOrigin(), Projectile.scale, se, 0);
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float ratios = ((float)i / (Projectile.oldPos.Length - 1));
                Color lerpColor = Color.Lerp(Color.White, Color.Orange, ratios);
                SB.Draw(tex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, lerpColor * (1 - ratios), Projectile.oldRot[i] + PiOver4 + rotSe, tex.ToOrigin(), Projectile.scale, se, 0);
            }
            return false;
        }
        public void UpdateToNextAttack(State id)
        {
            Projectile.netUpdate = true;
            AttackType = id;
            Timer *= 0;
        }

    }
}
