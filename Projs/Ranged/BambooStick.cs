using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class BambooStick : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(3);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if(Timer >10)
            {
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 450))
                    Projectile.HomingTarget(target.Center, -1, 19f, 10f);

            }
            if (Projectile.IsOutScreen())
                return;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 30), DustID.GrassBlades);
            d.scale *= Main.rand.NextFloat(1.1f, 1.3f);
            d.velocity = Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(1.2f, 1.8f);
            d.noGravity = true;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0;i<16;i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(10), DustID.JungleGrass);
                d.scale *= Main.rand.NextFloat(1.1f, 1.3f);
                d.velocity = RandVelTwoPi(1f, 3.4f);
                d.noGravity = true;
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(lightColor);
            return false;
        }
    }
}
