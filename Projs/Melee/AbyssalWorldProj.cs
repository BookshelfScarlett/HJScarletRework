using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class AbyssalWorldProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4, 2);
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.height = Projectile.width = 16;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            UpdateParticles();
            
        }

        private void UpdateParticles()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            new SmokeParticle(Projectile.Center.ToRandCirclePos(6f), RandVelTwoPi(1f, 3f), RandLerpColor(Color.DarkSlateBlue, Color.Aqua), 40, RandRotTwoPi, 1, 0.24f).SpawnToNonPreMult();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rotFix = PiOver4;
            Projectile.DrawGlowEdge(Color.White, rotFix: rotFix);
            Projectile.DrawProj(lightColor, rotFix: rotFix);
            return false;
        }


    }
}
