using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class WildPointerThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<WildPointerThrown>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting();
        private bool CanHomingToTarget
        {
            get => Projectile.ai[0] == 1;
            set => Projectile.ai[0] = value ? 1 : 0;
        }
        private enum Styles
        {
            Attack,
            Homing,
            Decay
        }
        private Styles AttackType
        {
            get => (Styles)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float Timer => ref Projectile.ai[2];
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            //有意为之
            Projectile.timeLeft = 150;
        }
        public bool Init = false;
        public bool FoundTarget = false;
        public NPC NeedTarget = null;
        public List<NPC> ValidTarget = [];
        public override void AI()
        {
            Projectile.light = Projectile.Opacity;
            //超出距离直接处死，避免这东西真的全图跑
            if (Vector2.Distance(Projectile.Center, Owner.MountedCenter) > 3600f)
                Projectile.Kill();

            switch (AttackType)
            {
                case Styles.Attack:
                    DoAttack();
                    break;
                case Styles.Homing:
                    DoHoming();
                    break;
                case Styles.Decay:
                    DoDecay();
                    break;
            }
            DrawDust();
            Init = true;
        }
        private void DoHoming()
        {
            //eu需要临时设定成0……或者也不用
            //锁住生命值
            Projectile.timeLeft = 100;
            Timer++;
            //一秒内没找到任何敌人，直接进入处死
            if (Timer > 60 && !FoundTarget)
            {
                AttackType = Styles.Decay;
                Projectile.netUpdate = true;
                Timer = 0;
                //天顶世界下，这玩意会炸你游戏.
                //毕竟他是野指针
            }

            //随机追踪这一块
            if (!FoundTarget)
            {
                foreach (var target in Main.ActiveNPCs)
                {
                    //野指针的攻击目标会选择几乎所有可能可以用的NPC
                    if (!target.CanBeChasedBy(Projectile))
                        continue;
                    ValidTarget.Add(target);
                }
                //而后我们随机从可用列表中选择一个
                NeedTarget = Utils.SelectRandom(Main.rand, ValidTarget.ToArray());
                FoundTarget = true;
                return;
            }
            if (!NeedTarget.CanBeChasedBy(Projectile) || NeedTarget == null)
            {
                AttackType = Styles.Decay;
                Projectile.netUpdate = true;
            }
            float angleToWhat = (NeedTarget.Center - Projectile.Center).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让矛朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.3f);
            //追踪敌人
            Projectile.HomingTarget(NeedTarget.Center, 9999f, 20f, 20f);
        }

        private void DoDecay()
        {

            Projectile.velocity *= 0.925f;
            Projectile.Opacity -= 0.02f;
            if (Projectile.Opacity == 0f)
                Projectile.Kill();
        }

        private void DoAttack()
        {
            Projectile.velocity *= 0.935f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Length() > 0.15f)
                return;

            Projectile.velocity *= 0f;
            if (CanHomingToTarget)
                AttackType = Styles.Homing;
            else
                AttackType = Styles.Decay;
            Projectile.netUpdate = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            AttackType = Styles.Decay;
            Projectile.netUpdate = true;
        }
        private void DrawDust()
        {
            //挥发性粒子
            Color Firecolor = Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, Main.rand.NextFloat(0, 1));
            if (!Init)
                new TurbulenceShinyOrb(Projectile.Center, 1f, Firecolor, 40, 0.1f, Main.rand.NextFloat(TwoPi)).Spawn();
            if (Main.rand.NextBool(4))
            {
                Vector2 spawnPos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver4 / 8 * Main.rand.NextBool().ToDirectionInt());
                Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Electric, Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 3f) * Clamp(Projectile.velocity.Length(), 0f, 1f));
                dust.noGravity = true;
                dust.color = Firecolor;
                dust.scale *= 0.5f * Projectile.Opacity;
                dust.position += Main.rand.NextFloat(10f, 12f) * Projectile.rotation.ToRotationVector2();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White * Projectile.Opacity, posMove: 1.2f, rotFix: PiOver4);
            Projectile.DrawProj(Color.White * Projectile.Opacity, drawTime: 1,rotFix: PiOver4);
            return false;
        }
    }
}
