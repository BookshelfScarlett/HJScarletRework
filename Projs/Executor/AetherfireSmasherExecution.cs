using ContinentOfJourney.Backgrounds;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static HJScarletRework.Projs.Executor.AetherfireSmasherName;

namespace HJScarletRework.Projs.Executor
{
    public class AetherfireSmasherExecution : HJScarletProj
    {
        public override string Texture => GetInstance<AetherfireSmasherProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public NPC LockTarget = null;
        public bool CanLock = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(30);
            Projectile.timeLeft = 2100;
        }
        public float OpacityGlow = 0;
        public float ScaleGlow = 0;
        public override void OnFirstFrame()
        {
            Projectile.velocity = Projectile.SafeDir() * 30f;
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            bool hit = Projectile.numHits > 0;
            OpacityGlow = Lerp(OpacityGlow, 1.1f, 0.22f);
            ScaleGlow = Lerp(ScaleGlow, 1.1f, 0.22f);
            UpdateParticle();
            Timer++;
            if (hit)
            {
                Projectile.rotation += .3f;
                if (LockTarget.IsLegal() && Projectile.numHits < 38)
                {
                    Projectile.velocity *= 0.60f;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, LockTarget.Center, 0.2f);
                }
                else
                    Projectile.Kill();
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                bool hasTarget = Projectile.GetTargetSafe(out NPC target, true, 1200f, true);
                if (Projectile.MeetMaxUpdatesFrame(Timer, 15))
                {
                    if (hasTarget)
                    {
                        if (!CanLock)
                        {
                            InitActiveDashParticle();
                            
                            Projectile.velocity = (target.Center - Projectile.Center).ToSafeNormalize() * 20f;
                            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30, 40, Projectile.velocity.ToRotation(), ToRadians(10f));
                            SoundEngine.PlaySound(HJScarletSounds.Misc_GunHit with { MaxInstances = 0 }, Projectile.Center);
                        }
                        UpdateHitParticle();
                        CanLock = true;
                        Projectile.HomingTarget(target.Center, -1, 18f, 10f);
                    }
                    else
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                else
                {
                    Projectile.velocity *= 0.89f;
                }
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

        public void InitActiveDashParticle()
        {
            float numberOfDusts = 10f;
            float rotFactor = 360f / numberOfDusts;
            Vector2 spawnPos = Projectile.Center.ToRandCirclePos(10f);
            PickTagColor(out Color baseColor, out Color targetColor);
            PickTagDust(out short HigherDust, out short BottemDust);
            for (int j = 0; j < numberOfDusts; j++)
            {
                spawnPos = Projectile.Center.ToRandCirclePos(10f);
                float rot = ToRadians(j * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(2.4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                new ShinyOrbParticle(spawnPos + offset, velOffset , RandLerpColor(baseColor, targetColor), 40, 0.8f).Spawn();
            }
            for (int j = 0; j < 10; j++)
            {
                int dType = Main.rand.NextBool() ? HigherDust : BottemDust;
                Dust d = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2CircularEdge(10f, 10f), dType);
                d.velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(1.2f, 4.2f);
                d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                d.noGravity = true;
            }
            for (int j = 0; j < 10; j++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi( 0.8f, 9.4f), RandLerpColor(baseColor, Color.DarkGray), 40, RandRotTwoPi, 1f, 0.28f).SpawnToNonPreMult();
            }
        }

        public void UpdateHitParticle()
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            if (Main.rand.NextBool(8))
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + Projectile.SafeDirByRot() * 5f;
                Vector2 dir = -Projectile.SafeDir();
                new StarShape(spawnPos.ToRandCirclePos(4f), dir * Main.rand.NextFloat(1.4f, 2.7f), RandLerpColor(Color.DarkViolet, Color.Violet), 0.7f, 40).Spawn();
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + Projectile.SafeDirByRot() * 5f;
                Vector2 dir = -Projectile.SafeDir();
                new ShinyCrossStar(spawnPos, dir * Main.rand.NextFloat(1.4f, 2.7f), RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, 0.4f, false).Spawn();
            }

            if (Main.rand.NextBool(8))
            {
                Vector2 spawnPos = Projectile.SafeDirByRot().RotatedBy(Main.rand.NextBool().ToDirectionInt() * PiOver2) * Main.rand.NextFloat(05f, 30f) + Projectile.Center;
                new ShinyOrbParticle(spawnPos, Projectile.velocity / (Main.rand.NextFloat(7.1f, 9.2f)), RandLerpColor(baseColor, targetColor), 40, 0.65f * Main.rand.NextFloat(0.78f, 1.1f)).Spawn();
            }
        }


        public override void OnKill(int timeLeft)
        {
            float numberOfDusts = 10f;
            float rotFactor = 360f / numberOfDusts;
            Vector2 spawnPos = Projectile.Center.ToRandCirclePos(10f);
            PickTagColor(out Color baseColor, out Color targetColor);
            PickTagDust(out short HigherDust, out short BottemDust);
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (LockTarget is null && target.IsLegal())
                LockTarget = target;
            PickTagColor(out Color baseColor, out Color targetColor);
            PickTagDust(out short HigherDust, out short BottemDust);
            SoundEngine.PlaySound(SoundID.Item89 with { MaxInstances = 0, Pitch = 0.7f, PitchVariance = 0.1f }, Projectile.Center);
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
            if (Projectile.numHits % 8 == 0)
            {
                Vector2 center = new Vector2(target.Center.X, target.Center.Y + 30f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, Vector2.Zero, ProjectileType<AetherfireSmasherVolcano>(), Projectile.damage * 2, Projectile.knockBack);

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Projectile.DrawProj(Color.White, 2, rotFix: PiOver4);
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
