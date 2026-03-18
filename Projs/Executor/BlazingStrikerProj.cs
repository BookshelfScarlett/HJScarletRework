using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class BlazingStrikerProj : HJScarletProj
    {
        public override string Texture => GetInstance<BlazingStriker>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public enum State
        {
            Shoot,
            Hit,
            ReadyHeavyHit,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public bool IsHit = false;
        public NPC CurTarget = null;
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = GetSeconds(10);
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            bool hit = AttackType == State.ReadyHeavyHit && target.Equals(CurTarget) || AttackType != State.ReadyHeavyHit;
            if (hit)
                return null;
            return false;
        }
        public override void OnFirstFrame()
        {
        }

        public override void ProjAI()
        {
            UpdateAttack();
            UpdateParticle();
        }

        private void UpdateAttack()
        {
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            switch (AttackType)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Hit:
                    DoHit();
                    break;
                case State.Return:
                    DoReturn();
                    break;
                case State.ReadyHeavyHit:
                    DoReadyHeavyHit();
                    break;
            }
        }


        private void DoShoot()
        {
            Projectile.rotation += 0.2f;
            Timer++;
            //这里会给一个ishit的判定。
            //如果第一次过来没有找到敌人的话，我们就直接选择返程
            if (Projectile.MeetMaxUpdatesFrame(Timer, 12) && !IsHit)
            {
                UpdateToNextAttack(State.Return);
            }
        }
        private void DoHit()
        {
            if (Projectile.MeetMaxUpdatesFrame(Timer, 27))
            {
                UpdateToNextAttack(State.ReadyHeavyHit);
            }
            else
            {
                Projectile.velocity *= 0.96f;
                Projectile.velocity.Y *= 1f;
                if (Projectile.velocity.Y < 30f)
                    Projectile.velocity.Y += 0.37f;
                Projectile.rotation = Projectile.SpeedAffectRotation(2, 2);
                Timer++;

            }
        }

        private void DoReturn()
        {

            bool buffer = Projectile.MeetMaxUpdatesFrame(Timer, 10f) && !IsHit;
            if (!buffer)
                Timer++;

            Projectile.scale = Lerp(Projectile.scale, 1f, 0.1f);
            Projectile.rotation += 0.2f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.HomingTarget(Owner.Center, -1, 16f, 20f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                if (!Owner.HasProj<BlazingStrikerFocusProj>(out int projID))
                {
                    if (Projectile.HJScarlet().AddFocusHit)
                        Owner.HJScarlet().FocusStrikeTime += 1;
                    if (Projectile.HJScarlet().FocusStrike)
                        ShootFocusProj(projID);
                }
                Projectile.Kill();
            }
        }
        private void ShootFocusProj(int projID)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
            Vector2 dir = Projectile.velocity.ToSafeNormalize();
            Vector2 vel = dir.RotatedBy(Main.rand.NextFloat(ToRadians(30f), ToRadians(60f)) * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(22f, 26f);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 12f, 40, vel.ToRotation(), ToRadians(30f));
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, vel, projID, Projectile.damage, 2f, Owner.whoAmI);
            proj.HJScarlet().FocusStrike = true;
            proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
        }

        private void DoReadyHeavyHit()
        {
            if (CurTarget != null && CurTarget.CanBeChasedBy())
            {
                //距离足够进直接修正这个速度
                if (Vector2.DistanceSquared(Projectile.Center, CurTarget.Center) < 70f * 70f)
                {
                    Projectile.velocity = (CurTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (20f);
                    Projectile.rotation = Projectile.velocity.ToRotation() - PiOver4;
                }
                else
                {
                    float angleOffset = WrapAngle(Projectile.AngleTo(CurTarget.Center) - Projectile.velocity.ToRotation());
                    angleOffset = Clamp(angleOffset, -0.2f, 0.2f);
                    Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset);
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() - PiOver4, 0.08f);
                    Projectile.scale = Lerp(Projectile.scale, 1.20f, 0.25f);
                    //控速
                    if (Projectile.velocity.Length() < 22f)
                        Projectile.velocity *= 1.15f;
                    else
                        Projectile.velocity *= 0.9f;
                }
            }
            else
            {
                //主要是安全性检查，找不到目标就退回去
                AttackType = State.Return;
            }
        }
        private void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            bool isState = Owner.HasProj<BlazingStrikerFocusProj>() && Main.rand.NextBool() && (AttackType == State.Hit || AttackType == State.ReadyHeavyHit);
            if (isState)
                return;
            if (Projectile.FinalUpdateNextBool())
                new SmokeParticle(Projectile.Center.ToRandCirclePos(30), -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 1.8f), RandLerpColor(Color.OrangeRed, Color.DarkGray), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.4f, 0.8f) * 0.32f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Projectile.FinalUpdateNextBool())
                new SmokeParticle(Projectile.Center.ToRandCirclePos(15), -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 1.8f), RandLerpColor(Color.OrangeRed, Color.DarkGray), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.4f, 0.8f) * 0.22f, Main.rand.NextBool()).SpawnToNonPreMult();
            if (Projectile.FinalUpdateNextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(30), DustID.Torch);
                d.velocity = -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 1.8f);
                d.noGravity = false;
            }
            if (Projectile.FinalUpdateNextBool(3) && Projectile.HJScarlet().FocusStrike)
                new EmptyRing(Projectile.Center.ToRandCirclePos(15), -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.2f, 1.8f), RandLerpColor(Color.OrangeRed, Color.DarkOrange), 40, Main.rand.NextFloat(0.5f, 0.8f) * 0.30f, 1f, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackType == State.Shoot)
            {
                OnTileCollideParticle(oldVelocity);
                AttackType = State.Return;
                Projectile.netUpdate = true;
                Timer = 0;
            }
            return false;
        }
        public void OnTileCollideParticle(Vector2 velo)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
            Vector2 vel = velo.ToSafeNormalize();
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 8.8f) * 1.2f;
                new SmokeParticle(pos, dVel, RandLerpColor(Color.DarkRed, Color.DarkGray), 40, RandRotTwoPi, 1f, 0.20f * Main.rand.NextFloat(0.50f, 1.1f), RandBoolen()).SpawnToNonPreMult();
            }
            for (int i = 0; i < 8; i++)
            {

                Vector2 pos = Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(32, 32)));
                Vector2 dVel = vel * Main.rand.NextFloat(4.4f, 4.8f) * 1.2f;
                new ShinyOrbHard(pos, dVel, RandLerpColor(Color.OrangeRed, Color.Orange), 40, Main.rand.NextFloat(0.4f, 0.8f)).SpawnToNonPreMult();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (AttackType != State.Hit)
                Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;

            switch (AttackType)
            {
                case State.Shoot:
                    IsHit = true;
                    UpdateToNextAttack(State.Hit);
                    CurTarget = target;
                    //从这里弹开。
                    Projectile.velocity = (Projectile.SafeDir() - Vector2.UnitY.RotatedBy(ToRadians(30) * -Projectile.direction) * 10f).ToSafeNormalize() * Projectile.velocity.Length() * 1f;
                    if (Projectile.HJScarlet().FocusStrike)
                        SoundEngine.PlaySound(SoundID.Item69 with { Pitch = -0.2f }, Projectile.Center);
                    else
                        SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
                    UpdateShootHitParticle();
                    break;

                case State.Return:
                    bool buffer = Projectile.MeetMaxUpdatesFrame(Timer, 10f);
                    if (!buffer && !IsHit)
                    {
                        IsHit = true;
                        UpdateToNextAttack(State.Hit);
                        CurTarget = target;
                        Projectile.velocity = (Projectile.SafeDir() - Vector2.UnitY.RotatedBy(ToRadians(30) * -Projectile.direction) * 10f).ToSafeNormalize() * 20 * 1f;
                        if (Projectile.HJScarlet().FocusStrike)
                            SoundEngine.PlaySound(SoundID.Item69 with { Pitch = -0.2f }, Projectile.Center);
                        else
                            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 0, Pitch = -0.2f }, Projectile.Center);
                        UpdateShootHitParticle();
                    }
                    break;

                case State.ReadyHeavyHit:
                    UpdateToNextAttack(State.Return);
                    //这里会简单过一个检查。
                    //有挂载锤的情况下我们不会做额外的特效和震屏，避免枪走挂载锤的风头
                    bool directReturn = CurTarget == null || Owner.HasProj<BlazingStrikerFocusProj>();
                    float pitch = Owner.HJScarlet().FocusStrikeTime * 0.1f;
                    if (Projectile.HJScarlet().FocusStrike)
                        pitch = GetInstance<BlazingStriker>().FocusStrikeTime * 0.1f;

                    if (!directReturn)
                    {
                        ScreenShakeSystem.AddScreenShakes(target.Center, 10f, 12, Projectile.velocity.ToRotation(), ToRadians(20f));
                        UpdateMiscHitParticle(target);
                        SoundStyle pickSound = HJScarletSounds.Smash_GroundHeavy with { MaxInstances = 3, Pitch = -0.2f, PitchVariance = 0.2f, Volume = 0.80f };
                        SoundEngine.PlaySound(pickSound, Projectile.Center);
                    }
                    else
                    {
                        ScreenShakeSystem.AddScreenShakes(target.Center, 5f, 32, Projectile.velocity.ToRotation(), ToRadians(20f));
                        SoundStyle pickSound = HJScarletSounds.Smash_GroundHeavy with { MaxInstances = 3, Pitch = -0.2f, PitchVariance = 0.2f, Volume = 0.80f };
                        SoundEngine.PlaySound(pickSound, Projectile.Center);
                        UpdateMiscHitParticle(target);
                    }
                    break;

                default:
                    SoundEngine.PlaySound(SoundID.Item69 with { MaxInstances = 4 }, Projectile.Center);
                    break;
            }
        }

        private void UpdateToNextAttack(State id)
        {
            Timer *= 0;
            AttackType = id;
            Projectile.netUpdate = true;
        }
        private void UpdateShootHitParticle()
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 vec = Projectile.velocity.ToRandVelocity(ToRadians(30f), 1.8f, 16.8f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Ash);
                d.velocity = vec * 1.2f;
                d.scale *= Main.rand.NextFloat(1f, 1.3f);
                d.noGravity = true;
            }
            for (int i = 0; i < 8; i++)
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity.ToRandVelocity(ToRadians(25f), 4.8f, 14f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), 0.2f).Spawn();
            }
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(8f), Main.rand.NextBool() ? DustID.Torch : DustID.YellowTorch);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(25f), 4.8f, 16f);
                d.scale *= Main.rand.NextFloat(1.2f, 1.8f) * 2f;
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(10f), Projectile.velocity.ToRandVelocity(ToRadians(20f), 4.7f, 18f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            }

        }
        private void UpdateMiscHitParticle(NPC target)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 vec = Projectile.velocity.ToRandVelocity(ToRadians(30f), 1.8f, 16.8f);
                Dust d = Dust.NewDustPerfect(target.Center, DustID.Ash);
                d.velocity = vec * 1.2f;
                d.scale *= Main.rand.NextFloat(1f, 1.3f);
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                new ShinyCrossStar(target.Center.ToRandCirclePos(8f), Projectile.velocity.ToRandVelocity(ToRadians(25f), 4.8f, 14f), RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), 0.2f).Spawn();
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(target.Center.ToRandCirclePos(8f), Main.rand.NextBool() ? DustID.Torch : DustID.YellowTorch);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(25f), 4.8f, 16f);
                d.scale *= Main.rand.NextFloat(1.2f, 1.8f) * 2f;
                d.noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                new SmokeParticle(target.Center.ToRandCirclePos(10f), Projectile.velocity.ToRandVelocity(ToRadians(20f), 4.7f, 18f), RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f, Main.rand.NextBool()).SpawnToPriorityNonPreMult();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteEffects se = Projectile.direction > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float ratios = ((float)i / (Projectile.oldPos.Length - 1));
                Color lerpColor = Color.Lerp(Color.White, Color.Orange, ratios);
                SB.Draw(tex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, lerpColor * (1 - ratios), Projectile.oldRot[i], tex.ToOrigin(), Projectile.scale, se, 0);
            }
            return false;
        }
    }
}
