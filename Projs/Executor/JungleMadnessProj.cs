using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class JungleMadnessProj: HJScarletProj
    {
        public override string Texture => GetInstance<JungleMadness>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            Return
        }
        public State AttackType
        {
            get => (State)Projectile.ai[1]; 
            set => Projectile.ai[1]=(float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 6;
            Projectile.extraUpdates = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
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
            Timer++;
            Projectile.rotation += 0.2f;
            if(Projectile.MeetMaxUpdatesFrame(Timer, 8))
            {
                AttackType = State.Return;
                Projectile.netUpdate = true;
                Timer = 0;
            }

        }
        public void DoReturn()
        {
            Projectile.rotation += 0.2f;
            Projectile.tileCollide = false;
            Projectile.HomingTarget(Owner.Center, -1, 18, 20);
            if (Projectile.IntersectOwnerByDistance(40))
            {
                if (Projectile.HJScarlet().AddFocusHit)
                    Owner.HJScarlet().ExecutionTime += 1;
                if (Projectile.HJScarlet().ExecutionStrike)
                    SpawnFocus();
                Projectile.Kill();
            }
        }

        public void SpawnFocus()
        {
            Vector2 vel = Vector2.UnitY * Main.rand.NextBool().ToDirectionInt() * 28f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileType<JungleMadnessFocusProj>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
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
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
