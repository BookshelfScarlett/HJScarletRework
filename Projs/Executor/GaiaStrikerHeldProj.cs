using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Data;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerHeldProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            JustBegin,
            InStaff
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Oscillation => ref Projectile.ai[2];
        public float AngleLerpValue = 0;
        public AnimationStruct Helper = new AnimationStruct(4);
        public bool PlaySound = false;
        public int StoredLifeTime = 0;
        public float RightClickHoldingTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.SetUpHeldProj();
            Projectile.noEnchantmentVisuals = true;
        }
        public override bool? CanDamage() => false;
        public override void OnFirstFrame()
        {
            StoredLifeTime = Projectile.timeLeft;
            Helper.MaxProgress[0] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 25 * Projectile.MaxUpdates;
            Helper.MaxProgress[2] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[3] = 25 * Projectile.MaxUpdates;
        }

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
                    proj.HJScarlet().HasExecutionMechanic = false;
                }
                ECSParticle.CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f, .4f, BlendState.Additive);
                ECSParticle.CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.30f, .4f, BlendState.Additive);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 80, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = -0.4f, Volume = .477f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -0.64f, Volume = .577f }, Projectile.Center);
                Projectile.AddExecutionTimeDelayed(ItemType<GaiaStriker>(), 6);
                Projectile.Kill();
                return;
            }
            switch (AttackState)
            {
                case State.JustBegin:
                    DoJustBegin();
                    break;
                case State.InStaff:
                    DoInStaff();
                    break;
            }
        }
        public void DoJustBegin()
        {
            if (!Helper.IsDone[0])
            {
                if (Helper.OnAnimationBegin(0))
                {
                    float angleToWhat = ToRadians(-105f);
                    if (Owner.direction < 0)
                        angleToWhat = ToRadians(-65f);
                    Projectile.spriteDirection = Owner.direction;
                    Projectile.rotation = angleToWhat;
                    CreateBloodyExplosion();
                    Projectile.Center = Owner.MountedCenter - Vector2.UnitX * Owner.direction * 100f;
                }

                if (Main.rand.NextBool(3))
                {
                    for (int i = 0; i < 2; i++)
                        BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(26) - Vector2.UnitX * 45f * Owner.direction, Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(-12f, 13f), 0.8f, Projectile.rotation, false);
                }
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
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
                AttackState = State.InStaff;
                Projectile.netUpdate = true;
            }
            DoFadingParticle();
            DoIdle();
        }
        public void DoInStaff()
        {
            //只有手持武器的情况下才允许发起这个切换
            if (Owner.HeldItem.type == ItemType<GaiaStriker>() && HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                if (Projectile.IsMe())
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<GaiaStrikerMountedProj>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    proj.originalDamage = Projectile.originalDamage;
                    proj.timeLeft = StoredLifeTime;
                }
                Projectile.Kill();
                return;
            }

            Owner.HJScarlet().holdingGaiaStaff = Owner.HeldItem.type == ItemType<GaiaStriker>();
            //使用物品，且手持的物品真的是一把武器，我们才让其准备发起攻击
            if (Owner.controlUseItem && Owner.HeldItem.IsWeapon())
            {
                HandleStaffCondition();
                StoredLifeTime = Projectile.timeLeft;
                AngleLerpValue = Lerp(AngleLerpValue, 1.01f, 0.10f);
            }
            else
            {
                DoIdle();
                //记得自然减少攻击冷却
                if (Timer > 0)
                    Timer--;
                AngleLerpValue = Lerp(AngleLerpValue, 0, 0.10f);
            }
            //位置修正
            Projectile.position.Y += Owner.gfxOffY;
        }
        /// <summary>
        /// 玩家下达指令时，仆从转为法杖的初始化状态
        /// </summary>
        public void HandleMinionToStaffBegin()
        {
            Projectile.extraUpdates = 0;
            //重新设定锤子状态
            float angleToWhat = ToRadians(-105f);
            if (Owner.direction < 0)
                angleToWhat = ToRadians(-65f);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = (angleToWhat);
            CreateBloodyExplosion();
            //让渐变出现的特效确保出现在玩家后方
            Projectile.Center = Owner.MountedCenter - Vector2.UnitX * Owner.direction * 100f;
        }
        public void CreateBloodyExplosion()
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
                proj.ai[2] = 1;
                proj.HJScarlet().HasExecutionMechanic = Main.rand.NextBool();
            }
            ECSParticle.CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f, .4f, BlendState.Additive);
            ECSParticle.CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.30f, .4f, BlendState.Additive);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 80, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = -0.4f, Volume = .477f }, Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -0.64f, Volume = .577f }, Projectile.Center);
        }


        public void HandleStaffCondition()
        {
            DoStaffModeIdle();
            //发射射线的起始位置实际上靠前，为了确保锤子的角度正确，需要重新设定一下点位
            Vector2 firePos = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * 60f;
            float tarRot = firePos.GetNormalVector2(Main.MouseWorld).ToRotation();
            Projectile.rotation = Projectile.rotation.AngleLerp(tarRot, AngleLerpValue);
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            //使用状态下让锤子挂载玩家手上，同时我们开始占用玩家的手持
            if (Owner.HeldItem.type == ItemType<GaiaStriker>())
            {
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.ControlPlayerArm(Projectile.rotation, 1);
            }
            //递增计时器
            Timer++;
            int count = 20;
            int dmg = Owner.HeldItem.type == ItemType<GaiaStriker>() ? Projectile.originalDamage : Projectile.originalDamage / 2;
            if (Timer > count * Projectile.MaxUpdates)
            {
                Vector2 dir = tarRot.ToRotationVector2();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), firePos, dir * 6f, ProjectileType<GaiaStrikerBeam>(), dmg, Projectile.knockBack, Projectile.owner);
                Projectile.timeLeft += 30;
                Timer = 0;
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Staff with { MaxInstances = 0, Pitch = .5f, PitchVariance = 0.1f, Volume = .45f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Blunt_Swing with { MaxInstances = 0, Pitch = -.5f, Volume = .45f }, Projectile.Center);
            }

        }

        public void DoStaffModeIdle()
        {
            //锤子应当朝向的位置
            Oscillation += ToRadians(1.5f);
            float anchorPosX = Owner.MountedCenter.X - Owner.direction * 0;
            float anchorPosY = Owner.MountedCenter.Y - (30f * MathF.Sin(Oscillation) / 9f);
            //uca联动，手持屠杀时调整位置
            if(HJScarletRework.CrossMod_UCA.TryFind<ModItem>("CarnageRay", out ModItem value))
            {
                if (Owner.HeldItem.type == value.Type)
                {
                    anchorPosY -= 100f;
                    anchorPosX += 0f * Owner.direction;
                }
            }
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, AngleLerpValue);

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
        /// <summary>
        /// 正常情况下在背后时的挂载
        /// </summary>
        public void DoIdle()
        {

            Projectile.timeLeft = StoredLifeTime;
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
        public int FrameCount = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Helper.IsDone[0])
            {
                return false;
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

            Color edgeDrawColor = Color.Red.ToAddColor();
            float easedProgress = Helper.GetAniProgress(1);
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, posBase + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeDrawColor * EaseInBack(easedProgress), Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            ApplyMeltShaderProjDraw(tex, posBase, rotFixer, ori, se, easedProgress);
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            Texture2D glow = HJScarletTexture.Particle_RingShiny.Value;
            Vector2 glowPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 50f * Projectile.scale - Main.screenPosition;
            float pro = EaseOutBack(AngleLerpValue);
            float scale = Projectile.scale * 0.25f * pro;
            Vector2 glowOri = glow.Size() / 2;
            float glowRot = Projectile.rotation;
            SB.Draw(glow, glowPos, null, Color.DarkRed * pro, glowRot, glowOri, scale, 0, 0);
            SB.EndShaderArea();
            return false;
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
            Color targetColor = Color.Lerp(Color.Black, Color.White, 0.30f);
            Color boxColor = Color.Lerp(targetColor, mainColor, EaseOutBack(AngleLerpValue));
            SB.Draw(tex, posBase, null, boxColor with { A = 255 }, Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            SB.EndShaderArea();
        }

    }
}
