using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ContainedBlastStickBullet : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public enum State
        {
            Shoot,
            Stick
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float MountedLerp => ref Projectile.localAI[0];
        public Vector2 MountedPos = Vector2.Zero;
        public NPC Target = null;
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 12;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 2;
            Projectile.Opacity = 0;
        }
        public override void ProjAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.1f);
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Stick:
                    DoStick();
                    break;
            }

        }

        private void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(4);
                float scale = 0.34f * Main.rand.NextFloat(0.7f, 1.1f) * Projectile.Opacity;
                ECSParticle.SmokeParticle(pos, Projectile.velocity / 2f, RandLerpColor(Color.WhiteSmoke, Color.White), 45, RandRotTwoPi, 1, scale, Main.rand.NextBool(), BlendState.Additive);
            }
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 2f, RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1f, 0.64f * Main.rand.NextFloat(0.7f, 1.1f), 0.2f);
        }

        public void DoStick()
        {
            if (Target.IsLegal())
            {
                Projectile.Center = Target.Center + MountedPos;
                Target.HJScarlet().isBeingStabByContainedStick = 5;
                MountedLerp = Lerp(MountedLerp, 1.01f, 0.21f);
            }
            else
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<ContainedBlastBoom>(), Projectile.originalDamage / 2, Projectile.knockBack, Projectile.owner);
            proj.ai[0] = 0;
            proj.rotation = Projectile.rotation;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.ai[2] = Projectile.ai[0];
            Projectile.AddExecutionTimeImmediate(ItemType<ContainedBlast>());
            if (AttackState == State.Shoot && target.HJScarlet().isBeingStabByContainedStick == 0)
            {
                Target = target;
                Projectile.rotation = Projectile.SafeDir().ToRotation();
                Projectile.velocity *= 0f;
                MountedPos = Projectile.Center - target.Center;
                AttackState = State.Stick;
                Projectile.timeLeft = GetSeconds(30) * Projectile.MaxUpdates;
                return;
            }
            else
            {

                Projectile.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] > 0)
                modifiers.SetCrit();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            float rotFixer = PiOver2 + Pi;
            int length = Projectile.oldPos.Length / 2;
            Texture2D sharpTear = HJScarletTexture.Particle_SharpTear;
            int particleSharpTearLength = 6;
            for (int i = particleSharpTearLength - 1; i >= 0; i--)
            {
                float ratios = i / (float)length;
                Vector2 lerpPos = Projectile.Center - Projectile.SafeDir() * (i) * 10f;
                Vector2 pos = lerpPos - Main.screenPosition;
                float rot = Projectile.rotation;
                Color color = Color.Lerp(Color.DarkGray, Color.WhiteSmoke, ratios).ToAddColor() * 0.50f * Clamp(Projectile.velocity.Length(), 0f, 1f) * Projectile.Opacity;
                Vector2 scale = new Vector2(0.85f, 1f);
                SB.Draw(sharpTear, pos, null, color * .75f, rot + PiOver2, sharpTear.ToOrigin(), Projectile.scale * scale, 0, 0);
            }
            float xLerp = Lerp(0.75f, 1f, MountedLerp);
            float yLerp = Lerp(1.2f, 1f, MountedLerp);
            float scaleLerp = Lerp(0.65f, 0.95f, MountedLerp);
            Vector2 projScale = Projectile.scale * new Vector2(xLerp, yLerp) * scaleLerp;

            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = i / (float)length;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0f);
                Vector2 pos = lerpPos + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i];
                Color color = Color.Lerp(Color.DarkGray, Color.Transparent, ratios).ToAddColor() * 0.964f * Projectile.Opacity;
                SB.Draw(tex, pos, null, color, rot + rotFixer, tex.ToOrigin(), projScale, 0, 0);
            }
            Vector2 projPos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(tex, projPos + (TwoPi / 16f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(50) * 0.5f * Projectile.Opacity, Projectile.rotation + rotFixer, tex.ToOrigin(), projScale, 0, 0);
            }
            SB.Draw(tex, projPos, null, Color.White.ToAddColor(250) * Projectile.Opacity, Projectile.rotation + rotFixer, tex.ToOrigin(), projScale, 0, 0);
            return false;
        }
    }
}
