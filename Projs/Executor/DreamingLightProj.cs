using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class DreamingLightProj : HJScarletProj
    {
        public override string Texture => GetInstance<DreamingLight>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7, 2);
        }
        public enum State
        {
            Shoot,
            Return
        }
        public AnimationStruct Helper = new AnimationStruct(3);
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void ExSD()
        {
            Projectile.penetrate = 10;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(45);
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 60;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateAttackAI()
        {
            switch (AttackState)
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
            Timer++;
            Projectile.rotation += 0.2f * Math.Sign(Projectile.velocity.X);
            if(Projectile.MeetMaxUpdatesFrame(Timer,6))
            {
                Projectile.netUpdate = true;
                Timer = 0;
                AttackState = State.Return;
            }
        }

        public void DoReturn()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                UpdateAnistateZero();
            }
            else
            {
                if (Timer == 0f)
                {
                    InitReturn();
                    PoweredUpHammer();
                    Timer += 1;
                }
                ReturnToOwner();
            }
        }

        public void PoweredUpHammer()
        {
            Vector2 velo = (-Projectile.velocity).ToSafeNormalize().RotatedBy(ToRadians(5f) * Main.rand.NextBool().ToDirectionInt()) * 18f;
        }

        public void InitReturn()
        {
            Projectile.velocity = (Projectile.Center - Owner.Center).ToSafeNormalize() * -12f;
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[2] with { MaxInstances = 0, Pitch = -0.20f }, Projectile.Center);
        }

        public void ReturnToOwner()
        {
            Projectile.rotation = (-Projectile.velocity).ToRotation();
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.IntersectOwnerByDistance(90))
            {
                Projectile.Kill();
            }
        }

        public void UpdateAnistateZero()
        {
            Projectile.velocity *= 0.92f;
            Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.velocity).ToRotation(), 0.05f);
        }
        public void UpdateParticle()
        {
            bool canNotUpdate = Projectile.IsOutScreen() || (AttackState== State.Return && Main.rand.NextFloat() > Helper.GetAniProgress(0));
            if(canNotUpdate)
                return;
            if (Projectile.FinalUpdate())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(25), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, RandRotTwoPi, 1, 0.5f, false, 0.2f).Spawn();
            if (Main.rand.NextBool(4))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.Green, Color.GreenYellow), 40, 0.8f * Main.rand.NextFloat(0.6f,1.2f)).Spawn();
            if (Main.rand.NextBool(3))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(32), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.185f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Projectile.FinalUpdateNextBool())
                new EmptyRing(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.DarkGreen), 40, 0.26f * Main.rand.NextFloat(0.8f,1.2f), 1, altRing: false).SpawnToNonPreMult();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White, rotFix:PiOver4);
            return false;
        }
    }
}
