using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class JudgementProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<Judgement>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6, 2);
        }
        public AnimationStruct Helper = new(3);
        public ref float Timer => ref Projectile.ai[0];
        public enum State
        {
            Shoot,
            Return
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void ExSD()
        {
            //气笑了
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.width = Projectile.height = 66;
            Projectile.SetupImmnuity(45);
            Projectile.timeLeft = 300;
            Projectile.scale =1;
            Projectile.ownerHitCheck = true;
        }

        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 30; 
            bool hasProj = Owner.HasProj<JudgementMinion>();
            if(hasProj)
            {
            }
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticles();
        }

        private void UpdateParticles()
        {
            if (Projectile.IsOutScreen())
                return;

            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemDiamond, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(12), DustID.GoldCoin, RandVelTwoPi(4f) + Projectile.velocity / 10f);
                dust.noGravity = true;
                dust.scale = 1.8f;
            }
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
            Timer += 1;
            Projectile.rotation += 0.2f;
            if(Projectile.MeetMaxUpdatesFrame(Timer, 10))
            {
                Projectile.netUpdate = true;
                Timer *= 0;
                AttackState = State.Return;
            }
        }

        public void DoReturn()
        {
            Projectile.rotation += 0.2f;
            Projectile.tileCollide = false;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if(Projectile.IntersectOwnerByDistance(60))
            {
                if (Projectile.HJScarlet().AddFocusHit)
                    Owner.HJScarlet().ExecutionTime += 1;
                if (Projectile.HJScarlet().ExecutionStrike)
                    SpawnExecutionProj();
                Projectile.Kill();
            }
        }

        public void SpawnExecutionProj()
        {
            //音效
            SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit with { MaxInstances = 0, Pitch = 0.5f }, Projectile.Center);
            //当前没有任何挂载锤，则正常进入挂载状态
            if (!Owner.HasProj<JudgementMinion>())
            {
                Projectile.Center.CirclrDust(24, 3f, DustID.HallowedWeapons, 10);
                Projectile lockHammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<JudgementMinion>(), Projectile.damage, 0f, Owner.whoAmI);
                ScreenShakeSystem.AddScreenShakes(lockHammer.Center, 60f, 100, lockHammer.velocity.ToRotation(), 0.1f);
                //处死射弹。
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.42f, 0.47f), Volume = 0.84f, MaxInstances = 1 }, target.Center);
            bool hasProj = Owner.HasProj<JudgementMinion>();
            Vector2 spawnPos =  target.Center;
            Vector2 vel = RandVelTwoPi(16f, 19f);
            if (!hasProj)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<JudgementStarNormal>(), Projectile.damage, 1f, Owner.whoAmI);
                ((JudgementStarNormal)proj.ModProjectile).TargetNPC = target;
            }
            else if (Projectile.numHits < 1)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 starPos = Owner.Center + Projectile.SafeDir().RotatedBy(PiOver2) * i * 25f - Owner.ToMouseVector2() * 100f;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), starPos, Owner.ToMouseVector2().RotatedBy(ToRadians(15f) * i) * 18f, ProjectileType<JudgementStarNormal>(), Projectile.damage / 2, 4f, Owner.whoAmI, ai2: i);
                    ((JudgementStarNormal)proj.ModProjectile).AttackState = JudgementStarNormal.State.OnExecution;
                    ((JudgementStarNormal)proj.ModProjectile).TargetNPC = target;
                }
            }


            for (int i = 0; i < 10; i++)
            {
                new ShinyCrossStar(target.Center.ToRandCirclePos(16f), RandVelTwoPi(1.3f,5f), RandLerpColor(Color.Goldenrod, Color.Orange), 120, RandRotTwoPi, 1f, 0.48f,false).Spawn();
            }
        }
    }
}
