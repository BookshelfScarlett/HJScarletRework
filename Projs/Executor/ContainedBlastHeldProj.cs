using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class ContainedBlastHeldProj : ExecutorHeldProj
    {
        public override string Texture => GetInstance<ContainedBlast>().Texture;
        public override int MinAttackRates => 5;
        public override int OriginalItemID => ItemType<ContainedBlast>();
        public ref float Timer => ref Projectile.ai[0];
        public ref float OnFireTimer => ref Projectile.localAI[0];
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
            Projectile.SetupImmnuity(-1);
            Projectile.width = Projectile.height = 40;

        }
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (HandleDeadOrAlive())
            {
                Projectile.Kill();
                return;
            }
            HandleOwnerState();
            HandleAttack();
            HandleMiscLerp();
            //处理白烟。
            float fireProgess = Utils.GetLerpValue(0, 1, OnFireTimer / (60f * Projectile.MaxUpdates), true);
            if (fireProgess > Main.rand.NextFloat())
            {
                Vector2 safedir = Projectile.SafeDirByRot();
                Vector2 shootPos = Projectile.Center + safedir * 75f + (safedir.RotatedBy(PiOver2) * 2f * Projectile.direction);
                Vector2 dir = (shootPos - Owner.Center).ToSafeNormalize();
                Vector2 vel = (-dir).RotatedBy(ToRadians(90f) * Owner.direction).ToRandVelocity(ToRadians(9f), 1.2f, 11.6f);
                ECSParticle.SmokeParticle(shootPos, vel, RandLerpColor(Color.White, Color.Lerp(Color.OrangeRed, Color.WhiteSmoke, 0.84f)), Main.rand.Next(40, 60), RandRotTwoPi, .41f, 0.24f * Main.rand.NextFloat(.75f, 1.2f), Main.rand.NextBool(), BlendState.Additive);
            }
        }

        public void HandleMiscLerp()
        {
            if (IsUsing)
            {
                Projectile.position += Main.rand.NextVector2Circular(1, 1);
                Timer++;
                OnFireTimer++;
                if (OnFireTimer > 60 * Projectile.MaxUpdates)
                    OnFireTimer = 60 * Projectile.MaxUpdates;
            }
            else
            {
                OnFireTimer--;
                if (OnFireTimer < 0)
                    OnFireTimer = 0;
                Timer--;
                if (Timer < 0)
                    Timer = 0;
            }
        }

        public bool IsAlterBullet = false;
        public void ShootBullet(Vector2 pos, Vector2 vel, int type, int damage)
        {
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, vel, type, damage, Projectile.knockBack, Projectile.owner);
            proj.originalDamage = Projectile.originalDamage;
            proj.HJScarlet().HasExecutionMechanic = Owner.HJScarlet().containedBlastBuffTime > 0 ? Main.rand.NextBool(4) : true;
            proj.ai[0] = Owner.HJScarlet().containedBlastBuffTime;
        }
        public void HandleAttack()
        {
            Vector2 offset2 = new(30 * Owner.direction, -5);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 firePos = Projectile.Center + offset2.RotatedBy(drawRot) + Projectile.SafeDirByRot() * 8f;
            HandleExecution();
            int attackSpeedHand = Owner.HJScarlet().containedBlastBuffTime > 0 ? ((AttackSpeed / 2)) <= 5 ? 5 : AttackSpeed / 2 : AttackSpeed;
            if (Timer > attackSpeedHand && IsUsing && Timer != 0 && Projectile.IsMe())
            {
                Timer = 0;
                ScreenShakeSystem.AddScreenShakes(firePos, 1f, 10, RandRotTwoPi, 6.2f);
                SlotId slotId1 = SoundEngine.PlaySound(HJScarletSounds.Misc_Boom with { Variants = [1], MaxInstances = 0, Pitch = 0.75f, Volume = .35f }, Projectile.Center);
                if (SoundEngine.TryGetActiveSound(slotId1, out ActiveSound sound) && Owner.HJScarlet().containedBlastBuffTime > 0)
                {
                    sound.Volume /= 2;
                }
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                float rnd = Owner.HJScarlet().containedBlastBuffTime > 0 ? Main.rand.NextFloat(.95f, 1.2f) * 1f : 1f;
                int bulletDamage = (int)(Projectile.originalDamage * rnd * Clamp((1 + 0.15f * Owner.HJScarlet().containedBlastBoomCount), 1f, 1.75f));
                if (IsAlterBullet)
                {
                    Vector2 bulletPos = firePos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-5f, 5f);
                    Vector2 bulletVel = dir * Main.rand.Next(14, 18);
                    ShootBullet(bulletPos, bulletVel, ProjectileType<ContainedBlastStickBullet>(), bulletDamage);
                }
                else
                {
                    Vector2 bulletPos = firePos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-5f, 5f);
                    Vector2 bulletVel = dir * Main.rand.Next(14, 18);
                    ShootBullet(bulletPos, bulletVel, ProjectileType<ContainedBlastShockBullet>(), bulletDamage);
                }
                IsAlterBullet = !IsAlterBullet;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 posOffset = Projectile.SafeDirByRot().RotatedBy(PiOver2) * Main.rand.NextFloat(-8f, 8f) + Projectile.SafeDirByRot() * 50f;
                    Vector2 vel = Projectile.SafeDirByRot() * Main.rand.NextFloat(1f, 23f);
                    ECSParticle.LightntingGlow(firePos + posOffset, vel, RandLerpColor(Color.White, Color.WhiteSmoke), 45, 1, 0.35f * Main.rand.NextFloat(0.75f, 1.1f));
                }
                for (int i = 0; i < 14; i++)
                {
                    Vector2 vel = dir.ToRandVelocity(ToRadians(10f), 1.8f, 34.8f);
                    Vector2 offset = dir.ToRandVelocity(ToRadians(0), 7, 11f);
                    Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir * 14f;
                    new SmokeParticle(firePos.ToRandCirclePos(10f) + posOffset, vel, RandLerpColor(Color.White, Color.Lerp(Color.OrangeRed, Color.WhiteSmoke, 0.84f)), Main.rand.Next(40, 60), RandRotTwoPi, .61f, 0.24f * Main.rand.NextFloat(.75f, 1.2f), Main.rand.NextBool()).SpawnToPriority();
                }
                Projectile.HJScarlet().ExecutionStrike = false;
            }
        }
        public override void OnExecution()
        {
            int count = 0;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ProjectileType<ContainedBlastStickBullet>())
                    continue;
                if (proj.owner != Owner.whoAmI)
                    continue;
                count++;
                Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), proj.Center, Vector2.Zero, ProjectileType<ContainedBlastBoom>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj2.ai[0] = 1;
                proj2.rotation = proj.rotation + PiOver4;
                proj2.HJScarlet().ExecutionStrike = true;
                proj.Kill();
            }
            Owner.HJScarlet().containedBlastBuffTime = GetSeconds(4) * count;
            Owner.HJScarlet().containedBlastBoomCount = count;
        }
        public bool HandleDeadOrAlive()
        {
            if (Owner.HeldItem.type != OriginalItemID)
            {
                return true;
            }
            Projectile.timeLeft = 2;
            return false;
        }

        public void HandleOwnerState()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ControlPlayerArm(Projectile.rotation);
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float drawRot, out SpriteEffects se);
            Vector2 offset = new(20 * Owner.direction, -0);
            drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            float fireProgess = Utils.GetLerpValue(0, 1, OnFireTimer / (60f * Projectile.MaxUpdates), true);
            float edgeProgress = EaseOutCubic(fireProgess);
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, drawPos + offset.BetterRotatedBy(drawRot) + Projectile.SafeDirByRot() * 0.28f + (TwoPi / 16f * i).ToRotationVector2() * 1.2f * edgeProgress, null, Color.White.ToAddColor(50) * edgeProgress, drawRot, rotPoint, Projectile.scale * .70f, se, 0);
            SB.Draw(tex, drawPos + offset.BetterRotatedBy(drawRot), null, Color.White, drawRot, rotPoint, Projectile.scale * .70f, se, 0);
            return false;
        }
    }
}
