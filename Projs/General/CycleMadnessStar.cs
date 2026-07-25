using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public AnimationStruct Helper = new(3);
        public int CurLifeTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.SetUpHeldProj(2);
            Projectile.Opacity = 0;
        }
        public override void OnFirstFrame()
        {
            CurLifeTime = Projectile.timeLeft;
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
            Timer++;
            float homingSpeed = Lerp(1, 30, progress);
            Projectile.HomingTarget(Owner.Center, -1, homingSpeed, 10);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                Owner.HJScarlet().cycleMadenessCrit += CycleMadness.CritsAdd;
                Owner.HJScarlet().cycleMadenssTimer = CycleMadness.CritsPerSecond;
                Projectile.Kill();
            }
        }

        public void DoFloating()
        {
            float distance = (Projectile.Center - Owner.Center).LengthSquared();
            float searchDist = 150;

            Projectile.scale = Lerp(Projectile.scale, 1.01f, 0.2f);
            Projectile.velocity *= .86f;
            Osci += ToRadians(2.5f);
            Vector2 floatingProgress = Projectile.Center + Vector2.UnitY * (int)(Math.Sin(Osci) * 5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, floatingProgress, 0.08f);
            CurLifeTime = Projectile.timeLeft;
            if (distance < (searchDist * searchDist) && Projectile.scale > 1.0f)
            {
                AttackState = State.Homing;
            }

        }
        public void DrawParticle()
        {
            if (Projectile.IsOutScreen())
                return;

            if (Main.rand.NextBool(6))
            {
                bool boolenValue = Main.rand.NextBool();
                Color c = boolenValue ? Color.White : Color.Black;
                BlendState bs = boolenValue ? BlendState.Additive : BlendState.NonPremultiplied;
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(11, 4), -Vector2.UnitY * Main.rand.NextFloat(0.1f, 1.2f) * 1.1f, c, Main.rand.Next(30, 45), 1, Projectile.scale * 0.20f * Main.rand.NextFloat(.9f, 1.15f), 0.2f, bs);
            }
            if (Main.rand.NextBool(4))
            {
                bool boolenValue = Main.rand.NextBool();
                Color c = boolenValue ? Color.White : Color.Black;
                BlendState bs = boolenValue ? BlendState.Additive : BlendState.NonPremultiplied;
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(11, 4), Main.rand.Next(1, 3), c, Main.rand.Next(30, 60), 1, 0.30f * Main.rand.NextFloat(.9f, 1.15f), blendState: bs);
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
            Projectile.DrawProj(Color.White, useOldPos: true);
            return false;
        }
    }
}
