using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class PureYinyoJade : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public enum State
        {
            Slowdown,
            WhiteShard,
            BlackShard
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
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override bool? CanDamage()
        {
            return AttackState != State.Slowdown || Projectile.localAI[1] > 19f;
        }
        public override void OnFirstFrame()
        {
            Projectile.localAI[0] = RandRotTwoPi;
        }
        public override void ProjAI()
        {

            switch (AttackState)
            {
                case State.Slowdown:
                    DoSlowdown();
                    break;
                case State.WhiteShard:
                    DoWhiteShard();
                    break;
                case State.BlackShard:
                    DoBlackShard();
                    break;
            }

        }
        public void DoBlackShard()
        {
                DoBlackShardParticle();
            if (Projectile.GetTargetSafe(out NPC target))
            {
                Projectile.HomingTarget(target.Center, -1, 12, 10);
                Projectile.rotation += .10f;
            }
        }

        public void DoWhiteShard()
        {
            if (Main.rand.NextBool(8))
                ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.3f, 1.3f) * -1, RandLerpColor(Color.WhiteSmoke, Color.White), Main.rand.Next(30, 70), 1, .5f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
            if (Main.rand.NextBool(8))
                ECSParticle.SnowCloud(Projectile.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(0.3f, 1.3f) * -1, RandLerpColor(Color.WhiteSmoke, Color.White), Main.rand.Next(30, 70), 1, .15f * Main.rand.NextFloat(.7f, 1.1f), 0.2f);
        }

        public void DoSlowdown()
        {
            if (BlackShard)
            {
                float totalTime = 35;
                float ratio = Clamp(Timer / totalTime, 0, 1);
                Timer++;
                Projectile.velocity *= .980f;
                Projectile.rotation += Lerp(.35f, .01f, ratio);
                DoBlackShardParticle();
                if (Projectile.MeetMaxUpdatesFrame(Timer, totalTime))
                {
                    AttackState = BlackShard ? State.BlackShard : State.WhiteShard;
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                //基本抄了一遍原版的代码。
                Projectile.rotation = Projectile.velocity.ToRotation();
                ref float localAi1 = ref Projectile.localAI[1];
                localAi1 += 1f;
                DoWhiteShard();
                Projectile.Opacity = (localAi1 - 5) / 5;
                if (localAi1 >= 20f)
                {
                    Projectile.velocity.Y += 0.15f;
                    Projectile.velocity.X *= 0.96f;
                }
                if (localAi1 >= 25f)
                {
                    localAi1 = 20f;
                    float? Xoffset = null;
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile flake = Main.projectile[i];
                        if (flake.whoAmI != Projectile.whoAmI && flake.active && flake.type == Projectile.type)
                        {
                            Vector2 dir = flake.position - Projectile.position;
                            if (dir.Length() <= 60)
                            {
                                Xoffset ??= 0;
                                Xoffset -= Math.Sign(dir.X) * (1 - dir.Length() / 60);
                            }
                        }
                    }
                    if (Xoffset.HasValue)
                    {
                        Projectile.velocity.X += Xoffset.Value * 0.25f;
                    }
                }
            }
        }
        public void DoBlackShardParticle()
        {
            if (Main.rand.NextBool(12))
            {
                Vector2 pos = Projectile.ToRandRec();
                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(1.1f, 3.2f);
                int lifeTime = Main.rand.Next(30, 70);
                float scale = Projectile.scale * Main.rand.NextFloat(0.75f, 1.1f) * .08f;
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.WhiteSmoke, .12f)), lifeTime, 1, scale, .5f, BlendState.NonPremultiplied);
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int shardID = BlackShard ? ItemID.DarkShard : ItemID.LightShard;
            Texture2D shard = TextureAssets.Item[shardID].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + Projectile.localAI[0];
            float scale = BlackShard ? Projectile.scale : Projectile.scale * .8f;
            Color edgeColor = BlackShard ? Color.White.ToAddColor() : Color.Black.ToAddColor(180);
            for (int i = 0; i < 8; i++)
            {
                float rotArgs = TwoPi * (float)i / 8;

                SB.Draw(shard, drawPos + rotArgs.ToRotationVector2() * 1.5f, null, edgeColor, rotation, shard.ToOrigin(), scale, 0, 0);
            }
            SB.Draw(shard, drawPos, null, Color.White, rotation, shard.ToOrigin(), scale, 0, 0);
            return false;
        }
    }
}
