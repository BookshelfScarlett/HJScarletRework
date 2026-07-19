using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace HJScarletRework.Projs.Executor
{
    public class PureYinyoExecution : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<PureYinyo>().Texture;
        public NPC CurTarget = null;
        public enum State
        {
            Shoot,
            Attack,
            Pumpout,
            ReStrike,
            BreakIntoShard,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;

        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(HJScarletSounds.DeathsToll_Toss with { MaxInstances = 0, Pitch = -.5f });
            Vector2 offset = Vector2.UnitY * -18f;
            for (int i = 0; i < 10; i++)
            {
                Vector2 rot = -Vector2.UnitY * Main.rand.NextFloat(4f, 8f);
                QuickYinyoSmokeWhite(Projectile.Center.ToRandCirclePosEdge(8, 10) + offset, rot, Main.rand.Next(30, 40), RandRotTwoPi, .751f, 0.90f * Main.rand.NextFloat(.7f, .9f));
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 rot = -Vector2.UnitY * Main.rand.NextFloat(4f, 8f);
                QuickYinyoSmokeBlack(Projectile.Center.ToRandCirclePosEdge(8, 10) + offset, rot, Main.rand.Next(30, 40), RandRotTwoPi, 1f, 0.95f * Main.rand.NextFloat(.7f, .9f));
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(3) + offset;
                Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Black, Color.DarkGray), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .64f, .15f, BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(3) + offset;
                Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .94f, .15f);
            }

        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Attack:
                    DoAttack();
                    break;
                case State.BreakIntoShard:
                    DoBreakIntoShard();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoBreakIntoShard()
        {
            Projectile.velocity *= .875f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (!Projectile.IsOutScreen() && Main.rand.NextFloat() > Clamp(Timer / 25f, 0, 1))
            {
                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(.7f, 3.2f);
                Vector2 pos = Projectile.ToRandRec();
                int lifeTime = Main.rand.Next(30, 70);

                if (Main.rand.NextBool())
                    ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), -Vector2.UnitY * Main.rand.NextFloat(0.8f, 4.2f), RandLerpColor(Color.Black, Color.DarkGray), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 0.9f) * .94f, .15f, BlendState.NonPremultiplied);
                else
                    ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), -Vector2.UnitY * Main.rand.NextFloat(0.8f, 4.2f), RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .94f, .15f);
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool())
                        QuickYinyoSmokeBlack(pos.ToRandCirclePosEdge(10), vel, lifeTime, RandRotTwoPi, scaleMult: .8f);
                    else
                        QuickYinyoSmokeWhite(pos.ToRandCirclePosEdge(10), vel, lifeTime, RandRotTwoPi, scaleMult: .8f);
                }
            }

            if (Projectile.MeetMaxUpdatesFrame(Timer, 25))
            {
                SoundEngine.PlaySound(HJScarletSounds.Frosthammer_SnowCharge with { MaxInstances = 0, Pitch = -.4f });
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 100, 120, Projectile.velocity.ToRotation(), 0, easingFunc: EaseOutExpo);
                for (int i = 0; i < 40; i++)
                {
                    Vector2 rot = Vector2.UnitX.RotatedBy(TwoPi * i / 40f);
                    QuickYinyoSmokeWhite(Projectile.Center.ToRandCirclePosEdge(8, 10), rot * 6f, Main.rand.Next(30, 40), RandRotTwoPi, .751f, 0.90f * Main.rand.NextFloat(.7f, .9f));
                }
                for (int i = 0; i < 40; i++)
                {
                    Vector2 rot = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0, Pi));
                    QuickYinyoSmokeBlack(Projectile.Center.ToRandCirclePosEdge(8, 10), RandVelTwoPi(0.1f, 4f), Main.rand.Next(30, 40), RandRotTwoPi, 1f, 0.95f * Main.rand.NextFloat(.7f, .9f));
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(16);
                    Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                    Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Black, Color.DarkGray), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .64f, .15f, BlendState.NonPremultiplied);
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(16);
                    Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                    Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .64f, .15f);
                }
                new CrossGlow(Projectile.Center, Color.White, 40, 1, 0.20f).Spawn();
                if (Projectile.IsMe())
                {
                    for (int i = 0; i < 21; i++)
                    {
                        float rot = Main.rand.NextFloat(ToRadians(15), ToRadians(180 - 15));
                        Vector2 vel = rot.ToRotationVector2() * -Main.rand.NextFloat(7f, 14f) * .7f;
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(8), vel, ProjectileType<PureYinyoJade>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        ((PureYinyoJade)proj.ModProjectile).BlackShard = false;
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        float rot = Main.rand.NextFloat(ToRadians(15), ToRadians(180 - 15));
                        Vector2 vel = rot.ToRotationVector2() * Main.rand.NextFloat(7f, 11f) * .7f;
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(8), vel, ProjectileType<PureYinyoJade>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        ((PureYinyoJade)proj.ModProjectile).BlackShard = true;

                    }
                }
                Projectile.Kill();
            }
        }

        public void DoReturn()
        {
            Projectile.rotation += .2f;
            Projectile.HomingTarget(Owner.Center, -1, 20, 20);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                Projectile.Kill();
        }


        public void DoAttack()
        {
            if (Projectile.numHits > 0)
                Projectile.rotation += .2f;
            else
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (CurTarget is null)
            {
                if (Projectile.GetTargetSafe(out NPC target, true))
                    CurTarget = target;
                else
                    SwitchNextState(State.Return);

            }
            else
            {
                if (CurTarget.IsLegal())
                    Projectile.HomingTarget(CurTarget.Center, -1, 20, 10);
                else
                    SwitchNextState(State.Return);
            }
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.numHits > 0)
            {

            }
            else
            {
                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(.7f, 1.2f);
                Vector2 pos = Projectile.ToRandRec();
                int lifeTime = Main.rand.Next(30, 70);
                if (Main.rand.NextBool())
                    ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), Projectile.velocity / 8f, RandLerpColor(Color.Black, Color.DarkGray), Main.rand.Next(30, 70), .45f, Main.rand.NextFloat(.7f, 1.1f) * .94f, .15f, BlendState.NonPremultiplied);
                else
                    ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), Projectile.velocity / 8f, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .94f, .15f);
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool())
                        QuickYinyoSmokeBlack(pos.ToRandCirclePosEdge(10), vel, lifeTime, RandRotTwoPi, scaleMult: .68f);
                    else
                        QuickYinyoSmokeWhite(pos.ToRandCirclePosEdge(10), vel, lifeTime, RandRotTwoPi, scaleMult: .68f);
                }
            }
        }

        public void DoShoot()
        {
            Projectile.extraUpdates = 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= .880f;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 24))
            {
                SwitchNextState(State.Attack);
                NPC target = Main.MouseWorld.FindClosestTarget(300, ignoreTiles: false);
                if (target.IsLegal())
                {
                    CurTarget = target;
                    Projectile.velocity = Projectile.Center.GetNormalVector2(CurTarget.Center) * 24f;
                    Projectile.extraUpdates = 2;
                }
                else
                {
                    if (Projectile.GetTargetSafe(out NPC target2, searchDistance: 1200f))
                    {
                        CurTarget = target2;
                        Projectile.velocity = Projectile.Center.GetNormalVector2(CurTarget.Center) * 24f;
                    }
                }
            }
            if (Projectile.IsOutScreen())
                return;
            Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(.7f, 4.2f);
            Vector2 pos = Projectile.ToRandRec();
            int lifeTime = Main.rand.Next(30, 70);
            if (Main.rand.NextBool(4))
            {
                if (Main.rand.NextBool())
                    ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), vel, RandLerpColor(Color.Black, Color.DarkGray), Main.rand.Next(30, 70), 0.4f, Main.rand.NextFloat(.7f, 1.1f) * .94f, .15f, BlendState.NonPremultiplied);
                else
                    ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePosEdge(16), vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .94f, .15f);
            }
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool())
                    QuickYinyoSmokeBlack(pos.ToRandCirclePosEdge(10), vel, lifeTime, RandRotTwoPi, opacityMult: .4f, scaleMult: .468f);
                else
                    QuickYinyoSmokeWhite(pos.ToRandCirclePosEdge(10), vel, lifeTime, RandRotTwoPi, scaleMult: .468f);
            }
        }
        public void SwitchNextState(State nextState)
        {
            Projectile.netUpdate = true;
            Timer = 0;
            switch (nextState)
            {
                case State.Attack:
                    break;
                case State.Pumpout:
                    AttackState = State.Pumpout;
                    Projectile.velocity = (Vector2.UnitY) * Main.rand.NextFloat(32f, 40f);
                    Projectile.tileCollide = false;
                    break;
                case State.ReStrike:
                    if (CurTarget.IsLegal())
                        Projectile.velocity = Projectile.Center.GetNormalVector2(CurTarget.Center) * 16f;
                    break;
                case State.BreakIntoShard:
                    break;
                case State.Return:
                    Projectile.tileCollide = false;
                    Projectile.timeLeft = GetSeconds(10);
                    Projectile.penetrate = -1;
                    break;
            }
            AttackState = nextState;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if ((AttackState == State.Attack || AttackState == State.ReStrike) && CurTarget.IsLegal() && CurTarget.Equals(target))
                return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Attack)
            {
                if (target.IsLegal())
                    CurTarget = target;
                Projectile.velocity *= .01f;
                SetHitParticle(target.Center);
                if (Projectile.numHits < 16)
                {
                    SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { Variants = [1], MaxInstances = 0, Pitch = -.8f, Volume = .60f });
                    return;
                }
                SwitchNextState(State.BreakIntoShard);
                Projectile.velocity = (-Vector2.UnitY) * Main.rand.NextFloat(43f, 48f);
                Projectile.rotation = Projectile.velocity.ToRotation();
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30, 120, Projectile.rotation, 0, easingFunc: EaseOutExpo);
                SoundEngine.PlaySound(HJScarletSounds.TheMars_Toss with { MaxInstances = 0, Pitch = -.4f, Volume = .85f });
                SoundEngine.PlaySound(HJScarletSounds.Smash_GroundHeavy with { MaxInstances = 0, Pitch = -.70f, Volume = .65f });
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos = target.Center.ToRandCirclePos(3);
                    Vector2 vel = Projectile.SafeDir().ToRandVelocity(ToRadians(24f), 0.3f, 17f) * 1.2f;
                    int lifeTime = Main.rand.Next(30, 70);
                    float rot = RandRotTwoPi;
                    QuickYinyoSmokeBlack(pos, vel, lifeTime, rot, scaleMult: Main.rand.NextFloat(.7f, 1.1f));
                }
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos = target.Center.ToRandCirclePos(3);
                    Vector2 vel = Projectile.SafeDir().ToRandVelocity(ToRadians(16f), 0.3f, 14f) * 1.2f;
                    int lifeTime = Main.rand.Next(30, 70);
                    float rot = RandRotTwoPi;
                    QuickYinyoSmokeWhite(pos, vel, lifeTime, rot, scaleMult: Main.rand.NextFloat(.7f, 1.1f));
                }
            }
        }
        private void SetHitParticle(Vector2 center)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(.7f, 6.4f);
                if (Main.rand.NextBool())
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Black, Color.DarkGray), Main.rand.Next(30, 70), .75f, Main.rand.NextFloat(.7f, 1.1f) * .8f, .15f, BlendState.NonPremultiplied);
                else
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .8f, .15f);
            }
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                    Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                    QuickYinyoSmokeBlack(pos, dir * Main.rand.NextFloat(1.7f, 2.8f), 40, RandRotTwoPi, Main.rand.NextFloat(.75f, 1f));
                }
                else
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                    Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                    QuickYinyoSmokeWhite(pos, dir * Main.rand.NextFloat(3.8f, 4.8f), 40, RandRotTwoPi, Main.rand.NextFloat(.75f, 1f));

                }
            }
        }

        public void QuickYinyoSmokeWhite(Vector2 pos, Vector2 vel, int lifeTime, float rot, float opacityMult = 1f, float scaleMult = 1f)
        {
            new SmokeParticle(pos, vel, Color.Black, lifeTime, rot, .48f * opacityMult, 0.35f * scaleMult, true).SpawnToPriorityNonPreMult();
            new SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.Lerp(Color.White, Color.LightGray, .019f)), lifeTime, rot, 1f * opacityMult, 0.3f * scaleMult, true).Spawn();
        }
        public void QuickYinyoSmokeBlack(Vector2 pos, Vector2 vel, int lifeTime, float rot, float opacityMult = 1f, float scaleMult = 1f)
        {
            new SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .8f * opacityMult, scaleMult * .35f, true).SpawnToPriorityNonPreMult();
            new SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.DarkViolet, .10f)), lifeTime, rot, 1f * opacityMult, 0.3f * scaleMult, true).SpawnToPriorityNonPreMult();
        }
        public override bool ShouldUpdatePosition()
        {
            return true;
        }
        public float Opac = 0;
        public float CrossGlowOpac = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D yinyo = TextureAssets.Projectile[ProjectileType<BookOfBalance_3>()].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Opac = AttackState == State.BreakIntoShard ? Lerp(Opac, 1f, 0.2f) : Lerp(Opac, 0f, 0.12f);
            CrossGlowOpac = AttackState == State.Attack ? Lerp(CrossGlowOpac, 1, .2f) : Lerp(CrossGlowOpac, 0, .2f);
            if (Opac > 0.02f)
            {
                for (int i = 0; i < 16; i++)
                {
                    SB.Draw(yinyo, drawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * Opac, Main.GlobalTimeWrappedHourly, yinyo.ToOrigin(), Projectile.scale * .65f * Opac, 0, 0);
                }
                SB.Draw(yinyo, drawPos, null, Color.White * Opac, Main.GlobalTimeWrappedHourly, yinyo.ToOrigin(), Projectile.scale * .65f * Opac, 0, 0);
            }
            Projectile.DrawGlowEdge(Color.White, rotFix: PiOver4);
            Projectile.DrawProj(Color.White, rotFix: PiOver4);
            if (CrossGlowOpac > .02f)
            {
                Texture2D cross = HJScarletTexture.Particle_KiraStarGlow.Value;
                SB.EnterShaderArea();
                SB.Draw(cross, drawPos, null, Color.WhiteSmoke * CrossGlowOpac, 0, cross.ToOrigin(), Projectile.scale * .25f, 0, 0);
                SB.EndShaderArea();
            }

            return false;
        }
    }
}
