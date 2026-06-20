using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class PureYinyoShard : HJScarletProj
    {
        public enum State
        {
            BlackShardSlow,
            BlackShardHoming,
            WhiteShardStandingStill,
            WhiteShardDisapper
        }
        public bool BlackShard
        {
            get => Projectile.ai[2] == 1f;
            set => Projectile.ai[2] = value ? 1f : 0f;
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        #region 初始化
        public override void ExSD()
        {
            if (BlackShard)
                BlackShardSD();
            else
                WhiteShardSD();
        }
        public void BlackShardSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public void WhiteShardSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
        }
        #endregion
        #region 管控伤害判定
        public override bool? CanHitNPC(NPC target)
        {
            if (BlackShard)
            {
                if (CurTarget.IsLegal() && target.Equals(CurTarget))
                    return null;
                return false;
            }
            else
                return null;
        }
        public override bool? CanDamage()
        {
            if (BlackShard)
                return AttackState != State.BlackShardSlow;
            else
                return true;
        }
        #endregion
        public override void OnFirstFrame()
        {
            Projectile.localAI[0] = RandRotTwoPi;
        }
        public override void ProjAI()
        {
            if (BlackShard)
            {
                BlackShardAI();
            }
            else
            {
                WhiteShardAI();
            }
        }
        #region 黑碎片AI
        public void BlackShardAI()
        {
            switch (AttackState)
            {
                case State.BlackShardSlow:
                    DoBlackShardSlow();
                    break;
                case State.BlackShardHoming:
                    DoBlackShardHoming();
                    break;
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 pos = Projectile.ToRandRec();
                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(1.1f,3.2f);
                int lifeTime = Main.rand.Next(30, 70);
                float scale = Projectile.scale * Main.rand.NextFloat(0.75f, 1.1f) * .08f;
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.WhiteSmoke, .12f)), lifeTime, 1, scale, .5f, BlendState.NonPremultiplied);
            }

        }

        public void DoBlackShardSlow()
        {
            Timer++;
            float ratio = Clamp(Timer / 25f, 0, 1);
            Projectile.velocity *= .93f;
            Projectile.rotation += Lerp(.35f, .01f, ratio);
            Projectile.velocity.Y += (float)(Main.rand.NextFloat(-2f, 2f) * (1.03f - ratio));

            if (Projectile.MeetMaxUpdatesFrame(Timer, 30f))
            {
                AttackState = State.BlackShardHoming;
                Timer = 0f;
                Projectile.netUpdate = true;
            }
        }

        public void DoBlackShardHoming()
        {
            if (Main.rand.NextBool(8))
            {
                QuickYinyoSmokeBlack(Projectile.Center.ToRandCirclePos(6), Vector2.UnitY * Main.rand.NextFloat(.7f, 1.3f), 40, RandRotTwoPi, 1f, 0.35f);
            }

            if (CurTarget.IsLegal())
            {
                Projectile.rotation += .10f;
                Projectile.HomingTarget(CurTarget.Center, -1, 13f, 10f);
            }
            else
            {
                if (Projectile.GetTargetSafe(out NPC target) && CurTarget is null)
                {
                    CurTarget = target;
                }
                else
                {
                    CurTarget = null;
                    Projectile.velocity *= .94f;
                    Projectile.rotation = Projectile.SpeedAffectRotation();
                }
            }
        }
        public void QuickYinyoSmokeBlack(Vector2 pos, Vector2 vel, int lifeTime, float rot, float opacityMult = 1f, float scaleMult = 1f)
        {
            new SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .8f * opacityMult, scaleMult * .35f, true).SpawnToPriorityNonPreMult();
            new SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.DarkViolet, .10f)), lifeTime, rot, 1f * opacityMult, 0.3f * scaleMult, true).SpawnToPriorityNonPreMult();
        }

        #endregion
        #region 白碎片AI
        public void WhiteShardAI()
        {
            switch (AttackState)
            {
                case State.WhiteShardStandingStill:
                    DoWhiteShardStandingStill();
                    break;
                case State.WhiteShardDisapper:
                    DoWhiteShardDisapper();
                    break;
            }

        }
        public void DoWhiteShardStandingStill()
        {
            float maxtime = 120f;
            Timer++;
            //归一化
            float ratios = Clamp(Timer / maxtime, 0f, 1f);
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
            Projectile.rotation += Lerp(.15f, .01f, ratios);
            if(Main.rand.NextBool(8))
                ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.3f, 1.3f) * -1, RandLerpColor(Color.WhiteSmoke, Color.White), Main.rand.Next(30, 70), 1, .5f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
            if (Projectile.MeetMaxUpdatesFrame(Timer, maxtime))
            {
                AttackState = State.WhiteShardDisapper;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }
        public void DoWhiteShardDisapper()
        {
            for (int i = 0; i < 8; i++)
                ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.7f, 3.3f) * -1, RandLerpColor(Color.WhiteSmoke, Color.White), Main.rand.Next(30, 70), 1, .7f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
            for (int i = 0; i < 6; i++)
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(1.6f), Main.rand.NextFloat(1.4f, 2.6f) * .37f, RandLerpColor(Color.WhiteSmoke, Color.White), 100, 0.1f * Main.rand.NextFloat(.7f, 1.1f), RandRotTwoPi).Spawn();
            for (int i = 0; i < 4; i++)
                new SnowCloud(Projectile.Center.ToRandCirclePos(3f), Vector2.Zero, Color.WhiteSmoke, Main.rand.Next(30, 40), RandRotTwoPi, 0.45f, .081f * Main.rand.NextFloat(.7f, 1.1f)).SpawnToPriority();
            Projectile.Kill();
        }


        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            int shardType = BlackShard ? ItemID.DarkShard : ItemID.LightShard;
            Texture2D shard = TextureAssets.Item[shardType].Value;
            Vector2 origin = shard.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float lerpValue = Clamp((float)(Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 1.5f), 0.5f, 1f);
            if (BlackShard)
            {
                for (int i = 0; i < 16; i++)
                {
                    SB.Draw(shard, drawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 2.5f * lerpValue, null, Color.White.ToAddColor(), Projectile.rotation + Projectile.localAI[0], origin, Projectile.scale, 0, 0);
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    SB.Draw(shard, drawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 1.25f * lerpValue, null, Color.Black * .85f, Projectile.rotation + Projectile.localAI[0], origin, Projectile.scale, 0, 0);
                }
            }
                SB.Draw(shard, drawPos, null, Color.White, Projectile.rotation + Projectile.localAI[0], origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}

