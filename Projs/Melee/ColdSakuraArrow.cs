using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class ColdSakuraArrow : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public ref float AttackCounter => ref Projectile.ai[1];
        public int CurTargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public List<NPC> TargetList = [];
        public int TotalTrailCounts = 104;
        public enum Style
        {
            Slowdown,
            ShootLaser
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
            Projectile.ToTrailSetting(14, 2);
        }
        public override void ExSD()
        {
            Projectile.height = 6;
            Projectile.width = 6;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 10;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
        }
        public override bool? CanDamage()
        {
            return Timer >= Projectile.MaxUpdates * 5f;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                 SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[1] with { MaxInstances = 0 }, Projectile.Center);
                SpawnFlower();
            }
            Timer++;
            if (Timer > Projectile.MaxUpdates * 5f)
                Timer = Projectile.MaxUpdates * 5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.GetTargetSafe(out NPC target, canPassWall: true))
            {
                Projectile.HomingTarget(target.Center, -1, 12f, 30f, 4f);
            }

            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            for (int i = 0; i < 2; i++)
                new StarShape(Projectile.Center - Projectile.velocity / 2 * i, Projectile.velocity.ToSafeNormalize(), RandLerpColor(Color.HotPink, Color.DeepPink), .80f, 40).Spawn();
            if (Main.rand.NextBool())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(5f), Projectile.velocity.ToRandVelocity(ToRadians(15), 1), RandLerpColor(Color.HotPink, Color.DeepPink), 40, RandRotTwoPi, 1f, 0.4f, 0.2f).Spawn();
        }
        public void SpawnFlower()
        {
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(2), DustID.WitherLightning, RandVelTwoPi(2f));
                d.rotation = RandRotTwoPi;
                d.velocity += Projectile.velocity.ToRandVelocity(0, 2);
                d.scale = 0.8f;
                d.noGravity = true;
            }
        }


        public override bool? CanHitNPC(NPC target)
        {

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnFlower();
            if (AttackCounter > 3f)
            {
                Projectile.Kill();
                return;
            }
            if (!TargetList.Contains(target))
                TargetList.Add(target);
            //每一次发起攻击都要重置一遍计时器，计时器也会控制candamage
            //因为我们不想让寒霜弹在同一个目标上多次造成伤害
            //Timer *= 0;
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[1] with { MaxInstances = 2 }, Projectile.Center);
            float searchDistance = 600f;
            List<NPC> legalTargetList = [];
            foreach (var tar in Main.ActiveNPCs)
            {
                bool legalTar = tar != target && tar.CanBeChasedBy();
                float distPerTar = Vector2.Distance(tar.Center, Projectile.Center);
                //别穿墙搜
                if (legalTar && distPerTar < searchDistance && !TargetList.Contains(tar))
                {
                    searchDistance = distPerTar;
                    legalTargetList.Add(tar);
                }
            }
            //没有可用单位时立刻处死寒霜弹，不用考虑别的
            if (legalTargetList.Count <= 0)
            {
                Projectile.Kill();
                return;
            }
            //将链表进行逆向操作，方便索引遍历
            legalTargetList.Reverse();
            //随机选择距离最近的其中两个单位，如果有可能的话。
            int maxIndex = Math.Min(legalTargetList.Count, 2);
            NPC targetThatHit = legalTargetList[Main.rand.Next(0, maxIndex)];
            Projectile.HJScarlet().GlobalTargetIndex = targetThatHit.whoAmI;
            //最后将当前敌对单位扔到全局的index里面
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
