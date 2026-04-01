using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DeathTollsExecution : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        private enum DoType
        {
            IsArcRotating,
            IsHanging
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private int FlareCounts
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public float RotProgress = 0;
        public override string Texture => GetInstance<DeathTollsProj>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }
        public override void ExSD()
        {
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.height = Projectile.width = 66;
            Projectile.timeLeft = 480;
            Projectile.extraUpdates = 3;
            Projectile.localNPCHitCooldown = 25 * Projectile.extraUpdates;
            Projectile.usesLocalNPCImmunity = true;

        }
        public override void AI()
        {
            Projectile.rotation += 0.15f;
            if(!Projectile.HJScarlet().FirstFrame)
            {
                for(int i =0;i<60;i++)
                {
                    PosList.Add(Vector2.Zero);
                    PosListAlt.Add(Vector2.Zero);
                }

            }
            DrawTrailingDust();
            switch (AttackType)
            {
                case DoType.IsArcRotating:
                    DoArcRotating();
                    break;
                case DoType.IsHanging:
                    DoHanging();
                    break;
            }
        }


        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.DarkMagenta, 8);
            Projectile.DrawProj(Color.White);
            return false;
        }
        public List<Vector2> PosList = [];
        public List<Vector2> PosListAlt = [];
        public List<float> RotList = [];
        private void DrawTrailingDust()
        {
            if (Main.rand.NextBool(6))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(32f), RandVelTwoPi(1f, 8f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, RandRotTwoPi, 1, 0.46f, false).Spawn();
            if (Main.rand.NextBool(6))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(32f), RandVelTwoPi(1f, 8f), RandLerpColor(Color.DarkViolet, Color.Violet), 40,  0.46f).Spawn();
        }

        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //非挂载情况返回不执行。
            if (AttackType != DoType.IsHanging)
                return;

            if (FlareCounts < 3)
                FlareCounts++;

            int flareDamage = (int)(Projectile.damage / 2 * Math.Log(1 + FlareCounts));
            for (int i = 0; i < FlareCounts; i++)
                NightmareArrowDrop(target, flareDamage);

            int leastHitCD = 6 * Projectile.extraUpdates;
            Projectile.localNPCHitCooldown -= 5 * Projectile.extraUpdates;
            if (Projectile.localNPCHitCooldown < leastHitCD)
                Projectile.localNPCHitCooldown = leastHitCD;
        }
        private void NightmareArrowDrop(NPC target, int flareDamage)
        {
            //这下面一长串都是为了处理……生成的
            //返程写的挺fuck的
            float xDist = Main.rand.NextFloat(10f, 100f) * Main.rand.NextBool().ToDirectionInt();
            float yDist = Main.rand.NextFloat(800f, 1000f);
            Vector2 srcPos = target.Center + new Vector2(xDist, -yDist);
            if (Projectile.owner != Main.myPlayer)
                return;

            //在滞留所有的射弹
            Projectile sparks = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), srcPos, Vector2.Zero, ProjectileType<DeathTollsArrow>(), flareDamage, 1.1f, Owner.whoAmI);
            ((DeathTollsArrow)sparks.ModProjectile).CurTarget = target;
        }

        private int StartSpinTime => 30 * Projectile.extraUpdates;
        
        private void DoArcRotating()
        {
            DoArcRot();
            //Timer应延后自增避免出现执行问题
            AttackTimer += 1;
            if(Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex, true, 1800, true))
                ReleaseDarkEnegry();
            if (RotateTime > 1)
            {
                //跳出去避免后续继续调用attacktimer的时候出问题
                Projectile.netUpdate = true;
                AttackType = DoType.IsHanging;
            }
        }
        private void DoHanging()
        {
            if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex, true, 1800f, true))
            {
                Projectile.HomingTarget(target.Center, 1800f, 24f, 20f);
                Projectile.extraUpdates = 4;
            }
            else
                Projectile.extraUpdates = 3;
        }
        //下面这段圆弧代码需要重置
        private float RotateTime = 0;
        private bool _isArcRotating = false;
        private float _arcStartRotation;
        private float TotalArcAngle;
        private float _originalSpeed;
        private float _prevArcAngle;
        private void DoArcRot()
        {
            //如果已反向，且AttackTimer不会在被置零，返回，不执行下方AI
            if (RotateTime < 2 && !_isArcRotating)
            {
                float wtf = TwoPi;
                //随机取用角度
                TotalArcAngle = Main.rand.NextBool() ? wtf : -wtf;
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                //尚未反向，缓存这个角度
                if (RotateTime == 0)
                    _prevArcAngle = TotalArcAngle;
                else
                    TotalArcAngle = -_prevArcAngle;
                Projectile.velocity /= 3;
            }
            if (_isArcRotating)
            {
                //首次画圆，执行0~StartSpin，第二次画圆，执行StartSpinTime ~ StartSpinTime * 2
                float progress = RotateTime == 0 
                    ? (float)AttackTimer / StartSpinTime
                    : (float)(AttackTimer - StartSpinTime) / StartSpinTime;
                float curRot = _arcStartRotation + TotalArcAngle * progress;
                //加速
                float speed = Projectile.velocity.Length() + 0.21f * AttackTimer;
                if (speed > _originalSpeed)
                    speed = _originalSpeed;

                Projectile.velocity = curRot.ToRotationVector2() * speed;
                //如果进程结束
                if (progress >= 1f)
                {
                    Projectile.velocity = curRot.ToRotationVector2() * _originalSpeed;
                    _isArcRotating = false;
                    //首次反向结束，重置计时器准备反向
                    AttackTimer = StartSpinTime;
                    RotateTime += 1;
                }
            }
        }
        private void ReleaseDarkEnegry()
        {
            //下方是一段基于主射弹当前速度而做出动态变化的射弹生成代码
            //表现来说是，衍生射弹的生成频率将会与主射弹的速度会成正比，并尽可能控制在需要的固定间隔内
            //基础生成间隔
            const int BaseSpawnSpeed = 20;
            //射弹的飞行速度
            const float BaseTravelSpeed = 22f;
            //最小生成间隔
            const float MinSpawnSpeed = 15f;
            //最大生成间隔
            const float MaxSpawnSpeed = 24;
            //计算当前速度的模长
            float curSpeed = Projectile.velocity.Length();
            //基于射弹速度间隔进行生成刻计算
            float dynamicSpawnSpeed = BaseTravelSpeed / curSpeed * BaseSpawnSpeed;
            //控制在合理范围内
            dynamicSpawnSpeed = Clamp(dynamicSpawnSpeed, MinSpawnSpeed, MaxSpawnSpeed);
            //向下取整
            int spawnRates = (int)Math.Round(dynamicSpawnSpeed);
            Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            if (AttackTimer % spawnRates == 0)
            {
                //这里的暗温魔能会必中
                float baseFlareSpeed = Main.rand.NextFloat(12f, 16f);
                //依据锤子当前的速度，以对数的形式给予伤害加成
                int flareDamage = (int)(Projectile.damage + 2 * (float)Math.Log(Projectile.velocity.Length() / 1.5));
                Vector2 velocity = direction * baseFlareSpeed;
                if (Projectile.owner != Main.myPlayer)
                    return;
                //鬼魂音效
                SoundEngine.PlaySound(SoundID.Item103 with { Volume = 0.5f ,MaxInstances = 4, Pitch = 0.7f });
                //生成
                Projectile flares = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileType<DeathTollsDarkEnergy>(), flareDamage, 1.1f, Owner.whoAmI, 0f, Main.rand.Next(3));
                flares.extraUpdates = 3;
                flares.tileCollide = false;
            }
        }
    }
}
