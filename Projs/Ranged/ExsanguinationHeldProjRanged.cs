using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class ExsanguinationHeldProjRanged : ExsanguinationHeldProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        protected override void PreAttack()
        {

        }
        protected override void UpdateGlobalReset()
        {

        }
        protected override void OnAttack()
        {
            ScarletSound(HJScarletSounds.Light_Fire, Projectile.Center, volume: 0.25f);
            Owner.PickAmmo(Owner.HeldItem, out int bulletType, out float speed, out int bulletDamage, out float knockback, out _);
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 safedir = Projectile.rotation.ToRotationVector2();
                Vector2 shootPos = Projectile.Center + safedir * 35f - (safedir.RotatedBy(PiOver2) * 3f * Projectile.direction);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shootPos - safedir * 30f + safedir.RotatedBy(PiOver2 * i) * 7f * Main.rand.NextFloat(), safedir * speed, bulletType, bulletDamage, knockback);
                proj.extraUpdates = 5;
                proj.ArmorPenetration = 100000;
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 safedir = Projectile.SafeDirByRot();
                Vector2 shootPos = Projectile.Center + safedir * 60f - (safedir.RotatedBy(PiOver2) * 3f * Projectile.direction);
                Vector2 dir = (shootPos - Owner.Center).ToSafeNormalize();
                Vector2 vel = (-dir).RotatedBy(ToRadians(70f) * Owner.direction).ToRandVelocity(ToRadians(9f), 1.2f, 4.6f);
                Dust d = Dust.NewDustPerfect(shootPos.ToRandCirclePos(4f), Main.rand.NextBool() ? DustID.Torch : DustID.OrangeTorch);
                d.velocity = vel;
                d.scale *= Main.rand.NextFloat(0.8f, 1.2f);
            }
        }
    }
}
