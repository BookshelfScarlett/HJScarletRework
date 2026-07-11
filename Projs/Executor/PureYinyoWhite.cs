using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class PureYinyoWhite : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => ProjPath + "PureYinyoProj";
        public enum State
        {
            Shoot,
            HomingBack
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(-1);
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            //if(WhiteHammer)
            //DoBlackHammer();
            //else
            DoWhiteHammer();
        }
        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    ShootedAI();
                    break;
                case State.HomingBack:
                    HomingBackAI();
                    break;
            }
        }

        public void DoWhiteHammer()
        {
            if (Projectile.IsOutScreen())
                return;

            if (Main.rand.NextBool(8))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(30f), Projectile.velocity / 4f, RandLerpColor(Color.WhiteSmoke, Color.LightGray), Main.rand.Next(30, 70), RandRotTwoPi, 1, Projectile.scale * Main.rand.NextFloat(.80f, 1.1f) * .3f, false, BlendState.Additive);
            if (Main.rand.NextBool(3))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(25f), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightGray), Main.rand.Next(30, 70), 1, Projectile.scale * Main.rand.NextFloat(0.75f, 1.1f) * .4f, 0.2f);
        }

        public void UpdateWhiteHammerParticle()
        {
        }

        public void DoBlackHammer()
        {
            if (Projectile.IsOutScreen())
                return;
        }
        public void UpdateBlackHammerParticle()
        {
        }

        public void ShootedAI()
        {
            Timer++;
            Projectile.rotation += .2f;
            if (Timer % 3 == 0 && Main.rand.NextBool(3))
            {
                Vector2 dir = Projectile.velocity.ToSafeNormalize();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(30), dir * Main.rand.NextFloat(1f, 7.5f) * .4f, ProjectileType<PureYinyoShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((PureYinyoShard)proj.ModProjectile).BlackShard = false;
                ((PureYinyoShard)proj.ModProjectile).AttackState = PureYinyoShard.State.WhiteShardStandingStill;
            }
            if (Projectile.MeetMaxUpdatesFrame(Timer, 15))
            {
                Projectile.tileCollide = false;
                AttackState = State.HomingBack;
                Projectile.velocity = Projectile.Center.GetNormalVector2(Owner.Center) * 5f;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }
        public void HomingBackAI()
        {
            Projectile.rotation += .2f;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                Projectile.Kill();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackState == State.HomingBack)
                return false;
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3);
                Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 9f);
                int lifeTime = Main.rand.Next(30, 70);
                float rot = RandRotTwoPi;
                new SmokeParticle(pos, vel, Color.Black, lifeTime, rot, .48f, 0.35f, true).SpawnToPriorityNonPreMult();
                new SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.Lerp(Color.White, Color.LightGray, .019f)), lifeTime, rot, 1f, 0.3f, true).Spawn();
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(16);
                Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .64f, .15f);
            }
            SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 0,Pitch = -.7f , Volume = .4f},Projectile.Center);
            Projectile.velocity = -Projectile.oldVelocity;
                AttackState = State.HomingBack;
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<PureYinyo>());
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = target.Center.ToRandCirclePos(3);
                Vector2 dir = target.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 9f);
                int lifeTime = Main.rand.Next(30, 70);
                float rot = RandRotTwoPi;
                new SmokeParticle(pos, vel, Color.Black, lifeTime, rot, .48f, 0.35f, true).SpawnToPriorityNonPreMult();
                new SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.Lerp(Color.White, Color.LightGray, .019f)), lifeTime, rot, 1f, 0.3f, true).Spawn();
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = target.Center.ToRandCirclePos(16);
                Vector2 dir = target.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .64f, .15f);
            }
            SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 0,Pitch = -.7f });
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, 2, 0, 0);
            Vector2 origin = frame.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 2f, frame, Color.White.ToAddColor(), Projectile.rotation, origin, Projectile.scale, 0, 0);
            }
            SB.Draw(tex, drawPos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
