using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class FrostHammerProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<FrostHammer>().Texture;
        public override Vector2 TileHitbox => new Vector2(16, 16);
        public enum State
        {
            Shoot,
            Return
        }
        public AnimationStruct Helper = new(3);
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
            Projectile.width = Projectile.height = 40;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.DamageType = ExecutorDamageClass.Instance;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void OnFirstFrame()
        {
            if (Owner.HasProj<FrostHammerExecution>() || !Projectile.HJScarlet().ExecutionStrike)
                return;
            Vector2 vel = -Owner.ToMouseVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(20f, 30f);
            Vector2 spawnPos = Owner.MountedCenter;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<FrostHammerExecution>(), Projectile.damage, Projectile.knockBack, Projectile.owner);


        }
        public override void ProjAI()
        {
            HandleAttackAI();
            HandleParticle();
        }


        public void HandleAttackAI()
        {
            //图本身没有帧数，这个只是作为一个计时器用的
            Projectile.frameCounter++;
            if (Projectile.frameCounter > Main.rand.Next(6, 9) * Projectile.MaxUpdates)
            {
                Projectile.frameCounter = 0;
                for (int i = 0; i < 8; i++)
                    new SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6f), Vector2.UnitY.ToRandVelocity(ToRadians(10f), 0.1f, 12f), RandLerpColor(Color.White, Color.SkyBlue), 40, RandRotTwoPi, 0.8f, 0.26f, true).Spawn();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * Main.rand.NextFloat(2f, 6f), ProjectileType<FrostHammerIceSpike>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                if (Owner.HasProj<FrostHammerExecution>() && !Projectile.TooAwayFromOwner(1300))
                    proj.ai[1] = 1;
            }

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
            Projectile.rotation += .2f;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 12))
                SwitchAttackState(State.Return);
        }

        public void DoReturn()
        {
            Projectile.ResetBoomerangReturn();
            Projectile.HomingTarget(Owner.MountedCenter, -1, 20f, 20f);
            Projectile.rotation += 0.2f;
            if (Projectile.IntersectOwnerByDistance(100))
            {
                Projectile.Kill();
            }
        }
        public void SwitchAttackState(State toState)
        {
            switch (toState)
            {
                case State.Shoot:

                    break;
                case State.Return:
                    Timer = 0;
                    AttackState = toState;
                    Projectile.netUpdate = true;
                    Projectile.tileCollide = false;
                    Projectile.ResetLocalNPCHitImmunity();
                    break;
            }
        }

        public void HandleParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(10))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.38f, Main.rand.NextBool()).Spawn();
            if (Main.rand.NextBool(8))
                new SnowCloud(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.56f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.09f, true).SpawnToPriority();
            if (Main.rand.NextBool(8))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(30), Vector2.UnitY * Main.rand.NextFloat(1.8f, 5.4f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 0, 1, 0.68f * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();
            if (Main.rand.NextBool(12))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(30), Vector2.UnitY * Main.rand.NextFloat(1.8f, 5.4f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 0, 1, 0.68f * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();
            if (Main.rand.NextBool(8))
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(40);
                    p.Velocity = Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 4.2f);
                    p.DrawColor = RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue);
                    p.Lifetime = 40;
                    p.Scale = Projectile.scale * Main.rand.NextFloat(0.8f, 1.1f) * .1f;
                    p.GlowCenterMult = 0.75f;
                });
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SwitchAttackState(State.Return);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                    new SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6f), Vector2.UnitY.ToRandVelocity(ToRadians(10f), 0.1f, 12f), RandLerpColor(Color.White, Color.SkyBlue), 40, RandRotTwoPi, 0.8f, 0.26f, true).Spawn();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, -Projectile.oldVelocity.ToRandVelocity(ToRadians(5f), 8) * (i + 1) - Vector2.UnitY * 15f, ProjectileType<FrostHammerIceSpike>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                if (Owner.HasProj<FrostHammerExecution>() && !Projectile.TooAwayFromOwner(1300))
                    proj.ai[1] = 1;


            }
            SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { Variants = [1], MaxInstances = 1, Pitch = -0.5f, Volume = 0.5f });
            int dustCount = 5;
            for (int i = 0; i < dustCount; ++i)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = Projectile.Center.ToRandCirclePos(10f) + dir * Main.rand.NextFloat(10f);
                new ShinyCrossStar(pos, RandVelTwoPi(2f, 4.9f), RandLerpColor(Color.LightSkyBlue, Color.RoyalBlue), 45, RandRotTwoPi, RandZeroToOne, Projectile.scale, false, 0.5f).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(0.2f, 7.4f);
                float scale = Main.rand.NextFloat(0.4f, 0.9f) * .2f;
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = pos;
                    p.Velocity = vel;
                    p.DrawColor = RandLerpColor(Color.LightBlue, Color.RoyalBlue);
                    p.Lifetime = 45;
                    p.Scale = scale;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.75f;
                });
            }

            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = Projectile.Center + RandVelTwoPi(10f, 30f);
                Vector2 vel = (Projectile.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(1f, 10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .25f, 0.28f, Main.rand.NextBool()).Spawn();
            }
            Projectile.velocity = Projectile.Center.GetNormalVector2(Owner.MountedCenter) * Projectile.velocity.Length();

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<FrostHammer>());
            if (Projectile.numHits % 3 == 0)
                SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { Variants = [1], MaxInstances = 1, Pitch = -0.5f, Volume = 0.5f });
            if (Projectile.numHits == 2)
                SwitchAttackState(State.Return);
            int dustCount = 5;
            for (int i = 0; i < dustCount; ++i)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = target.Center.ToRandCirclePos(10f) + dir * Main.rand.NextFloat(10f);
                new ShinyCrossStar(pos, RandVelTwoPi(2f, 4.9f), RandLerpColor(Color.LightSkyBlue, Color.RoyalBlue), 45, RandRotTwoPi, RandZeroToOne, Projectile.scale, false, 0.5f).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(0.2f, 7.4f);
                float scale = Main.rand.NextFloat(0.4f, 0.9f) * .2f;
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = pos;
                    p.Velocity = vel;
                    p.DrawColor = RandLerpColor(Color.LightBlue, Color.RoyalBlue);
                    p.Lifetime = 45;
                    p.Scale = scale;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.75f;
                });
            }

            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.White, Color.RoyalBlue);
                Vector2 spawnPos = target.Center + RandVelTwoPi(10f, 30f);
                Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * Main.rand.NextFloat(1f, 10f);
                new SnowCloud(spawnPos, vel, Firecolor, 40, Main.rand.NextFloat(TwoPi), .25f, 0.28f, Main.rand.NextBool()).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
