using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;


namespace HJScarletRework.Projs.Melee
{
    public class SpearofEscapeBoom : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = Projectile.height = 200;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 45;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                new CrossGlow(Projectile.Center, Color.Orange, 45, 1f, Projectile.scale * 0.4f, false).Spawn();
                //强行用粉色的烟来把这里的范围覆盖起来
                for (int j = 0; j < 15; j++)
                {
                    new SmokeParticle(Projectile.Center, RandVelTwoPi(2f, 8f), RandLerpColor(Color.Orange, Color.Black), 45, RandRotTwoPi, 1f, Projectile.scale * Main.rand.NextFloat(0.8f, 1.26f)).SpawnToPriorityNonPreMult();
                }
                //starShape,往周围扩散，速度最好更快
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Projectile.Center.ToRandCirclePos(50);
                    new StarShape(Projectile.Center.ToRandCirclePos(15f), (spawnPos - Projectile.Center).ToRandVelocity(0, 2f, 8f) * 1.85f, RandLerpColor(Color.OrangeRed, Color.Orange), Projectile.scale * 1.2f, 40).Spawn();
                }
                //crossStar，在周围生成
                for (int i = 0; i < 50; i++)
                {
                    Vector2 spawnPos2 = Projectile.Center.ToRandCirclePos(100);
                    Vector2 dir = (spawnPos2 - Projectile.Center).ToSafeNormalize();
                    new ShinyCrossStar(Projectile.Center + dir * Main.rand.NextFloat(5f), dir * Main.rand.NextFloat(4f, 8f), RandLerpColor(Color.Orange, Color.OrangeRed), 40, RandRotTwoPi, 1f, Projectile.scale, 0.2f).Spawn();
                }
                //生成环绕型的shiny orb
                for (int i = 0; i < 50; i++)
                {
                    Vector2 spawnPos3 = Projectile.Center.ToRandCirclePosEdge(50);
                    new TurbulenceShinyOrb(spawnPos3, 1.8f, RandLerpColor(Color.Orange, Color.OrangeRed), 100, Projectile.scale * Main.rand.NextFloat(0.48f, 0.76f), RandRotTwoPi).Spawn();
                }
                //最后使用原版的粒子系统，在中键这里覆盖直接的火焰粒子
                //这里主要是为了更多的增效，而且另一方面也是为了使用不同的形状
                for (int i = 0; i < 50; i++)
                {
                    Dust fire = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(10), Main.rand.NextBool() ? DustID.Torch : DustID.OrangeTorch);
                    fire.velocity = RandVelTwoPi(1.2f, 3f);
                    fire.scale = Projectile.scale * Main.rand.NextFloat(0.882f, 1.1f);
                    fire.noGravity = false;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
