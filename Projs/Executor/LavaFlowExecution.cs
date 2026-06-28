using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class LavaFlowExecution : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<LavaFlowProj>().Texture;
        public Vector2 PosOffsetFix => Projectile.SafeDirByRot() * 20f;
        public enum State
        {
            Attack,
            Stab
        }
        public NPC StabTarget = null;
        public Vector2 StoredPosition = Vector2.Zero;
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { MaxInstances = 0, Pitch = .25f });
            SoundEngine.PlaySound(SoundID.Item64 with { MaxInstances = 0, Pitch = .55f });
        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Attack:
                    DoAttack();
                    break;
                case State.Stab:
                    DoStab();
                    break;
            }
        }

        public void DoStab()
        {
            if (StabTarget.IsLegal())
            {
                StabTarget.HJScarlet().isBeingStabByLavaFlow = true;
                //不舍去这个size/2会导致中心点不在贴图上，很奇怪
                Projectile.Center = StabTarget.Center + StoredPosition;
                Vector2 dir = -Projectile.SafeDirByRot();
                Vector2 offset = Projectile.SafeDirByRot() * 35f * Projectile.scale;
                if (Main.rand.NextBool(4))
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(15) + offset, dir * Main.rand.NextFloat() * 12f, RandLerpColor(Color.OrangeRed, Color.Orange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .36f, .12f);
                if (Main.rand.NextBool(3))
                    ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(15) + offset, dir * Main.rand.NextFloat() * 12f, RandLerpColor(Color.OrangeRed, Color.DarkOrange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .1f, .60f);
                if (Main.rand.NextBool(5))
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(15) + offset, dir * Main.rand.NextFloat() * 12f, RandLerpColor(RandLerpColor(Color.OrangeRed, Color.Orange), Color.LightYellow), Main.rand.Next(60, 75), RandRotTwoPi, 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .25f, true, BlendState.Additive);
            }
            else
            {
                Projectile.Kill();
            }
        }

        public void DoAttack()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            Vector2 offset = Projectile.SafeDir() * 15f;
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4) - offset, Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.Orange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .46f, .12f);
            if (Main.rand.NextBool())
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePosEdge(6) - offset, Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.DarkOrange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .1f, .60f);
            if (Main.rand.NextBool())
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6) - offset, Projectile.velocity / 8f, RandLerpColor(RandLerpColor(Color.OrangeRed, Color.Orange), Color.LightYellow), Main.rand.Next(60, 75), RandRotTwoPi, 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .25f, true, BlendState.Additive);
            for (int i = -1; i < 2; i += 2)
            {
                for (int k = 0; k < 2; k++)
                    ECSParticle.HRShinyOrb(Projectile.Center - Projectile.SafeDir() * k * 10f + Projectile.SafeDir().RotatedBy(PiOver2 * i) * 8f, Projectile.SafeDir().RotatedBy(PiOver2 * i) * 1.1f, RandLerpColor(Color.OrangeRed, Color.DarkOrange), Main.rand.Next(30, 45), 1f, Main.rand.NextFloat(.7f, .9f) * Projectile.scale * .1f, .60f);
            }

        }

        public override void OnKill(int timeLeft)
        {
            if (!Fireball)
                return;
            for (int i = 0; i < 46; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(16);
                ECSParticle.StarShape(pos, Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(0.3f, 1f) * 10f, RandLerpColor(Color.Orange, Color.OrangeRed), Main.rand.Next(0, 55), 1, 0.8f * Main.rand.NextFloat(.7f, 1.1f), .89f, BlendState.Additive);
            }
            for (int i = 0; i < 4; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY.ToRandVelocity(ToRadians(10f), 9f, 13f), ProjectileType<LavaFlowBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { MaxInstances = 0, Pitch = .65f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 0, Pitch = .65f }, Projectile.Center);

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.IsLegal() && AttackState == State.Attack && !target.HJScarlet().isBeingStabByLavaFlow)
            {
                StabTarget = target;
                Projectile.rotation = Projectile.SafeDir().ToRotation();
                Projectile.velocity *= 0f;
                StoredPosition = Projectile.Center - target.Center;
                //简化一下特效的循环
                Projectile.localNPCHitCooldown = 30 * Projectile.MaxUpdates;
                Projectile.timeLeft = GetSeconds(13) * Projectile.MaxUpdates + Projectile.MaxUpdates * 10;
                AttackState = State.Stab;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { MaxInstances = 0, Pitch = .65f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 0, Pitch = .65f }, Projectile.Center);
                return;
            }
            if (target.HJScarlet().isBeingStabByLavaFlow && AttackState == State.Attack)
            {
                Fireball = true;
                Projectile.timeLeft = 2;
            }
        }
        public bool Fireball = false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            Vector2 offsetValue = PosOffsetFix;
            int drawLength = Projectile.oldPos.Length;
            if (AttackState == State.Attack)
            {
                for (int i = drawLength - 3; i >= 0; i--)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                        continue;
                    Vector2 trailingDrawPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.05f) + Projectile.PosToCenter() - offsetValue;
                    float faded = 1 - i / (float)drawLength;
                    //平方放缩
                    faded = MathF.Pow(faded, 2);
                    Color trailColor = Color.Lerp(Color.OrangeRed, Color.Lerp(Color.Orange, Color.White, 0.9f), faded) * 0.70f;
                    float opa = Lerp(0.85f, 1f, faded);
                    trailColor = trailColor.ToAddColor((byte)(Lerp(0, 0, faded)));
                    float scaleMult = Lerp(0.5f, .95f, faded);
                    SB.Draw(tex, trailingDrawPos, null, trailColor, Projectile.oldRot[i] + PiOver4, orig, Projectile.scale * scaleMult, 0, 0);
                    SB.Draw(tex, trailingDrawPos, null, trailColor, Projectile.oldRot[i] + PiOver4, orig, Projectile.scale * scaleMult, 0, 0);
                }
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + (TwoPi / 8 * i).ToRotationVector2() * 1.2f - offsetValue, null, Color.White.ToAddColor(), Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            SB.Draw(projTex, drawPos - offsetValue, null, Color.White, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
