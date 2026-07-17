using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Projectiles;
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
using HJScarletRework.Items.Weapons.Requirement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerHeldProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            JustBegin,
            Attack,
            Staff,
            ReadyToStrike
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float StrikeTime => ref Projectile.ai[2];
        public AnimationStruct Helper = new(7);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = GetSeconds(60);
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 0;
        }

        public override bool? CanDamage()
        {
            return AttackState == State.Attack;
        }
        public float Oscillation = 0;
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 25 * Projectile.MaxUpdates;
            Helper.MaxProgress[2] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[3] = 25 * Projectile.MaxUpdates;
   
        }
        public bool JustPressedFunction = false;
        public bool StaffMode = false;
        public float BeingTargetRotation = 0;
        public float TargetRotation = 0;
        public bool HeavyStrikeReset = false;
        public int Flip = -1;
        public bool PlaySound = false;
        public int TotalStrikeTime = 30;
        public bool PumpoutBullet = true;
        public List<int> BloodyProjIndex = [];
        public override void ProjAI()
        {
            Projectile.timeLeft = 2;
            switch (AttackState)
            {
                case State.JustBegin:
                    DoJustBegin();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.Staff:
                    DoStaff();
                    break;
                case State.ReadyToStrike:
                    DoReadyToStrike();
                    break;
            }
        }
        /// <summary>
        /// 初始情况
        /// </summary>
        public void DoJustBegin()
        {
            if (!Helper.IsDone[2])
            {
                //重新设定锤子状态

                if (Helper.OnAnimationBegin(2))
                {
                    float angleToWhat = ToRadians(-105f);
                    if (Owner.direction < 0)
                        angleToWhat = ToRadians(-65f);
                    Projectile.spriteDirection = Owner.direction;
                    Projectile.rotation = (angleToWhat);
                    HandlePumpOut();
                    Projectile.Center = Owner.MountedCenter - Vector2.UnitX * Owner.direction * 100f;
                }
                Helper.UpdateAniState(2);
            }
            else if (!Helper.IsDone[3])
            {
                Helper.UpdateAniState(3);
                if (Helper.GetAniProgress(3) > 0.40f && !PlaySound)
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
            }
            else
            {
                Timer++;
                if (Timer > Projectile.MaxUpdates * 30f)
                {
                    PlaySound = false;
                    Timer = 0;
                    AttackState = State.Attack;
                    Projectile.netUpdate = true;
                    PumpoutBullet = true;
                    JustPressedFunction = false;
                }
            }
            DoIdle();
            DoFadingParticle(1f);

        }

        #region 法杖模式下的处理
        public bool Using => Owner.HeldItem.type == ItemType<GaiaStriker>() && Owner.controlUseItem;
        public void DoStaff()
        {
            if (!JustPressedFunction && Owner.HeldItem.type == ItemType<GaiaStriker>() && HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                //重击过程中如果不存在射弹，立刻处死出去
                //这里是要做一个处理，但是再做处理太麻烦了，处死原地生成一个新的射弹更容易
                if (Projectile.IsMe())
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    proj.originalDamage = Projectile.originalDamage;
                }
                Projectile.Kill();
                return;
            }
            Owner.HJScarlet().holdingGaiaStaff = Owner.HeldItem.type == ItemType<GaiaStriker>();
            if (Owner.controlUseItem && Owner.HeldItem.IsWeapon())
            {
                HandleStaffCondition();
            }
            else
            {
                DoIdle();
                if (Timer > 0)
                    Timer--;
            }
            Projectile.position.Y += Owner.gfxOffY;
        }

        public void HandleStaffToMinionBegin()
        {

        }

        public void HandleStaffToMinion()
        {

        }

        public void DoStaffModeIdle()
        {
            //锤子应当朝向的位置
            Oscillation += ToRadians(1.5f);
            float anchorPosX = Owner.MountedCenter.X - Owner.direction * 0;
            float anchorPosY = Owner.MountedCenter.Y - (30f * MathF.Sin(Oscillation) / 9f);
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.21f);

        }

        public void HandleStaffCondition()
        {
            DoStaffModeIdle();
            Projectile.rotation = Projectile.Center.GetNormalVector2(Main.MouseWorld).ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            //使用状态下让锤子挂载玩家手上，同时我们开始占用玩家的手持
            if (Owner.HeldItem.type == ItemType<GaiaStriker>())
            {
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.ControlPlayerArm(Projectile.rotation, 1);
            }
            Timer++;
            int count = 20; 
            int dmg = Owner.HeldItem.type == ItemType<GaiaStriker>() ? Projectile.originalDamage : Projectile.originalDamage / 2;
            if(Timer > count * Projectile.MaxUpdates)
            {
                Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * 60f;
                Vector2 dir = pos.GetNormalVector2(Main.MouseWorld);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, dir * 6f, ProjectileType<GaiaStrikerBeam>(), dmg, Projectile.knockBack, Projectile.owner);
                Timer = 0;
            }
        }

        #endregion
        #region 仆从模式下的处理
        /// <summary>
        /// 攻击
        /// </summary>
        public void DoAttack()
        {
            if (!JustPressedFunction && Owner.HeldItem.type == ItemType<GaiaStriker>() && HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                HandleMinionToStaffBegin();
            }
            if (JustPressedFunction)
                HandleConvertingAnimationMinionToStaff();
            else
                HandleMinionAttack();
        }
        /// <summary>
        /// 玩家下达指令时，仆从转为法杖的初始化状态
        /// </summary>
        public void HandleMinionToStaffBegin()
        {
            Projectile.extraUpdates = 0;
            StaffMode = true;
            JustPressedFunction = true;

            //重新设定锤子状态
            float angleToWhat = ToRadians(-105f);
            if (Owner.direction < 0)
                angleToWhat = ToRadians(-65f);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = (angleToWhat);
            HandlePumpOut();
            //让渐变出现的特效确保出现在玩家后方
            Projectile.Center = Owner.MountedCenter - Vector2.UnitX * Owner.direction * 100f;
        }
        public void HandlePumpOut()
        {
            if (!PumpoutBullet)
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
        public void HandleConvertingAnimationMinionToStaff()
        {
            if (!Helper.IsDone[0])
            {
                if (Main.rand.NextBool(3))
                {
                    for (int i = 0; i < 2; i++)
                        BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(26) - Vector2.UnitX * 45f * Owner.direction, Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(-12f, 13f), 0.8f, Projectile.rotation, false);
                }
                UpdateBeginAnimation();
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                UpdateMidAnimation();
                Helper.UpdateAniState(1);
                if (Helper.GetAniProgress(1) > 0.40f && !PlaySound)
                {
                    SoundEngine.PlaySound(HJScarletSounds.Gaia_Charge with { MaxInstances = 0, Pitch = 0.5f, Volume = .65f }, Projectile.Center);
                    SoundEngine.PlaySound(HJScarletSounds.Gaia_Smash with { MaxInstances = 0, Pitch = -0.5f, Volume = .45f }, Projectile.Center);
                    PlaySound = true;
                    for (int i = 0; i < 64; i++)
                    {
                        Vector2 pos = Projectile.Center.ToRandCirclePos(40, 60) - Vector2.UnitX * 25f * Owner.direction;
                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-16f, 17f), RandLerpColor(Color.Red, Color.Black), Main.rand.Next(30, 48), RandRotTwoPi, 0.98f, Main.rand.NextFloat(.75f, 1.1f) * .4f * Projectile.scale, Main.rand.NextBool(), BlendState.NonPremultiplied);
                    }
                }
            }
            else
            {
                JustPressedFunction = false;
                AttackState = State.Staff;
            }
            DoFadingParticle();
            DoIdle();
        }
        public NPC CurTarget = null;
        public void HandleMinionAttack()
        {
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1300, canPassWall: true) && !StaffMode)
            {
                Projectile.extraUpdates = 2;
                Projectile.scale = Lerp(Projectile.scale, 0.95f, 0.02f);
                //重击处理，在这里执行
                if (HeavyStrikeReset)
                {
                    if(Main.rand.NextBool(9))
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

                    if (!Helper.IsDone[4])
                    {
                        StrikeTime = 0;
                        Projectile.position.X = Lerp(Projectile.position.X, CurTarget.position.X, .12f);
                        Projectile.rotation = Projectile.SpeedAffectRotation() + RandRot;
                        Projectile.velocity *= .92f;
                        Projectile.velocity.Y *= 1f;
                        Helper.UpdateAniState(4);
                    }
                    else if (!Helper.IsDone[5])
                    {
                        if (StrikeTime > 0)
                        {
                            Helper.UpdateAniState(5);
                            Projectile.velocity *= .92f;
                            Projectile.rotation = Projectile.SpeedAffectRotation() + Projectile.velocity.ToRotation() - RandRot;
                        }
                        else
                        {
                            Projectile.extraUpdates = 3;
                            Projectile.HomingTarget(target.Center, -1, 27, 5);
                            Projectile.rotation = Projectile.rotation.AngleLerp( Projectile.velocity.ToRotation() - PiOver2,0.42f);
                        }
                    }
                    else
                    {
                        Projectile.extraUpdates = 2;
                        StrikeTime = 0;
                        HeavyStrikeReset = false;
                    }
                }
                else
                {
                    if (Helper.Progress[4] > 0)
                        Helper.Progress[4]-= 2;
                    CurTarget = target;
                    Projectile.rotation += 0.2f;
                    Projectile.HomingTarget(target.Center, -1, 16f, 5f);
                    if(Main.rand.NextBool(3))
                    {
                        Vector2 pos = Projectile.Center.ToRandCirclePos(24.6f);
                        Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat() * 4f;
                        BloodyMetaball.SpawnParticle(pos, vel * 1.35f, 0.15f, vel.ToRotation(), true);
                    }
                    if(Main.rand.NextBool(3))
                    {
                        Vector2 pos = Projectile.Center.ToRandCirclePos(30f);
                        Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat() * 7f;
                        new Fire(pos, vel, RandLerpColor(Color.DarkRed, Color.Crimson), 40, RandRotTwoPi, 1f, 0.2f * Main.rand.NextFloat(.92f,1.1f)).SpawnToNonPreMult();
                    }
                }
            }
            else if (HeavyStrikeReset)
            {
                //重击过程中如果不存在射弹，立刻处死出去
                //这里是要做一个处理，但是再做处理太麻烦了，处死原地生成一个新的射弹更容易
                if (Projectile.IsMe())
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    proj.originalDamage = Projectile.originalDamage;
                }
                Projectile.Kill();
            }
            else if (!HeavyStrikeReset)
            {
                Projectile.extraUpdates = 0;
                Projectile.scale = Lerp(Projectile.scale, 1.01f, 0.2f);
                DoIdle();
            }
        }
        public void DoFadingParticle(float mult = 1f)
        {
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
        public void UpdateBeginAnimation()
        {
        }

        public void UpdateMidAnimation()
        {
        }

        #endregion
        public void DoReadyToStrike()
        {
        }

        public void DoIdle()
        {
            Projectile.timeLeft = 2;
            //锤子应当朝向的位置
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
        public float RandRot = 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!HeavyStrikeReset)
            {
                if (StrikeTime > 30)
                {
                    HeavyStrikeReset = true;
                    Projectile.velocity = -Vector2.UnitY * 34f + target.velocity.ToSafeNormalize() * Clamp(target.velocity.Length(), 0f, 17f);
                    SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
                    //下面这些是动画进程必要的初始化。
                    Helper.MaxProgress[4] = 23 * Projectile.MaxUpdates;
                    Helper.MaxProgress[5] = 15 * Projectile.MaxUpdates;
                    Helper.IsDone[4] = false;
                    Helper.IsDone[5] = false;
                    Helper.Progress[4] = 0;
                    Helper.Progress[5] = 0;
                    RandRot = RandRotTwoPi;
                    DoHeavyStrikeParticle(target.Center);
                }
                else
                {
                    SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 1, Pitch = -.54f, Volume = .44f }, Projectile.Center);
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
            else
            {
                if (StrikeTime == 0 && Helper.IsDone[4])
                {
                    DoHeavyStrikeParticle(target.Center);
                    Projectile.velocity = Vector2.UnitY * 28f + target.velocity.ToSafeNormalize() * Clamp(target.velocity.Length(), 0f, 17f);
                    SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { Pitch = .3f }, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30f, 40, Projectile.velocity.ToRotation(), ToRadians(30f));
                    StrikeTime = 1;
                }
            }
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


        public override bool ShouldUpdatePosition()
        {
            return AttackState == State.Attack;
        }
        #region 绘制
        public override bool PreDraw(ref Color lightColor)
        {
            if (!StaffMode)
            {
                DoSmallHammerDraw(ref lightColor);
            }
            else
            {
                DoHeldHammerProjDraw(ref lightColor);
            }
            return false;
        }

        public void DoHeldHammerProjDraw(ref Color lightColor)
        {
            if (!Helper.IsDone[0])
            {
                return;
            }

            Texture2D tex = Projectile.GetTexture();
            Vector2 ori = tex.ToOrigin();
            float rotFixer = Projectile.spriteDirection > 0 ? PiOver4 : -PiOver4 + Pi;
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 offsetPos = new Vector2(0, 0);
            Vector2 posBase = Projectile.Center - Main.screenPosition + offsetPos.RotatedBy(Projectile.rotation);
            int length = Projectile.oldPos.Length;
            if (Helper.IsDone[1])
            {
                for (int i = length - 1; i > 0; i--)
                {
                    Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter() + offsetPos.RotatedBy(Projectile.rotation);
                    float rot = Projectile.oldRot[i];
                    float ratios = (1 - (float)i / length);
                    int aValue = (int)(Lerp(10, 255, EaseInCubic(ratios)));
                    Color c = Color.Lerp(Color.Red, Color.White, ratios) with { A = (byte)aValue } * ratios;
                    SB.Draw(tex, pos, null, c, rot + rotFixer, ori, Projectile.scale, se, 0);
                }
            }

            Color edgeDrawColor = Color.Crimson.ToAddColor();
            float easedProgress = Helper.GetAniProgress(1);
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, posBase + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeDrawColor * EaseInBack(easedProgress), Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            ApplyMeltShaderProjDraw(tex, posBase, rotFixer, ori, se, easedProgress);
        }
        public void ApplyMeltShaderProjDraw(Texture2D tex, Vector2 posBase, float rotFixer, Vector2 ori, SpriteEffects se, float progress)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            GD.Textures[0] = tex;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            GD.Textures[1] = HJScarletTexture.Noise_Misc.Value;
            GD.SamplerStates[1] = SamplerState.PointClamp;
            Effect shader = HJScarletShader.EdgeMeltsShader;
            shader.Parameters["progress"].SetValue((1 - EaseOutCubic(progress)));
            shader.Parameters["InPutTextureSize"].SetValue(tex.Size());
            shader.Parameters["EdgeColor"].SetValue(Color.Red.ToVector4());
            shader.Parameters["EdgeWidth"].SetValue(.01f);
            shader.CurrentTechnique.Passes[0].Apply();

            Color mainColor = Color.Lerp(Color.White, Color.DarkRed, 0.14f);
            SB.Draw(tex, posBase, null, mainColor with { A = 255 }, Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            SB.EndShaderArea();

        }
        public void DoSmallHammerDraw(ref Color lightColor)
        {
            if (!Helper.IsDone[2])
                return;
            //一堆基础数据
            Texture2D tex = TextureAssets.Projectile[ProjectileType<GaiaStrikerProj>()].Value;
            Vector2 ori = tex.Size() / 2;
            int length = Projectile.oldPos.Length;
            float rotFixer = Projectile.spriteDirection > 0 ? PiOver4 : -PiOver4 + Pi;
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Helper.IsDone[3])
            {
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
            Vector2 posBase = Projectile.Center - Main.screenPosition;
            Color edgeDrawColor = Color.Crimson.ToAddColor();
            float easedProgress = Helper.GetAniProgress(3);
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, posBase + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeDrawColor * EaseInBack(easedProgress), Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            ApplyMeltShaderProjDraw(tex, posBase, rotFixer, ori, se, easedProgress);
            //if(HeavyStrikeReset)
            float progress2 = Helper.GetAniProgress(4);
            if (progress2 > .02f)
            {
                SB.EnterShaderArea();
                Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
                float scale = Projectile.scale * 0.35f;
                float eased = EaseOutBack(progress2);
                Vector2 dir = Projectile.rotation.ToRotationVector2() * 30f;
                Vector2 glowPos = Projectile.Center - Main.screenPosition + dir;
                SB.Draw(glow, glowPos, null, Color.DarkRed * eased, 0, glow.ToOrigin(), scale, 0, 0);
                SB.Draw(glow, glowPos, null, Color.Red * eased, 0, glow.ToOrigin(), scale * .95f, 0, 0);
                SB.Draw(glow, glowPos, null, Color.White * eased, 0, glow.ToOrigin(), scale * .90f, 0, 0);
                SB.EndShaderArea();
            }
        }
        #endregion
    }
}
