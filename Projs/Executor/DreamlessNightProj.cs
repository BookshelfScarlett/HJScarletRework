using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// 当务之急是把这把锤子的代码整理一下。。。
    /// </summary>
    public class DreamlessNightProj : HJScarletProj
    {
        public override string Texture => GetInstance<DreamlessNight>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            BackStrike,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public AnimationStruct Helper = new(3);
        public List<NPC> StoredNPC = [];
        public List<NPC> BackHitNPC = [];
        public NPC CurBackChasingTarget = null;
        public bool SearchingTarget = true;
        public bool DontSpawnAdditionHammer = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.penetrate = 10;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(45);
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 60;
            if (DontSpawnAdditionHammer)
                SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[0] with { MaxInstances = 0, Pitch = -0.5f }, Projectile.Center);
        }


        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateAttackAI()
        {
            switch (AttackType)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoShoot()
        {
            Timer++;
            Projectile.rotation += 0.15f;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 5))
                UpdateToNextAttackID(State.Return);
        }
        public void DoReturn()
        {
            //缓冲一段时间，我们需要让锤子逐渐静止并渐入为黑色
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                UpdateAnistateZero();
            }
            else
            {
                if (Timer == 0f)
                {
                    InitReturn();
                    PoweredUpHammer();
                    Timer += 1;
                }
                ReturnToOwner();
            }

        }

        public void PoweredUpHammer()
        {
            if (!Owner.HasProj<DreamlessNightMinion>() || DontSpawnAdditionHammer)
                return;
            Vector2 velo = (-Projectile.velocity).ToSafeNormalize().RotatedBy(ToRadians(5f) * Main.rand.NextBool().ToDirectionInt()) * 18f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.MountedCenter, velo, Projectile.type, Projectile.damage / 2, 1f, Owner.whoAmI);
            proj.extraUpdates = 1;
            proj.HJScarlet().HasExecutionMechanic = false;
            ((DreamlessNightProj)proj.ModProjectile).DontSpawnAdditionHammer = true;
        }

        public void InitReturn()
        {
            Projectile.velocity = (Projectile.Center - Owner.Center).ToSafeNormalize() * -12f;
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[2] with { MaxInstances = 0, Pitch = -0.20f }, Projectile.Center);
            for (int i = 0; i < 16; i++)
            {
                Vector2 vel = (Projectile.velocity).ToRandVelocity(ToRadians(12f), 1f, 8f);
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16);
                ShadowNebula.SpawnParticle(spawnPos + vel.ToRandVelocity(ToRadians(12f), 1f, 8f), vel, Main.rand.NextFloat(0.1f, 0.135f) * 1.1f, HJScarletTexture.Texture_WhiteCircle.Value);
            }
        }

        public void ReturnToOwner()
        {
            Projectile.rotation = (-Projectile.velocity).ToRotation();
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.IntersectOwnerByDistance(50))
            {
                if (Projectile.HJScarlet().AddFocusHit && !Owner.HasProj<DreamlessNightMinion>(out int projID))
                    Owner.HJScarlet().ExecutionTime += 1;
                Projectile.Kill();
            }
        }

        public void UpdateAnistateZero()
        {
            Projectile.velocity *= 0.92f;
            Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.velocity).ToRotation(), 0.05f);
        }

        public void UpdateToNextAttackID(State id)
        {
            Projectile.netUpdate = true;
            AttackType = id;
            Timer = 0;
        }

        public void UpdateParticle()
        {
            //一些神秘的情况
            //yysy应该可以改为用状态机，但是还是有点麻烦了
            bool canNotUpdate = Projectile.IsOutScreen() || (AttackType == State.Return && Main.rand.NextFloat() > Helper.GetAniProgress(0));
            if (canNotUpdate)
                return;
            if (Projectile.FinalUpdate())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(25), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, RandRotTwoPi, 1, 0.5f, false, 0.2f).Spawn();
            if (Projectile.FinalUpdate())
                new SmokeParticle(Projectile.Center.ToRandCirclePos(32), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.155f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Projectile.FinalUpdateNextBool(3))
                new EmptyRing(Projectile.Center.ToRandCirclePos(16), Projectile.SafeDir() * Main.rand.NextFloat(0f, 2.1f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.2f, 1, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool spawn = AttackType == State.Shoot || (AttackType == State.Return && Timer == 0f);
            //如果允许生成梦境之花。则进入这个AI
            //注意我们不会重复生成
            if (spawn && !StoredNPC.Contains(target))
            {
                StoredNPC.Add(target);
                if (Projectile.numHits < 1)
                    ShootNightLaser(target);
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPos = target.Center.ToRandCirclePos(0f);
                        Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 2f, 4f);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<DreamlessNightArrow>(), Projectile.damage / 3, 0f, Owner.whoAmI);
                        ((DreamlessNightArrow)proj.ModProjectile).CurSpeed = 1;
                    }
                }

            }
            //在命中的时候，我们才生成需要的仆从
            //当然，前提是条件合理
            if (!Owner.HasProj<DreamlessNightMinion>(out int projID) && Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, projID, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                proj.rotation = Projectile.rotation;
                for (int i = 0; i < 8; i++)
                    new Fire(target.Center.ToRandCirclePos(6), RandVelTwoPi(0.1f, 8.8f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1, 0.25f).SpawnToNonPreMult();
            }
        }

        public void ShootNightLaser(NPC target)
        {
            if (DontSpawnAdditionHammer)
                return;
            for (int i = 0; i < 8; i++)
            {
                Vector2 spawnPos = target.Center.ToRandCirclePos(0f);
                Vector2 vel = RandVelTwoPi(18f, 24f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<DreamlessNightArrow>(), Projectile.damage / 3, 0f, Owner.whoAmI);
                ((DreamlessNightArrow)proj.ModProjectile).CurSpeed = 0;
            }

            SoundEngine.PlaySound(HJScarletSounds.Dream_Toss with { MaxInstances = 0, Volume = 1.5f }, Projectile.Center);
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                float rotArgs = ToRadians(360f / count * i);
                Vector2 spawnPos = (Projectile.velocity.ToRotation() + PiOver4 + rotArgs).ToRotationVector2() * 10f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, spawnPos, ProjectileType<DreamlessNightBeam>(), Projectile.damage, 1f, Owner.whoAmI);
                proj.penetrate = 4;
                proj.extraUpdates = 1;
                proj.timeLeft = 80;
                ((DreamlessNightBeam)proj.ModProjectile).BeamState = DreamlessNightBeam.BeamType.SplitBeam;
                for (int j = 0; j < 3; j++)
                    ShadowNebula.SpawnParticle(target.Center, RandVelTwoPi(0.2f, 1.2f) + spawnPos.ToSafeNormalize() * 1.1f, 0.15f, HJScarletTexture.Texture_WhiteCircle.Value);

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rotFixer = PiOver4;
            Projectile.DrawGlowEdge(Color.Purple, rotFix: rotFixer);
            Projectile.DrawProj(Color.Lerp(Color.White, Color.Black, Helper.GetAniProgress(0)), rotFix: rotFixer);
            return false;
        }
    }
}
