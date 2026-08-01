using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheHeldProj : ExecutorHeldProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override int OriginalItemID => ItemType<CrimsonScythe>();
        public AnimationStruct Helper = new AnimationStruct(3);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1.25f;
        public float Width = 1.25f;
        public bool ThirdSwing = false;
        public float SwingTime = 0;
        public float StopTiming = 0;
        public List<Vector2> OldAimPos = [];
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }

        public override void ExSD()
        {
            //TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            Projectile.SetUpHeldProj(10);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

        }
        public override void OnFirstFrame()
        {
            ThirdSwing = SwingTime > 2;
            if (ThirdSwing)
            {
                Projectile.SetupImmnuity(45);
                ScarletSound(HJScarletSounds.Tlipoca_Swing, Projectile.Center, 0.75f, 1, 0.14f, 0.1f, 1);
                Helper.MaxProgress[0] = (int)(AttackSpeed * .65f);
                Helper.MaxProgress[1] = (int)(AttackSpeed * .3f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * .95f);
            }
            else
            {
                ScarletSound(HJScarletSounds.Tlipoca_Swing, Projectile.Center, 0.75f, 1, 0.1f + 0.14f * SwingTime, 0.1f, 2);
                Helper.MaxProgress[0] = (int)(AttackSpeed * .65f);
                Helper.MaxProgress[2] = (int)(AttackSpeed * .95f);
                Width = Height *= Lerp(1f, 1.12f, SwingTime / 2f);
            }
            BeginTargetRotation = Owner.Center.ToMouseVector2().ToRotation();
            TargetRotation = BeginTargetRotation;
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            if (OldAimPos.Count > 5 * Projectile.MaxUpdates)
                OldAimPos.RemoveAt(0);
        }
        public void HandleSoulStoneReaper()
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ProjectileType<CrimsonScytheSoulStone>())
                    continue;
                if (proj.owner != Owner.whoAmI)
                    continue;
                if (!proj.friendly)
                    continue;
                if (((CrimsonScytheSoulStone)proj.ModProjectile).AttackState != CrimsonScytheSoulStone.State.Idle)
                    continue;
                float _ = float.NaN;
                Vector2 beamBeginPos = Owner.Center;
                Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 130;
                bool c = Collision.CheckAABBvLineCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
                if (!c)
                    continue;
                if (Owner.HJScarlet().crimsonScytheAttackCounter > 0)
                    ((CrimsonScytheSoulStone)proj.ModProjectile).BreakAI = CrimsonScytheSoulStone.BreakType.ExecutionStrike;
                else
                    ((CrimsonScytheSoulStone)proj.ModProjectile).BreakAI = CrimsonScytheSoulStone.BreakType.Return;
                ((CrimsonScytheSoulStone)proj.ModProjectile).AttackState = CrimsonScytheSoulStone.State.Explosion;
                ((CrimsonScytheSoulStone)proj.ModProjectile).InitVector2= Owner.Center.GetNormalVector2(proj.Center);
            }
        }
        public override void OnExecution()
        {
            Owner.HJScarlet().crimsonScytheAttackCounter = 20;
            ScarletSound(HJScarletSounds.Misc_ManaClearUse, Owner.Center, 0.55f, 1, -0.84f, 0.2f);
        }
        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ControlPlayerArm(Projectile.rotation);

        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            if (Helper.Progress[2] <= 0)
            {
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;
            }
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }

        public void UpdateAnimation()
        {
            if (StopTiming > 0)
            {
                StopTiming--;
                return;
            }
            if (!ThirdSwing)
            {
                UpdateHalfCircleSwingAnimation();
            }
            else
            {
                UpdateFullCircleSwingAnimation();
            }
        }
        #region 全向的第三挥砍
        public void UpdateFullCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdtaeFullCircleBegin();
                HandleSoulStoneReaper();
            }
            else if (!Helper.IsDone[1])
            {
                UpdtaeFullCircleEnd();
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);
            }
            else
            {
                SwingTime = -1;
                Projectile.Kill();
            }

        }
        public void UpdtaeFullCircleEnd()
        {
            Helper.UpdateAniState(1);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(1));
            float beginAngle = 415f * Flip.ToDirectionInt();
            float endAngle = 420 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width + .24f, Height + .24f, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.65f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        public void UpdtaeFullCircleBegin()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -210f * Flip.ToDirectionInt();
            float endAngle = 415f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width + .24f, Height + .24f, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.65f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width + .24f, Height + .24f, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.65f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120;
                OldAimPos.Add(slashPosFinal);

                if (easedProgress >= 0.98f)
                    return;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.991f, 1.01f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    Vector2 posOff = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * Projectile.spriteDirection) * i * 1.4f;
                    pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.67f, 0.85f));
                    dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    ECSParticle.SmokeParticle(pos + posOff, vel * 9.3f, RandLerpColor(Color.DarkRed, Color.Black), Main.rand.Next(16, 41), RandRotTwoPi, 0.3670f * (easedProgress), Main.rand.NextFloat(.75f, 1.15f) * Lerp(0.395f, 0.595f, easedProgress), false, BlendState.AlphaBlend);
                }

                if (Main.rand.NextBool(1))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 115, Main.rand.NextFloat(.31f, .8f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 0.45f, 0.15f, BlendState.Additive);
                }
            }
        }
        #endregion
        #region 起手两挥砍
        public void UpdateHalfCircleSwingAnimation()
        {
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();
                HandleSoulStoneReaper();

            }
            else if (!Helper.IsDone[2] && !Main.mouseLeft)
            {
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);

                if (Main.mouseLeft || Owner.HeldItem.type != OriginalItemID)
                {
                    Projectile.Kill();
                }
                UpdateFinalAnimation();
            }
            else
                Projectile.Kill();

        }
        public void UpdateBeginAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(0));
            float beginAngle = -195f * Flip.ToDirectionInt();
            float endAngle = 185f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.5f * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 120;
                OldAimPos.Add(slashPosFinal);
                if (easedProgress >= 0.98f)
                    return;
                for (int i = 0; i <= 4; i += 2)
                {
                    //Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.991f, 1.01f));
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 120, Main.rand.NextFloat(0.67f, 0.85f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                    if (Main.rand.NextBool(3))
                        ECSParticle.SmokeParticle(pos + vel * i * 1.5f, vel * 8.3f, RandLerpColor(Color.DarkRed, Color.Black), Main.rand.Next(16, 41), RandRotTwoPi, 0.970f * (1 - easedProgress), Main.rand.NextFloat(.75f, 1.15f) * Lerp(0.357f, 0.9f, easedProgress), false, BlendState.AlphaBlend);
                }
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 115, Main.rand.NextFloat(.41f, .95f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 0.45f, 0.15f, BlendState.Additive);
                }
            }
        }
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(1);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseOutBack(Helper.GetAniProgress(1));
            float beginAngle = 185f * Flip.ToDirectionInt();
            float endAngle = 195 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }
        /// <summary>
        /// 收尾动画，在开始收尾的时候这里就不会占用玩家的itemTime了
        /// <br>在这个动画下按下左键会强制进行下一次的攻击</br>
        /// </summary>
        public void UpdateFinalAnimation()
        {
            Helper.UpdateAniState(2);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseInCubic(Helper.GetAniProgress(2));
            float beginAngle = 185f * Flip.ToDirectionInt();
            float endAngle = 183f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.5f * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .015f);
        }
        #endregion
        #region 处理处死
        public override void OnKill(int timeLeft)
        {
            HandleExecution();
            if(ThirdSwing)
            {
                Projectile.HJScarlet().ExecutionStrike = false;
            }
            if (Main.mouseLeft && Projectile.ai[0] == 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((CrimsonScytheHeldProj)proj.ModProjectile).Flip = !Flip;
                ((CrimsonScytheHeldProj)proj.ModProjectile).SwingTime = SwingTime + 1;
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = Projectile.HJScarlet().ExecutionStrike;
            }
            else if (!ThirdSwing || Projectile.ai[0] != 0)
            {
                ScarletSound(HJScarletSounds.Misc_ManaClearUse, Owner.Center, 0.55f, 1, -0.84f, 0.2f);
                Owner.ScarletHeal(2);
                for (int i = 0; i < 92; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.78f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    BloodyMetaballAlt.SpawnParticle(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 60, RandVelTwoPi(1.4f, 2.1f), 0.145f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    ECSParticle.SnowCloud(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 75, RandVelTwoPi(0.7f, 1.2f), Color.Red, 45, 1, 0.45f, 0.2f);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(.1f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    BloodyMetaballAlt.SpawnParticle(pos + dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-1, 1.1f) * 15, dir * Main.rand.NextFloat(1.4f, 2.1f), 0.745f, dir.ToRotation(), false, true);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 125f, Main.rand.NextFloat(1.41f, 1.58f));
                    Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2);
                    BloodyMetaballAlt.SpawnParticle(pos + dir * Main.rand.NextFloat(-42 * (-Flip.ToDirectionInt() * Owner.direction), 1.1f) * 1.5f, dir * Main.rand.NextFloat(1.4f, 2.1f), 0.745f, dir.ToRotation(), false, true);
                }
            }
        }
        #endregion
        #region 处理命中
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override bool? CanHitNPC(NPC target)
        {
            bool noSwing = (ThirdSwing && Helper.IsDone[0]) || (!ThirdSwing && Helper.IsDone[0]) || StopTiming > 0;
            if (noSwing)
                return false;
            if (!ThirdSwing)
                return null;
            if (EaseOutCubic(Helper.GetAniProgress(0)) < 0.97f)
                return null;
            if (!CacheTargetList.ContainsKey(target))
                return null;
            if (CacheTargetList.TryGetValue(target, out int value))
            {
                if (value < 2)
                    return null;
            }
            return false;
        }
        public Dictionary<NPC, int> CacheTargetList = [];
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //处理音效
            HitSoundHandler(target);
            //目标不可用，别播放下面的特效。
            if (!target.IsLegal())
                return;
            if (CacheTargetList.ContainsKey(target))
            {
                CacheTargetList[target] += 1;
            }
            else
            {

                CacheTargetList.TryAdd(target, 1);
            }
            if (CacheTargetList.TryGetValue(target, out int v))
            {
                if (v <= 1)
                {
                    float rot = Projectile.Center.GetNormalVector2(target.Center).ToRotation();
                    if (ThirdSwing)
                    {
                        float util = Utils.GetLerpValue(0, 12, Projectile.numHits, true);
                        int lerpValue = (int)(Lerp(30, 10, util));
                        ScreenShakeSystem.AddScreenShakes(target.Center, lerpValue, lerpValue, rot, 0, easingFunc: EaseOutExpo);
                    }
                    else
                    {
                        float util = Utils.GetLerpValue(0, 12, Projectile.numHits, true);
                        int lerpValue = (int)(Lerp(10, 0, util));
                        int lerpTime = (int)(Lerp(25, 0, util));
                        ScreenShakeSystem.AddScreenShakes(target.Center, lerpValue, lerpTime, rot, 0, easingFunc: EaseOutExpo);
                    }
                }
            }

            HitEffectsHandler(target, hit, damageDone);
            HitFirstEffectHandler(target);
            PlayerEffectHandler();
            SoulStoneSpawn(target);
        }

        public void SoulStoneSpawn(NPC target)
        {
            //灵魂石
            if (Owner.ownedProjectileCounts[ProjectileType<CrimsonScytheSoulStone>()] < 30)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * Projectile.spriteDirection);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, dir.ToRandVelocity(ToRadians(75f), 8f, 13f), ProjectileType<CrimsonScytheSoulStone>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                proj.originalDamage = Projectile.damage;
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
        }
        public void PlayerEffectHandler()
        {
            //在这里给玩家加成
            Owner.HJScarlet().antiKnockbackTime = 30;
            if (Owner.HJScarlet().crimsonScytheAttackCounter > 0)
            {
                if (Projectile.numHits < 1)
                {
                    Owner.HJScarlet().crimsonScytheAttackCounter--;
                    Owner.HJScarlet().crimsonScytheDefense += CrimsonScythe.DefensePerAdd;
                }
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ProjectileType<CrimsonScytheSoulStone>())
                        continue;
                    if (proj.owner != Owner.whoAmI)
                        continue;
                    if (!proj.friendly)
                        continue;
                    if (((CrimsonScytheSoulStone)proj.ModProjectile).AttackState != CrimsonScytheSoulStone.State.Idle)
                        continue;
                    proj.HJScarlet().ExecutionStrike = true;
                    ((CrimsonScytheSoulStone)proj.ModProjectile).AttackState = CrimsonScytheSoulStone.State.Explosion;
                    ((CrimsonScytheSoulStone)proj.ModProjectile).BreakAI = CrimsonScytheSoulStone.BreakType.ExecutionStrike;
                    break;
                }
            }
        }
        public void HitFirstEffectHandler(NPC target)
        {
            //只有第一次攻击命中才会给卡肉
            if (Projectile.numHits > 0)
                return;
            StopTiming = 35;
            Projectile.AddExecutionTimeImmediate(OriginalItemID);
        }
        public void HitEffectsHandler(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //float rot = Projectile.rotation + PiOver2 * Projectile.spriteDirection;
            if (!ThirdSwing)
            {
                //普通挥击下最多只生成12次特效，别产太多了
                if (Projectile.numHits < 12)
                    HitSparkle(target, hit, damageDone);
            }
            else
            {
                ScreenDarknessSystem.AddScreenDarkness(.85f, 2, 1, 12, EaseInCubic, EaseInCubic);
                if (Projectile.numHits < 12)
                    HitSparkleHeavy(target, hit, damageDone);
            }
        }

        public void HitSoundHandler(NPC target)
        {
            //处理音效
            float pitch = ThirdSwing ? 0.2f : -0.4f + SwingTime * .2f;
            int t = ThirdSwing ? 2 : 1;
            ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, volume: 0.6f, instances: 1, pitch: pitch, pitchVariance: .05f, variantType: t);
        }

        public void HitSparkleHeavy(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int reverse = Projectile.spriteDirection;
            //Vector2 dir = Projectile.Center.GetNormalVector2( target.Center);
            Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * reverse);
            for (int i = 0; i < 40; i++)
            {
                ECSParticle.SmokeParticle(target.Center, dir.ToRandVelocity(ToRadians(35), 1.2f, 22.5f), RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .52f, blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 40; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Red, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .28f, Main.rand.NextBool(), blendstate: BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 26; i++)
            {
                ECSParticle.LiliesFire(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Black, Color.Red), 40, RandRotTwoPi, 1, 0.2f * Main.rand.NextFloat(0.45f, 1.3f));
            }
            for (int i = 0; i < 26; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.DarkRed, Color.Red), 40, 1, 0.92f * Main.rand.NextFloat(0.95f, 1.3f), blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 42; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(25), 1.9f, 44f);
                BloodyMetaballAlt.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.5f, vel.ToRotation() - Pi, true);
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(15), 12f, 56f);
                BloodyMetaballAlt.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.92f, vel.ToRotation(), false, true);
            }
        }

        public void HitSparkle(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int reverse = Projectile.spriteDirection;
            Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2 * reverse);
            //Vector2 dir = Projectile.Center.GetNormalVector2( target.Center);
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(target.Center, dir.ToRandVelocity(ToRadians(35), 1.2f, 22.5f), RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .52f, blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Red, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .28f, Main.rand.NextBool(), blendstate: BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.LiliesFire(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Black, Color.Red), 40, RandRotTwoPi, 1, 0.2f * Main.rand.NextFloat(0.45f, 1.3f));
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.DarkRed, Color.Red), 40, 1, 0.92f * Main.rand.NextFloat(0.95f, 1.3f), blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(25), 1.9f, 39f);
                BloodyMetaballAlt.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.5f, vel.ToRotation() - Pi, true);
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(15), 12f, 44f);
                BloodyMetaballAlt.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.92f, vel.ToRotation(), false, true);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 128;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        #endregion
        #region 绘制
        public void RenderPixelated(SpriteBatch spriteBatch)
        {

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.31f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkRed * 0.80f, 0.55f);
            DrawSlash(texture, Color.Red * 0.40f, 0.40f);
            DrawSlash(texture, Color.IndianRed * 0.140f, 0.350f);


            texture = HJScarletTexture.Texture_SwordSlash.Value;
            effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.21f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DarkRed * 0.55f, 0.95f);
            DrawSlash(texture, Color.Red * 0.40f, 0.50f);
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.05f);
            DrawSlash(texture, Color.Lerp(Color.Crimson, Color.White, 0.760f) * 0.75f, 0.85f, 1f);
            DrawSlash(texture, Color.Lerp(Color.IndianRed, Color.White, 0.790f) * 0.75f, 0.90f, 1f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(.1f, .1f, 0, 0), new Vector2(-Main.GlobalTimeWrappedHourly * 1.395f, 0), new Vector2(1, 2), Color.Crimson);
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.Red, 0.60f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White, 0.45f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * beginMult + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            if (ThirdSwing)
            {
                Color c = Color.White;
                float endPro = EaseInCubic(Helper.GetAniProgress(1));
                float endPro2 = EaseInBack(Helper.GetAniProgress(1));
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, Color.Red.ToAddColor() * (1 - endPro2), drawRotation, rotationPoint, Projectile.scale * (1 - endPro), flipSprite, 0);
                SB.Draw(tex, drawPosition, null, c * (1 - endPro2), drawRotation, rotationPoint, Projectile.scale * (1 - endPro), flipSprite, 0);
                SB.EnterShaderArea();
                Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
                Vector2 pos = drawPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * 95f * Projectile.scale * (1 - endPro);
                float glowScale = Projectile.scale * .15f * (1 - endPro);
                SB.Draw(glow, pos, null, Color.DarkRed, drawRotation, glow.Size() / 2, glowScale, flipSprite, 0);
                SB.Draw(glow, pos, null, Color.Red, drawRotation, glow.Size() / 2, glowScale * .95f, flipSprite, 0);
                SB.Draw(glow, pos, null, Color.White, drawRotation, glow.Size() / 2, glowScale * .92f, flipSprite, 0);
                SB.EndShaderArea();
            }
            else
            {
                float time = SwingTime / 3f;
                Color c = Color.Lerp(Color.White, Color.Black, Helper.GetAniProgress(2));
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, drawPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f * time, null, Color.Red.ToAddColor(), drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
                SB.Draw(tex, drawPosition, null, c, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            }
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            Texture2D texture = HJScarletTexture.Texture_SwordSlashWhite.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(0.41f, 0.53f, 0.12f, 0.12f), Vector2.One, Vector2.One);
            DrawSlash(texture, Color.Black * 0.6f, 0.75f, 0.99f);
            DrawSlash(texture, Color.DarkRed * 0.150f, 0.60f, 0.99f);
            SB.EndShaderArea();
            return false;
        }
        #endregion
    }
}
