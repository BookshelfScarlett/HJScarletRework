using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static HJScarletRework.Projs.Executor.AetherfireSmasherName;

namespace HJScarletRework.Projs.Executor
{
    public class AetherfireSmasherProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<AetherfireSmasher>().Texture;
        public enum State
        {
            Shoot,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public bool ActiveDash = false;
        public bool ActiveMiscDashHit = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.ownerHitCheck = true;
            Projectile.width = Projectile.height = 60;
            Projectile.SetupImmnuity(30);
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDir() * 32f, ProjectileType<AetherfireSmasherExecution>(), Projectile.damage, 2f, Owner.whoAmI);
                proj.HJScarlet().ExecutionStrike = true;
                Projectile.Kill();
            }
            Projectile.velocity = Projectile.velocity.ToRandVelocity(ToRadians(10f), 18f, 23f);
        }
        public override void ProjAI()
        {
            switch (AttackState)
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
            Projectile.rotation += 0.2f * (Projectile.numHits < 1).ToInt();
            bool isHitNPC = Projectile.numHits > 0;
            if (ActiveDash)
            {
                OpacityGlow = Lerp(OpacityGlow, 1.1f, 0.22f);
                ScaleGlow = Lerp(ScaleGlow, 1.1f, 0.22f);
                UpdateHitParticle();
            }
            else
            {
                UpdateParticle();
            }
            //掷出一定时间再去考虑索敌的事情。
            if (Projectile.MeetMaxUpdatesFrame(Timer, 3f + ActiveMiscDashHit.ToInt() * 5f))
            {
                if (Projectile.GetTargetSafe(out NPC target, true, 1200, canPassWall: true) && Projectile.numHits < 1)
                {
                    if (!ActiveDash)
                    {
                        Projectile.velocity = (target.Center - Projectile.Center).ToSafeNormalize() * 18f;
                        InitActiveDashParticle();
                        SoundEngine.PlaySound(SoundID.Item69 with { MaxInstances = 0, Pitch = -0.4f, PitchVariance = 0.1f, Volume = 0.7f }, Projectile.Center);
                    }
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    ActiveDash = true;
                    Projectile.HomingTarget(target.Center, -1, 18f, 10f, 40f);
                }
                if (isHitNPC)
                {
                    Projectile.velocity *= 0.94f;
                    float rot = (Projectile.velocity).ToSafeNormalize().ToRotation();
                    Projectile.rotation = Projectile.rotation.AngleLerp(rot, 0.2f);
                }
            }
            else if (ActiveMiscDashHit)
            {
                Projectile.velocity *= 0.97f;
            }
            if (Projectile.MeetMaxUpdatesFrame(Timer, (18 + (Projectile.numHits > 0).ToInt() * 18)))
            {
                if (isHitNPC)
                {
                    SpawnFireball();
                    Projectile.velocity = (-(Projectile.Center - Owner.Center)).ToSafeNormalize() * 18f;
                    InitActiveDashParticle();
                    SoundEngine.PlaySound(SoundID.Item110 with { MaxInstances = 0, Pitch = -0.2f, PitchVariance = 0.1f }, Projectile.Center);
                }
                Projectile.netUpdate = true;
                Timer = 0;
                AttackState = State.Return;
            }
        }

        public void InitActiveDashParticle()
        {
                PickTagColor(out Color baseColor, out Color targetColor);
            PickTagDust(out short HigherDust, out short BottemDust);
            float numberOfDusts = 10f;
            float rotFactor = 360f / numberOfDusts;
            Vector2 spawnPos = Projectile.Center.ToRandCirclePos(10f);
            for (int j = 0; j < numberOfDusts; j++)
            {
                spawnPos = Projectile.Center.ToRandCirclePos(10f);
                float rot = ToRadians(j * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(2.4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                new ShinyOrbParticle(spawnPos + offset, velOffset - Projectile.SafeDir() * Main.rand.NextFloat(0.8f, 3.7f), RandLerpColor(baseColor, targetColor), 40, 0.8f).Spawn();
            }
            for (int j = 0; j < 10; j++)
            {
                    int dType = Main.rand.NextBool() ? HigherDust : BottemDust;
                Dust d = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2CircularEdge(10f, 10f), dType);
                d.velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(1.2f, 4.2f) - Projectile.SafeDir() * Main.rand.NextFloat(2.8f, 8.7f);
                d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                d.noGravity = true;
            }
            for (int j = 0; j < 10; j++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(4f), -Projectile.velocity.ToRandVelocity(ToRadians(30f), 0.8f, 9.4f), RandLerpColor(baseColor, Color.DarkGray), 40, RandRotTwoPi, 1f, 0.28f).SpawnToNonPreMult();
            }
        }

        public void SpawnFireball()
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePosEdge(4f), Projectile.velocity.ToRandVelocity(ToRadians(25f), 8f, 12f), ProjectileType<AetherfireSmasherFireball>(), Projectile.damage / 3, 1f, Owner.whoAmI);
            }
        }

        public void UpdateHitParticle()
        {
            if (Main.rand.NextBool(6))
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + Projectile.SafeDirByRot() * 5f;
                Vector2 dir = Projectile.SafeDir();
                new StarShape(spawnPos.ToRandCirclePos(4f), dir * Main.rand.NextFloat(1.4f, 2.7f), RandLerpColor(Color.DarkViolet, Color.Violet), 0.7f, 40).Spawn();
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + Projectile.SafeDirByRot() * 5f;
                Vector2 dir = Projectile.SafeDir();
                new ShinyCrossStar(spawnPos, dir * Main.rand.NextFloat(1.4f, 2.7f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, 0.4f, false).Spawn();
            }

            if (Main.rand.NextBool(8))
            {
                PickTagColor(out Color baseColor, out Color targetColor);
                Vector2 spawnPos = Projectile.SafeDirByRot().RotatedBy(Main.rand.NextBool().ToDirectionInt() * PiOver2) * Main.rand.NextFloat(05f, 30f) + Projectile.Center;
                new ShinyOrbParticle(spawnPos, Projectile.velocity / (Main.rand.NextFloat(7.1f, 9.2f)), RandLerpColor(baseColor, targetColor), 40, 0.65f * Main.rand.NextFloat(0.78f, 1.1f)).Spawn();
            }
        }

        public void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, -1, 20f, 10f);
            Projectile.rotation += 0.2f;
            OpacityGlow = Lerp(OpacityGlow, 0f, 0.12f);
            ScaleGlow = Lerp(ScaleGlow, 0f, 0.12f);
            if (Main.rand.NextBool(4))
                UpdateParticle();

            if (Projectile.IntersectOwnerByDistance())
            {
                Projectile.Kill();
            }
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            PickTagColor(out Color baseColor, out Color targetColor);
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30f);
                new ShinyOrbParticle(pos, (pos - Projectile.Center).ToSafeNormalize().RotatedBy(PiOver2) * 1.2f, RandLerpColor(baseColor, targetColor), 40, 0.8f).Spawn();
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(12f);
                new SmokeParticle(spawnPos, Projectile.velocity / 7f, RandLerpColor(baseColor, Color.DarkGray), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.137f, 0.21f)).Spawn();
            }

        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimePass(ItemType<AetherfireSmasher>());
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 1, Pitch = -0.5f, Volume = 0.78f }, Projectile.Center);
            PickTagColor(out Color baseColor, out Color targetColor);
            PickTagDust(out short HigherDust, out short BottemDust);
            if (AttackState == State.Shoot)
            {
                Projectile.velocity = Projectile.SafeDir() * 17f;
                Projectile.netUpdate = true;
                Vector2 spawnPos = target.Center;
                float numberOfDusts = 36f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                    Vector2 velOffset = new Vector2(2.4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                    new ShinyOrbParticle(spawnPos + offset, velOffset, RandLerpColor(baseColor, targetColor), 40, 0.8f).Spawn();
                }
                for (int i = 0; i < 30; i++)
                {
                    int dType = Main.rand.NextBool() ? HigherDust : BottemDust;
                    Dust d = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2CircularEdge(10f, 10f), dType);
                    d.velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(1.2f, 4.2f);
                    d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                    d.noGravity = true;
                }
            }
        }
        private void PickTagDust(out short HigherDust, out short BottemDust)
        {
            NameType pickType = Owner.name.SelectedName();
            switch (pickType)
            {
                //绯红书架 - 红
                case NameType.TrueScarlet:
                    HigherDust = DustID.CrimsonTorch;
                    BottemDust = DustID.RedTorch;
                    break;
                //雾梯/查 - 金
                case NameType.WutivOrChaLost:
                    BottemDust = DustID.GoldCoin;
                    HigherDust = DustID.YellowTorch;
                    break;
                //雪莉/安安/kino - 蓝， 溯月先暂时放在这里，后面考虑单独粒子
                case NameType.SherryOrAnnOrKino:
                    HigherDust = DustID.BlueTorch;
                    BottemDust = DustID.GemSapphire;
                    break;
                case NameType.Shizuku:
                    HigherDust = DustID.IceTorch;
                    BottemDust = DustID.WhiteTorch;
                    break;
                //锯角 - 紫
                case NameType.SerratAntler:
                    HigherDust = DustID.PurpleTorch;
                    BottemDust = DustID.WhiteTorch;
                    break;
                //樱羽艾玛 - 粉色
                case NameType.Emma:
                    HigherDust = DustID.PinkTorch;
                    BottemDust = DustID.WhiteTorch;
                    break;
                //神人漂浮女 - 绿
                case NameType.Hanna:
                    HigherDust = DustID.TerraBlade;
                    BottemDust = DustID.GreenTorch;
                    break;
                //其他 - 正常
                default:
                    HigherDust = DustID.OrangeTorch;
                    BottemDust = DustID.Torch;
                    break;
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.SelectedName())
            {
                case NameType.TrueScarlet:
                    baseColor = Color.LightCoral;
                    targetColor = Color.Crimson;
                    break;
                //查 -- 金
                case NameType.WutivOrChaLost:
                    baseColor = Color.LightGoldenrodYellow;
                    targetColor = Color.Yellow;
                    break;
                //银九 - 粉
                case NameType.Emma:
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                case NameType.SherryOrAnnOrKino:
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                case NameType.Shizuku:
                    baseColor = Color.LightSkyBlue;
                    targetColor = Color.AliceBlue;
                    break;

                //聚胶 - 紫
                case NameType.SerratAntler:
                    targetColor = Color.White;
                    baseColor = Color.DarkViolet;
                    break;
                case NameType.Hanna:
                    baseColor = Color.LimeGreen;
                    targetColor = Color.White;
                    break;
                default:
                    targetColor = Color.DarkRed;
                    baseColor = Color.OrangeRed;
                    break;
            }
        }
        public float OpacityGlow = 0f;
        public float ScaleGlow = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.SafeDirByRot() * 5f;
            Projectile.DrawProj(Color.White, drawTime: 2, rotFix: PiOver4);
            SB.EnterShaderArea();
            Texture2D kiraStar = HJScarletTexture.Particle_KiraStarGlow.Value;
            Texture2D ring = HJScarletTexture.Particle_RingHard.Value;
            Texture2D orbs = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            float opValue = Projectile.Opacity * OpacityGlow;
            float scaleValue = Projectile.scale * ScaleGlow;
            SB.Draw(ring, pos, null, Color.Purple * opValue, 0, ring.ToOrigin(), scaleValue * 0.45f, 0, 0);
            SB.Draw(orbs, pos, null, Color.DarkViolet * opValue, 0, orbs.ToOrigin(), scaleValue * 0.45f, 0, 0);
            SB.Draw(kiraStar, pos, null, Color.Violet * opValue, 0, kiraStar.ToOrigin(), scaleValue * 0.15f, 0, 0);
            SB.EndShaderArea();

            return false;
        }
    }
}
