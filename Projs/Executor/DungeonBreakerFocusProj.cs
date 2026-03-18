using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonBreakerFocusProj : HJScarletProj
    {
        public override string Texture => GetInstance<DungeonBreakerProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            SmashToWall,
            Return
        }
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public List<Vector2> StoredCenter = [];
        public List<Rectangle> StoredRec = [];
        public ref float Timer => ref Projectile.ai[0];
        public int BounceTime = 0;
        public int TotalBouceTime = 15;
        public float CurRotation = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 32;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = 4;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.scale = 0.8f;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
            UpdateParticlesTrail();
        }
        public void UpdateAttackAI()
        {
            switch (AttackType)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.SmashToWall:
                    DoSmashToWall();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        private void DoSmashToWall()
        {
        }

        public void DoShoot()
        {
            StoredRec.Add(Projectile.Hitbox);
            StoredCenter.Add(Projectile.Center);
            Timer++;
            Projectile.rotation += 0.2f;
        }
        public void UpdateParticlesTrail()
        {
            for (int i = 0; i < StoredCenter.Count; i++)
            {
                Vector2 spawnPos = StoredCenter[i].ToRandCirclePos(16);
                if (Projectile.FinalUpdateNextBool(6))
                {
                    //new ShinyOrbParticle(spawnPos, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0.5f).Spawn();
                    //new ShinyCrossStar(spawnPos, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, RandRotTwoPi,1f, 0.24f).Spawn();
                }
            }
        }

        public void DoReturn()
        {
            Projectile.rotation += 0.2f;
            Projectile.ResetBoomerangReturn();
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.IntersectOwnerByDistance())
            {
                Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            bool hit = false;
            for (int i = 0; i < StoredRec.Count; i++)
            {
                hit = Collision.CheckAABBvAABBCollision(StoredRec[i].TopLeft(), StoredRec[i].Size(), targetHitbox.TopLeft(), targetHitbox.Size());
            }
            return hit;
        }
        public void UpdateToNextAttack(State id)
        {
            Projectile.netUpdate = true;
            AttackType = id;
            Timer *= 0;
        }
        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.FinalUpdateNextBool())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(32), Projectile.velocity.ToRandVelocity(ToRadians(15f), 2.4f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, RandRotTwoPi, 1f, 0.5f, 0.2f).Spawn();
            if (Projectile.FinalUpdateNextBool())
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple);
                d.scale *= Main.rand.NextFloat(0.75f, 1.2f);
            }    
        }
        public override void OnFirstFrame()
        {

            StoredCenter.Add(Projectile.Center);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;
            if(AttackType==State.Shoot)
            {
                UpdateOnTileMovement(oldVelocity);
                UpdateOnTileParticle(oldVelocity);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<DungeonBreakShockwave>(), Projectile.damage, 1f, Owner.whoAmI);
                ResetToSmashWall();
            }
            return false;
        }

        private void ResetToSmashWall()
        {
            //后续的情况下不再进入下方的所有状态
            //设置为零速度，且后续我们不在更新速度
            Projectile.velocity = Vector2.Zero;
            //略微嵌入进去
            Projectile.position += Vector2.UnitY * 45f;
            CanSmashDust = true;
            //天顶世界下变成……钢管。
            SoundEngine.PlaySound(Main.zenithWorld ? HJScarletSounds.Pipes : HJScarletSounds.Smash_GroundHeavy, Projectile.Center);
            Timer = 1;
            CurRotation = Owner.direction > 0 ? MathHelper.PiOver4 : -(MathHelper.Pi + MathHelper.PiOver4);
        }

        public void UpdateOnTileMovement(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 1, Pitch = -0.5f + 0.1f * BounceTime }, Projectile.Center);
        }
        public void UpdateOnTileParticle(Vector2 oldVelocity)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(6f) + oldVelocity.ToRandVelocity(0f,4f,8.4f) + Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-1.4f, 1.9f) + Projectile.velocity.ToRandVelocity(0f, 0f,4.8f);
                Color drawColor = RandLerpColor(Color.MidnightBlue, Color.RoyalBlue);
                new ShinyCrossStar(spawnPos, vel, drawColor, 40, RandRotTwoPi, 1f, 0.5f, 0.2f).Spawn();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            //Projectile.DrawGlowEdge(Color.RoyalBlue);
            Projectile.DrawProj(Color.LightBlue);
            return false;
        }
    }
}
