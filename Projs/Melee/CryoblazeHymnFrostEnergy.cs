using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CryoblazeHymnFrostEnergy : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public enum Style
        {
            Slowdown,
            Attacking,
            HomingBack
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int BoucneTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8, 2);
        }
        public float Speed = 0f;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 500;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            if(!Projectile.HJScarlet().FirstFrame)
            {
                Speed = Projectile.velocity.Length();
            }
            ParticleSpawn();
            if (AttackType == Style.Slowdown)
            {
                Projectile.velocity *= 0.93f;
                Timer++;
                if (Timer < 20f)
                    return;
                Projectile.netUpdate = true;
                AttackType = Style.Attacking;
                Timer = 0f;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Timer++;
                //如果是首次过来，而且没有命中的情况，我们才让寒霜弹自主索敌
                if (Projectile.GetTargetSafe(out NPC target, AttackType == Style.Attacking, 1200, true) && Timer > 10f)
                    Projectile.HomingTarget(target.Center, -1f, 12f, 20f);
                else
                {
                    if(Projectile.velocity.LengthSquared() < Speed * Speed)
                    {
                        Projectile.velocity *= 1.1f;
                    }
                }
            }
            
        }
        public void ParticleSpawn()
        {
            //本质素材复用，但是这里的粒子总体来说，一定程度上降低了生成数量，因为这里可能场上会有非常非常多
            //向上喷火的粒子，使用原版的更加合适一些。
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            Vector2 pos = Projectile.Center.ToRandCirclePos(4f);
            Vector2 vel = -Projectile.velocity / 4;
            //这里的球比起雪球，更接近于霜火球
            //因此，会沿途留下烟火轨迹
            //烟的粒子会靠后一点，并且会先生成让原版的冰粒子正常绘制
            new SmokeParticle(pos - Projectile.SafeDir() * 15f, vel * 0.5f, RandLerpColor(Color.DeepSkyBlue, Color.White), 20, 0.5f, 1f, 0.12f).SpawnToPriorityNonPreMult();
            //生成光球类粒子。增效
            new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity.ToRandVelocity(ToRadians(10f), -1.2f), RandLerpColor(Color.DeepSkyBlue, Color.LightBlue), 15, 0.35f).Spawn();
        }
        public override bool? CanDamage()
        {
            bool canDamage = Timer > 10f && AttackType != Style.Slowdown;
            return canDamage;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item48 with { MaxInstances = 1, Volume = 0.754f }, Projectile.Center);
            for (int i = 0; i < 10; i++)
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(2f), 0.8f, RandLerpColor(Color.SkyBlue, Color.AliceBlue), 40, Main.rand.NextFloat(0.1f, 0.12f), RandRotTwoPi, true).Spawn();
            for (int i = 0; i < 5; i++)
                new SmokeParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(1.2f), RandLerpColor(Color.DeepSkyBlue, Color.Gray), 20, RandRotTwoPi, 1f, 0.2f).Spawn();
            //每一次发起攻击都要重置一遍计时器，计时器也会控制candamage
            //因为我们不想让寒霜弹在同一个目标上多次造成伤害
            Timer *= 0;
            float searchDistance = 600f;
            List<NPC> legalTargetList = [];
            foreach (var tar in Main.ActiveNPCs)
            {
                bool legalTar = tar != target && tar.CanBeChasedBy();
                float distPerTar = Vector2.Distance(tar.Center, Projectile.Center);
                //别穿墙搜索
                if (legalTar && distPerTar < searchDistance && Collision.CanHit(Projectile.Center, 1, 1, tar.Center, 1, 1))
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
            //最后将当前敌对单位扔到全局的index里面
            Projectile.HJScarlet().GlobalTargetIndex = targetThatHit.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = 6;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.90f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.LightSkyBlue, Color.Lerp(Color.LightSkyBlue, Color.Blue, rads * 0.7f), (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.5f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.DeepSkyBlue.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.65f, 0, 0);
            return false;
        }
    }
}
