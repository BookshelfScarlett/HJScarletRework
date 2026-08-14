using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class WeHaveBookshelfBook : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(Globals.Enums.VanillaAsset.Item, ItemID.Book);
        public ref float Timer => ref Projectile.ai[0];
        public enum State
        {
            Shoot,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 600;

        }
        public override void OnFirstFrame()
        {
            Projectile.localAI[0] = RandRotTwoPi;
        }
        public override void ProjAI()
        {
            if (AttackState == State.Shoot)
            {
                float totalTime = 35;
                float ratio = Clamp(Timer / totalTime, 0, 1);
                Timer++;
                Projectile.velocity *= .980f;
                Projectile.rotation += Lerp(.35f, .01f, ratio);
                DoBlackShardParticle();
                if (Main.rand.NextBool(9))
                    ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.3f, 1.3f) * -1, RandLerpColor(Color.GreenYellow, Color.LimeGreen), Main.rand.Next(30, 70), 1, .5f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
                if (Projectile.MeetMaxUpdatesFrame(Timer, totalTime))
                {
                    AttackState = State.Homing;
                    Timer = 0;
                    Projectile.netUpdate = true;
                }

            }
            else
            {
                if (Main.rand.NextBool(9))
                    ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.3f, 1.3f) * -1, RandLerpColor(Color.GreenYellow, Color.LimeGreen), Main.rand.Next(30, 70), 1, .5f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);

                {
                    //下方用于尝试模拟布朗运动
                    //每帧允许进行更新的加速度大小
                    float forceScale = 0.071f; // 每帧随机力的大小
                    Vector2 randomForce = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * forceScale;
                    //加速，然后减速，我们只会让他动一下
                    Projectile.velocity += randomForce;
                    Projectile.velocity *= .97f;
                    //限制最大速度。
                    float maxSpeed = 1.6f;
                    if (Projectile.velocity.LengthSquared() > maxSpeed * maxSpeed)
                        Projectile.velocity *= .9f;
                    //转角根据进程来逐渐变慢
                    Projectile.rotation += .01f;
                    Projectile.velocity *= .9f;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }

        public void DoBlackShardParticle()
        {
        }

        public override bool? CanDamage()
        {
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.7f, 3.3f) * -1, RandLerpColor(Color.GreenYellow, Color.Green), Main.rand.Next(30, 70), 1, .7f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
            for (int i = 0; i < 6; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(1.6f), Main.rand.NextFloat(1.4f, 2.6f) * .37f, RandLerpColor(Color.LimeGreen, Color.Green), 100, 1, .1f * Main.rand.NextFloat(.7f, 1.1f), glowMult: .45f);
            }
            //for (int i = 0; i < 4; i++)
            //    new SnowCloud(Projectile.Center.ToRandCirclePos(3f), Vector2.Zero, Color.WhiteSmoke, Main.rand.Next(30, 40), RandRotTwoPi, 0.45f, .081f * Main.rand.NextFloat(.7f, 1.1f)).SpawnToPriority();

            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White, useOldPos: true, rotFix: Projectile.localAI[0]);
            return false;
        }
    }
}
