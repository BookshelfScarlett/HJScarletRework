using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerMountedProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<GaiaStrikerProj>().Texture;
        public AnimationStruct Helper = new AnimationStruct(4);
        public enum State
        {
            JustBegin,
            Attack,
            HeavyStrike,
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float StrikeTime => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = GetSeconds(60) * 2;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 0;
        }
        public override void OnFirstFrame()
        {
            StoredLifeTime = Projectile.timeLeft;
            Helper.MaxProgress[0] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 25 * Projectile.MaxUpdates;
            Helper.MaxProgress[2] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[3] = 25 * Projectile.MaxUpdates;

        }
        public bool JustPressedFunction = false;
        public bool PlaySound = false;
        public int TotalStrikeTime = 20;
        public float Oscillation = 0;
        public float RandRot = 0;
        public bool HeavyStrikeReset = false;
        public NPC CurTarget = null;
        public bool ShouldCreate = true;
        public int StoredLifeTime = 0;
        public float RightClickHoldingTime = 0;
        public override void ProjAI()
        {
            if (Projectile.timeLeft < 20)
            {
                //爆开
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = RandVelTwoPi(0.9f, 6.4f);
                    BloodyMetaball.SpawnParticle(pos, vel, 0.35f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = RandVelTwoPi(0.9f, 9.4f);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.75f, vel.ToRotation(), false);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.15f, RandRotTwoPi, true);
                }
                for (int i = 0; i < GaiaStriker.BloodBulletCount; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(6f, 10f), ProjectileType<GaiaStrikerBloodyBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    proj.ai[2] = 4;
                    proj.HJScarlet().HasExecutionMechanic = false;
                }
                ECSParticle.CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f, .4f, BlendState.Additive);
                ECSParticle.CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.30f, .4f, BlendState.Additive);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 80, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = -0.4f, Volume = .477f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -0.64f, Volume = .577f }, Projectile.Center);
                Projectile.Kill();
                return;
            }
            if (Owner.controlUseTile && Owner.HeldItem.type == ItemType<GaiaStriker>())
            {
                Projectile.timeLeft = StoredLifeTime;
                if (Projectile.FinalUpdate() || Projectile.extraUpdates == 0)
                    RightClickHoldingTime++;
                if (RightClickHoldingTime % 20 == 0 && RightClickHoldingTime !=0)
                {
                    SoundEngine.PlaySound(HJScarletSounds.Gaia_Charge with { MaxInstances = 1, Pitch = 0.1f * RightClickHoldingTime / 20, Volume = .67f }, Projectile.Center);
                    for (int i = 0; i < 36; i++)
                        ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(0), RandVelTwoPi(.7f, 21f), RandLerpColor(Color.Red, Color.DarkRed), 40, 1, 0.25f, Main.rand.NextFloat(0.9f, 1.1f) * 0.40f, Main.rand.NextBool(), BlendState.NonPremultiplied);
                    for (int i = 0; i < 18; i++)
                        ECSParticle.BloodDrop(Projectile.Center.ToRandCirclePosEdge(10, 50), -Vector2.UnitY.RotateRandom(PiOver4) * Main.rand.NextFloat(4f, 18f), RandLerpColor(Color.DarkRed, Color.Black), 60, 1, 0.10f * Main.rand.NextFloat(.9f, 1.2f), 1, true, BlendState.AlphaBlend);
                }
            }
            else
            {
                if (RightClickHoldingTime > 0)
                    RightClickHoldingTime--;
            }
            if (RightClickHoldingTime > GetSeconds(1))
            {
                //爆开
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = RandVelTwoPi(0.9f, 6.4f);
                    BloodyMetaball.SpawnParticle(pos, vel, 0.35f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = RandVelTwoPi(0.9f, 9.4f);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.75f, vel.ToRotation(), false);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.15f, RandRotTwoPi, true);
                }
                for (int i = 0; i < GaiaStriker.BloodBulletCount; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(6f, 10f), ProjectileType<GaiaStrikerBloodyBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    proj.ai[2] = 3;
                }
                ECSParticle.CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f, .4f, BlendState.Additive);
                ECSParticle.CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.30f, .4f, BlendState.Additive);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 80, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = -0.4f, Volume = .477f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -0.64f, Volume = .577f }, Projectile.Center);
                Projectile.AddExecutionTimeDirectly(ItemType<GaiaStriker>(), 6);
                Projectile.Kill();
                return;
            }

            switch (AttackState)
            {
                case State.JustBegin:
                    DoJustBegin();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.HeavyStrike:
                    DoHeavyStrike();
                    break;
            }
        }
        #region 起始渐入动画
        public void DoJustBegin()
        {
            if (!Helper.IsDone[0])
            {
                if (Helper.OnAnimationBegin(0))
                    DoOnAnimationBeginState2();
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                Helper.UpdateAniState(1);
                if (Helper.GetAniProgress(1) > .40f && !PlaySound)
                    DoAnimationStateOneMidSound();
            }
            else
            {
                Timer++;
                if (Timer > Projectile.MaxUpdates * 30)
                {
                    Timer = 0;
                    AttackState = State.Attack;
                    Projectile.netUpdate = true;
                }
            }
            UpdateFadingParticle();
            UpdateIdleState();
        }

        public void DoAnimationStateOneMidSound()
        {
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Charge with { MaxInstances = 0, Pitch = 0.4f, Volume = .65f }, Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Smash with { MaxInstances = 0, Pitch = -0.4f, Volume = .45f }, Projectile.Center);
            PlaySound = true;
            for (int i = 0; i < 64; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(40, 60) - Vector2.UnitX * 15f * Owner.direction;
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-16f, 17f), RandLerpColor(Color.Red, Color.Black), Main.rand.Next(30, 48), RandRotTwoPi, 0.98f, Main.rand.NextFloat(.75f, 1.1f) * .4f * Projectile.scale, Main.rand.NextBool(), BlendState.NonPremultiplied);
            }

        }

        public void DoOnAnimationBeginState2()
        {
            float angleToWhat = ToRadians(-105f);
            if (Owner.direction < 0)
                angleToWhat = ToRadians(-65f);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = angleToWhat;
            CreateBloodyExplosion();
            Projectile.Center = Owner.MountedCenter - Vector2.UnitX * Owner.direction * 100f;
        }
        #endregion

        #region 仆从挂载攻击
        public void DoAttack()
        {
            if (!JustPressedFunction && Owner.HeldItem.type == ItemType<GaiaStriker>() && HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                RefreshProjStatement(ProjectileType<GaiaStrikerHeldProj>());
                return;
            }

            if (Helper.Progress[2] > 0)
                Helper.Progress[2] -= 2;
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1300, canPassWall: true))
            {
                Projectile.extraUpdates = 2;
                Projectile.scale = Lerp(Projectile.scale, .95f, 0.02f);
                Projectile.rotation += .2f;
                Projectile.HomingTarget(target.Center, -1, 16f, 5f);
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(24.6f);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat() * 4f;
                    BloodyMetaball.SpawnParticle(pos, vel * 1.35f, 0.15f, vel.ToRotation(), true);
                }
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30f);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat() * 7f;
                    new Fire(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.2f * Main.rand.NextFloat(.92f, 1.1f)).SpawnToNonPreMult();
                }
                StoredLifeTime = Projectile.timeLeft;
                CurTarget = target;
            }
            else
            {
                Projectile.extraUpdates = 0;
                Projectile.scale = Lerp(Projectile.scale, 1.01f, 0.2f);
                UpdateIdleState();
            }
        }
        #endregion

        #region 连续重击
        public void DoHeavyStrike()
        {
            if (!JustPressedFunction && Owner.HeldItem.type == ItemType<GaiaStriker>() && HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                RefreshProjStatement(ProjectileType<GaiaStrikerHeldProj>());
                return;
            }

            if (Main.rand.NextBool(9))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(60);
                Vector2 vel = Projectile.velocity / 4f;
                BloodyMetaball.SpawnParticle(pos + Projectile.velocity / 4f, vel, Main.rand.NextFloat(0.90f, 1.1f) * .41f, Projectile.velocity.ToRotation());
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(60);
                float rot = RandRotTwoPi;
                float scale = Main.rand.NextFloat(.26f, .38f) * 0.615f;
                Vector2 speed = RandVelTwoPi(0.1f, 2.4f) * 1.18f;
                new Fire(pos, speed, RandLerpColor(Color.Crimson, Color.DarkRed), 40, rot, 0.75f, scale * 1.05f).SpawnToPriorityNonPreMult();
            }
            if (CurTarget.IsLegal())
            {
                //重击的情况下存续时间要一直存储
                Projectile.timeLeft = StoredLifeTime;
                UpdateStrikeIfTargetIsLegal();
            }
            else
            {
                RefreshProjStatement(Type);
            }
        }
        public void UpdateStrikeIfTargetIsLegal()
        {
            if (!Helper.IsDone[2])
            {
                StrikeTime = 0;
                Projectile.position.X = Lerp(Projectile.position.X, CurTarget.position.X, .12f);
                Projectile.rotation = Projectile.SpeedAffectRotation() + RandRot;
                Projectile.velocity *= .92f;
                Projectile.velocity.Y *= 1f;
                Helper.UpdateAniState(2);
            }
            else if (!Helper.IsDone[3])
            {
                if (StrikeTime > 0)
                {
                    Helper.UpdateAniState(3);
                    Projectile.velocity *= .92f;
                    Projectile.rotation = Projectile.SpeedAffectRotation() + Projectile.velocity.ToRotation() - RandRot;
                }
                else
                {
                    Projectile.extraUpdates = 3;
                    Projectile.HomingTarget(CurTarget.Center, -1, 27, 5);
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() - PiOver2, 0.42f);
                }
            }
            else
            {
                Projectile.extraUpdates = 2;
                StrikeTime = 0;
                AttackState = State.Attack;
                Projectile.netUpdate = true;
            }

        }
        #endregion

        #region 全局方法
        public void RefreshProjStatement(int projID)
        {
            //重击过程中如果不存在射弹，立刻处死出去
            //这里是要做一个处理，但是再做处理太麻烦了，处死原地生成一个新的射弹更容易

            if (Projectile.IsMe())
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, projID, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.originalDamage = Projectile.originalDamage;
                //传值应当传storedtimeleft
                proj.timeLeft = StoredLifeTime;
            }
            Projectile.Kill();
        }
        public void CreateBloodyExplosion()
        {
            if (!ShouldCreate)
                return;
            //爆开
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                Vector2 vel = RandVelTwoPi(0.9f, 6.4f);
                BloodyMetaball.SpawnParticle(pos, vel, 0.35f, RandRotTwoPi, true);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                Vector2 vel = RandVelTwoPi(0.9f, 9.4f);
                BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.75f, vel.ToRotation(), false);
                BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.15f, RandRotTwoPi, true);
            }
            for (int i = 0; i < GaiaStriker.BloodBulletCount; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(6f, 10f), ProjectileType<GaiaStrikerBloodyBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.ai[2] = 1;
                proj.HJScarlet().HasExecutionMechanic = true;
            }
            new CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f).Spawn();
            new CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.28f).Spawn();
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 80, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = -0.4f, Volume = .477f }, Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -0.64f, Volume = .577f }, Projectile.Center);
        }
        public void UpdateFadingParticle()
        {
            float mult = 1;
            if (Main.rand.NextBool(4))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(30) - dir.RotatedBy(PiOver2) * -5f * mult * Owner.direction, Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(-12f, 13f), 0.8f, Projectile.rotation, false);
                }
            }
            if (Main.rand.NextBool())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30) - dir.RotatedBy(PiOver2) * -5f * mult * Owner.direction;
                    ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-16f, 17f), RandLerpColor(Color.Red, Color.Black), Main.rand.Next(30, 48), RandRotTwoPi, 0.78f, Main.rand.NextFloat(.75f, 1.1f) * .3f * Projectile.scale, Main.rand.NextBool(), BlendState.NonPremultiplied);
                }
            }
            if (Main.rand.NextBool() && !PlaySound)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    Vector2 pos = Projectile.Center.ToRandCirclePos(40, 100) - dir * 60f * mult - dir.RotatedBy(PiOver2) * Owner.direction * -5f;
                    ECSParticle.BloodDrop(pos, dir.RotatedByRandom(ToRadians(15)) * Main.rand.NextFloat(17, 21f), RandLerpColor(Color.DarkRed, Color.Black), 120, 1f, Main.rand.NextFloat(.75f, 1.1f) * .15f, 1, true, BlendState.AlphaBlend);
                }
            }
        }
        public void UpdateIdleState()
        {
            //锤子应当朝向的位置
            Projectile.timeLeft = StoredLifeTime;
            Projectile.velocity *= .01f;
            Oscillation += ToRadians(2.5f);
            float anchorPosX = Owner.MountedCenter.X - Owner.direction * (130f);
            float anchorPosY = Owner.MountedCenter.Y - (60f * MathF.Sin(Oscillation) / 9f);
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = ToRadians(-105f);
            if (Owner.direction < 0)
                angleToWhat = ToRadians(-65f);
            Projectile.spriteDirection = Owner.direction;
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
        }



        public override bool? CanDamage()
        {
            return AttackState == State.Attack || (AttackState == State.HeavyStrike && Helper.IsDone[2]);
        }
        public override bool ShouldUpdatePosition()
        {
            return AttackState != State.JustBegin;
        }
        #endregion
        #region 攻击命中管理
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //下落的重击造成暴击和双倍伤害
            if (AttackState == State.HeavyStrike)
            {
                modifiers.SetCrit();
                modifiers.SourceDamage *= 2;
            }
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Attack)
            {
                if (StrikeTime > TotalStrikeTime)
                {
                    ResetHeavyStrikeStatement();

                    Projectile.velocity = -Vector2.UnitY * 34f + target.velocity.ToSafeNormalize() * Clamp(target.velocity.Length(), 0f, 17f);
                    SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Volume = .9f}, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
                    //下面这些是动画进程必要的初始化。
                    CurTarget = target;
                    DoHeavyStrikeParticle(target.Center);
                    AttackState = State.HeavyStrike;
                }
                else
                {
                    SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 1, Pitch = -.54f,  PitchVariance = .1f,Volume = .34f }, Projectile.Center);
                    StrikeTime += 1;
                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 pos = Projectile.Center.ToRandCirclePos(44.6f, 10);
                        Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat() * 4f;
                        BloodyMetaball.SpawnParticle(pos, vel * 1.65f, 0.25f, vel.ToRotation(), true);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 pos = Projectile.Center.ToRandCirclePos(5f);
                        Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat() * 13f;
                        new Fire(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.2f * Main.rand.NextFloat(.92f, 1.1f)).SpawnToNonPreMult();
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 dir = -Vector2.UnitY;
                        Vector2 pos = Projectile.Center.ToRandCirclePos(40, 100);
                        ECSParticle.BloodDrop(pos, dir.RotatedByRandom(ToRadians(15)) * Main.rand.NextFloat(17, 21f), RandLerpColor(Color.DarkRed, Color.Black), 120, 1f, Main.rand.NextFloat(.75f, 1.1f) * .15f, 1, true, BlendState.AlphaBlend);
                    }
                }
            }
            else if (AttackState == State.HeavyStrike)
            {
                if (StrikeTime == 0 && Helper.IsDone[2])
                {
                    DoHeavyStrikeParticle(target.Center);
                    Projectile.velocity = Vector2.UnitY * 28f + target.velocity.ToSafeNormalize() * Clamp(target.velocity.Length(), 0f, 17f);
                    SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Pitch = .3f,Volume = .9f }, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
                    StrikeTime = 1;
                }
            }
        }
        public void ResetHeavyStrikeStatement()
        {
            Helper.MaxProgress[2] = 23 * Projectile.MaxUpdates;
            Helper.MaxProgress[3] = 15 * Projectile.MaxUpdates;
            Helper.IsDone[2] = false;
            Helper.IsDone[3] = false;
            Helper.Progress[2] = 0;
            Helper.Progress[3] = 0;
            RandRot = RandRotTwoPi;
        }
        public void DoHeavyStrikeParticle(Vector2 center)
        {
            for (int i = 0; i < 44; i++)
            {
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(15f), 1, 12);
                BloodyMetaball.SpawnParticle(center + vel.ToRandVelocity(0, 1, 4), vel * 2.5f, Main.rand.NextFloat(.85f, 1.2f) * .24f, vel.ToRotation(), true);
            }
            for (int i = 0; i < 24; i++)
            {
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(15f), 1, 12);
                BloodyMetaball.SpawnParticle(center + vel.ToRandVelocity(0, 1, 4), vel * 2.5f, Main.rand.NextFloat(.85f, 1.2f) * .74f, vel.ToRotation(), false);
            }
            for (int i = 0; i < 24; i++)
            {
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(15f), 1, 28);
                ECSParticle.SmokeParticle(center + vel.ToRandVelocity(0, 1, 4), vel * 1.5f, RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 1, 0.25f * Main.rand.NextFloat(.75f, 1.2f), Main.rand.NextBool(), BlendState.NonPremultiplied);
            }

        }
        #endregion

        #region 绘制管理
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Helper.IsDone[0])
                return false;
            Texture2D tex = Projectile.GetTexture();
            Vector2 ori = tex.ToOrigin();
            float rotFixer = Projectile.spriteDirection > 0 ? PiOver4 : -PiOver4 + Pi;
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 posBase = Projectile.Center - Main.screenPosition;

            ApplyTrail(tex, rotFixer, ori, se);
            ApplyProjDraw(posBase, tex, rotFixer, ori, se);
            ApplyGlowCenter(posBase, se);
            return false;
        }

        public void ApplyGlowCenter(Vector2 posBase, SpriteEffects se)
        {
            float progress2 = Helper.GetAniProgress(2);
            if (progress2 > .02f)
            {
                SB.EnterShaderArea();
                Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
                float scale = Projectile.scale * 0.35f;
                float eased = EaseOutBack(progress2);
                Vector2 dir = Projectile.rotation.ToRotationVector2() * 30f;
                Vector2 glowPos = posBase + dir;
                SB.Draw(glow, glowPos, null, Color.DarkRed * eased, 0, glow.ToOrigin(), scale, se, 0);
                SB.Draw(glow, glowPos, null, Color.Red * eased, 0, glow.ToOrigin(), scale * .95f, se, 0);
                SB.Draw(glow, glowPos, null, Color.White * eased, 0, glow.ToOrigin(), scale * .90f, se, 0);
                SB.EndShaderArea();
            }
        }

        public void ApplyProjDraw(Vector2 posBase, Texture2D tex, float rotFixer, Vector2 ori, SpriteEffects se)
        {
            Color edgeDrawColor = Color.Crimson.ToAddColor();
            float easedProgress = Helper.GetAniProgress(1);
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, posBase + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeDrawColor * EaseInBack(easedProgress), Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            tex.ApplyMeltShader(Color.Red, 1 - EaseOutCubic(easedProgress));

            Color mainColor = Color.Lerp(Color.White, Color.DarkRed, 0.14f);
            SB.Draw(tex, posBase, null, mainColor with { A = 255 }, Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            SB.EndShaderArea();

        }
        public void ApplyTrail(Texture2D tex, float rotFixer, Vector2 ori, SpriteEffects se)
        {
            if (!Helper.IsDone[1])
                return;
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i];
                float ratios = (1 - (float)i / length);
                int aValue = (int)(Lerp(180, 255, EaseInCubic(ratios)));
                Color c = Color.Lerp(Color.Red, Color.White, ratios) with { A = (byte)aValue } * ratios;
                SB.Draw(tex, pos, null, c, rot + rotFixer, ori, Projectile.scale, se, 0);
            }
        }
        #endregion
    }
}
