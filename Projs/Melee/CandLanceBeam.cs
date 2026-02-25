using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CandLanceBeam : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public List<NPC> TargetThatAlreadyHit = [];
        public int TargetHitTime
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public NPC TargetThatNextCanHit
        {
            get => Main.npc[(int)Projectile.ai[1]];
            set => Projectile.ai[1] = value.whoAmI;
        }
        public List<Vector2> LegalSpawnPos = [];
        public bool HasInflicedConfused = false;
        public ref float Timer => ref Projectile.ai[2];
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            //下方会考虑手动控制处死，或者之类的。
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        private float SearchTargetDistance = 600f;
        private int TotalTargetHitTime = 3;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Blue);
            float offset = 0;
            Timer += 1;

            if (!Projectile.HJScarlet().FirstFrame)
            {
                InitDust();
                if(HJScarletMethods.HasFuckingCalamity)
                {
                    SearchTargetDistance = 1800f;
                    Projectile.timeLeft = 1000;
                    TotalTargetHitTime = 9;
                }
            }

            if (TargetThatAlreadyHit.Count > 0)
                offset = LatterBeamAI(offset);
            else
                FirstBeamAI();

            //轨迹粒子
            GeneralParticle(offset);
        }

        private void GeneralParticle(float offset)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 speed = Projectile.SafeDir() * 0.8f;
            for (int k = 0; k < 2; k++)
            {
                Color waterColor = RandLerpColor(Color.DeepSkyBlue, Color.SkyBlue);
                Vector2 veloffset = Projectile.velocity / 2 * k;
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(3, 3) + veloffset;
                if (offset > 0.1f)
                    SpawnWaterDust(spawnPos);
                new ShinyOrbParticle(spawnPos, speed, waterColor, 40, 0.32f * Main.rand.NextFloat(0.8f, 1.2f), glowCenterScale: 0.2f).Spawn();
                new TurbulenceGlowOrb(spawnPos, 0.2f, waterColor, 40, 0.10f, Main.rand.NextFloat(TwoPi), false).Spawn();
            }
        }
        public void FirstBeamAI()
        {
            //尝试性去搜距离最近的敌人
            if (Timer < 30f)
                return;
            if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex))
                Projectile.HomingTarget(target.Center, 600f, 14f, 20f);
        }
        public float LatterBeamAI(float offset)
        {
            if (!TargetThatNextCanHit.CanBeChasedBy() || TargetThatNextCanHit == null)
            {
                Projectile.Kill();
                return 0f;
            }
            float angleOffset = WrapAngle(Projectile.AngleTo(TargetThatNextCanHit.Center) - Projectile.velocity.ToRotation());
            //此处，需要锐角拐弯
            angleOffset = Clamp(angleOffset, -0.8f, 0.8f);
            Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset);
            offset = Math.Abs(Projectile.velocity.ToRotation() - (TargetThatNextCanHit.Center - Projectile.Center).ToRotation());
            if (Projectile.velocity.Length() < 8f)
                Projectile.velocity *= 1.1f;
            else
                Projectile.velocity *= 0.9f;
            return offset;
        }
        public override bool? CanDamage()
        {
            bool canhit = (TargetThatAlreadyHit.Count > 0 && Timer > 10f) || (TargetThatAlreadyHit.Count == 0 && Timer > 30f);
            return canhit;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (int)Math.Sign(target.Center.X - Owner.Center.X);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //由于这是个类似于射线的东西，我不需要考虑处理贴图转角一类的情况，直接新建一个额外的射弹，然后做伪追踪更好
            //如果目标单位不在列表里面，我们把当前单位添加进去
            if (Main.rand.NextBool(5) && !HasInflicedConfused)
            {
                target.AddBuff(BuffID.Confused, GetSeconds(5));
                HasInflicedConfused = true;
            }
            TargetHitTime += 1;
            if (!TargetThatAlreadyHit.Contains(target))
                TargetThatAlreadyHit.Add(target);
            //如果同时攻击了超过了需要的单位，处死他 
            if (TargetHitTime >= (TotalTargetHitTime))
            {
                Projectile.timeLeft = 2;
                return;
            }
            //创建一个链表，搜索附近可能的单位
            float searchDist = SearchTargetDistance;
            List<NPC> availableTarget = [];
            foreach (NPC needTar in Main.ActiveNPCs)
            {
                bool legalTarget = needTar != target && needTar.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, needTar.Center, 1, 1);
                float distPerTar = Vector2.Distance(needTar.Center, Projectile.Center);
                if (legalTarget && distPerTar < searchDist && !TargetThatAlreadyHit.Contains(needTar))
                {
                    searchDist = distPerTar;
                    //把可用单位甩进去，因为我们需要最后使用一个最靠近的单位
                    availableTarget.Add(needTar);
                }
            }
            //确保链表正确
            if (availableTarget.Count == 0)
            {
                Projectile.Kill();
                return;
            }
            //将链表进行逆向操作，方便索引遍历
            availableTarget.Reverse();
            //随机选择距离最近的其中两个单位，如果有可能的话。
            int maxIndex = Math.Min(availableTarget.Count, 2);
            NPC targetThatHit = availableTarget[Main.rand.Next(0, maxIndex)];
            //最后，直接生成新的射弹
            var src = Projectile.GetSource_FromThis();
            Projectile anotherShot = Projectile.NewProjectileDirect(src, Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Owner.whoAmI, TargetHitTime, targetThatHit.whoAmI);
            //将列表继承至此
            ((CandLanceBeam)anotherShot.ModProjectile).TargetThatAlreadyHit.AddRange(TargetThatAlreadyHit);
            //添加需要的攻击单位
            ((CandLanceBeam)anotherShot.ModProjectile).TargetThatAlreadyHit.Add(targetThatHit);
            ((CandLanceBeam)anotherShot.ModProjectile).HasInflicedConfused = HasInflicedConfused;
            Projectile.Kill();
        }
        private void InitDust()
        {
            if (TargetThatAlreadyHit.Count > 0)
                return;
            for (int i = 0; i < 15; i++)
            {
                float rotOffset = (float)i / TwoPi; 
                new TurbulenceGlowOrb(Projectile.Center + Projectile.SafeDir().RotatedBy(ToRadians(rotOffset)) * 4f, 0.8f, RandLerpColor(Color.LightCyan, Color.LightBlue), 40, 0.1f, Projectile.velocity.ToRandVelocity(ToRadians(10f)).ToRotation()).SpawnToNonPreMult();
            }
        }
        private void SpawnWaterDust(Vector2 needVel)
        {
            //而后，我们生成一段冲击波，模拟地牢水流
            for (int k = 0; k < 2; k++)
            {
                Vector2 spawnPos = needVel;
                Dust d = Dust.NewDustPerfect(spawnPos, DustID.UnusedWhiteBluePurple);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(30f), 2f);
                d.scale *= 1.4f;
                d.position -= d.velocity.SafeNormalize(Vector2.UnitX) * -Main.rand.NextFloat(10f);
                d.noGravity = true;
            }
        }
    }
}
