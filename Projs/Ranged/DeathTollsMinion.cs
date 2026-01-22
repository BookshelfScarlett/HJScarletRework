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
    public class DeathTollsMinion : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => GetInstance<DeathTollsMainProj>().Texture;
        private ref float Timer => ref Projectile.ai[1];
        private ref float ShootTimer => ref Projectile.ai[2];
        private int CurrentTimeLeft;
        private bool IsReadyToDead = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 1500;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals= true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override void AI()
        {
            if(Timer == 0)
                CurrentTimeLeft = Projectile.timeLeft;

            DrawTrailingDust();
            //锤子生命值即将结束的时候，让锤子本身直接去撞击距离最近的敌人
            if (Projectile.timeLeft < 200 && !IsReadyToDead)
            {
                SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss with { MaxInstances = 0}, Projectile.Center);
                IsReadyToDead = true;
                ShootTimer = 0;
            }
            if (IsReadyToDead)
            {
                //发起撞击前，先做好一段时间的准备
                if (ShootTimer < 45f)
                    UpdateMinionReadyToStrikeState();
                DirectlyStrikeToTarget();
                //超出屏幕范围，做掉锤子
                if (HJScarletMethods.OutOffScreen(Projectile.Center, 1.2f))
                    Projectile.Kill();
                DrawFireDust();
            }
            else
            {
                if (Owner.JustPressLeftClick())
                {
                    //更新当前的生存时间
                    CurrentTimeLeft = Projectile.timeLeft;
                    ShootLaserIfNeed();
                }
                else
                {
                    //锁定当前的生存时间避免处死，这里只有在玩家发起攻击的时候才会消耗生命值
                    //至于其他情况，无所谓，我故意的
                    Projectile.timeLeft = CurrentTimeLeft;
                }
                //无论如何，更新仆从的运动状态
                UpdateMinionIdleState();
            }
        }
        private void DrawFireDust()
        {
            //速度为零的情况下，降低火焰生成
            if (Projectile.velocity == Vector2.Zero && Main.rand.NextBool())
                return;
            //在这里绘制火焰粒子
            Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Color Firecolor = RandLerpColor(Color.Black, Color.DarkViolet);
            new Fire(Projectile.Center, fireVelocity * 4.5f, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.25f).SpawnToPriorityNonPreMult(); 
        }
        private void ShootLaserIfNeed()
        {
            float rate = 18f;
            ShootTimer += 1;
            //延后更新一定时间，确保其正确地进入射击状态
            if (ShootTimer < rate + 2f)
                return;
            Vector2 anchorPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 18f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), anchorPos, (Owner.LocalMouseWorld() - anchorPos).SafeNormalize(Vector2.UnitX) * 12f, ProjectileType<DeathTollsStreak>(), Projectile.damage * 2, Projectile.knockBack, Owner.whoAmI);
            SoundEngine.PlaySound(Utils.SelectRandom(Main.rand, HJScarletSounds.Hammer_Shoot) with { Pitch = 0.8f, Volume = 0.64f}, Projectile.Center);
            //设定为46确保其不会出现可能的情况
            ShootTimer -= rate + 2f;
        }
        private void DirectlyStrikeToTarget()
        {
            ShootTimer += 1;
            if (ShootTimer == 45f)
            {
                SoundEngine.PlaySound(HJScarletSounds.Misc_MagicStaffFire, Projectile.Center);
                Vector2 dir = (Owner.LocalMouseWorld() - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.extraUpdates = 4;
                Projectile.velocity = dir * 18f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }
        private void DrawTrailingDust()
        {
            
            Timer += 1;
            //正弦波频率
            float freq = 0.15f;
            //振幅
            float amp = 8f;
            Vector2 direction = Projectile.rotation.ToRotationVector2();
            //基础速度
            Vector2 speedValue = -direction * 5f;
            for (int i = -1; i < 2; i+= 2)
            {
                //基础横向偏移，用于控制射弹与路径的距离。
                float baseOffset = 0f;
                //让相位差不变，使他们在零点上同步
                float angle = Timer * freq;
                //曲线1使用Sin，曲线2使用-Sin确保反向运动
                float wave = (float)Math.Cos(angle) * i;
                //计算垂直方向向量。
                Vector2 perpendDir = direction.RotatedBy(PiOver2);
                //最终确定生成位置的偏差
                Vector2 waveOffset = perpendDir * wave * amp + perpendDir * baseOffset;
                //修改粒子生成位置。
                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 8f + waveOffset;
                //计算例子速度，粒子需要在零点反向运动。因为总体上，他们是在原点位置被“推开”的
                //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
                float verticleVel = (float)Math.Cos(angle) * 1.4f * i;
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


        private float Oscillation = 0;
        private void UpdateMinionIdleState()
        {
            bool switchAnchorPosToOverHead = Owner.JustPressLeftClick();
            if (!switchAnchorPosToOverHead)
                ShootTimer = 0;
            float offset = 90f * switchAnchorPosToOverHead.ToInt();
            Vector2 center = switchAnchorPosToOverHead ? Owner.LocalMouseWorld() : Owner.MountedCenter;
            //递增的值越大，锤子的摆动幅度越大
            Oscillation += 0.025f;
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            float anchorX = Owner.MountedCenter.X - Owner.direction * 88f * (!switchAnchorPosToOverHead).ToInt();
            float anchorY = Owner.MountedCenter.Y - (offset + 60f * (MathF.Sin(Oscillation) / 9f));
            Vector2 anchorPos = new(anchorX, anchorY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = ((center - Projectile.Center) * switchAnchorPosToOverHead.ToDirectionInt()).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
            
        }
        //这里从上面复制过来的
        private void UpdateMinionReadyToStrikeState()
        {
            Vector2 center = Owner.LocalMouseWorld();
            //递增的值越大，锤子的摆动幅度越大
            Oscillation += 0.025f;
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 safeInitAnchor = Owner.MountedCenter + (Owner.MountedCenter - Owner.LocalMouseWorld()).SafeNormalize(Vector2.UnitX) * 140f;
            safeInitAnchor.Y = safeInitAnchor.Y - 60f * (MathF.Sin(Oscillation) / 9f); 
            Vector2 anchorPos = safeInitAnchor;
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.1f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = (center - Projectile.Center).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.1f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, 8, rotFix:PiOver4);
            Projectile.DrawProj(Color.White, 2, rotFix:PiOver4);
            return false;
        }
    }
}
