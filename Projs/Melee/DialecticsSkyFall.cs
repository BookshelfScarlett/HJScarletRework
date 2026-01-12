using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Dialectics = ContinentOfJourney.Items.Dialectics;
using Materialism = ContinentOfJourney.Projectiles.Meelee.Materialism;

namespace HJScarletRework.Projs.Melee
{
    public class DialecticsSkyFall : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => GetInstance<Materialism>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6, 2);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 4;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        
        public override void AI()
        {
            if(Projectile.GetTargetSafe(out NPC target, true,600f))
                Projectile.HomingTarget(target.Center, 1800f, 20f, 20f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), DustID.MushroomSpray);
            d.velocity = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(12f,16f);
            d.scale *= Main.rand.NextFloat(1.2f, 1.4f);
            d.noGravity = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            SoundEngine.PlaySound(HJScarletSounds.Dialectics_Hit with { MaxInstances = 1, Pitch = 0.5f, PitchVariance = 0.2f ,Volume = 0.7f}, Projectile.Center);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0;i < 15;i++)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(PiOver4 / 16 * Main.rand.NextBool().ToDirectionInt()));
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Vector2 vel = dir * Main.rand.NextFloat(12f, 14f);
                new StarShape(spawnPos, vel, Color.Blue, 0.8f, 40).Spawn();
                new StarShape(spawnPos, vel, Color.White, 0.4f, 40).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix: PiOver4);
            Projectile.DrawProj(Color.White, rotFix: PiOver4);
            return false;
        }
    }
}
