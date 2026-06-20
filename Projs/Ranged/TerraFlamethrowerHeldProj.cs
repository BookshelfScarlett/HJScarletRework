using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class TerraFlamethrowerHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<TerraFlamethrower>().Texture + "Alt";
        public int AttackTime => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime, 2);
        public ref float Timer => ref Projectile.ai[0];
        public ref float ExtraTimer => ref Projectile.localAI[0];
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.noEnchantmentVisuals = true;
            Projectile.width = Projectile.height = 16;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public bool JustPressRight = false;
        public override void OnFirstFrame()
        {
            Vector2 dir = Owner.ToMouseVector2();
            for (int i = 0; i < 16; i++)
            {
                Vector2 vel = -dir.ToRandVelocity(ToRadians(4f), -8.8f, 10.8f) + Owner.velocity;
                Vector2 offset = dir.ToRandVelocity(ToRadians(0), 6f, 9f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 posOffset = offset + Owner.velocity + Main.rand.NextVector2Circular(10f, 5f);
                new ShinyCrossStar(Owner.Center.ToRandCirclePos(20f) + posOffset - Vector2.UnitY * 12f, vel, RandLerpColor(Color.DarkGreen, Color.Green), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false, 0.2f).Spawn();
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = -dir.ToRandVelocity(ToRadians(4f), -10.8f, 10.8f) + Owner.velocity;
                Vector2 offset = dir.ToRandVelocity(ToRadians(0), 7, 11f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 posOffset = offset + Owner.velocity + Main.rand.NextVector2Circular(10f, 5f);
                new SmokeParticle(Owner.Center.ToRandCirclePos(20f) + posOffset - Vector2.UnitY * 12f, vel, RandLerpColor(Color.DarkGreen, Color.Gray), 40, RandRotTwoPi, 1f, 0.34f, Main.rand.NextBool()).SpawnToPriority();
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 1, Pitch = -0.25f }, Owner.Center);
        }
        public override void ProjAI()
        {
            if (CheckOwnerDead())
                return;
            UpdatePlayerState();
            UpdateHeldAnimation();
            if(Owner.controlUseTile && !JustPressRight)
            {
                JustPressRight = true;
            }
            if (JustPressRight)
            {
                Owner.HJScarlet().heldProjReUseTime = 40;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<TerraFlamethrowerTank>(), Projectile.damage, 5f, Owner.whoAmI);
                Vector2 dir = Owner.ToMouseVector2();
                for (int i = 0; i < 16; i++)
                {
                    Vector2 vel = -dir.ToRandVelocity(ToRadians(4f), -8.8f, 10.8f) + Owner.velocity;
                    Vector2 offset = dir.ToRandVelocity(ToRadians(0), 6f, 9f) * Main.rand.NextBool().ToDirectionInt();
                    Vector2 posOffset = offset + Owner.velocity + Main.rand.NextVector2Circular(10f, 5f);
                    new ShinyCrossStar(Owner.Center.ToRandCirclePos(20f) + posOffset - Vector2.UnitY * 12f, vel, RandLerpColor(Color.DarkGreen, Color.Green), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false, 0.2f).Spawn();
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = -dir.ToRandVelocity(ToRadians(4f), -10.8f, 10.8f) + Owner.velocity;
                    Vector2 offset = dir.ToRandVelocity(ToRadians(0), 7, 11f) * Main.rand.NextBool().ToDirectionInt();
                    Vector2 posOffset = offset + Owner.velocity + Main.rand.NextVector2Circular(10f, 5f);
                    new SmokeParticle(Owner.Center.ToRandCirclePos(20f) + posOffset - Vector2.UnitY * 12f, vel, RandLerpColor(Color.DarkGreen, Color.Gray), 40, RandRotTwoPi, 1f, 0.34f, Main.rand.NextBool()).SpawnToPriority();
                }
                SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeTossAlt with { Variants = [1], MaxInstances = 1, Pitch = -0.25f }, Owner.Center);
                Projectile.Kill();
                return;
            }
            UpdateAttack();
            Projectile.netUpdate = true;

            base.ProjAI();
        }

        public void UpdateAttack()
        {
            Vector2 fireSpawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2().SafeNormalize(Vector2.UnitY) * Owner.HeldItem.width * 0.31f;
            Vector2 fireShootVelocity = Projectile.rotation.ToRotationVector2() * Owner.HeldItem.shootSpeed;
            DrawGlowingFireParticle(fireSpawnPosition);
            if (Projectile.MeetMaxUpdatesFrame(ExtraTimer, 13) || ExtraTimer == 0)
            {
                SoundEngine.PlaySound(HJScarletSounds.HymnFireball_Release with { MaxInstances = 0, Pitch = -.35f, PitchVariance = .1f });
                ExtraTimer = 0;
            }
            ExtraTimer++;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 2))
            {
                Timer = 0;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), fireSpawnPosition.ToRandCirclePos(2), fireShootVelocity, ProjectileType<TerraFlamethrowerFlame>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public void DrawGlowingFireParticle(Vector2 pos)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(1f, 3f);
                //ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.Green, Color.White), 40, RandRotTwoPi, 1f, .118f, true, BlendState.Additive);
                ECSParticle.HRShinyOrb(pos.ToRandCirclePos(4f), vel, RandLerpColor(Color.LimeGreen, Color.DarkGreen), 40, 1, 0.03f * Main.rand.NextFloat(.8f, 1.2f), 0.8f);
            }
        }

        private void UpdateHeldAnimation()
        {
            //震动这把枪。
            //Projectile.position += Main.rand.NextVector2Circular(1.3f, 1.3f);
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
        }

        public bool CheckOwnerDead()
        {
            bool ifStillUse = (Owner.channel) && !Owner.noItems && !Owner.CCed;
            if (!ifStillUse)
            {            
                Projectile.Kill();
                return true;
            }
            return false;
        }
        private void UpdatePlayerState()
        {
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemAnimation = Owner.itemTime = 2;
            Owner.ControlPlayerArm(Projectile.rotation);
            Projectile.Center = Owner.MountedCenter;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = new(15 * Owner.direction, 0);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D tex2 = TextureAssets.Item[ItemType<TerraFlamethrower>()].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 rotationPoint = tex.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex2, drawPos + offset.RotatedBy(drawRot), null, Color.White, drawRot, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * .7f, flipSprite, default);
            return false;
        }
    }
}
