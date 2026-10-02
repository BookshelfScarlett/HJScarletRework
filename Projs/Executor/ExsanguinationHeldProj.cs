using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class ExsanguinationHeldProj : HJScarletRangedWeaponoutClass
    {
        public override int AttackSpeed => 2;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<Exsanguination>().Texture;
        public override int OriginalItemID => ItemType<Exsanguination>();
        public override float HoldoutDrawScale => .5f;
        public override bool HoldoutEdgeEnable => false;
        public override Vector2 HoldoutOffset => new Vector2(15, 5);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override int ProjExtraUpdates => 0;
        protected override void UpdateRecoil()
        {
            // 复写后什么都不做，这样可以让武器不执行后坐力动画
        }
        protected override void UpdateWeaponUsing()
        {
            Projectile.position += Main.rand.NextVector2Circular(1.3f, 1.3f);
        }
        protected override void PreAttack()
        {
            if (!Owner.GetExecutionSrike())
                Projectile.HJScarlet().ExecutionStrike = false;
            if (Owner.GetExecutionSrike() && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Owner.HJScarlet().ExecutionBuffTimeStored.TryAdd(OriginalItemID, GetSeconds(5));
                ScarletSound(HJScarletSounds.Light_CrackedShield, Owner.Center, volume: .45f);
                Owner.RemoveExecutionProgress(OriginalItemID);
            }
        }
        protected override void UpdateGlobalReset()
        {
            Projectile.HJScarlet().ExecutionStrikeManual = false;
        }
        protected override void OnAttack()
        {
            if (Owner.HJScarlet().ExecutionBuffTimeStored.TryGetValue(OriginalItemID, out int value))
                Projectile.HJScarlet().ExecutionStrikeManual = true;
            int damage = Projectile.originalDamage;
            if (Projectile.HJScarlet().ExecutionStrikeManual)
                damage = (int)(Projectile.originalDamage * 1.12f);
            Owner.PickAmmo(Owner.HeldItem, out _, out _, out _, out _, out int ammoID);
            bool homing = ammoID == ItemID.ChlorophyteBullet;
            int bulletType = homing ? ProjectileType<ExsanguinationHomingBullet>() : ProjectileType<ExsanguinationBulletProj>();
            Player owner = Main.player[Projectile.owner];
            if (owner.HeldItem is not null && owner.HeldItem.ModItem is not null && owner.HeldItem.ModItem is Exsanguination exsam && exsam.RangerMode)
            {
                ScarletSound(HJScarletSounds.Light_Fire, Projectile.Center, volume: 0.25f);
                Owner.PickAmmo(Owner.HeldItem, out int RangedbulletType, out float speed, out int bulletDamage, out float knockback, out _);
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 safedir = Projectile.rotation.ToRotationVector2();
                    Vector2 shootPos = Projectile.Center + safedir * 35f - (safedir.RotatedBy(PiOver2) * 3f * Projectile.direction);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shootPos - safedir * 30f + safedir.RotatedBy(PiOver2 * i) * 7f * Main.rand.NextFloat(), safedir * speed, RangedbulletType, bulletDamage, knockback);
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
            else
            {
                ScarletSound(HJScarletSounds.Light_Fire, Projectile.Center, volume: 0.25f);
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 safedir = Projectile.rotation.ToRotationVector2();
                    Vector2 shootPos = Projectile.Center + safedir * 60f - (safedir.RotatedBy(PiOver2) * 5f * Projectile.direction);
                    Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), shootPos - safedir * 40f + safedir.RotatedBy(PiOver2 * i) * 7f * Main.rand.NextFloat(), safedir * 10f, bulletType, damage, Projectile.knockBack);
                    if (!homing)
                        proj.HJScarlet().HasExecutionMechanic = !Projectile.HJScarlet().ExecutionStrikeManual;
                }
                for (int i = 0; i < 4; i++)
                {
                    Vector2 safedir = Projectile.SafeDirByRot();
                    Vector2 shootPos = Projectile.Center + safedir * 65f - (safedir.RotatedBy(PiOver2) * 5f * Projectile.direction);
                    Vector2 dir = (shootPos - Owner.Center).ToSafeNormalize();
                    Vector2 vel = (-dir).RotatedBy(ToRadians(70f) * Owner.direction).ToRandVelocity(ToRadians(9f), 1.2f, 4.6f);
                    Dust d = Dust.NewDustPerfect(shootPos.ToRandCirclePos(4f), Main.rand.NextBool() ? DustID.Torch : DustID.OrangeTorch);
                    d.velocity = vel;
                    d.scale *= Main.rand.NextFloat(0.8f, 1.2f);
                }
            }
        }
    }
}
