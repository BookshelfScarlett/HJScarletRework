using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class RuneWizardProj : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Typeless;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 0;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.GetTargetSafe(out NPC target))
                Projectile.HomingTarget(target.Center, -1, 12f, 12f);
            if (Projectile.IsOutScreen())
                return;
            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = Projectile.velocity / 8f;
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6f) - vel * i, DustID.Terra);
                d.velocity = vel;
                d.scale *= Main.rand.NextFloat(0.8f, 1.4f);
                d.noGravity = true;
                Dust d2 = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4f) - vel*i, DustID.RedTorch);
                d2.velocity = vel;
                d2.scale *= Main.rand.NextFloat(0.8f, 1.4f);
                d2.noGravity = true;
            }
            GeneralParticle();

        }
        private void GeneralParticle()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 speed = Projectile.SafeDir() * 0.8f;
                Color waterColor = RandLerpColor(Color.DarkGoldenrod, Color.DarkSeaGreen);
                Vector2 veloffset = Projectile.velocity / 2;
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(3, 3) + veloffset;
                new TurbulenceGlowOrb(spawnPos, 0.22f, waterColor, 40, 0.18f, Main.rand.NextFloat(TwoPi), false).Spawn();
        }
        public override void OnFirstFrame()
        {
            for(int i = 0;i<36;i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4), DustID.Terra);
                d.velocity = RandVelTwoPi(0f, 3.6f);
                d.scale *= Main.rand.NextFloat(0.8f, 1.4f);
                d.noGravity = true;
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
