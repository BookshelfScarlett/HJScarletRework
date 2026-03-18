using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class TheMossFocusProj: HJScarletProj
    {
        public override string Texture => GetInstance<TheMossProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Attacking,
            Hanging,
            Return
        }
        public int StrikingTime
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[2];
        public List<NPC> HitTargetList = [];
        public NPC CurHittingTarget = null;
        public bool IsHitting = false;
        public int TotalHitTime = 3;
        public int HitTargetCounts = 6;

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 66;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            UpdateAttack();
            UpdateParticle();
        }
        public override bool? CanHitNPC(NPC target)
        {
            if ((CurHittingTarget != null && CurHittingTarget.Equals(target)) || AttackType == State.Return)
                return null;
            return false;
        }

        private void UpdateAttack()
        {
            if (Projectile.TooAwayFromOwner() || Projectile.timeLeft < 100 || HitTargetList.Count > HitTargetCounts)
                AttackType = State.Return;
            Timer++;
            switch (AttackType)
            {
                case State.Attacking:
                    DoAttacking();
                    break;
                case State.Hanging:
                    DoHanging();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }

        }

        private void DoReturn()
        {
            Projectile.tileCollide = false;
            Projectile.rotation += 0.15f;
            Projectile.timeLeft = 100;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                if (Projectile.HJScarlet().AddFocusHit)
                    Owner.HJScarlet().FocusStrikeTime += 1;
                Projectile.Kill();
            }
        }

        private void DoAttacking()
        {
            //找到当前目标直接跳转到挂载状态.
            if (Projectile.GetTargetSafe(out NPC target, true, 600, false))
            {
                CurHittingTarget = target;
                AttackType = State.Hanging;
                Projectile.netUpdate = true;
                Projectile.rotation += 0.15f;
            }
            else
            {
                //Timer临时控制一下初始状态，避免因为一开始没有目标导致直接被处死
                if (Timer > Projectile.MaxUpdates * 30f)
                {
                    //在满足计时器之后，如果找不到新的目标，都会返程至玩家手上
                    AttackType = State.Return;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.rotation = Math.Abs(Projectile.velocity.X)/3 + Math.Abs(Projectile.velocity.Y)/3;
                }
            }
        }

        private void DoHanging()
        {
            Projectile.rotation += 0.2f;
            //挂载，如果挂载目标已经不符合挂载条件，跳回Attacking的AI重新搜索目标
            if (CurHittingTarget.CanBeChasedBy() && CurHittingTarget != null)
                Projectile.HomingTarget(CurHittingTarget.Center, -1, 16f, 20f);
            else
                AttackType = State.Attacking;
        }
        private void UpdateParticle()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Crimson);
            if (HJScarletMethods.OutOffScreen(Projectile.Center) || IsHitting)
                return;

            if (Projectile.numUpdates == 0 && Main.rand.NextBool(3))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity.ToRandVelocity(ToRadians(10), 1.2f, 1.8f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.5f, 1.1f), false).SpawnToNonPreMult();

            if (Projectile.numUpdates == 0 && Main.rand.NextBool(5))
                new EmptyRing(Projectile.Center.ToRandCirclePos(30f), -Projectile.velocity.ToRandVelocity(ToRadians(10), 1.2f, 1.8f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.25f, 1f, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
            Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(30f), DustID.CorruptionThorns);
            d.velocity = -Projectile.velocity.ToRandVelocity(ToRadians(15f), 1.2f, 1.8f);
            d.scale *= Main.rand.NextFloat(1.2f, 1.4f);
            d.noGravity = true;

            if (Projectile.numUpdates == 0 && Main.rand.NextBool(5))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity.ToRandVelocity(ToRadians(10), 1.2f, 1.8f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.5f, 1.1f), true).SpawnToNonPreMult();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TileCollideParticle(oldVelocity);
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        private void TileCollideParticle(Vector2 velo)
        {
            //SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
            Vector2 vel = velo.ToSafeNormalize();
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 8.8f);
                new SmokeParticle(pos, dVel, RandLerpColor(Color.DarkViolet, Color.Purple), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 8; i++)
            {

                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 4.8f);
                new ShinyOrbHard(pos, dVel, RandLerpColor(Color.DarkGreen, Color.SeaGreen), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 0, Pitch = -0.8f }, target.Center);
            if (StrikingTime < TotalHitTime)
            {
                UpdateHitParticle(target.Center);
                StrikeCurTarget(target);
                IsHitting = true;
            }
            else
            {
                SearchNextTarget(target);
                IsHitting = false;
            }
        }

        private void UpdateHitParticle(Vector2 center)
        {
            for (int i = 0; i < 24; i++)
                new Fire(center.ToRandCirclePos(6f), RandVelTwoPi(1,7f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, RandRotTwoPi, 1f, 0.1f).SpawnToNonPreMult();
            for (int i = 0; i < 24; i++)
                new ShinyOrbHard(center.ToRandCirclePos(10f), RandVelTwoPi(3, 8f), RandLerpColor(Color.DarkGreen, Color.SeaGreen), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            for (int i = 0; i < 7; i++)
                new EmptyRing(Projectile.Center.ToRandCirclePos(20f), -RandVelTwoPi(2f, 4f), RandLerpColor(Color.DarkViolet, Color.Purple), 60, 0.25f * Main.rand.NextFloat(0.75f,1.1f), 1f, altRing: Main.rand.NextBool()).SpawnToNonPreMult();

        }
        private void StrikeCurTarget(NPC target)
        {
            if (CurHittingTarget is null)
                return;
            //当前攻击的目标是同一个，增加攻击次数
            if (target.whoAmI == CurHittingTarget.whoAmI)
                StrikingTime++;
        }
        private void SearchNextTarget(NPC target)
        {
            if (!HitTargetList.Contains(target))
                HitTargetList.Add(target);
            //如果同时攻击了超过了需要的单位，处死他 
            //创建一个链表，搜索附近可能的单位
            float searchDist = 600;
            List<NPC> availableTarget = [];
            foreach (NPC needTar in Main.ActiveNPCs)
            {
                bool legalTarget = needTar != target && needTar.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, needTar.Center, 1, 1);
                float distPerTar = Vector2.Distance(needTar.Center, Projectile.Center);
                if (legalTarget && distPerTar < searchDist && !HitTargetList.Contains(needTar))
                {
                    searchDist = distPerTar;
                    //把可用单位甩进去，因为我们需要最后使用一个最靠近的单位
                    availableTarget.Add(needTar);
                }
            }
            //确保链表正确
            if (availableTarget.Count == 0)
            {
                Projectile.timeLeft = 2;
                return;
            }
            //将链表进行逆向操作，方便索引遍历
            availableTarget.Reverse();
            //随机选择距离最近的其中两个单位，如果有可能的话。
            int maxIndex = Math.Min(availableTarget.Count, 2);
            NPC targetThatHit = availableTarget[Main.rand.Next(0, maxIndex)];
            CurHittingTarget = targetThatHit;
            StrikingTime = 0;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
