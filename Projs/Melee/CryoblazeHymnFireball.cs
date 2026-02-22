using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CryoblazeHymnFireball : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public enum Style
        {
            Attack,
            Bounce,
            Slowdown
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public float Speed = 0f;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(13, 2);
        }
        public int BounceTime = 0;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = 1;
            Projectile.scale *= 1.1f;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 100;
        }
        public override void AI()
        {
            if(!Projectile.HJScarlet().FirstFrame)
            {
                Speed = Projectile.velocity.Length();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            switch(AttackType)
            {
                case Style.Attack:
                    DoAttack();
                    break;
                case Style.Bounce:
                    DoBounce();
                    break;
                case Style.Slowdown:
                    DoSlowdown();
                    break;
            }
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            for (int i = 0; i < 2; i++)
            {
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(4f), 0.5f, RandLerpColor(Color.Orange, Color.OrangeRed), 30, Main.rand.NextFloat(0.1f, 0.12f), RandRotTwoPi).Spawn();
                new SmokeParticle(Projectile.Center.ToRandCirclePos(8f), -Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.DarkGray), 30, RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.1f).SpawnToPriorityNonPreMult();
            }
            Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(10f), 0.8f, 1.4f);
            new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f), vel, RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.3f, ToRadians(10f)).Spawn();
        }

        private void DoAttack()
        {
            Timer++;
            Projectile.velocity *= 0.86f;
            if(Timer > 30f)
            {
                //回弹
                AttackType = Style.Bounce;
                Projectile.netUpdate = true;
                Projectile.timeLeft = 200;
            }
        }

        private void DoBounce()
        {
            if (Projectile.GetTargetSafe(out NPC target))
            {
                Projectile.HomingTarget(target.Center, -1, 24f, 20f);
            }
            else
            {
                if (Projectile.velocity.LengthSquared() < 16f * 16f)
                    Projectile.velocity *= 1.1f;
            }
        }

        private void DoSlowdown()
        {
            Projectile.velocity *= 0.98f;
            Projectile.Opacity -= 0.1f;
            if (Projectile.Opacity <= 0f)
                Projectile.Kill();

        }

        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, GetSeconds(5));
            if(AttackType == Style.Bounce)
            {
                AttackType = Style.Slowdown;
                Projectile.netUpdate = true;
            }
        }

        public override bool? CanDamage() => AttackType != Style.Slowdown && Timer > 10f;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = 6;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.90f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.Orange, Color.Lerp(Color.Orange, Color.OrangeRed, rads * 0.7f), (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.50f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.Orange.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.65f, 0, 0);
            return false;
        }
    }
}
