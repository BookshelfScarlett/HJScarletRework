using HJScarletRework.Assets.Registers;
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

namespace HJScarletRework.Projs.Ranged
{
    public class DeathTollsNightmareArrow : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        private enum DoType
        {
            IsSpawned,
            IsChasing,
            IsHit
        }
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        //新建一个
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
        }
        public override bool ShouldUpdatePosition() => AttackType != DoType.IsSpawned;
        public override bool? CanDamage() => AttackType != DoType.IsSpawned; 
        public override void AI()
        {
            //直接在AI里写死刷新，下方会手动控制这个射弹的处死逻辑
            Lighting.AddLight(Projectile.Center, TorchID.White);
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case DoType.IsSpawned:
                    DoSpawned();
                    break;
                case DoType.IsChasing:
                    DoChasing();
                    break;
                case DoType.IsHit:
                    DoHit();
                    break;
            }
            if (AttackType != DoType.IsChasing)
                Projectile.timeLeft = 150;

            if ((Projectile.Center - Owner.Center).Length() > 2400f)
                Projectile.Kill();

            DrawTrailingDust();

        }
        // float amp2 = 35f;
        private void DrawTrailingDust()
        {
            //正弦波频率
            float freq = 0.2f;
            //振幅
            float amp2 = 12f;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f / Projectile.extraUpdates;
            for (int i = 0; i < 2; i++)
            {
                //基础横向偏移，用于控制射弹与路径的距离。
                float baseOffset = 0;
                //让相位差不变，使他们在零点上同步
                float angle = AttackTimer * freq;
                //曲线1使用Sin，曲线2使用-Sin确保反向运动
                float wave = (float)Math.Sin(angle);
                if (i == 1) wave = -wave;
                //计算垂直方向向量。
                Vector2 perpendDir = direction.RotatedBy(PiOver2);
                //最终确定生成位置的偏差
                Vector2 waveOffset = perpendDir * wave * amp2 + perpendDir * baseOffset;
                //修改粒子生成位置。
                Vector2 spawnPosition = Projectile.Center + waveOffset;
                //计算例子速度，粒子需要在零点反向运动。因为总体上，他们是在原点位置被“推开”的
                //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
                float verticleVel = (float)Math.Cos(angle) * 0.8f;
                if (i == 1) verticleVel = -verticleVel;
                Vector2 realVel = speedValue + perpendDir * verticleVel;
                //最终生成粒子。
                
                //跳过屏幕外绘制
                if (HJScarletMethods.OutOffScreen(spawnPosition))
                    continue;
                Color drawColor = i == 0 ? Color.Black : new(75, 0, 130);
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, drawColor, 140, 1.2f, i != 0 ? BlendStateID.Additive : BlendStateID.Alpha);
                shinyOrbParticle.Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType != DoType.IsChasing)
                return;

            Projectile.netUpdate = true;
            AttackTimer = 0f;
            AttackType = DoType.IsHit;
        }
        #region AI方法合集
        private void DoChasing()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true, 1800f))
            {
                Projectile.HomingTarget(target.Center, 1800f, 24f, 10f);
                Projectile.extraUpdates = 3;
            }
            else
                Projectile.extraUpdates = 2;
            AttackTimer += 1;
        }
        private void DoHit()
        {
            AttackTimer += 1;
            float progress = AttackTimer / 45f;
            Projectile.velocity *= 0.987f;
            Projectile.scale = Lerp(1f, 0f, progress);
            if (Projectile.scale is 0f)
                Projectile.Kill();
        }
        private void DoSpawned()
        {
            //获取相对差
            float xOffset = Projectile.localAI[0];
            float yOffset = Projectile.localAI[1];
            //转化为实际点位
            Vector2 offsetPos = new(-xOffset, -yOffset);
            //获取目标位置。
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                //更新射弹位置
                Projectile.Center = target.Center + offsetPos;
            }
            else
            {
                //不合规的时候立刻跳转消失
                Projectile.Kill();
                return;
            }
            AttackTimer += 1;
            float progress = AttackTimer / 5f;
            Projectile.scale = Clamp(EaseInCubic(progress), 0f, 1f);
            //给予一个向下的初速度。
            Projectile.velocity = new Vector2(0f, 4f);
            //速度。
            if (AttackTimer < 25f)
                return;

            SoundStyle pick = Utils.SelectRandom(Main.rand, HJScarletSounds.Hammer_Shoot);
            SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit with { Volume = 0.8f, Pitch = 0.5f, MaxInstances = 3 }, Projectile.Center);
            AttackType = DoType.IsChasing;
            AttackTimer = 0;
            Projectile.netUpdate = true;
            //重新给予射弹向上的初速度。
        }
        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}