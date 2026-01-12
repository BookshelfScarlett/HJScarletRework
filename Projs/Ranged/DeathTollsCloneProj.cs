using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class DeathTollsCloneProj : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Ranged;
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

        public override string Texture => GetInstance<DeathTollsMainProj>().Texture;
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
            Projectile.DrawGlowEdge(Color.DarkMagenta, 6);
            Projectile.DrawProj(Color.White);
            return false;
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
            Projectile sparks = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), srcPos, Vector2.Zero, ProjectileType<DeathTollsNightmareArrow>(), flareDamage, 1.1f, Owner.whoAmI);
            sparks.ai[2] = target.whoAmI;
            sparks.localAI[0] = xDist;
            sparks.localAI[1] = yDist;
        }

        private int StartSpinTime => 30 * Projectile.extraUpdates;
        
        private void DoArcRotating()
        {
            DoArcRot();
            //Timer应延后自增避免出现执行问题
            AttackTimer += 1;
            if(Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex, true, 1800))
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
            Projectile.rotation += 0.1f;
            if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex, true, 1800f))
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
                Projectile.rotation = _arcStartRotation + TotalArcAngle * progress;
                //加速
                float speed = Projectile.velocity.Length() + 0.21f * AttackTimer;
                if (speed > _originalSpeed)
                    speed = _originalSpeed;

                Projectile.velocity = Projectile.rotation.ToRotationVector2() * speed;
                //如果进程结束
                if (progress >= 1f)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * _originalSpeed;
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
        private void DrawTrailingDust()
        {
            //正弦波频率
            float freq = 0.2f;
            //振幅
            float amp = 35f;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            for (int i = -1; i < 2; i+= 2)
            {
                //基础横向偏移，用于控制射弹与路径的距离。
                float baseOffset = 5f;
                //让相位差不变，使他们在零点上同步
                float angle = AttackTimer * freq;
                //曲线1使用Sin，曲线2使用-Sin确保反向运动
                float wave = (float)Math.Sin(angle) * i;
                //计算垂直方向向量。
                Vector2 perpendDir = direction.RotatedBy(PiOver2);
                //最终确定生成位置的偏差
                Vector2 waveOffset = perpendDir * wave * amp + perpendDir * baseOffset;
                //修改粒子生成位置。
                Vector2 spawnPosition = Projectile.Center + waveOffset;
                //计算例子速度，粒子需要在零点反向运动。因为总体上，他们是在原点位置被“推开”的
                //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
                float verticleVel = (float)Math.Cos(angle) * 1.2f * i;
                Vector2 realVel = speedValue + perpendDir * verticleVel;
                //跳过屏幕外绘制
                if (HJScarletMethods.OutOffScreen(spawnPosition))
                    continue;
                //最终生成粒子。
                Color drawColor = i > 0 ? Color.Black : new(75, 0, 130);
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, drawColor, 140, 1.2f, i < 0 ? BlendStateID.Additive : BlendStateID.Alpha);
                shinyOrbParticle.Spawn();
            }
        }
    }
}
