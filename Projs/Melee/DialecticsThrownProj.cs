using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Net.NetworkInformation;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DialecticsThrownProj : ThrownSpearProjClass
    {
        public override string Texture => HJScarletItemProj.Proj_Dialectics.Path;
        public enum WaveStyle
        {
            Sin,
            Square,
            Paraline,
            ParaSquare
        }
        public WaveStyle CurWaveStyle = WaveStyle.Sin;
        public ref float DrawCubeAndBallTimer => ref Projectile.localAI[0];
        public bool IsHit = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public Vector2 MountedTargetPos = Vector2.Zero; 
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            if (Vector2.Distance(Projectile.Center, Owner.MountedCenter) > 1800f)
                Projectile.Kill();
            DrawCubeAndBallTimer += 0.025f;
            switch (CurWaveStyle)
            {
                case WaveStyle.Sin:
                    DrawSinWave();
                    SinWaveAttack();
                    break;
                case WaveStyle.Square:
                    DrawSquareWave();
                    SquareWaveAttack();
                    break;
                case WaveStyle.Paraline:
                    DrawParalineWave();
                    ParalineWaveAttack();
                    break;
                case WaveStyle.ParaSquare:
                    DrawParaSquareWave();
                    ParaSquareWaveAttack();
                    break;
            }
        }

        private void ParaSquareWaveAttack()
        {
            //平行波，沿途滞留可追踪敌人的小方块
            if (Timer % 5 == 0)
            {
                for (int k = -1; k < 2; k+=2)
                {
                    Vector2 vel2 = Projectile.SafeDir().RotatedBy(PiOver4 *k);
                    Projectile cube = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), vel2 * -13, ProjectileType<DialecticsCubeProj>(), Projectile.damage / 3, 1f);
                    cube.rotation = Projectile.velocity.ToRotation();
                }
            }

        }
        //平行波：超高速飞行，命中敌人时候刺入，并于天上降下矛
        private void ParalineWaveAttack()
        {
            if (Projectile.HJScarlet().GlobalTargetIndex == -1)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                return;
            }
            //此处，为矛绘制超级冲击波
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 starVel = dir * Main.rand.NextFloat(12f, 14f);
            Vector2 starPos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
            //这里的特效需要进行控制
            if (Main.rand.NextBool())
            {
                for (int i = 0; i < 1; i++)
                {
                    new StarShape(starPos - dir * 30f, starVel, Color.LightBlue.RandLerpTo(Color.MediumBlue), 0.8f, 40).Spawn();
                    new StarShape(starPos - dir * 30f, starVel, Color.White, 0.4f, 40).Spawn();
                }
            }
            if (Projectile.GetTargetSafe(out NPC target, true))
            {
                Projectile.Center = target.Center + MountedTargetPos;
                if(Projectile.timeLeft % 60 == 0)
                {
                    for (int i = 0; i < 2 ;i++)
                    {
                        Vector2 spawnPos = new Vector2(target.Center.X + Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(10f, 200f), target.Center.Y + Main.rand.NextFloat(1200f, 1800f));
                        Vector2 vel = (spawnPos - target.Center).SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(12f, 16f);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<DialecticsSkyFall>(), Projectile.damage, 12f, Owner.whoAmI);
                        proj.rotation = vel.ToRotation();
                        proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    }
                }
            }
        }
        //矩形波：命中后追加AimlabBox
        private void SquareWaveAttack()
        {
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustID.DungeonWater);
                d.velocity = Projectile.velocity /3;
                d.scale *= Main.rand.NextFloat(1.2f, 1.4f);
                d.noGravity = true;
            }
            if(IsHit)
            {
                Projectile.velocity *= 0.95f;
                Projectile.Opacity-=0.1f;
                if (Projectile.Opacity == 0f)
                    Projectile.Kill();
            }
        }
        private void SinWaveAttack()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustID.MushroomSpray);
                d.velocity = Projectile.velocity /3;
                d.scale *= Main.rand.NextFloat(1.2f, 1.4f);
                d.noGravity = true;
            }

        }
        public override bool? CanDamage() => true;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CurWaveStyle == WaveStyle.Paraline)
            {
                Projectile.velocity *= 0f;
            }
            else if(CurWaveStyle == WaveStyle.Square)
            {
                if(Owner.HasProj<DialecticsSpiningBlock>(out int projID))
                {
                    //过一个是否有mark的标记
                    modifiers.SourceDamage *= (1f + 0.1f * target.HJScarlet().Dialectics_HitTime);
                    target.HJScarlet().Dialectics_HitTime += 1;
                }
                else
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, projID, 0, 0, Owner.whoAmI);
                    proj.rotation = Projectile.rotation;
                    proj.scale = 0;
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                }
                IsHit = true;
                target.HJScarlet().Dialectics_Timer = 180;
            }
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.HJScarlet().GlobalTargetIndex == -1 &&CurWaveStyle == WaveStyle.Paraline)
            {
                Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
                MountedTargetPos = Projectile.Center - target.Center;
            }
            if (CurWaveStyle == WaveStyle.Square)
            {
                SoundEngine.PlaySound(HJScarletSounds.Dialectics_Hit with { MaxInstances = 0, Pitch = 0.5f, PitchVariance = 0.2f }, Projectile.Center);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        #region 绘制不同的波形轨迹
        //自变量
        public float Timer = 0f;
        //相位值
        public float Omega = 3f;
        //波形幅度
        public float Amp = 20f;
        private void DrawParalineWave(bool stopTimer = false)
        {
            Projectile.extraUpdates = 7;
            Timer += 1;
            
            //这里的特效需要进行控制
            if (Projectile.HJScarlet().GlobalTargetIndex != -1)
                return;
            //特效
            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = Projectile.SafeDir() * Main.rand.NextFloat(1.2f, 1.4f);
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(6f, 6f);
                new StarShape(spawnPos, -vel, Color.Blue.RandLerpTo(Color.MediumBlue), 0.7f, 40).Spawn();
                new StarShape(spawnPos, -vel, Color.White, 0.3f, 40).Spawn();
            }
            for (int k = -1; k < 2; k += 2)
            {
                for (int i = 0; i <= 4; i++)
                {
                    Vector2 drawPos = Projectile.Center - Projectile.SafeDir() * 10f;
                    Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2);
                    Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                    float sacle = 0.1f;
                    new TrailGlowBall(drawPos + offset * k * (Amp - 10f) + Projectile.velocity / 4 * i, Projectile.SafeDir() * 2, drawColor, 40, sacle * 0.7f,true).Spawn();
                    new TrailGlowBall(drawPos + offset * k * (Amp - 10f) + Projectile.velocity / 4 * i, Projectile.SafeDir() * 2, Color.White, 40, sacle * 0.4f,true).Spawn();
                }
            }
            
        }

        /// <summary>
        /// 平行的矩形波
        /// </summary>
        private void DrawParaSquareWave()
        {
            //轨迹特效
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = Projectile.SafeDir() * Main.rand.NextFloat(1.2f, 1.4f) * 2f * Main.rand.NextBool().ToDirectionInt();
                new TurbulenceShinyCube(Projectile.Center, vel, Color.White.RandLerpTo(Color.Blue), 20, Projectile.rotation, 0.8f, 0.5f).Spawn();
            }
            Vector2 drawPos = Projectile.Center - Projectile.SafeDir() * 10f;
            Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2);
            Timer += 1;
            Vector2 vel2 = Projectile.velocity / 3;
            //孩子们别怕，我带着我的答辩代码来了
            //画高电平
            if (Timer < Omega)
            {
                for (int k = -1; k < 2; k += 2)
                {
                    for (int i = -3; i <= 5; i++)
                    {
                        Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                        Vector2 pos = drawPos + offset * k * Amp + Projectile.velocity / 5 * i;
                        new HRShinyOrb(pos, vel2, drawColor, 40, 0f, 1f, 0.1f).Spawn();
                        new HRShinyOrb(pos, vel2, Color.White, 40, 0f, 1f, 0.05f).Spawn();
                    }
                }
            }
            //画由高到低的阶梯
            else if (Timer == Omega)
            {
                for (float j = Amp; j >= Amp - 10f; j -= 1f)
                {
                    for (int k = -1; k < 2; k += 2)
                    {
                        Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                        Vector2 pos = drawPos + offset * k * j + Projectile.velocity / 5;
                        new HRShinyOrb(pos, vel2, drawColor, 40, 0f, 1f, 0.1f).Spawn();
                        new HRShinyOrb(pos, vel2, Color.White, 40, 0f, 1f, 0.05f).Spawn();
                    }
                }
            }
            //画低电平
            else if (Timer > Omega && Timer < Omega * 2f)
            {
                for (int k = -1; k < 2; k += 2)
                {
                    for (int i = -3; i <= 5; i++)
                    {
                        Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                        Vector2 pos = drawPos + offset * k * (Amp - 10f) + Projectile.velocity / 5 * i;
                        new HRShinyOrb(pos, vel2, drawColor, 40, 0f, 1f, 0.1f).Spawn();
                        new HRShinyOrb(pos, vel2, Color.White, 40, 0f, 1f, 0.05f).Spawn();

                    }
                }
            }
            //画低到高的阶梯
            else
            {
                for (float j = Amp - 10f; j <= Amp; j += 1f)
                {
                    for (int k = -1; k < 2; k += 2)
                    {
                        Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                        Vector2 pos = drawPos + offset * k * j + Projectile.velocity / 5;
                        new HRShinyOrb(pos, vel2, drawColor, 40, 0f, 1f, 0.1f).Spawn();
                        new HRShinyOrb(pos, vel2, Color.White, 40, 0f, 1f, 0.05f).Spawn();

                    }
                }
                Timer = 0;
            }
        }
        //绘制矩形波
        private void DrawSquareWave()
        {
            //DrawParalineWave(true);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2);
            Timer += 1f;
            //孩子们，力大砖飞来了！
            Vector2 drawPos = Projectile.Center - Projectile.SafeDir() * 10f;
            Vector2 vel = Projectile.velocity / 3;
            //画高电平
            if (Timer < Omega)
            {
                for (int i = -5; i <= 5; i++)
                {
                    Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                    Vector2 pos = drawPos + offset * Amp + Projectile.velocity / 5 * i;
                    new HRShinyOrb(pos, vel, drawColor, 25, 0, 1, 0.1f).Spawn();
                    new HRShinyOrb(pos, vel, Color.White, 25, 0, 1, 0.07f).Spawn();

                }
            }
            //高电平到低电平的阶跃
            else if (Timer == Omega)
            {
                for (float j = Amp; j >= -Amp; j -= 2f)
                {
                    Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                    Vector2 pos = drawPos + offset * j;
                    new HRShinyOrb(pos, vel, drawColor, 25, 0, 1, 0.1f).Spawn();
                    new HRShinyOrb(pos, vel, Color.White, 25, 0, 1, 0.07f).Spawn();
                }
            }
            //画低电平
            else if (Timer > Omega && Timer < Omega * 2f)
            {
                for (int i = -5; i <= 5; i++)
                {
                    Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                    Vector2 pos = drawPos + offset * -Amp + Projectile.velocity / 5 * i;
                    new HRShinyOrb(pos, vel , drawColor, 25, 0, 1, 0.1f).Spawn();
                    new HRShinyOrb(pos, vel, Color.White, 25, 0, 1, 0.07f).Spawn();

                }
            }
            //低电平到高电平的阶跃
            else
            {
                for (float j = -Amp; j <= +Amp; j += 2f)
                {
                    Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                    Vector2 pos = drawPos + offset * j;
                    new HRShinyOrb(pos, vel, drawColor, 25, 0, 1, 0.1f).Spawn();
                    new HRShinyOrb(pos, vel, Color.White, 25, 0, 1, 0.07f).Spawn();

                }
                //记得重置
                Timer = 0;
            }
        }
        private void DrawSinWave()
        {
            //DrawParalineWave(true);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 speedOffset = Projectile.velocity / 5;
            Vector2 dir = Projectile.SafeDir();
            Vector2 mountedPos = Projectile.Center - dir * 10f;
            Timer += 0.8f;
            //总体在底下绘制一些别的粒子，这里用的是树叶
            Vector2 randomSpeed = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(1.2f, 2.0f);
            for (int i = 0; i < 5; i++)
            {
                Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                Vector2 spawnPos = mountedPos + dir.RotatedBy(PiOver2) * MathF.Sin(Timer - i * 0.16f) * (25.0f);
                new HRShinyOrb(spawnPos - speedOffset * i, dir * 1.2f, drawColor, 25, 0, 1, 0.1f).Spawn();
                new HRShinyOrb(spawnPos - speedOffset * i, dir * 1.2f, Color.White, 25, 0, 1, 0.07f).Spawn();
            }
            //第二条line
            for (int i = 0; i < 5;i++)
            {
                Color drawColor = Color.MediumBlue.RandLerpTo(Color.Blue);
                Vector2 spawnPos = mountedPos + dir.RotatedBy(PiOver2) * MathF.Sin(Timer - i * 0.16f) * (-25.0f);
                new HRShinyOrb(spawnPos - speedOffset * i, dir * 1.2f, drawColor, 25, 0, 1, 0.1f).Spawn();
                new HRShinyOrb(spawnPos - speedOffset * i, dir * 1.2f, Color.White, 25, 0, 1, 0.07f).Spawn();
            }
        }
        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int i = 0; i < 10; i++)
            {
                float rads = (float)i / 10;
                Color drawColor = (Color.Lerp(Color.Blue, Color.LightSkyBlue, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + Projectile.SafeDir() * 20f - Projectile.velocity * 0.7f * i, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.8f, 1.5f), 0, 0);
            }
            Projectile.DrawGlowEdge(Color.Blue, rotFix: PiOver4);
            Projectile.DrawProj(Color.White, 4, rotFix: PiOver4);
            DrawCubeAndBall(ref lightColor);
            return false;
        }
        public void DrawCubeAndBall(ref Color lightColor)
        {
            //此处，结合上方的Timer，让球体与方块绕着射弹本身动起来
            Vector2 cubePos = GetPos(15f, true, 20f, true);
            Vector2 cubePos2 = GetPos(-15f, false, 20f, true);
            Vector2 ballPos = GetPos(15f, false, 20f, false);
            Vector2 ballPos2 = GetPos(-15f, true, 20f, false);
            //给这些贴图绘制描边。
            for (int cubeTime = 0; cubeTime < 8; cubeTime++)
            {
                Vector2 posMove = ToRadians(cubeTime * 60f).ToRotationVector2() * 2f;
                Color drawColor = Color.White with { A = 0 };
                QuickDrawCube(cubePos + posMove, drawColor);
                QuickDrawCube(cubePos2 + posMove, drawColor);
                QuickDrawBall(ballPos + posMove, drawColor);
                QuickDrawBall(ballPos2 + posMove, drawColor);
            }
            QuickDrawCube(cubePos, Color.White);
            QuickDrawCube(cubePos2, Color.White);
            QuickDrawBall(ballPos, Color.White);
            QuickDrawBall(ballPos2, Color.White);
        }
        private Vector2 GetPos(float posOffset, bool left, float dirOffset, bool reverseFloating)
        {
            Vector2 spearDir = Projectile.rotation.ToRotationVector2();
            Vector2 circleOffset = spearDir * (MathF.Sin(DrawCubeAndBallTimer) * 5f);
            return Projectile.Center + spearDir * posOffset + spearDir.RotatedBy(PiOver2 * left.ToDirectionInt()) * dirOffset + circleOffset * reverseFloating.ToDirectionInt() - Main.screenPosition;
        }
        private void QuickDrawCube(Vector2 pos, Color color)
        {
            float rotSpeed = DrawCubeAndBallTimer * 10f - 24f;
            Texture2D cube = HJScarletTexture.Specific_DialectCube.Value;
            SB.Draw(cube, pos, null, color, ToRadians(rotSpeed), cube.Size() / 2, Projectile.scale * 0.8f, 0, 0);
        }
        private void QuickDrawBall(Vector2 pos, Color color)
        {
            float rotSpeed = DrawCubeAndBallTimer * 10f - 24f;
            Texture2D ball = HJScarletTexture.Specific_DialectBall.Value;
            SB.Draw(ball, pos, null, color, ToRadians(rotSpeed), ball.Size() / 2, Projectile.scale * 0.8f, 0, 0);

        }
    }
}
