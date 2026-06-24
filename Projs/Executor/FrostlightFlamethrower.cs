using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FrostlightFlamethrower : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Frostlight>();
        public override string Texture => GetInstance<FrostlightHeldProj>().Texture;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime *Projectile.MaxUpdates, 20 * Projectile.MaxUpdates);
        public ref float Timer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public ref float HeldAnimationHelper => ref Projectile.ai[2];
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public AnimationStruct Helper = new(3);
        public List<Vector2> OldAimPos = [];
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.SetUpHeldProj(10);
            Projectile.netImportant = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = (int)(AttackSpeed * 0.5f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * 1.2f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * 0.3f);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 0, Pitch = .82f });
            SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { MaxInstances = 0, Pitch = .82f });
            TargetRotation = BeginTargetRotation;
        }
        public override void ProjAI()
        {
            HandleHeldProjState();
            HandleAttackAnimation();
            HandlePlayerState();
            if (Owner.CheckExecution(OriginalItemID) && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Owner.RemoveExecutionProgress(OriginalItemID);
                Timer = 0;
                Owner.direction = (Main.MouseWorld.X - Owner.Center.X > 0).ToDirectionInt();
                Vector2 ownerToSky = new Vector2(Owner.Center.X + 250 * Owner.direction, Owner.Center.Y) + new Vector2(0, -500) - Owner.Center;
                Vector2 skyDir = -(ownerToSky).ToSafeNormalize();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<FrostlightHeldProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                proj.originalDamage = Projectile.damage;
                ((FrostlightHeldProj)proj.ModProjectile).BeginTargetRotation = skyDir.ToRotation();
                ((FrostlightHeldProj)proj.ModProjectile).Flip = 1;
                ((Frostlight)Owner.HeldItem.ModItem).AlterMode = false;
                Projectile.Kill();
            }
        }

        public void HandleHeldProjState()
        {
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            bool ifStillUse = (Owner.controlUseItem) && !Owner.noItems && !Owner.CCed;
            if (!ifStillUse || Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

        }
        public float Rotations = 0;

        public void HandleAttackAnimation()
        {
            //if (!Helper.IsDone[0])
            UpdateBeginAnimation();
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
            }
            else
            {
                Vector2 fireSpawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2().SafeNormalize(Vector2.UnitY) * 85;
                Vector2 fireShootVelocity = Projectile.rotation.ToRotationVector2() * Owner.HeldItem.shootSpeed * .5f;
                //DrawGlowingFireParticle(fireSpawnPosition);
                if (Projectile.MeetMaxUpdatesFrame(ShootTimer, 13) || ShootTimer == 0)
                {
                    SoundEngine.PlaySound(HJScarletSounds.HymnFireball_Release with { MaxInstances = 0, Pitch = -.35f, PitchVariance = .1f });
                    ShootTimer = 0;
                }
                ShootTimer++;
                Timer++;
                if (Projectile.MeetMaxUpdatesFrame(Timer, 2))
                {
                    Timer = 0;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), fireSpawnPosition.ToRandCirclePos(10), fireShootVelocity, ProjectileType<FrostlightFlamethrowerFlame>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.HJScarlet().HasExecutionMechanic = true;
                }
            }
        }

        public void UpdateBeginAnimation()
        {
            //末尾角度，也是下一个动画进程的起始角度
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.GetNormalVector2(Main.MouseWorld), .03f);
            float tarRot = Projectile.velocity.ToRotation();
            float beginRot = Projectile.rotation;
            float value = WrapAngle(tarRot - beginRot);
            Projectile.rotation = beginRot + value;
            //更新当前的转角
            float rot = Projectile.rotation;
            //将其投影到矩阵上，并进行形变
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, 1, 1f);
            //而后再转化为射弹的目标指向，这个tarPos同时拥有指向和武器模长的信息。而不是一个单位向量
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f;
            //这样，Scale就是tarPos的向量模长
            Projectile.scale = tarPos.Length();
            //武器的角度为（起始角度 + 目标角度）的值
            Projectile.rotation = tarPos.ToRotation();
            //最后，粒子效果
            HandleParticle();
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
            if (Main.rand.NextBool(6))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                posBase += Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-60f, 60f);
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

        public void HandlePlayerState()
        {
            if (Helper.IsDone[0])
            //Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.ControlPlayerArm(Projectile.rotation);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.HJScarlet().ExecutionStrike)
                return false;
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + PiOver4 + (Projectile.spriteDirection == -1 ? PiOver2 : 0);
            Vector2 origin = new Vector2(Projectile.spriteDirection == -1 ? tex.Width : 0, tex.Height);
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -25f + Vector2.UnitX.RotatedBy(Projectile.rotation).RotatedBy(PiOver2) * 0;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex, realDrawPos, null, Color.White, rotation, origin, Projectile.scale, se, 0);

            SB.EnterShaderArea();

            Texture2D star = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            Vector2 pos = drawPos + dir * 60f * Projectile.scale;
            float scale = Projectile.scale * .285f * easedProgress;
            SB.Draw(star, pos, null, Color.RoyalBlue * .9f * easedProgress, 0, star.ToOrigin(), scale * 0.95f, 0, 0);
            SB.Draw(star, pos, null, Color.SkyBlue * .9f * easedProgress, 0, star.ToOrigin(), scale * .90f, 0, 0);
            SB.Draw(star, pos, null, Color.LightBlue * .85f * easedProgress, 0, star.ToOrigin(), scale * 0.85f, 0, 0);


            //出于我不清楚的原因，这里得重置两次批次，才能确保正常
            SB.EndShaderArea();
            SB.EndShaderArea();
            return false;
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
    }
}
