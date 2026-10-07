using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using System;
using Terraria;

namespace HJScarletRework.ReVisual.Projs
{
    public class RePurplePufferHeldProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public override string Texture => GetInstance<ContinentOfJourney.Items.PurplePuffer>().Texture;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
        }
        public override void ProjAI()
        {
            int duration = Owner.itemAnimationMax; // Define the duration the projectile will exist in frames

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }
            Projectile.velocity = Owner.ToMouseVector2();
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dir = Math.Sign(Owner.LocalMouseWorld().X - Owner.Center.X);
            Projectile.spriteDirection = dir;
            Owner.heldProj = Projectile.whoAmI; // Update the player's held projectile id
            Owner.ChangeDir(dir);
            Owner.ControlPlayerArm(Projectile.rotation);

            float pullBack = 7f;
            float animationProgress = 0.5f - Owner.itemTime / (float)Owner.itemTimeMax;
            if (animationProgress < .4f)
                pullBack -= 2.75f * (float)Math.Pow((.6f - animationProgress) / .6f, 2f);
            Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * pullBack;

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float drawRot, out SpriteEffects se);
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, Projectile.scale, se, 0);
            return false;
        }
    }
    public class RePurplePufferProj : ReVisualProjectile
    {
        protected override int ApplyProjectile => ProjectileType<PurplePuffer>();
        protected override int TrailLength => 10;
        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (player.GetModPlayer<ReVisualPlayer>().reVisualPurplePuffer)
            {
                projectile.rotation = projectile.velocity.ToRotation();
                if (TrailLength > 0)
                {
                    OldRotationList.Add(projectile.rotation);
                    OldPositionList.Add(projectile.Center);
                    if (OldRotationList.Count > TrailLength)
                        OldRotationList.RemoveAt(0);
                    if (OldPositionList.Count > TrailLength)
                        OldPositionList.RemoveAt(0);
                }
                ECSParticle.SmokeParticle(projectile.Center.ToRandCirclePos(3), projectile.velocity / 4f, RandLerpColor(Color.DarkViolet, Color.Violet), 40,
                    RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .20f, blendstate: BlendState.AlphaBlend);
                ECSParticle.ShinyCrossStarECS(projectile.Center.ToRandCirclePos(6), projectile.velocity / 4f, RandLerpColor(Color.DarkViolet, Color.Violet), 40,
                    1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
                return false;
            }
            return base.PreAI(projectile);
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            return base.PreKill(projectile, timeLeft);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            Player player = projectile.GetPlayer();
            if (player.GetModPlayer<ReVisualPlayer>().reVisualPurplePuffer)
            {
                Texture2D fireball = HJScarletTexture.Texture_FireBall.Value;
                Texture2D fireballPixel = HJScarletTexture.Texture_FireBallPixel.Value;
                Texture2D glow = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                Vector2 drawPos = projectile.Center - Main.screenPosition;
                int length = OldPositionList.Count;
                SB.EnterShaderArea();
                SB.FastDraw(glow, drawPos, Color.DarkViolet, 0, glow.Size() / 2f, projectile.scale * .5f, 0);
                SB.EndShaderArea();
                float overallScale = .45f;
                for (int i = 0; i < length; i++)
                {
                    Vector2 oldPos = OldPositionList[i] - Main.screenPosition;
                    float oldRot = OldRotationList[i] + PiOver2;
                    //图竖直
                    float progress = 1 - (i / (float)length);
                    float xMult = Lerp(1f, .1f, (progress));
                    float yMult = Lerp(1.2f, .4f, progress);
                    Vector2 scale = new Vector2(xMult, yMult) * projectile.scale * overallScale;
                    Color c = Color.Lerp(Color.DarkViolet, Color.Lerp(Color.Violet, Color.Purple, .63f), EaseInOutQuad(progress));
                    float opac = Lerp(1f, .49f, EaseInOutExpo(progress));
                    Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Violet, Color.DarkViolet, 1f), EaseOutCubic(progress));
                    SB.FastDraw(fireballPixel, oldPos, pixelColor.ToAddColor(200) * opac * 1.15f, oldRot, fireballPixel.Size() / 2f, scale * .95f, 0);
                    SB.FastDraw(fireball, oldPos + Main.rand.NextVector2Circular(5, 5) * (1 - progress),
                        c.ToAddColor() * opac * .65f * progress, oldRot, fireball.Size() / 2f, scale * .85f, 0);
                }
                SB.FastDraw(fireball, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f), Color.DarkViolet.ToAddColor() * 1.5f,
                    projectile.rotation + PiOver2, fireball.Size() / 2f, projectile.scale * new Vector2(.68f, 1.2f) * overallScale, 0);
                SB.FastDraw(fireball, drawPos, Color.White.ToAddColor(0) * .90f, projectile.rotation + PiOver2, fireball.Size() / 2f,
                    projectile.scale * .95f * overallScale * new Vector2(0.68f, 1.2f), 0);
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
