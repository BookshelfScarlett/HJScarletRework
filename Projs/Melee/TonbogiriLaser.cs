using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class TonbogiriLaser : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 10;
            Projectile.width = Projectile.height = 8;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 3; i++)
            {
                Vector2 safeVel = Projectile.velocity / 3;
                //new ShinyOrbParticle(Projectile.Center - safeVel * i, safeVel, Color.DarkViolet, 40, 0.9f).Spawn();
                //new TrailShinyOrb(Projectile.Center - safeVel * i, safeVel, Color.DarkViolet, 40, 0.1f, true).Spawn();
                new TrailGlowBall(Projectile.Center - safeVel * i, safeVel, Color.DarkViolet, 40, 0.08f, true).Spawn();
                new TrailGlowBall(Projectile.Center - safeVel * i, safeVel, Color.White, 40, 0.04f, true).Spawn();
                //Dust d = Dust.NewDustPerfect(Projectile.Center - safeVel * i, DustID.WitherLightning);
                //d.velocity = safeVel;
                //d.position += Projectile.SafeDirByRot().RotatedBy(PiOver2) * MathF.Sin(Projectile.localAI[0]) * 10f;
                //Projectile.localAI[0] += ToRadians(1 * i);
                //d.noGravity = true;
                //d.scale *= 0.8f;
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
