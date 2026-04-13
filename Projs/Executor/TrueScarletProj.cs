using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TrueScarletProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<TrueScarlet>().Texture;
        public enum State
        { 
            Shoot,
            Buffer,
            Return
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
            Projectile.width = Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(60);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
            Projectile.scale = 0.78f;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                float rot = RandRotTwoPi;
                float scale = Main.rand.NextFloat(.26f, .38f) * 0.515f;
                bool isAlt = Main.rand.NextBool();
                new SmokeParticle(pos, Projectile.velocity / 7f, RandLerpColor(Color.Crimson, Color.DarkRed), 40, rot, 1, scale * 1.1f, isAlt).SpawnToPriorityNonPreMult();
                //new SmokeParticle(pos, Projectile.velocity / 7f, RandLerpColor(Color.DarkRed, Color.Red), 40, rot, 1, scale, isAlt).Spawn();
            }
            if (Main.rand.NextBool(9))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30);
                Vector2 vel = Projectile.velocity / 7f;
                float scale = Main.rand.NextFloat(0.45f, 0.475f) * 0.525f;
                bool useAlt = Main.rand.NextBool();
                //new EmptyRing(pos, vel, RandLerpColor(Color.Crimson, Color.DarkRed), 40, scale, 1f, altRing: useAlt).SpawnToNonPreMult();
                //new EmptyRing(pos, vel, RandLerpColor(Color.Crimson, Color.DarkRed), 40, scale * 1.1f, 1f, altRing: !useAlt).SpawnToNonPreMult();
            }
            if (Main.rand.NextBool(5))
            {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                    float scale = 0.965f * 0.65f;
                float rot = RandRotTwoPi;
                new ShinyOrbParticle(pos, Projectile.velocity / 7f, Color.DarkRed, 40, scale).SpawnToNonPreMult();
                new ShinyOrbParticle(pos, Projectile.velocity / 7f, Color.Red, 40, scale).Spawn();
                new ShinyOrbParticle(pos, Projectile.velocity / 7f, RandLerpColor(Color.Silver, Color.White), 40,  scale * 0.6f).Spawn();
            }
        }

        public void UpdateAttackAI()
        {
            switch(AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Buffer:
                    DoBuffer();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoReturn()
        {
            Projectile.rotation += 0.12f;
            Projectile.HomingTarget(Owner.Center, -1, 20, 12);
            if (Projectile.IntersectOwnerByDistance(70))
                Projectile.Kill();
        }

        public void DoBuffer()
        {
        }

        public void DoShoot()
        {
            Projectile.rotation += 0.12f;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 9))
                UpdateNextState(State.Return);
        }

        public void UpdateNextState(State id)
        {
            AttackState = id;
            Projectile.netUpdate = true;
            Timer = 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.Crimson);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
