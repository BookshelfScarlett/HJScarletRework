using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class JungleMadnessFocusProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<JungleMadnessProj>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(5);
        }
        public enum State
        {
            Shoot,
            Return
        }
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public int TotalHitTime => 12;
        public AnimationStruct Helper = new AnimationStruct(3);
        public bool IsHit = false;
        public NPC CurTarget = null;

        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 38;
            Helper.MaxProgress[1] = 38;
            if (Projectile.GetTargetSafe(out NPC target, false))
                CurTarget = target;
            SetUpPushParticle(1);
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }
        public void UpdateAttackAI()
        {
            switch (AttackType)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoShoot()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.velocity *= 0.86f;
                Projectile.rotation = Projectile.SpeedAffectRotation();
                Projectile.timeLeft = GetSeconds(10);
            }
            else
            {
                if (CurTarget.CanBeChasedBy() && CurTarget != null)
                {
                    Projectile.HomingTarget(CurTarget.Center, -1, 20f, 20f);
                    Projectile.rotation += 0.2f;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item76, Projectile.Center);
                    UpdateToNextAttack(State.Return);
                }
            }
        }
        public void UpdateToNextAttack(State id)
        {
            Projectile.netUpdate = true;
            AttackType = id;
            Timer = 0;
        }

        public void DoReturn()
        {
            if (!Helper.IsDone[1] && IsHit)
            {
                Projectile.velocity *= 0.84f;
                Projectile.rotation = Projectile.SpeedAffectRotation();
                Helper.UpdateAniState(1);

                return;
            }
            else if (Helper.IsDone[1])
            {
                if (Timer == 0f)
                {
                    Timer = 1;
                    SoundEngine.PlaySound(SoundID.Item76 with { MaxInstances = 0, Pitch = -0.15f }, Projectile.Center);
                    SetUpPushParticle(-1);
                }
            }
            Projectile.rotation += 0.2f;
            Projectile.HomingTarget(Owner.Center, -1, 20, 10);
            if (Projectile.IntersectOwnerByDistance(50f))
                Projectile.Kill();
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.FinalUpdate())
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants);
                d.scale = Main.rand.NextFloat(0.8f, 1.15f);
                d.noGravity = true;
            }
            if (Projectile.FinalUpdate())
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass);
                d.scale = Main.rand.NextFloat(0.8f, 1.15f);
                d.noGravity = true;
            }
            if (Projectile.FinalUpdateNextBool(3))
            {
                new EmptyRing(Projectile.Center.ToRandCirclePosEdge(16f), RandVelTwoPi(0.4f,1.4f), RandLerpColor(Color.DarkGreen, Color.Green), 40, 0.25f, 1, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
            }


        }
        public void SetUpPushParticle(int reverse)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(16), DustID.JunglePlants);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(20f), 1f, 2.4f) * reverse;
                d.scale = 1.65f;
                d.noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { MaxInstances = 2, Pitch = 0.3f }, Projectile.Center);
            if (AttackType == State.Shoot && Helper.IsDone[0])
            {
                IsHit = true;
                Timer += 1;
                Vector2 dir = Vector2.UnitX.RotatedBy(ToRadians(360f / TotalHitTime * Timer)) * 8f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ProjectileType<JungleMadnessLeafs>(), Projectile.damage, 1f, Owner.whoAmI);
                ((JungleMadnessLeafs)proj.ModProjectile).AIStatement = JungleMadnessLeafs.LeafState.Execution;
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                proj.timeLeft = GetSeconds(2);
                if (Timer <= TotalHitTime)
                    return;
                Projectile.velocity = (Projectile.Center - Owner.Center).ToSafeNormalize().RotatedBy(Main.rand.NextBool().ToDirectionInt() * ToRadians(55)) * 36f;
                SetUpPushParticle(1);
                SoundEngine.PlaySound(SoundID.Item76 with { MaxInstances = 1, Pitch = -0.5f }, Projectile.Center);
                UpdateToNextAttack(State.Return);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
