using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
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
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 38;
            Helper.MaxProgress[1] = 38;
            if (Projectile.GetTargetSafe(out NPC target, false))
                CurTarget = target;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }
        public void UpdateAttackAI()
        {
            switch(AttackType)
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
                    UpdateToNextAttack(State.Return);
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
            Projectile.rotation += 0.2f;
            Projectile.HomingTarget(Owner.Center, -1, 20, 10);
            if(Projectile.IntersectOwnerByDistance(50f))
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

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType == State.Shoot && Helper.IsDone[0])
            {
                Timer += 1;
                IsHit = true;
                if (Timer < TotalHitTime)
                    return;
                Projectile.velocity = (Projectile.Center - Owner.Center).ToSafeNormalize().RotatedBy(Main.rand.NextBool().ToDirectionInt() * ToRadians(65)) * 36f;
                UpdateToNextAttack(State.Return);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
