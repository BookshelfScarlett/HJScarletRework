using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader.UI.ModBrowser;

namespace HJScarletRework.Projs.Executor
{
    public class FrostHammerProj : HJScarletProj
    {
        public override string Texture => GetInstance<FrostHammer>().Texture;
        public override Vector2 TileHitbox => new Vector2(16, 16);
        public enum State
        {
            Shoot,
            Return
        }
        public AnimationStruct Helper = new (3);
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
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            HandleAttackAI();
            HandleParticle();
        }


        public void HandleAttackAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8 * Projectile.MaxUpdates)
            {
                Projectile.frameCounter = 0;
                for(int i =0;i<8;i++)
                new SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6f), Vector2.UnitY.ToRandVelocity(ToRadians(10f), 0.1f, 12f), RandLerpColor(Color.White, Color.SkyBlue), 40, RandRotTwoPi, 0.8f, 0.26f, true).Spawn();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * Main.rand.NextFloat(2f, 6f), ProjectileType<FrostHammerIceSpike>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            }

            switch(AttackState)
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
            if(Projectile.IntersectOwnerByDistance(100))
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
            if(Main.rand.NextBool(10))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.38f,Main.rand.NextBool()).Spawn();
            if(Main.rand.NextBool(8))
                new SnowCloud(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.56f, Projectile.scale * Main.rand.NextFloat(0.7f, 1.21f) * 0.09f,true).SpawnToPriority();
            if (Main.rand.NextBool(8))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(30), Vector2.UnitY * Main.rand.NextFloat(1.8f, 5.4f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 0, 1, 0.68f * Main.rand.NextFloat(0.8f,1.1f)).Spawn();
            if (Main.rand.NextBool(12))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(30), Vector2.UnitY * Main.rand.NextFloat(1.8f, 5.4f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 0, 1, 0.68f * Main.rand.NextFloat(0.8f,1.1f)).Spawn();
            if(Main.rand.NextBool(8))
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<FrostHammer>());
            if(Projectile.numHits > 6)
                SwitchAttackState(State.Return);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
