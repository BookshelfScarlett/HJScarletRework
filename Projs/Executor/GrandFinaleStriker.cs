using HJScarletRework.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;

namespace HJScarletRework.Projs.Executor
{
    public class GrandFinaleStriker : HJScarletProj
    {
        public override string Texture => GetInstance<GrandFinaleProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Spawn,
            Strike,
            Fade
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public NPC CurTarget = null;
        public AnimationStruct Helper = new AnimationStruct(3);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 22;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
        }

        public void UpdateAttackAI()
        {
            switch(AttackState)
            {
                case State.Spawn:
                    DoSpawn();
                    break;
                case State.Strike:
                    DoStrike();
                    break;
                case State.Fade:
                    DoFade();
                    break;
            }
        }

        public void DoFade()
        {
            Projectile.velocity *= 0.915f;
            Projectile.rotation = Projectile.SpeedAffectRotation();
            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.2f);
        }

        public void DoStrike()
        {
            if (!CurTarget.IsLegal())
            {
                UpdateNextState(State.Fade);
                return;
            }
            else
                StrikeAI();
        }
        public void StrikeAI()
        {
            //距离足够进直接修正这个速度
            if (Vector2.DistanceSquared(Projectile.Center, CurTarget.Center) < 70f * 70f)
            {
                Projectile.velocity = (CurTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (30f);
                Projectile.rotation = Projectile.velocity.ToRotation() - PiOver4;
            }
            else
            {
                float angleOffset = WrapAngle(Projectile.AngleTo(CurTarget.Center) - Projectile.velocity.ToRotation());
                angleOffset = Clamp(angleOffset, -0.2f, 0.2f);
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset);
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() - PiOver4, 0.12f);
                Projectile.scale = Lerp(Projectile.scale, 1.10f, 0.25f);
                //控速
                if (Projectile.velocity.Length() < 30f)
                    Projectile.velocity *= 1.45f;
                else
                    Projectile.velocity *= 0.9f;
            }

        }

        public void DoSpawn()
        {
            Timer++;
            Projectile.rotation = Projectile.SpeedAffectRotation() * Projectile.spriteDirection;
            Projectile.velocity *= 0.90f;
            Projectile.scale = Lerp(Projectile.scale, 0.80f, 0.2f);
            if(Projectile.MeetMaxUpdatesFrame(Timer,18))
            {
                Projectile.scale = 0.8f;
                UpdateNextState(CurTarget.IsLegal() ? State.Strike : State.Fade);
            }
        }
        public void UpdateNextState(State id)
        {
            AttackState = id;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(AttackState == State.Strike)
            {
                UpdateNextState(State.Fade);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
