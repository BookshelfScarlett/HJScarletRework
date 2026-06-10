using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerFlareGun : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.FlareGun);
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public AnimationStruct Helper = new(3);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.spriteDirection = 1;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return Helper.IsDone[1];
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 15;
            Helper.MaxProgress[1] = 10;
            Helper.MaxProgress[2] = 20;
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeTossAlt with { Variants = [3], MaxInstances = 0, Pitch = -0.5f });
        }
        public float Osci = 0f;
        public Vector2 ShootSkyPosition = Vector2.Zero;
        public Vector2 BeginPosition = Vector2.Zero;
        public override void ProjAI()
        {
            Projectile.spriteDirection = ((Main.MouseWorld.X - Owner.MountedCenter.X) > 0).ToDirectionInt();
            if (!Helper.IsDone[1])
            {
                Osci += ToRadians(5f);
                Projectile.timeLeft = GetSeconds(5);
                float mountedRot = ToDegrees(Owner.ToMouseVector2().ToRotation());
                Vector2 mountedPos = Vector2.UnitX.RotatedBy(ToRadians(mountedRot + 30 * Projectile.spriteDirection)) * -180f + Owner.MountedCenter;
                if (Timer == 0)
                {
                    Vector2 dir = (mountedPos - Projectile.Center).ToSafeNormalize();
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 vel = dir.ToRandVelocity(ToRadians(15f), -.2f, 15f);
                        new SmokeParticle(Projectile.Center + vel.ToSafeNormalize() * i * .5f, dir.ToRandVelocity(ToRadians(15f), -.2f, 15f), RandLerpColor(Color.OrangeRed, Color.White), 40, RandRotTwoPi, 1, 0.3f * Main.rand.NextFloat(.5f, .9f), true).Spawn();
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        new StarShape(Projectile.Center, dir.ToRandVelocity(ToRadians(15f), -.2f, 15f), RandLerpColor(Color.OrangeRed, Color.Orange), Main.rand.NextFloat(.3f, .6f), 40).Spawn();
                    }
                }
                mountedPos.Y += (float)Math.Sin(Osci / 10f) * 15f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.3f);
                Projectile.position += Main.rand.NextVector2Circular(1.4f, 1.4f);
            }
            if (Timer < 20f)
            {
                Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.1f);
                Projectile.rotation += Main.rand.NextFloat(ToRadians(5f), ToRadians(5f)) * Main.rand.NextBool().ToDirectionInt();
                Timer++;
                return;
            }

            if (!Helper.IsDone[0])
            {
                if (Helper.GetAniProgress(0) == 0)
                    SoundEngine.PlaySound(HJScarletSounds.Air_HeavyFlow with { Pitch = .7f, Variants = [2], PitchVariance = 0.1f, MaxInstances = 0 });
                Projectile.rotation += 1.492f * -Projectile.spriteDirection;
                PlayRotationParticle();
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                if (Helper.GetAniProgress(1) == 0)
                {
                    BeginPosition = Main.MouseWorld;
                    ShootSkyPosition = new Vector2(Lerp(Owner.ToClampMouseVector2().X, Owner.Center.X, 0.30f), Main.screenPosition.Y - 200f);
                }
                Helper.UpdateAniState(1);
                float pro = EaseOutBack(Helper.GetAniProgress(1));
                Vector2 skyPos = Vector2.Lerp(BeginPosition, ShootSkyPosition, pro);
                Projectile.rotation = Projectile.rotation.AngleLerp((skyPos - Projectile.Center).ToRotation(), pro);
                Projectile.rotation += Main.rand.NextFloat(ToRadians(10f), ToRadians(10f)) * Main.rand.NextBool().ToDirectionInt();

            }
            else if (!Helper.IsDone[2])
            {
                if (Helper.GetAniProgress(2) == 0)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * -50f;
                    SoundEngine.PlaySound(HJScarletSounds.Misc_Boom with { Variants = [2], MaxInstances = 0, Pitch = 0.8f });
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirByRot() * 16f, ProjectileType<SundownerFlare>(), 0, 0, Owner.whoAmI);
                    proj.originalDamage = Projectile.originalDamage;

                    PlayParticle();
                }
                if (Helper.GetAniProgress(2) > .4f)
                {
                    Projectile.velocity.Y += .90f;
                    Projectile.rotation += ToRadians(1f) * -Projectile.spriteDirection;
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.35f);
                }
                else
                {
                    Projectile.rotation += ToRadians(5f) * -Projectile.spriteDirection;
                    Projectile.velocity *= .67f;
                    Projectile.velocity.Y += .30f;
                }

                Helper.UpdateAniState(2, 10);
            }
            else
            {
                Projectile.velocity.Y += 2.7f;
                Projectile.Kill();
            }
        }

        public void PlayRotationParticle()
        {
            Vector2 rnd = RandDirTwoPi * Projectile.scale;
            Vector2 vRnd = -rnd * .5f + rnd.RotatedBy(PiOver2) * 2.5f;
            for (int i = -1; i <= 1; i += 2)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + rnd * 20f * i, DustID.DesertTorch, vRnd * i);
                dust.noGravity = true;
                dust.scale = 1.1f;
            }
        }

        private void PlayParticle()
        {
            for (int i = 0; i < 36; i++)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(8f) - Projectile.SafeDirByRot() * 10f, Projectile.SafeDirByRot().ToRandVelocity(ToRadians(20f), 0.8f, 14f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false, 0.2f).Spawn();
            }
            for (int i = 0; i < 20; i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(10f) - Projectile.SafeDirByRot() * 10f, Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(20f), 0.7f, 18f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriority();
            }
            for (int i = 0; i < 16; i++)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDirByRot() * 20f, -Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5f), 0.8f, 8f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false, 0.2f).Spawn();
            }
            for (int i = 0; i < 20; i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDirByRot() * 20f, -Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(5f), 0.7f, 8f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriority();
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 rotationPoint = tex.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Owner.gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (!Helper.IsDone[1] && Helper.IsDone[0])
            {
                for (int i = 0; i < 8; i++)
                    SB.Draw(tex, drawPos + ToRadians(i * 360f / 8f).ToRotationVector2() * 2, null, Color.White.ToAddColor(), drawRot, rotationPoint, Projectile.scale * 1f, flipSprite, default);
            }
            SB.Draw(tex, drawPos, null, Color.Lerp(Color.Transparent, Color.White, Projectile.Opacity), drawRot, rotationPoint, Projectile.scale * 1f, flipSprite, default);
            if (!Helper.IsDone[0] && Helper.GetAniProgress(0) > 0)
            {
                Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
                Vector2 ori = ring.ToOrigin();
                Rectangle cutSource = ring.Bounds;
                //切边。
                cutSource.Height /= 2;
                //重新设定原点
                ori = new Vector2(cutSource.Width / 2, cutSource.Height);
                SB.EnterShaderArea();
                SB.Draw(ring, drawPos, cutSource, Color.DarkRed, drawRot, ori, Projectile.scale * .09f, flipSprite, 0);
                SB.Draw(ring, drawPos, cutSource, Color.OrangeRed, drawRot, ori, Projectile.scale * .09f, flipSprite, 0);
                SB.Draw(ring, drawPos, null, Color.OrangeRed * .85f, drawRot + Pi + PiOver2 + PiOver4, ring.ToOrigin(), Projectile.scale * .09f, flipSprite, 0);
                SB.EndShaderArea();
            }
            return false;
        }
    }
}
