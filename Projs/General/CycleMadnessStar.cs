using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using System;
using Terraria;

namespace HJScarletRework.Projs.General
{
    public class CycleMadnessStar : HJScarletProj
    {
        public enum State
        {
            Floating,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Osci => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.ai[0];
        public ref float RandRotation => ref Projectile.localAI[0];
        public ref float RingScale => ref Projectile.localAI[1];
        public AnimationStruct Helper = new(3);
        public int CurLifeTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.SetUpHeldProj(1);
            Projectile.timeLeft = GetSeconds(3) * Projectile.MaxUpdates;
            Projectile.Opacity = 0;
            Projectile.scale = 0;
        }
        public override void OnFirstFrame()
        {
            CurLifeTime = Projectile.timeLeft;
            RandRotation = RandRotTwoPi;
            base.OnFirstFrame();
        }

        public override void ProjAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.2f);
            switch (AttackState)
            {
                case State.Floating:
                    DoFloating();
                    break;
                case State.Homing:
                    DoHoming();
                    break;
            }
        }

        public void DoHoming()
        {
            Projectile.timeLeft = CurLifeTime;
            Projectile.rotation = Projectile.rotation.AngleTowards((Projectile.Center.GetNormalVector2(Owner.Center)).ToRotation(), .2f);
            float maxtimer = 30;
            float progress = Utils.GetLerpValue(0, maxtimer, Timer, true);
            RingScale = Lerp(RingScale, 0f, .05f);
            DrawParticle();
            Timer++;
            float homingSpeed = Lerp(1, 30, progress);
            Projectile.HomingTarget(Owner.Center, -1, homingSpeed, 10);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox) && Owner.HJScarlet().cycleMadnessLevel > 0)
            {
                Owner.HJScarlet().cycleMadnessCrit += CycleMadness.CritsAdd;
                Owner.AddBuff(BuffType<CycleMadnessBuff>(), GetSeconds(CycleMadness.CritsPerSecond) * Owner.HJScarlet().cycleMadnessLevel);
                ScarletSound(HJScarletSounds.TheMars_Hit, Projectile.Center, .45f, 1, .6f);
                Projectile.Kill();
            }
        }

        public void DoFloating()
        {
            RingScale = Lerp(RingScale, 1f, .05f);
            float distance = (Projectile.Center - Owner.Center).LengthSquared();
            float searchDist = 16 * 20;
            if (Owner.HJScarlet().cycleMadnessLevel == 2)
                searchDist *= 2;

            Projectile.scale = Lerp(Projectile.scale, 1.2f, 0.05f);
            Projectile.rotation = Projectile.SpeedAffectRotation() + RandRotation;
            Projectile.velocity *= .89f;
            Osci += ToRadians(2.5f);
            Vector2 floatingProgress = Projectile.Center + Vector2.UnitY * (int)(Math.Sin(Osci) * 5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, floatingProgress, 0.08f);
            CurLifeTime = Projectile.timeLeft;
            FloatingParticle();
            if (distance < (searchDist * searchDist) && Projectile.scale > 1.0f)
            {
                AttackState = State.Homing;
            }

        }
        public void FloatingParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(8))
            {
                bool boolenValue = Main.rand.NextBool();
                Color c = Color.White;
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(16), -Vector2.UnitY * Main.rand.NextFloat(0.1f, 1.2f) * 1.1f, c, Main.rand.Next(30, 45), 1, Projectile.scale * 0.30f * Main.rand.NextFloat(.9f, 1.05f), 0.2f);
            }
        }
        public void DrawParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
            {
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(16), Projectile.velocity / 8f, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 45), 1f, Main.rand.NextFloat(.9f, 1.1f) * .051f, .85f);
            }

        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(4), Main.rand.Next(2, 5), Color.White, Main.rand.Next(30, 60), 1, 0.30f * Main.rand.NextFloat(.9f, 1.15f));
            }
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D oldtex = Projectile.GetTexture();
            Vector2 oldtexOrig = oldtex.Size() / 2f;
            Texture2D ring = HJScarletTexture.Particle_RingHard.Value;
            Vector2 ringOrig = ring.Size() / 2f;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 oldpos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color c = Color.Lerp(Color.WhiteSmoke, Color.White, ratios).ToAddColor(225);
                float oldscale = Lerp(.14f, 1f, ratios);
                float opa = Lerp(.1f, 1f, ratios) * Projectile.Opacity;
                Vector2 sharpScale = new Vector2(1, 1f) * Projectile.scale;
                Vector2 sharpPos = oldpos - new Vector2(0, 0).RotatedBy(Projectile.oldRot[i]);
                float oldRot = Projectile.oldRot[i];
                SB.FastDraw(oldtex, sharpPos, c * opa, oldRot, oldtexOrig, sharpScale * oldscale, 0);
            }
            SB.EnterShaderArea();
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 oldpos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color c = Color.Lerp(Color.WhiteSmoke, Color.White, ratios).ToAddColor(225);
                float oldscale = Lerp(.14f, 1f, ratios);
                float opa = Lerp(.1f, 1f, ratios) * Projectile.Opacity;
                Vector2 sharpScale = new Vector2(1, 1f) * Projectile.scale;
                Vector2 sharpPos = oldpos - new Vector2(0, 0).RotatedBy(Projectile.oldRot[i]);
                float oldRot = Projectile.oldRot[i];
                SB.FastDraw(ring, sharpPos, c * opa, oldRot, ringOrig, sharpScale * oldscale * .65f * RingScale, 0);
            }
            SB.EndShaderArea();
            return false;
        }
    }
}
