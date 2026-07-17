using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{

    public class PestilenceFlowerHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<PestilenceFlower>();
        public override string Texture => GetInstance<PestilenceFlower>().Texture;
        public AnimationStruct Helper = new(2);
        public ref float Timer => ref Projectile.ai[0];
        public ref float AnimationTimer => ref Projectile.localAI[0];
        public ref float AnimationState => ref Projectile.localAI[1];

        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
        }
        public override void OnFirstFrame()
        {
            PrevProgress1 = -1;
            Helper.MaxProgress[0] = (int)(AttackSpeed * .75f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .35f);
        }
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override void ProjAI()
        {
            Projectile.timeLeft = 2;
            if (Owner.HeldItem.type != OriginalItemID)
            {
                Helper.IsDone[0] = false;
                Helper.IsDone[1] = false;
                Helper.Progress[0] -= 3;
                if (Helper.GetAniProgress(0) < 0.1f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                HoldAttacks();
            }
            HoldIdleState();
            if (!IsUsing)
                return;

            Owner.ChangeDir(Projectile.direction);
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.ControlPlayerArm(Projectile.rotation, 2);
        }

        private void HoldIdleState()
        {
            Vector2 targetMountedPosition = Owner.GetToMouseVector2(Projectile.Center) * 150f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetMountedPosition.ToSafeNormalize(), .05f);
            float tarRot = Projectile.velocity.ToRotation();
            float beginRot = Projectile.rotation;
            float value = WrapAngle(tarRot - beginRot);
            Projectile.rotation = beginRot + value;
            bool reverse = (!Helper.IsDone[0] && Owner.HeldItem.type != OriginalItemID) || Owner.controlUseTile;
            Vector2 tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * 60;
            if (reverse)
                tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * 0;
            Projectile.Center = Vector2.Lerp(Projectile.Center, tarPos, .05f);
            Projectile.position.Y += (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f);
            Projectile.spriteDirection = Projectile.direction = ((Owner.LocalMouseWorld().X - Projectile.Center.X) > 0).ToDirectionInt();
        }
        public float PrevProgress1 = 0;
        private void HoldAttacks()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
            }
            else
            {
                if (IsUsing)
                {
                    PrevProgress1 = Helper.Progress[1];
                    if (Helper.OnAnimationBegin(1))
                        SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { Pitch = 0.71f }, Projectile.Center);
                    if (!Helper.IsDone[1])
                        Helper.UpdateAniState(1);
                    else
                        Timer++;
                }
                else
                {
                    if (PrevProgress1 == Helper.Progress[1])
                        SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { Pitch = 0.9f }, Projectile.Center);

                    Helper.Progress[1]--;
                    Helper.IsDone[1] = false;
                    if (Helper.Progress[1] <= 0)
                        Helper.Progress[1] = 0;
                }
                if (Timer > AttackSpeed / 5 && Helper.IsDone[1])
                {
                    SoundEngine.PlaySound(HJScarletSounds.HymnFireball_Release with { MaxInstances = 0, Pitch = -.9f, PitchVariance = .1f, Volume = .55f }, Projectile.Center);
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 pos = Projectile.Center.ToRandCirclePosEdge(4);
                        Vector2 vel = Projectile.SafeDir().RotateRandom(ToRadians(15f)).ToSafeNormalize() * Main.rand.NextFloat(0.9f, 1.1f) * 7f;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, vel, ProjectileType<PestilenceFlowerHunger>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        Timer = 0;
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 dVel = Projectile.SafeDir().ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 18f);
                        new SmokeParticle(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, dVel, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).Spawn();
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 dVel = Projectile.SafeDir().ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(4f, 18f);
                        new ShinyOrbHard(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, dVel, RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, Main.rand.NextFloat(0.4f, 0.8f)).Spawn();
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        ECSParticle.SnowCloud(Projectile.Center.ToRandCirclePos(6f) + Projectile.SafeDir() * 0f, RandVelTwoPi(0f, 6f), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, RandRotTwoPi, 1f, 0.07f * Main.rand.NextFloat(0.50f, 1.1f), BlendState.Additive);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(0, 4), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, 1, .2f * Main.rand.NextFloat(.5f, 1.1f) * .15f, 0.4f, BlendState.Additive);
                    }
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float progress1 = EaseOutBack(Helper.GetAniProgress(0));
            float progress2 = EaseOutBack(Helper.GetAniProgress(1));
            float globalTimeProgress = Lerp(0.95f, 1.05f, (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f)));
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation - ToRadians(30) + (Projectile.spriteDirection == -1 ? -(PiOver2 + ToRadians(30)) : 0);
            Vector2 origin = tex.Size() / 2;
            Vector2 realDrawPos = drawPos;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, realDrawPos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f * progress2, null, Color.LimeGreen.ToAddColor() * progress2, rotation, origin, Projectile.scale * progress2, se, 0);
            SB.Draw(tex, realDrawPos, null, Color.Lerp(Color.Transparent, Color.White, progress1), rotation, origin, Projectile.scale * progress1, se, 0);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            float ringScale = Projectile.scale * 0.17f * globalTimeProgress * progress2;
            Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
            float ringRot = Projectile.spriteDirection == -1 ? Projectile.rotation + PiOver4 + ToRadians(80) : Projectile.rotation + PiOver4 + ToRadians(15) - PiOver2;
            SB.Draw(ring, realDrawPos, null, Color.DarkGreen * .25f * progress2, ringRot, ring.ToOrigin(), ringScale, 0, 0);
            SB.Draw(ring, realDrawPos, null, Color.DarkGreen * .25f * progress2, ringRot + PiOver2, ring.ToOrigin(), ringScale, 0, 0);
            SB.Draw(ring, realDrawPos, null, Color.Green * .25f * progress2, ringRot + Pi, ring.ToOrigin(), ringScale, 0, 0);
            SB.Draw(ring, realDrawPos, null, Color.Green * .25f * progress2, ringRot - PiOver2, ring.ToOrigin(), ringScale, 0, 0);
            SB.EnterShaderArea();
            SB.Draw(ring, realDrawPos, null, Color.LimeGreen * .75f * progress2, ringRot, ring.ToOrigin(), ringScale, 0, 0);
            ring = HJScarletTexture.Particle_CrossGlow.Value;
            SB.Draw(ring, realDrawPos, null, Color.DarkGreen * .5f * progress2, Projectile.rotation, ring.ToOrigin(), ringScale * 2f, 0, 0);
            SB.Draw(ring, realDrawPos, null, Color.White * .5f * progress2, Projectile.rotation, ring.ToOrigin(), ringScale * 1.8f, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
