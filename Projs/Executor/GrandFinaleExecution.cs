using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class GrandFinaleExecution : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override string Texture => GetInstance<GrandFinaleProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Spawn,
            Lock,
            Strike,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.SetupImmnuity(10, ImmnuityType.Static);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
            Projectile.noEnchantmentVisuals = true;
            Projectile.scale *= 0.85f;
            Projectile.penetrate = -1;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.timeLeft = 2;
            if (Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }
            UpdateAttackAI();
            UpdateParticle();
        }

        private void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(40);
                new HRShinyOrb(pos, RandVelTwoPi(4f,8f), RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, 0, 1, 0.1f).Spawn();
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(40);
                new LightningParticle(pos, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.Blue), 40, Projectile.velocity.ToRotation() + PiOver2 + Main.rand.NextFloat(-PiOver4,PiOver4), 0.4f).Spawn();
            }

        }

        public void UpdateAttackAI()
        {
            switch(AttackState)
            {
                case State.Spawn:
                    DoSpawn();
                    break;
                case State.Lock:
                    DoLock();
                    break;
                case State.Strike:
                    DoStrike();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoReturn()
        {
            Projectile.rotation += 0.2f;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.IntersectOwnerByDistance())
                Projectile.Kill();
        }

        public void DoStrike()
        {
        }

        public void DoLock()
        {
            
            Projectile.rotation += 0.16f;
            if (Projectile.GetTargetSafe(out NPC target, true, 1800f, true))
            {
                Projectile.HomingTarget(target.Center, -1, 20f, 12f);
                if (Projectile.FinalUpdate())
                {
                    Timer++;
                    if (Timer < 60f)
                    {
                        float rotArgs = ToRadians(360f / 60f * Timer);
                        if (Timer % 10 == 0)
                        {
                            Vector2 spawnPos = Projectile.Center + (rotArgs + Main.rand.NextFloat(ToRadians(-10f), ToRadians(10f))).ToRotationVector2() * 1200f;
                            Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(22f, 28f);
                            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(60f), ProjectileType<GrandFinaleStriker>(), (int)(Projectile.damage * 0.50f), 12f, Owner.whoAmI);
                            ((GrandFinaleStriker)proj.ModProjectile).CurTarget = target;
                            proj.rotation = vel.ToRotation();
                            proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                        }
                    }
                    else
                        Timer = 0;
                }
            }
            else
                UpdateNextState(State.Return);
        }

        public void DoSpawn()
        {
            Projectile.velocity *= 0.94f;
            Projectile.rotation = Projectile.SpeedAffectRotation();
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 15))
                UpdateNextState(State.Lock);

        }
        public void UpdateNextState(State id)
        {
            AttackState = id;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
              Vector2 spawnPos = new Vector2(target.Center.X + Main.rand.NextFloat(400f, 900f) * Main.rand.NextBool().ToDirectionInt(), target.Center.Y - Main.rand.NextFloat(1200f, 1600f));
            Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * 18f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<GrandFinaleLightning>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
