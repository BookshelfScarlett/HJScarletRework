using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FleshGrinderProj : HJScarletProj
    {
        public override string Texture => GetInstance<FleshGrinder>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackType
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
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(90);
            Projectile.penetrate = 3;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;

        }
        public override void ProjAI()
        {
            UpdateAttack();
            UpdateParticle();
        }

        private void UpdateAttack()
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
        private void DoReturn()
        {

            Projectile.tileCollide = false;
            Projectile.rotation += 0.2f;
            Projectile.HomingTarget(Owner.Center, -1, 12f, 20f);
            if (!Projectile.Hitbox.Intersects(Owner.Hitbox))
                return;

            if (Projectile.HJScarlet().AddFocusHit)
                Owner.HJScarlet().ExecutionTime += 1;
            if (Projectile.HJScarlet().ExecutionStrike)
                CanFocusStrike();

            Projectile.Kill();
        }
        private void CanFocusStrike()
        {
            if (Owner.HasProj<FleshGrinderFocusProj>(out int projID))
                return;
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
            Vector2 dir = -Projectile.velocity.ToSafeNormalize();
            Vector2 vel = dir.RotatedBy(Main.rand.NextFloat(ToRadians(30f), ToRadians(60f)) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(22f, 26f);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 12f, 40, vel.ToRotation(), ToRadians(30f));
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, vel, projID, Projectile.originalDamage, 2f, Owner.whoAmI);
            proj.HJScarlet().ExecutionStrike = true;
            proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;

            for (int i = 0; i < 16; i++)
            {
                Vector2 dVel = vel.ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 18f);
                new SmokeParticle(Projectile.Center.ToRandCirclePos(6f), dVel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 dVel = vel.ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 18f);
                new ShinyOrbHard(Projectile.Center.ToRandCirclePos(6f), dVel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }

        private void DoShoot()
        {
            Projectile.rotation += 0.2f;
            Timer++;
            if (Timer > 10 * Projectile.MaxUpdates)
            {
                Timer = 0;
                Projectile.netUpdate = true;
                AttackType = State.Return;
            }
        }


        private void UpdateParticle()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            if (Projectile.numUpdates == 0 && Main.rand.NextBool(3))
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity.ToRandVelocity(ToRadians(10), 1.2f, 1.8f), RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.5f, 1.1f), false).SpawnToNonPreMult();
            }
            Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(30f), DustID.Blood);
            d.velocity = -Projectile.velocity.ToRandVelocity(ToRadians(15f), 1.2f, 1.8f);
            d.scale *= Main.rand.NextFloat(1.2f, 1.4f);
            d.noGravity = true;


            if (Projectile.numUpdates == 0 && Main.rand.NextBool(5))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity.ToRandVelocity(ToRadians(10), 1.2f, 1.8f), RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.5f, 1.1f), true).SpawnToNonPreMult();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackType == State.Shoot)
            {
                TileCollideParticle(oldVelocity);
                AttackType = State.Return;
                Timer *= 0f;
                Projectile.netUpdate = true;
                Projectile.tileCollide = false;
            }
            return false;
        }

        private void TileCollideParticle(Vector2 velo)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
            Vector2 vel = velo.ToSafeNormalize();
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 8.8f);
                new SmokeParticle(pos, dVel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 8; i++)
            {

                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 4.8f);
                new ShinyOrbHard(pos, dVel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }


        public override void OnFirstFrame()
        {
            Projectile.originalDamage = Projectile.damage;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 2, Pitch = -0.2f }, target.Center);
            for (int i = 0; i < 24; i++)
            {
                new Fire(target.Center.ToRandCirclePos(4f), RandVelTwoPi(6f), RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.1f).SpawnToNonPreMult();
            }
            for (int i = 0; i < 24; i++)
            {
                new ShinyOrbHard(target.Center.ToRandCirclePos(6f), RandVelTwoPi(4f), RandLerpColor(Color.DarkRed, Color.Crimson), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.Crimson);
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
