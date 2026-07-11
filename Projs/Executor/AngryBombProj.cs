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
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class AngryBombProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public AnimationStruct Helper = new(3);
        public NPC Target = null;
        public ref float BombRotation => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.noEnchantmentVisuals = true;
            Projectile.noEnchantments = true;

        }
        public override void OnFirstFrame()
        {
            BombRotation = RandRotTwoPi;
            Helper.MaxProgress[0] = 15 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 16 * Projectile.MaxUpdates;
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                Helper.MaxProgress[0] = 8 * Projectile.MaxUpdates;
                Helper.MaxProgress[1] = 7 * Projectile.MaxUpdates;
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 0, Pitch = .42f, PitchVariance = 0.05f }, Projectile.Center);
            if (!Projectile.IsMe())
                return;
            for (int i = 0; i < 24; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(4), Projectile.velocity.ToRandVelocity(ToRadians(15), 0.3f, 11f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.9f, 1.1f) * 0.12f, true, BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(4), Projectile.velocity.ToRandVelocity(ToRadians(15), 0.3f, 8f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, 1f, Main.rand.NextFloat(0.9f, 1.1f) * 0.42f, 0.2f);
            }

        }

        public override void ProjAI()
        {
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation();
            UnstableExplosion();
            if (Projectile.HJScarlet().ExecutionStrike)
                Projectile.position += Main.rand.NextVector2Circular(4f, 4f);
            //掷出时的减速
            if (!Helper.IsDone[0])
            {
                float progress = Helper.GetAniProgress(0);
                UpdateSlowdownAI(progress);
                PlayParticle(progress);
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                Timer++;
                UpdateChargingAI();
                Helper.UpdateAniState(1);
            }
            else
            {
                BombRotation = Lerp(BombRotation, 0, 0.02f);
                ChasingParticle();
                if (Target.IsLegal())
                {
                    if (Timer > 0)
                    {
                        ChasingInit();
                    }
                    Projectile.HomingTarget(Target.Center, -1, 8, 10);

                }
                else
                {
                    Projectile.Kill();
                }
            }
        }
        public void ChasingInit()
        {
            Projectile.velocity = Projectile.Center.GetNormalVector2(Target.Center) * (6.7f + Projectile.HJScarlet().ExecutionStrike.ToInt() * 1.1f);
            Timer = -1;
            SoundEngine.PlaySound(HJScarletSounds.Blunt_Swing with { MaxInstances = 0, Pitch = 0.5f }, Projectile.Center);
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 34; i++)
                {
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(16), (-Projectile.velocity).ToRandVelocity(ToRadians(5), -6.3f, 3.8f), RandLerpColor(Color.Orange, Color.OrangeRed), 25, RandRotTwoPi, 1f, Main.rand.NextFloat(0.9f, 1.1f) * 0.13f, true, BlendState.NonPremultiplied);
                }
                for (int i = 0; i < 24; i++)
                {
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(16), (-Projectile.velocity).ToRandVelocity(ToRadians(5), -6.3f, 3.8f), RandLerpColor(Color.OrangeRed, Color.Orange), 25, 1f, Main.rand.NextFloat(0.9f, 1.1f) * 0.47f, 0.2f);
                }

                SoundEngine.PlaySound(HJScarletSounds.Misc_AirFlowAlt with { MaxInstances = 0, Pitch = -0.5f, Variants = [2] }, Projectile.Center);
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(16), (-Projectile.velocity).ToRandVelocity(ToRadians(5), -4.3f, 3.8f), RandLerpColor(Color.OrangeRed, Color.DarkOrange), 25, RandRotTwoPi, 1f, Main.rand.NextFloat(0.9f, 1.1f) * 0.12f, true, BlendState.NonPremultiplied);
                }
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(16), (-Projectile.velocity).ToRandVelocity(ToRadians(5), -4.3f, 3.8f), RandLerpColor(Color.OrangeRed, Color.Orange), 25, 1f, Main.rand.NextFloat(0.9f, 1.1f) * 0.42f, 0.2f);
                }
            }
        }
        public void ChasingParticle()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4) + (Projectile.rotation + BombRotation).ToRotationVector2().RotatedBy(-PiOver2) * 12f * Projectile.scale, DustID.Torch);
                d.velocity = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(7f), 0.8f, 1.4f);
                d.scale = Main.rand.NextFloat(0.9f, 1.1f);
                d.noGravity = true;
            }

        }

        public void PlayParticle(float progress)
        {
            if (Projectile.IsOutScreen())
                return;
            Dust d = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation + BombRotation).ToRotationVector2().RotatedBy(-PiOver2) * 12f * Projectile.scale, DustID.Torch);
            d.velocity = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5f), 0.8f, 1.2f);
            d.scale = Main.rand.NextFloat(0.9f, 1.1f);
            d.noGravity = true;
        }

        public void UpdateChargingAI()
        {
            Projectile.velocity *= (0.905f - Projectile.HJScarlet().ExecutionStrike.ToInt() * 0.01f);
            if (Projectile.numUpdates % 2 == 0 && !Projectile.HJScarlet().ExecutionStrike)
                Projectile.position += Main.rand.NextVector2Circular(2, 2);
            float progress = EaseOutBack(Helper.GetAniProgress(1));
            BombRotation += Lerp(0.15f, 0.01f, progress);
            ChargingParticle();
            PosLerp = Helper.GetAniProgress(1);
            if (Projectile.GetTargetSafe(out NPC target, true, 3600))
                Target = target;

        }
        public void ChargingParticle()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4) + (Projectile.rotation + BombRotation).ToRotationVector2().RotatedBy(-PiOver2) * 12f * Projectile.scale, DustID.Torch);
                d.velocity = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(7f), 0.8f, 1.4f);
                d.scale = Main.rand.NextFloat(0.9f, 1.1f);
                d.noGravity = true;
            }
        }
        public void UnstableExplosion()
        {
            if (Projectile.FinalUpdateNextBool(70) && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.AddExecutionTimeImmediate(ItemType<AngryBomb>());
                Projectile.Kill();
            }
        }

        public void UpdateSlowdownAI(float progress)
        {
            Projectile.velocity *= (0.905f - Projectile.HJScarlet().ExecutionStrike.ToInt() * 0.01f);
            BombRotation += Lerp(0.3f, 0.15f, progress);
            Projectile.scale = Lerp(1f, 1.25f, EaseOutBack(progress));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<AngryBomb>());
            if (Projectile.ai[2] == 1 && Projectile.HJScarlet().ExecutionStrike)
            {
                Vector2 dir = Projectile.Center.GetNormalVector2(Owner.MountedCenter).RotatedByRandom(Pi);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 18, ProjectileType<AngryBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                proj.extraUpdates = 2;
                proj.HJScarlet().ExecutionStrike = true;

            }
        }
        public override bool? CanDamage()
        {
            return (Helper.IsDone[0] || !Projectile.HJScarlet().ExecutionStrike);
        }
        public override bool PreKill(int timeLeft)
        {
            for (int j = 0; j < 22; j++)
            {
                Vector2 dir = RandDirTwoPi;
                Vector2 pos = Projectile.Center.ToRandCirclePos(7f) + dir * Main.rand.NextFloat(0f, 3f);
                ECSParticle.ShinyCrossStarECS(pos, dir.ToRandVelocity(ToRadians(20f), 0f, 3.9f), RandLerpColor(Color.OrangeRed, Color.Orange), Main.rand.Next(15, 50), 1f, 0.75f * Main.rand.NextFloat(.7f, .9f), .2f);

            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(7f);
                Vector2 vel = RandVelTwoPi(.1f, 7.9f);
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .75f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .105f * Main.rand.NextFloat(.6f, 1f), .32f);
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f) + RandVelTwoPi(.1f, 2.9f);
                Vector2 vel = (-Projectile.SafeDir()).ToRandVelocity(ToRadians(10f), 1.2f, 7.5f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, vel);
                d.noGravity = true;
                d.scale = 1.2f * Main.rand.NextFloat(.9f, 1.1f);
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 2.9f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, vel);
                d.scale = 1.2f * Main.rand.NextFloat(.9f, 1.1f);
                d.noGravity = true;
            }
            for (int i = 0; i < 40; i++)
            {
                Vector2 vel = RandVelTwoPi(0.1f, 6.7f);

                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(16f) + vel.ToSafeNormalize() * Main.rand.NextFloat() * 2f;
                Color color = RandLerpColor(Color.Lerp(Color.DarkOrange, Color.Black, 0.60f), Color.DarkGray);
                float scale = 0.244f * Main.rand.NextFloat(0.55f, 1.1f);
                new SmokeParticle(spawnpos, vel, color, Main.rand.Next(10, 41), RandRotTwoPi, Main.rand.NextFloat(.75f, 1f) * 0.95f, scale, true).SpawnToPriorityNonPreMult();
            }
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 18; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(16);
                    ECSParticle.StarShape(pos, Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(0.3f, 1f) * 7f, RandLerpColor(Color.Orange, Color.OrangeRed), Main.rand.Next(0, 55), 1, 0.8f * Main.rand.NextFloat(.7f, 1.1f), .89f, BlendState.Additive);
                }

                SoundEngine.PlaySound(HJScarletSounds.Frostwave_Boom with { MaxInstances = 0, Pitch = 0.872f, Volume = 0.85f }, Projectile.Center);
            }
            else
                SoundEngine.PlaySound(HJScarletSounds.Misc_GunHit with { MaxInstances = 0, Pitch = -0.172f + Main.rand.NextFloat(-0.1f, 0.1f), Volume = 0.65f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { MaxInstances = 0, Pitch = 0.82f, Volume = 1 }, Projectile.Center);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 4f, 6, RandRotTwoPi);
            return true;
        }
        public float PosLerp = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            int angryFrame = (Helper.IsDone[0] || Projectile.HJScarlet().ExecutionStrike).ToInt();
            Rectangle frame = tex.Frame(1, 2, 0, angryFrame);
            Vector2 ori = frame.Size() / 2;
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //绘制残影
            float oriScale = 1f;
            float scale = 1f;
            int length = 4;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.White, Color.Red, (1 - rads)).ToAddColor(100) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(tex, Projectile.oldPos[i] + Projectile.PosToCenter(), frame, edgeColor, rot + BombRotation, ori, oriScale * scale * Projectile.scale, 0, 0);
                scale *= 0.995f;
            }
            if (Helper.IsDone[0])
            {
                for (int i = 0; i < 16; i++)
                {
                    SB.Draw(tex, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(4 * PosLerp, 4 * PosLerp) + (TwoPi / 16 * i).ToRotationVector2() * 2 * PosLerp, frame, Color.Red.ToAddColor(), Projectile.rotation + BombRotation, ori, Projectile.scale, se, 0);
                }
            }
            Color color = Projectile.HJScarlet().ExecutionStrike ? Color.Red : Color.White;
            SB.Draw(tex, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation + BombRotation, ori, Projectile.scale, se, 0);
            return false;
        }
    }
}
