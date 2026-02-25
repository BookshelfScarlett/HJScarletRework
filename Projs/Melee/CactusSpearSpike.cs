using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CactusSpearSpike : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.PineNeedleFriendly);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(2);
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 60;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.scale = 0.74f;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(6f), DustID.Poisoned);
                d2.scale *= Main.rand.NextFloat(1.0f, 1.3f);
                d2.velocity = Projectile.SafeDirByRot() * -Main.rand.NextFloat(2f, 4f);
                d2.noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.97f;
            Projectile.velocity.X *= 1f;
            if (Projectile.velocity.Y < 30f)
                Projectile.velocity.Y += 0.18f;
            
        }
        public override bool? CanHitNPC(NPC target)
        {
            bool canHit = Projectile.HJScarlet().GlobalTargetIndex == -1 || (Projectile.HJScarlet().GlobalTargetIndex != -1 && Projectile.ToHJScarletNPC() != target);
            return canHit;
        }
        public override bool PreKill(int timeLeft)
        {
           for (int i = 0;i< 8;i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(2f) + Projectile.velocity.ToRandVelocity(ToRadians(2f), -14f, 14f);
                Dust d = Dust.NewDustPerfect(spawnPos, DustID.JungleGrass);
                d.scale *= Main.rand.NextFloat(0.75f, 0.97f);
                d.velocity = RandVelTwoPi(2f);
                d.noGravity = true;
            }

            return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(lightColor, rotFix:PiOver2);
            return false;
        }
    }
}
