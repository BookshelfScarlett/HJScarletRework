using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DreamlessNightMinion : HJScarletProj
    {
        public override string Texture => GetInstance<DreamlessNightProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Idle,
            ReadyStrike
        }
        public State MinionProjAI
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public AnimationStruct Helper = new(3);
        public float Oscillation = 0;
        public bool UseVelocity = false;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 1500;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 0;
        }
        public override bool ShouldUpdatePosition()
        {
            return UseVelocity;
        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss with { Volume = 0.75f }, Projectile.Center);
            Helper.MaxProgress[0] = GetSeconds(20);
            Helper.MaxProgress[1] = 85;
        }
        public override void ProjAI()
        {
            Oscillation += ToRadians(2.5f);
            if (!Helper.IsDone[0])
                UpdateBeforeReadyStrike();
            else
                UpdateReadyStrike();
            UpdateHammerParticle();
        }

        public void UpdateHammerParticle()
        {
            if(Main.rand.NextBool(8))
            {
                Rectangle rec = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width, Projectile.height));
                Vector2 starPos = Main.rand.NextVector2FromRectangle(rec);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(5), 0.1f, 0.4f);
                Color starColor = RandLerpColor(Color.DarkViolet, Color.Violet);
                new KiraStar(starPos, starVel, starColor, 60, 0, 1, 0.25f, fadeIn: true).SpawnToNonPreMult();
                new FusableBall(starPos, starVel, RandLerpColor(Color.DarkViolet, Color.Purple), 60, 0.475f, new Vector2(0.5f, 0.5f) * 0.230f).SpawnToPriority();
            }
            if (Main.rand.NextFloat() < Helper.GetAniProgress(1))
            {
                if (Main.rand.NextBool(4))
                    new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(25), -Projectile.SafeDirByRot()* Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, RandRotTwoPi, 1, 0.5f, false, 0.2f).Spawn();
                if (Main.rand.NextBool(4))
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(32), -Projectile.SafeDirByRot() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.155f, Main.rand.NextBool()).SpawnToNonPreMult();
                if (Main.rand.NextBool(4))
                    new EmptyRing(Projectile.Center.ToRandCirclePos(16), -Projectile.SafeDirByRot() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.2f, 1, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
            }
        }

        public void UpdateBeforeReadyStrike()
        {
            Projectile.timeLeft = 300;
            MinionProjAI = State.Idle;

            bool isPressingLeftClick = Owner.JustPressLeftClick();
            UpdateIdleAI(isPressingLeftClick);

            Timer += isPressingLeftClick.ToInt();
            if (isPressingLeftClick)
            {
                Helper.UpdateAniState(0);
                ShooDreamLaser();
            }

        }

        public void UpdateReadyStrike()
        {
            if (!Helper.IsDone[1])
            {
                if (Helper.GetAniProgress(1) == 0f)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 vel = (Projectile.velocity).ToRandVelocity(ToRadians(12f), 1f, 8f);
                        Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16);
                        ShadowNebula.SpawnParticle(spawnPos + vel.ToRandVelocity(ToRadians(12f), 1f, 8f), vel, Main.rand.NextFloat(0.1f, 0.135f) * 1.1f, HJScarletTexture.Texture_WhiteCircle.Value);
                    }

                    SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss with { Volume = 0.75f }, Projectile.Center);
                }
                Helper.UpdateAniState(1);
                UpdateIdlePosBeforeStrike();
            }
            else
            {
                UseVelocity = true;
                if (Projectile.velocity.Equals(Vector2.Zero))
                {
                    Projectile.velocity = (Main.MouseWorld - Projectile.Center).ToSafeNormalize() * 24f;
                    StrikeInit();
                }
                Projectile.extraUpdates = 3;
                for (int j = 0; j < 8; j++)
                {
                    Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(12f), 1f, 1.2f);
                    Vector2 spawnPos2 = Projectile.Center.ToRandCirclePos(4) + Projectile.SafeDir() * 5f * j;
                    ShadowNebula.SpawnParticle(spawnPos2, vel2, Main.rand.NextFloat(0.1f, 0.135f) * 1.1f, HJScarletTexture.Texture_WhiteCircle.Value);
                }
                if (Projectile.FinalUpdateNextBool())
                {
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 4, RandLerpColor(Color.Violet, Color.DarkViolet), 80, RandRotTwoPi, 1, 0.24f, Main.rand.NextBool()).SpawnToNonPreMult();
                        
                }
            }
        }
        public void StrikeInit()
        {

            SoundEngine.PlaySound(HJScarletSounds.Misc_MagicStaffFire with { MaxInstances = 2, Pitch = -0.4f, Volume = 0.5f }, Projectile.Center);
            ScreenShakeSystem.AddScreenShakes(Owner.Center, 60f, 180, Projectile.velocity.ToRotation(), ToRadians(0f));
            for (int i = 0; i < 16; i++)
            {
                Vector2 vel = (Projectile.velocity).ToRandVelocity(ToRadians(12f), 1f, 8f);
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16);
                ShadowNebula.SpawnParticle(spawnPos + vel.ToRandVelocity(ToRadians(12f), 1f, 8f), vel, Main.rand.NextFloat(0.1f, 0.135f) * 1.1f, HJScarletTexture.Texture_WhiteCircle.Value);
            }

        }
        public void UpdateIdlePosBeforeStrike()
        {
            //在即将结束的时候，我们把锤子往后移一点
            bool readyToStrike = Helper.GetAniProgress(1) > 0.75f;
            float xValue = readyToStrike.ToInt() * 95f + 100f;
            float lerpValue = 0.15f - readyToStrike.ToInt() * 0.08f;
            Vector2 aimVec = Owner.LocalMouseWorld();
            float anchorPosX = Owner.MountedCenter.X - Owner.direction * xValue;
            float anchorPosY = Owner.MountedCenter.Y - (60f * MathF.Sin(Oscillation) / 9f);
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new(anchorPosX, anchorPosY);
            anchorPos =Owner.MountedCenter - Owner.ToMouseVector2() * xValue;
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, lerpValue);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家  
            float angleToWhat = (aimVec - Projectile.Center).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
        }

        public void ShooDreamLaser()
        {
            int getItemUseSpeed = (int)Owner.ApplyWeaponAttackSpeed(GetInstance<DreamlessNight>().Item, GetInstance<DreamlessNight>().Item.useTime, 10);
            if (Timer < getItemUseSpeed)
                return;
            Vector2 spawnPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 15f;
            Vector2 dir = (spawnPos - Main.MouseWorld).ToSafeNormalize();
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, dir * -16f, ProjectileType<DreamlessNightBeam>(), Projectile.damage, 1f, Owner.whoAmI);
            ((DreamlessNightBeam)proj.ModProjectile).BeamState = DreamlessNightBeam.BeamType.MinionBeam;
            SpawnDreamParticle(spawnPos);
            SoundEngine.PlaySound(HJScarletSounds.Hammer_Shoot[1] with { MaxInstances = 2, Pitch = 0.5f }, spawnPos);
            Timer = 0;
        }

        private void SpawnDreamParticle(Vector2 spawnPos)
        {
            int count = 4;
            //生成四个射弹，画花。
            //这里射弹是没有伤害的
            for (int i = 0; i < count; i++)
            {
                float rotArgs = ToRadians(360f / count * i);
                Vector2 vel = (Projectile.rotation + ToRadians(30) + rotArgs).ToRotationVector2() * 5f;
                Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<DreamlessNightBeam>(), 0, 1f, Owner.whoAmI);
                proj2.extraUpdates = 2;
                proj2.timeLeft = 30;
                ((DreamlessNightBeam)proj2.ModProjectile).BeamState = DreamlessNightBeam.BeamType.DecorationBeam;
                ((DreamlessNightBeam)proj2.ModProjectile).MaxSpeed = 8f;
                for (int j = 0; j < 2; j++)
                {
                    float nebulaScale = Main.rand.NextFloat(0.51f, 0.525f) * 1.4f;
                    ShadowNebulaVector2.SpawnParticle(spawnPos, Vector2.Zero, Vector2.One * nebulaScale, vel.ToRotation(), 40, HJScarletTexture.Particle_KiraStar.Value);
                }
            }
        }

        public void UpdateIdleAI(bool isLeftClicking)
        {
            if (isLeftClicking)
            {
                //锤子应当朝向的位置
                Vector2 aimVec = Owner.LocalMouseWorld();
                float anchorPosX = Owner.MountedCenter.X - Owner.direction * 150f;
                float anchorPosY = Owner.MountedCenter.Y - (60f * MathF.Sin(Oscillation) / 9f + 130);
                //递增的值越大，锤子的摆动幅度越大
                //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
                Vector2 anchorPos = new(anchorPosX, anchorPosY);
                //实际更新位置
                Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
                //计算锤子需要的朝向。
                //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
                float angleToWhat = (aimVec - Projectile.Center).SafeNormalize(Vector2.One).ToRotation();
                //最后使用lerp来让锤子朝向得到修改。
                Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
            }
            else
            {
                //锤子应当朝向的位置
                Vector2 aimVec = Owner.MountedCenter;
                float anchorPosX = Owner.MountedCenter.X - Owner.direction * 60f;
                float anchorPosY = Owner.MountedCenter.Y - (60f * MathF.Sin(Oscillation) / 9f);
                //递增的值越大，锤子的摆动幅度越大
                //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
                Vector2 anchorPos = new(anchorPosX, anchorPosY);
                //实际更新位置
                Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
                //计算锤子需要的朝向。
                //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
                float angleToWhat = ToRadians(115f);
                if (Owner.direction < 0)
                    angleToWhat = ToRadians(60f);
                //最后使用lerp来让锤子朝向得到修改。
                Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, Projectile.Center - Main.screenPosition + ToRadians(60 * i).ToRotationVector2() * 2f, null, Color.DarkViolet.ToAddColor(), Projectile.rotation + PiOver4, tex.ToOrigin(), Projectile.scale * 1.2f, 0, 0);
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp( Color.White, Color.Black, Helper.GetAniProgress(1)), Projectile.rotation + PiOver4, tex.ToOrigin(), Projectile.scale * 1.2f, 0, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}
