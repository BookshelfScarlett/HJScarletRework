using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class AbyssalWorldProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public AnimationStruct Helper = new(3);
        public NPC Target = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4, 2);
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.height = Projectile.width = 16;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                Helper.MaxProgress[0] = 30;
                Helper.MaxProgress[1] = 30;
                return;
            }
            if (!Helper.IsDone[0])
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Helper.UpdateAniState(0);
                Projectile.velocity *= 0.96f;
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1200f))
                    Target = target;

            }
            else if (!Helper.IsDone[1])
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Helper.UpdateAniState(1);
                Projectile.velocity *= 0.96f;
                ExtraParticle();
            }
            else
            {
                if (Target.IsLegal())
                {
                    if (Projectile.extraUpdates == 1)
                    {
                        Projectile.extraUpdates = 100;
                    }
                        Projectile.rotation = ((Target.Center - Projectile.Center)).ToSafeNormalize().ToRotation();
                    Projectile.HomingTarget(Target.Center, -1, 20f, 1f);
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }
                UpdateParticles();
            
        }

        private void ExtraParticle()
        {
        }

        private void UpdateParticles()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            new SmokeParticle(Projectile.Center.ToRandCirclePos(6f), RandVelTwoPi(1f, 3f), RandLerpColor(Color.DarkSlateBlue, Color.Aqua), 40, RandRotTwoPi, 1, 0.24f).SpawnToNonPreMult();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rotFix = PiOver4;
            Projectile.DrawGlowEdge(Color.White, rotFix: rotFix);
            Projectile.DrawProj(lightColor, rotFix: rotFix);
            return false;
        }


    }
}
