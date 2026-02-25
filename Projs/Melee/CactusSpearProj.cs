using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CactusSpearProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + $"Proj_{nameof(CactusSpear)}";
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(3, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.timeLeft = GetSeconds(5);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4f) + Projectile.SafeDirByRot() * -40f, Main.rand.NextBool() ? DustID.JunglePlants: DustID.JungleGrass);
                d.scale *= Main.rand.NextFloat(0.75f, 0.97f);
                d.velocity = Projectile.SafeDirByRot() * -Main.rand.NextFloat(1.2f, 1.8f);
            }
            if (Main.rand.NextBool())
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(6f) + Projectile.SafeDirByRot() * Main.rand.NextFloat(20f, 60f), DustID.Poisoned);
                d2.scale *= Main.rand.NextFloat(1.0f, 1.3f);
                d2.velocity = Projectile.SafeDirByRot() * -Main.rand.NextFloat(2f, 4f);
                d2.noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.99f;
            Projectile.velocity.X *= 1f;
            if (Projectile.velocity.Y < 10f)
                Projectile.velocity.Y += 0.05f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 36; i++)
            {
                Vector2 dVel = -oldVelocity.ToRandVelocity(ToRadians(40f), 0.2f, 2.1f);
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + oldVelocity.ToSafeNormalize() * 15f;
                Dust d = Dust.NewDustPerfect(spawnPos, DustID.JungleGrass);
                d.scale *= Main.rand.NextFloat(0.75f, 0.97f);
                d.velocity = dVel;
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 36; i++)
            {
                Vector2 dVel = -Projectile.velocity.ToRandVelocity(ToRadians(40f), 0.2f, 2.1f);
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + Projectile.velocity.ToSafeNormalize() * 15f;
                Dust d = Dust.NewDustPerfect(spawnPos, DustID.JungleGrass);
                d.scale *= Main.rand.NextFloat(0.75f, 0.97f);
                d.velocity = dVel;
                d.noGravity = true;
            }

            if(Projectile.numHits < 1)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<CactusSpearSpike>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(lightColor, rotFix: PiOver4, useOldPos: true);
            return false;
        }
    }
}
