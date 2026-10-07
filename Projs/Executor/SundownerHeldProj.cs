using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerHeldProj : HJScarletRangedWeaponoutClass
    {
        public override string Texture => GetInstance<Sundowner>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override int OriginalItemID => ItemType<Sundowner>();
        public override float HoldoutDrawScale => base.HoldoutDrawScale;
        public override Vector2 HoldoutOffset => new Vector2(0, -10);
        public override bool HoldoutEdgeEnable => false;
        protected override void UpdateRecoil()
        {
            if (!Owner.IsHolding(OriginalItemID))
                return;
            if (Projectile.CheckExecution() && Projectile.numUpdates == 0)
            {
                //将武器标记为发起处决模式
                Projectile.HJScarlet().ExecutionStrike = true;
                Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), Owner.Center, Vector2.Zero, ProjectileType<SundownerFlareGun>(), 0, 0, Owner.whoAmI);
                proj.originalDamage = Projectile.originalDamage;
                //移除处决进程
                Owner.RemoveExecutionProgress();
                Owner.HJScarlet().tacticalExecutionInputCache = 0;
            }
            Projectile.HJScarlet().ExecutionStrike = false;
        }
        //后坐力
        protected override void UpdateWeaponUsing()
        {
            Projectile.position += Main.rand.NextVector2Circular(1.3f, 1.3f);
            Projectile.position += -Projectile.SafeDirByRot() * Main.rand.NextFloat(5f, 10f);
        }
        public int Reverse = 1;
        protected override void PreAttack()
        {
            base.PreAttack();
        }
        protected override void OnAttack()
        {
            Vector2 offset2 = new(0 * Owner.direction, -10f);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 firePos = Projectile.Center + offset2.RotatedBy(drawRot);
            bool nonStop = !Owner.HasProj<SundownerFlare>() && !Owner.HasProj<SundownerFlareGun>() && !Projectile.HJScarlet().ExecutionStrike;
            if (Projectile.IsMe())
            {
                ScreenShakeSystem.AddScreenShakes(firePos, 1f, 10, Projectile.rotation + Pi, 0f);

                SlotId slotId1 = SoundEngine.PlaySound(HJScarletSounds.Misc_Boom with { Variants = [1], MaxInstances = 0, Pitch = 0.5f, Volume = .35f }, Projectile.Center);
                if (SoundEngine.TryGetActiveSound(slotId1, out var sound) && !nonStop)
                {
                    sound.Volume /= 2;
                    sound.Pitch = 0.2f;
                }
                for (int i = -1; i < 2; i += 2)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), firePos + Projectile.SafeDirByRot() * 30f + Projectile.SafeDirByRot().RotatedBy(PiOver2) * i * 10f, Projectile.rotation.ToRotationVector2() * 20, ProjectileType<SundownerAmmo>(), Projectile.originalDamage, 0, Owner.whoAmI);
                    if (nonStop)
                        proj.HJScarlet().HasExecutionMechanic = true;
                    ((SundownerAmmo)proj.ModProjectile).CanPlaySound = i == -1;
                }
                if (Reverse == 1)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), firePos + Projectile.SafeDirByRot() * 30f + Projectile.SafeDirByRot().RotatedBy(PiOver2) * i * -10f, Projectile.rotation.ToRotationVector2() * 10, ProjectileType<SundownerFireball>(), (int)(Projectile.originalDamage * .5f), 0, Owner.whoAmI);
                        proj.extraUpdates += 1;
                    }
                }
                Reverse *= -1;
                Vector2 firePos2 = firePos - Projectile.SafeDirByRot() * 30f;
                Vector2 dir2 = Projectile.SafeDirByRot() * -1;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 vel = dir2.ToRandVelocity(ToRadians(10f), 0.8f, 10.8f);
                    Vector2 offset = dir2.ToRandVelocity(ToRadians(0), 6f, 9f);
                    Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir2 * 0f;
                    ECSParticle.ShinyCrossStarECS(firePos.ToRandCirclePos(20f) + posOffset, vel, RandLerpColor(Color.OrangeRed, Color.Red), 40, 1f, Main.rand.NextFloat(0.5f, 0.8f) * .7f, .2f);
                }
                for (int i = 0; i < 12; i++)
                {
                    Vector2 vel = dir2.ToRandVelocity(ToRadians(10f), 0.8f, 10.8f);
                    Vector2 offset = dir2.ToRandVelocity(ToRadians(0), 7, 11f);
                    Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir2 * 0f;
                    new SmokeParticle(firePos2.ToRandCirclePos(10f) + posOffset, vel, RandLerpColor(Color.White, Color.Lerp(Color.OrangeRed, Color.Red, 0.4f)), 40, RandRotTwoPi, 1f, 0.20f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
                }

                Vector2 dir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 14; i++)
                {
                    Vector2 vel = dir.ToRandVelocity(ToRadians(10f), 1.8f, 20.8f);
                    Vector2 offset = dir.ToRandVelocity(ToRadians(0), 6f, 9f);
                    Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir * 20f;
                    ECSParticle.ShinyCrossStarECS(firePos.ToRandCirclePos(20f) + posOffset, vel, RandLerpColor(Color.OrangeRed, Color.Orange), 40, 1f, Main.rand.NextFloat(0.5f, 0.8f), .2f);
                }
                for (int i = 0; i < 16; i++)
                {
                    Vector2 vel = dir.ToRandVelocity(ToRadians(10f), 1.8f, 30.8f);
                    Vector2 offset = dir.ToRandVelocity(ToRadians(0), 7, 11f);
                    Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir * 20f;
                    new SmokeParticle(firePos.ToRandCirclePos(10f) + posOffset, vel, RandLerpColor(Color.White, Color.Lerp(Color.OrangeRed, Color.Gold, 0.4f)), 40, RandRotTwoPi, 1f, 0.34f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
                }
            }
        }
    }
}
