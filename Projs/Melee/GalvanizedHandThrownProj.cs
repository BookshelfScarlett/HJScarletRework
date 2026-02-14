using ContinentOfJourney.Items.Placables.Furniture.Life;
using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class GalvanizedHandThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<GalvanizedHand>().Texture;
        public enum Style
        {
            Shoot,
            Stab
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public float AttackTime = 0;
        public Vector2 StoredPosition = Vector2.Zero;
        public float OriginalSpeed = -1f;
        public bool ShouldPull = false;
        public bool BeginPull = false;
        public float DeadTime = 120f;
        /// <summary>
        /// 手持射弹完全接管其行为
        /// </summary>
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(28, 2);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return AttackType == Style.Shoot;
        }
        public override void AI()
        {
            if(!Projectile.HJScarlet().FirstFrame)
                OriginalSpeed = Projectile.velocity.Length();
            ActiveAI();
            //玩家死亡时候立刻击杀射弹
            if (Owner.dead || !Owner.active || Projectile.TooAwayFromOwner())
                Projectile.Kill();
        }
        public void ActiveAI()
        {
            if (AttackType == Style.Shoot)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (!HJScarletMethods.OutOffScreen(Projectile.Center))
                    DrawParticles();
            }
            else
            {
                Timer++;
                if (Timer > DeadTime)
                    Timer = DeadTime;
                //这里stab成功后，会手动控制玩家的指向
                if (Projectile.GetTargetSafe(out NPC target, false, canPassWall: true))
                {
                    StabAI(target);
                }
                else
                {
                    //没有合适的目标立刻处死，我草忘了
                    Projectile.Kill();
                }
            }
        }
        private void StabAI(NPC target)
        {
            Projectile.Center = target.Center + StoredPosition;
            Vector2 dir = (target.Center - Owner.MountedCenter).ToSafeNormalize();
            //立刻锁住玩家的手臂
            Owner.ChangeDir(((Projectile.Center.X - Owner.MountedCenter.X) > 0).ToDirectionInt());
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, dir.ToRotation() - PiOver2);

            DrawMountedParticle();
            if (Owner.JustPressRightClick() && ShouldPull == false && Owner.HeldItem.type == ItemType<GalvanizedHandThrown>())
            {
                SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Charge, Owner.Center);
                ShouldPull = true;
            }
            if (ShouldPull)
            {
                Owner.maxFallSpeed = 180;
                Owner.velocity = dir * 80;
                if(!BeginPull)
                {
                    BeginPull = true;
                    ScreenShakeSystem.AddScreenShakes(Owner.Center, 80 * Owner.direction, 23, Owner.velocity.ToRotation(), 0.2f, true, 1000);
                }
                //发起冲刺时在玩家身后绘制粒子作为残影。
                //这里本质素材复用，但是放过我吧我不想再做更多的效果了
                SpawnOwnerParticle();
                //改为了判距离
                //不需要过于精确，只要大概就行了
                if ((Owner.Center - target.Center).Length() < 50f)
                {
                    //震屏。
                    Owner.maxFallSpeed = 0;
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, -80 * Owner.direction, 23, Owner.velocity.ToRotation(), 0.2f, true, 1000);
                    for (int i = 0; i < 8; i++)
                    {
                        float rotArgs = ToRadians(360f / 8 * i);
                        Vector2 arrowDir = Owner.velocity.ToSafeNormalize().RotatedBy(rotArgs);
                        Projectile arrow = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, arrowDir * 12f, ProjectileType<GalvanizedHandArrow>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        arrow.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    }
                    SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit, Owner.Center);
                    Projectile.Kill();
                    //三倍伤害，单次的判定
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<LightBiteArrow>(), Projectile.damage * 3, 0f, Owner.whoAmI);
                    Owner.HJScarlet().galvanizedHandProjHanging = false;
                    Owner.velocity = Projectile.rotation.ToRotationVector2() * -OriginalSpeed * 0.8f;

                }
            }

        }

        private void SpawnOwnerParticle()
        {
            Vector2 dir = Owner.velocity.ToSafeNormalize();
            //本质素材复用
            //但是我不想做更多特效了 
            //饶了我吧。
            for (int i = -1; i < 2; i += 1)
            {
                Vector2 shapePos = Owner.Center + dir.RotatedBy(PiOver2) * 15f * i + dir * 10f - dir * Math.Abs(i) * 15f;
                Vector2 shapeVel = dir * -Main.rand.NextFloat(16f);
                float shapeScale = Main.rand.NextFloat(0.65f, 0.75f);
                new StarShape(shapePos.ToRandCirclePosEdge(6.2f) - dir *Main.rand.NextFloat(12f), shapeVel, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), shapeScale, 40).Spawn();
                new ShinyOrbParticle(shapePos.ToRandCirclePos(3.1f), shapeVel * 0.5f, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), 40,shapeScale * 1.1f).Spawn();

            }
        }

        private void DrawMountedParticle()
        {
            Vector2 dir = Projectile.SafeDirByRot();
            if (Main.rand.NextBool())
                return;

            for (int i = -1; i < 2; i += 1)
            {
                Vector2 shapePos = Projectile.Center + dir.RotatedBy(PiOver2) * 15f * i + dir * 70f - dir * Math.Abs(i) * 15f;
                Vector2 shapeVel = dir * -Main.rand.NextFloat(16f * (Timer / DeadTime));
                float shapeScale = (Timer / DeadTime) * Main.rand.NextFloat(0.65f, 0.75f);
                new StarShape(shapePos.ToRandCirclePosEdge(6.2f) - dir *Main.rand.NextFloat(12f), shapeVel, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), shapeScale, 40).Spawn();
                new ShinyOrbParticle(shapePos.ToRandCirclePos(3.1f), shapeVel * 0.5f, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), 40,shapeScale * 1.1f).Spawn();
            }
        }

        public void DrawParticles()
        {
            for (int i = -1; i < 2; i += 1)
            {
                Vector2 spawnDustPos = Projectile.Center + Projectile.SafeDirByRot().RotatedBy(PiOver2) * 15f * i;
                Vector2 vel = Projectile.velocity / 3;
                if (Main.rand.NextBool())
                {
                    Vector2 shapePos = spawnDustPos.ToRandCirclePosEdge(3.2f);
                    float shapeScale = Main.rand.NextFloat(0.60f, 0.75f);
                    new StarShape(shapePos, vel * 1.2f, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), shapeScale, 30).Spawn();
                }
                if (Main.rand.NextBool(3))
                {
                    float speed = Main.rand.NextFloat(0.8f, 1.1f);
                    float randDir = RandRotTwoPi;
                    int seedVa = Main.rand.Next(0, 10000);
                    float scale = 0.6f;
                    Vector2 pos = spawnDustPos.ToRandCirclePos(1.2f);
                    new TurbulenceShinyOrb(pos, speed, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), 80, scale, randDir, seedValue: seedVa).Spawn();
                    new TurbulenceShinyOrb(pos, speed, Color.White, 80, scale * 0.5f, randDir, seedValue: seedVa).Spawn();
                }
            }

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType == Style.Shoot)
            {
                StoredPosition = Projectile.Center - target.Center;
                AttackType = Style.Stab;
                Projectile.netUpdate = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.extraUpdates = 0;
                Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
                SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { MaxInstances = 1}, Projectile.Center);
                HitParticle(target.Center);
            }
        }

        private void HitParticle(Vector2 center)
        {
            Vector2 dir = Projectile.SafeDirByRot();
            for (int i = -1; i < 2; i += 1)
            {
                //生成主方向上的粒子
                for (int j = 0; j < 35; j++)
                {
                    Vector2 spawnDustPos = Projectile.Center + dir.RotatedBy(PiOver2) * 15f * i + dir * 20f;
                    Vector2 vel = dir * Main.rand.NextFloat(4f, 14f);
                    new StarShape(spawnDustPos.ToRandCirclePosEdge(3.2f), vel, RandLerpColor(Color.RoyalBlue, Color.CornflowerBlue), Main.rand.NextFloat(0.65f, 0.75f), 40).Spawn();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fix = PiOver4 + PiOver2;
            float alphaRatio = Clamp((1 - (Timer / 30f)), 0, 1);
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            DrawTrail(Color.RoyalBlue, 15f, alphaRatio);
            DrawTrail(Color.DeepSkyBlue, 12f, alphaRatio);
            DrawTrail(Color.White, 8f, alphaRatio);
            SB.EnterShaderArea();
            DrawBack(Color.DeepSkyBlue, 15f, 0.85f *alphaRatio);
            SB.EndShaderArea();
            Projectile.DrawGlowEdge(Color.White * (Timer / 120f), rotFix:fix);
            Projectile.DrawProj(Color.White, 2,.4f, rotFix: fix);
            return false;
        }
        public void DrawBack(Color trailColor, float primitiveHeight, float alphaValue = 1f)
        {
            Asset<Texture2D> tex = HJScarletTexture.ColorMap_Aqua.Texture;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(tex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(10, tex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(1.3f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = tex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            List<ScarletVertex> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < totalpoints - 1; i++)
            {
                if (validPosition[i+1] - validPosition[i] == Vector2.Zero)
                    continue;
                float progress = (float)i / (totalpoints - 1);
                float rot = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 posOffset = new Vector2(0, primitiveHeight * 0.9f).RotatedBy(rot);
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition - rot.ToRotationVector2() * 40f;
                QuickGetClass(ref list, oldCenter, posOffset, progress);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }

        }
        public void DrawTrail(Color trailColor, float primitiveHeight, float alphaValue = 1f)
        {
            Asset<Texture2D> tex = HJScarletTexture.Trail_ManaStreak.Texture;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(tex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(20, tex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -3.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(1f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);

            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = tex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            List<ScarletVertex> list = [];
            List<ScarletVertex> list2 = [];
            List<ScarletVertex> list3 = [];
            int totalpoints = validPosition.Count;
            for (int i = 0; i < totalpoints - 1; i++)
            {
                if (validPosition[i+1] - validPosition[i] == Vector2.Zero)
                    continue;
                float progress = (float)i / (totalpoints - 1);
                float rot = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 rotDir = rot.ToRotationVector2();
                Vector2 primiWidth = new Vector2(0, primitiveHeight * 0.9f).RotatedBy(rot);

                //这里的顶点实际绘制位置需要手动做一下偏移对上矛尖
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition - rotDir * 80f;
                Vector2 offset2 = rotDir.RotatedBy(PiOver2) * 15f;
                QuickGetClass(ref list, oldCenter, primiWidth, progress);
                QuickGetClass(ref list2, oldCenter + offset2, primiWidth, progress);
                QuickGetClass(ref list3, oldCenter - offset2, primiWidth, progress);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list2.ToArray(), 0, list.Count - 2);
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list3.ToArray(), 0, list.Count - 2);
            }
        }
        public void QuickGetClass(ref List<ScarletVertex> list,Vector2 oldCenter, Vector2 posOffset, float progress)
        {
            ScarletVertex upClass = new(oldCenter - posOffset, Color.White, new Vector3(progress, 0, 0f));
            ScarletVertex downClass = new(oldCenter + posOffset, Color.White, new Vector3(progress, 1, 0f));
            list.Add(upClass);
            list.Add(downClass);
        }
    }
}
