using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<Sundowner>().Texture;
        public override ClassCategory Category => ClassCategory.Ranged;
        public ref float Timer => ref Projectile.ai[0];
        public AnimationStruct Helper = new(2);
        public bool CanShoot = false;
        public bool JustSpawned = false;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.SetupImmnuity(-1);
            Projectile.width = Projectile.height = 40;
            Projectile.Opacity = 0;
        }
        public override void OnFirstFrame()
        {
            JustSpawned = true;
            Helper.MaxProgress[0] = 20;
            Helper.MaxProgress[1] = 10;
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (Projectile.Opacity < 0.98f)
                Projectile.Opacity = Lerp(Projectile.Opacity, 1, 0.15f);
            else
                Projectile.Opacity = 1;
            if (HandleDeadOrAlive())
            {
                Projectile.Kill();
                return;
            }
            HandleOwnerState();
            HandleRecoil();
            HandleAttack();
        }
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;

        public bool HandleDeadOrAlive()
        {
            if (Owner.HeldItem.type != ItemType<Sundowner>())
            {
                return true;
            }
            Projectile.timeLeft = 2;
            return false;
        }
        /// <summary>
        /// 直接管理的工具方法。
        /// </summary>
        /// <returns></returns>
        public bool HandleExecution()
        {
            if (Owner.CheckExecution(ItemType<Sundowner>()) && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ProjectileType<SundownerFlareGun>(), 0, 0, Owner.whoAmI);
                proj.originalDamage = Projectile.originalDamage;
                Owner.RemoveExecutionProgress(ItemType<Sundowner>());


                return true;
            }
            Owner.HJScarlet().CanExecution = false;
            return false;
        }
        public void HandleAttack()
        {
            Vector2 offset2 = new(0 * Owner.direction, -10f);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 firePos = Projectile.Center + offset2.RotatedBy(drawRot);
            HandleExecution();
            bool nonStop = !Owner.HasProj<SundownerFlare>() && !Owner.HasProj<SundownerFlareGun>() && !Projectile.HJScarlet().ExecutionStrike;
            if (Timer % 10 == 0 && Projectile.IsMe())
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
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), firePos + Projectile.SafeDirByRot() * 30f + Projectile.SafeDirByRot().RotatedBy(PiOver2) * i * 10f, Projectile.rotation.ToRotationVector2() * 20, ProjectileType<SundownerAmmo>(), Projectile.originalDamage, 0, Owner.whoAmI);
                    if (nonStop)
                        proj.HJScarlet().HasExecutionMechanic = true;
                    ((SundownerAmmo)proj.ModProjectile).CanPlaySound = i == -1;
                }
                for (int i = -1; i < 2; i += 2)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), firePos + Projectile.SafeDirByRot() * 30f + Projectile.SafeDirByRot().RotatedBy(PiOver2) * i * 10f, Projectile.rotation.ToRotationVector2() * 10, ProjectileType<SundownerFireball>(), Projectile.originalDamage, 0, Owner.whoAmI);
                    if (i == -1 && !Owner.HasProj<SundownerFlare>() && !Owner.HasProj<SundownerFlareGun>() && !Projectile.HJScarlet().ExecutionStrike)
                        proj.HJScarlet().HasExecutionMechanic = true;
                    proj.extraUpdates += 1;
                }
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
            Projectile.HJScarlet().ExecutionStrike = false;
        }
        public void ResetAniState()
        {
            Helper.IsDone[0] = false;
            Helper.Progress[0] = 0;
        }
        public void HandleRecoil()
        {
            if (!IsUsing)
            {
                Timer = 9;
            }
            else
            {
                Projectile.position += Main.rand.NextVector2Circular(1.3f, 1.3f);
                Projectile.position += -Projectile.SafeDirByRot() * Main.rand.NextFloat(5f, 10f);
                Timer++;
                if (Timer > 60f)
                    Timer = 1;
            }
        }
        public void HandleOwnerState()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation);
            if (!JustSpawned)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter, 0.2f);
                if ((Projectile.Center - Owner.MountedCenter).LengthSquared() < 5f)
                    JustSpawned = true;
            }
            else
            {
                Projectile.Center = Owner.MountedCenter;

            }
            Owner.heldProj = Projectile.whoAmI;
        }

        public Vector2 DrawOffset = Vector2.Zero;

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = new(0 * Owner.direction, -10f);
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition + DrawOffset;
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 rotationPoint = tex.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Owner.gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex, drawPos + offset.RotatedBy(drawRot), null, Color.Lerp(Color.Transparent, Color.White, Projectile.Opacity), drawRot, rotationPoint, Projectile.scale * 1f, flipSprite, default);
            return false;
        }
    }
}
