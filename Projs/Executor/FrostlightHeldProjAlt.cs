using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FrostlightHeldProjAlt : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Frostlight>();
        public override string Texture => GetInstance<FrostlightHeldProj>().Texture;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, 20 * Projectile.MaxUpdates);
        public ref float Timer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public ref float HeldAnimationHelper => ref Projectile.ai[2];
        public ref bool IsAlterModeNow => ref GetInstance<Frostlight>().AlterMode;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.SetUpHeldProj(10);
            Projectile.netImportant = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public float EdgeValue = 0;
        public float RingValue = 0;
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public bool RightClicker = false;
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            if (Owner.CheckExecution(OriginalItemID) && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Owner.RemoveExecutionProgress(OriginalItemID);
                Timer = 0;
                RightClicker = true;
                ((Frostlight)Owner.HeldItem.ModItem).AlterMode = true;
            }
            if (RightClicker)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<FrostlightFlamethrower>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((FrostlightFlamethrower)proj.ModProjectile).BeginTargetRotation = Projectile.rotation;
                Projectile.Kill();
            }
            HandleProjAttack();
            HandleParticle();
            HandlePlayerState();
            HandleProjStatement();
        }
        public override void OnKill(int timeLeft)
        {
            //处死时的粒子
            //需注意的是处决姿态下，粒子不会播报
            if (Projectile.HJScarlet().ExecutionStrike)
                return;
            Vector2 dir = Projectile.SafeDirByRot();
            for (int i = 0; i < 60; i++)
            {
                Vector2 pos = dir * Main.rand.NextFloat(-50, 50) + Projectile.Center;
                Vector2 vel = dir.ToRandVelocity(ToRadians(5f), .5f, 2.5f);
                ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), vel, RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 45), 1, Projectile.scale * Main.rand.NextFloat(.7f, .98f) * .8f, .2f);
            }
            for (int i = 0; i < 60; i++)
            {
                Vector2 pos = dir * Main.rand.NextFloat(-50, 70) + Projectile.Center;
                Vector2 vel = dir.ToRandVelocity(ToRadians(5f), .5f, 2.5f);
                ECSParticle.SnowCloud(pos.ToRandCirclePosEdge(18), vel, RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 45), 1, Projectile.scale * Main.rand.NextFloat(.7f, .98f) * .58f, .08f * Main.rand.NextFloat(0.4f, 0.8f));
            }
            for (int i = 0; i < 60; i++)
            {
                Vector2 pos = Main.MouseWorld.ToRandCirclePos(100);
                Vector2 vel = -Vector2.UnitY.ToRandVelocity(ToRadians(5f), .5f, 2.5f);
                ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), vel, RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 45), 1, Projectile.scale * Main.rand.NextFloat(.7f, .98f) * .8f, .2f);
            }
            for (int i = 0; i < 60; i++)
            {
                Vector2 pos = Main.MouseWorld.ToRandCirclePos(100);
                Vector2 vel = -Vector2.UnitY.ToRandVelocity(ToRadians(5f), .5f, 2.5f);

                ECSParticle.SnowCloud(pos.ToRandCirclePosEdge(18), vel, RandLerpColor(Color.SkyBlue, Color.LightBlue), Main.rand.Next(30, 45), 1, Projectile.scale * Main.rand.NextFloat(.7f, .98f) * .58f, .08f * Main.rand.NextFloat(0.4f, 0.8f));
            }
            SoundEngine.PlaySound(HJScarletSounds.Frostwave_LightRelease with { MaxInstances = 0, Pitch = .36f });
        }
        public void HandleProjStatement()
        {
            Timer++;
            //实际发射火球的AI
            if (Timer > (AttackSpeed / 10))
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                SoundEngine.PlaySound(HJScarletSounds.Misc_AirFlowAlt with { MaxInstances = 1, Pitch = -0.7f, PitchVariance = .12f, Volume = 0.3f });
                NPC target = HJScarletMethods.FindClosestTarget(Main.MouseWorld, 240);
                bool reverse = Main.rand.NextBool();
                dir = dir.RotatedBy(PiOver2 * reverse.ToDirectionInt()).RotatedBy(Main.rand.NextFloat(ToRadians(-10f), ToRadians(60f))*-reverse.ToDirectionInt());
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), posBase, dir*Main.rand.NextFloat(15f,18f), ProjectileType<FrostlightFrostball>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                proj.ai[1] = Main.rand.Next(50, 300);
                proj.ai[2] = Main.rand.NextFloat(4.5f, 7.5f);
                proj.HJScarlet().HasExecutionMechanic = true;
                if (target.IsLegal())
                    ((FrostlightFrostball)proj.ModProjectile).CurTarget = target;
                Timer = 0;
            }
            NPC target2 = Main.MouseWorld.FindClosestTarget(240);
            RingValue = target2.IsLegal() ? Lerp(RingValue, 1f, 0.02f) : Lerp(RingValue,0.35f,.02f);
        }

        public void HandlePlayerState()
        {
            Owner.ControlPlayerArm(Projectile.rotation);
            int dir = (Main.MouseWorld.X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemAnimation = Owner.itemTime = 2;
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            bool ifStillInUse = (Main.mouseLeft || Owner.controlUseTile) && !Owner.noItems && !Owner.CCed;
            if (ifStillInUse)
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();

        }

        public void HandleParticle()
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            if (Main.rand.NextBool(6))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                posBase += Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-30f, 30f);
                if (Main.rand.NextBool(3))
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    new Fire(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 50), RandRotTwoPi, 0.57f, Main.rand.NextFloat(.9f, 1.1f) * .25f).SpawnToPriority();
                }
                else
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    ECSParticle.SmokeParticle(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 50), RandRotTwoPi, 0.57f, Main.rand.NextFloat(.9f, 1.1f) * .45f, Main.rand.NextBool(), BlendState.Additive);
                }
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                Vector2 setPos = posBase + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-20f, 21f) + dir * Main.rand.NextFloat(-60f, 60f);
                Vector2 vel = -Vector2.UnitY.RotatedByRandom(ToRadians(15)) * Main.rand.NextFloat(4f, 12f);
                float offsetX = setPos.X - posBase.X;

                bool needMirror = (offsetX > 0 && vel.X < 0) || (offsetX < 0 && vel.X > 0);
                if (needMirror)
                    setPos.X = posBase.X - offsetX;
                ECSParticle.ShinyCrossStarECS(setPos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.8f, 1.1f) * .38f, .2f);
            }
        }

        public void HandleProjAttack()
        {
            float mirroredX = Owner.Center.X * 2 - Owner.ToClampMouseVector2().X;
            float finalX = Lerp(mirroredX, Owner.Center.X, .70f);
            Vector2 ownerToSky = new Vector2(finalX, Owner.Center.Y - 500) - Owner.MountedCenter;
            Vector2 skyDir = ownerToSky.ToSafeNormalize();
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, skyDir, HeldAnimationHelper);
            float targetRotaiton = Projectile.velocity.ToRotation();
            float currentRotation = Projectile.rotation;
            float value = WrapAngle(targetRotaiton - currentRotation);
            Projectile.rotation = currentRotation + value;
            EdgeValue = Lerp(EdgeValue, 1f, 0.01f);
            if(Projectile.FinalUpdate())
            {
                HeldAnimationHelper += .01f;
                if (HeldAnimationHelper >= .05f)
                    HeldAnimationHelper = .05f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Owner.HasProj<FrostlightFlamethrower>())
                return false;

            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + PiOver4 + (Projectile.spriteDirection == -1 ? PiOver2 : 0);
            Vector2 origin = new Vector2(Projectile.spriteDirection == -1 ? tex.Width : 0, tex.Height);
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -25f + Vector2.UnitX.RotatedBy(Projectile.rotation).RotatedBy(PiOver2) * 0;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for(int i =0;i<12;i++)
            SB.Draw(tex, realDrawPos + (TwoPi / 12f * i).ToRotationVector2() * 1.2f, null, Color.White.ToAddColor() * EdgeValue, rotation, origin, Projectile.scale, se, 0);
            SB.Draw(tex, realDrawPos, null, Color.White, rotation, origin, Projectile.scale, se, 0);
 
            SB.EnterShaderArea();

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Texture2D star = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 pos = drawPos + dir * 60f * Projectile.scale;
            float scale = Projectile.scale * .285f * Clamp( Math.Abs(Math.Sign(Main.timeForVisualEffects)),0.2f,1f) * 1.1f;
            SB.Draw(star, pos, null, Color.RoyalBlue * .9f, 0, star.ToOrigin(), scale * 0.95f, 0, 0);
            SB.Draw(star, pos, null, Color.SkyBlue* .9f, 0, star.ToOrigin(), scale * .90f, 0, 0);
            SB.Draw(star, pos, null, Color.LightBlue * .85f, 0, star.ToOrigin(), scale * 0.85f, 0, 0);
            star = HJScarletTexture.Particle_RingShiny.Value;
            SB.Draw(star, pos, null, Color.LightBlue * .35f, 0, star.ToOrigin(), scale * 0.35f, 0, 0);
            //出于我不清楚的原因，这里得重置两次批次，才能确保正常
            SB.EndShaderArea();
            SB.EndShaderArea();
            return false;
        }
    }
}
