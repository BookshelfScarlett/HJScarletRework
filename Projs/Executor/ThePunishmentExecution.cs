using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
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

namespace HJScarletRework.Projs.Executor
{
    public class ThePunishmentExecution : HJScarletProj
    {
        public override string Texture => GetInstance<ThePunishmentProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public NPC TargetNPC = null;
        public AnimationStruct Helper = new(3);
        public float Oscillation = 0;
        public float ColorLerpValue = 0;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            //15秒，大概30次？
            Projectile.SetupImmnuity(30, ImmnuityType.Static);
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            ColorLerpValue = Main.rand.NextFloat(Pi);
            Helper.MaxProgress[0] = 50;
            Helper.MaxProgress[1] = GetSeconds(15);
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.timeLeft = 2;
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.rotation = Projectile.SpeedAffectRotation();
                Projectile.velocity *= 0.90f;
                return;
            }
            Oscillation += ToRadians(2.3f);
            ColorLerpValue += ToRadians(1.5f);

            if (!Helper.IsDone[1])
            {
                if (Projectile.GetTargetSafe(out NPC target))
                {
                    Projectile.rotation += 0.2f;
                    if (Projectile.FinalUpdate())
                    {
                        Helper.UpdateAniState(1);
                        Timer++;
                    }
                    TargetNPC = target;
                    Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.HallowedWeapons : DustID.GemDiamond, Projectile.velocity * 0.25f + Main.rand.NextVector2CircularEdge(5f, 5f) * 1.2f);
                    d.noGravity = true;
                    Projectile.HomingTarget(target.Center, -1, 18f, 12f);

                }
                else
                {
                    Projectile.velocity *= 0;
                    UpdateIfNoTargetNearby();
                    TargetNPC = null;
                }
            }
            else
            {
                SpawnOnKillParticle();
                SoundEngine.PlaySound(HJScarletSounds.Dream_Toss with { MaxInstances = 0, Pitch = 0.5f },Projectile.Center);
                Projectile.Kill();
            }
        }

        public void SpawnOnKillParticle()
        {
            for (int i = 0; i < 24; i++)
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(7f), RandVelTwoPi(0f, 8f), RandLerpColor(Color.Orange, Color.Goldenrod), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.34f, 0.45f), false).Spawn();
            for (int i = 0; i < 6; i++)
                new KiraStar(Projectile.Center.ToRandCirclePosEdge(4f), RandVelTwoPi(1f, 3f), RandLerpColor(Color.DarkOrange, Color.Goldenrod), 40, 0, 1, 0.34f).Spawn();
        }

        public void UpdateIfNoTargetNearby()
        {
            //基本的挂机状态，此处使用了正弦曲线
            float mountedPosX = Owner.MountedCenter.X - 65f * Owner.direction;
            float mountedPosY = Owner.MountedCenter.Y + 100f * (MathF.Sin(Oscillation) / 9f);
            Vector2 anchorPos = new Vector2(mountedPosX, mountedPosY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.1f / Projectile.extraUpdates);
            float angleToWhat = ToRadians(120f);
            if (Owner.direction < 0)
                angleToWhat = ToRadians(60f);
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.1f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return TargetNPC != null && TargetNPC.Equals(target);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = 0.15f, PitchVariance = 0.15f }, Projectile.Center);

            if(Projectile.numHits % 2 == 0)
                SpawnPunishmentStar(target);
            else
            {
                for(int i = 0;i<16;i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(5f), Main.rand.NextBool() ? DustID.GemDiamond : DustID.HallowedWeapons);
                    d.velocity = RandVelTwoPi(1.2f, 4.6f);
                    d.scale *= Main.rand.NextFloat(0.8f, 1.54f);
                    d.noGravity = true;
                }
            }
        }

        public void SpawnPunishmentStar(NPC target)
        {
            //生成两个。随机位置。
            SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 0, Pitch = 0.35f, PitchVariance = 0.15f }, Projectile.Center);
            Vector2 vel = RandVelTwoPi(38f, 41f);
            for (int i = -1; i < 2; i += 2)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel * i, ProjectileType<ThePunishmentStar>(), Projectile.damage, 1f, Owner.whoAmI);
                ((ThePunishmentStar)proj.ModProjectile).TargetNPC = target;
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        new StarShape(Projectile.Center.ToRandCirclePosEdge(3), vel.ToRandVelocity(0, -1f, 8f).RotatedBy(k * PiOver2) * i, RandLerpColor(Color.Orange, Color.Goldenrod), 0.8f, 40).Spawn();
                        new StarShape(Projectile.Center.ToRandCirclePosEdge(3), vel.ToRandVelocity(0, -1f, 4.5f).RotatedBy(PiOver4 + k * PiOver2) * i, RandLerpColor(Color.Orange, Color.Goldenrod), 0.8f, 40).Spawn();
                    }
                }
            }
            for (int j = 0; j < 60; j++)
            {
                Vector2 rotArg = ToRadians(360f / 60f * j).ToRotationVector2();
                new ShinyCrossStar(Projectile.Center + rotArg * 30f, rotArg * 1.2f, RandLerpColor(Color.DarkOrange, Color.DarkGoldenrod), 40, rotArg.ToRotation(), 1f, 0.4f, false).Spawn();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color lerpValue = Color.Lerp(Color.Orange, Color.LightGoldenrodYellow, MathF.Sin(ColorLerpValue));
            float lerpValue2 = Math.Abs(MathF.Sin(ColorLerpValue));
            Projectile.DrawGlowEdge(lerpValue, rotFix: +PiOver4, posMove : 2 + 1.2f* lerpValue2);
            Projectile.DrawProj(lightColor, rotFix: +PiOver4);

            return false;
        }
    }
}
