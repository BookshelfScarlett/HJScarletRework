using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
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
            SoundEngine.PlaySound(SoundID.Item76, Projectile.Center);
            Vector2 vel = Vector2.UnitY * Main.rand.NextBool().ToDirectionInt() * 28f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileType<JungleMadnessExecution>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
            ((JungleMadnessExecution)proj.ModProjectile).CurTarget = Projectile.ToHJScarletNPC();
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
            
            return false;
        }
        public override void OnFirstFrame()
        {
            Projectile.originalDamage = Projectile.damage;
            base.OnFirstFrame();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { MaxInstances = 2, Pitch = 0.3f}, Projectile.Center);
            if (Projectile.numHits < 1)
                SpawnLeafs(target.whoAmI);
        }

        public void SpawnLeafs(int targetIndex)
        {
            for (int i = -1; i < 2; i++)
            {
                Vector2 dir = Projectile.SafeDir().RotatedBy(ToRadians(8)* i) * 5f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ProjectileType<JungleMadnessLeafs>(), Projectile.damage, 1f, Owner.whoAmI);
                ((JungleMadnessLeafs)proj.ModProjectile).AIStatement = JungleMadnessLeafs.LeafState.Normal;
                proj.HJScarlet().GlobalTargetIndex = targetIndex;
                proj.timeLeft = 45;
                proj.localNPCHitCooldown = -1;
                proj.penetrate = 3;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.GreenYellow);
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
