using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneTenctacle : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => base.Category;
        public ref float Timer => ref Projectile.localAI[1];
        public ref float ScaleProgress => ref Projectile.localAI[0];
        public ref float SpeedXProgress => ref Projectile.ai[0];
        public ref float SpeedYProgress => ref Projectile.ai[1];
        public Vector2 InitPos = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 5;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public void CheckIfMeetCrash()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(Projectile.velocity.X) < 1f)
                    Projectile.velocity.X = -Projectile.velocity.X;
                else
                    Projectile.Kill();
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(Projectile.velocity.Y) < 1f)
                    Projectile.velocity.Y = -Projectile.velocity.Y;
                else
                    Projectile.Kill();
            }

        }
        public override void AI()
        {
            if(!Projectile.HJScarlet().FirstFrame)
            {
                InitDust();
            }
            Timer++;
            CheckIfMeetCrash();
            Vector2 center10 = Projectile.Center;
            Projectile.scale = 1f - ScaleProgress;
            Projectile.width = (int)(10f * Projectile.scale);
            Projectile.height = Projectile.width;
            Projectile.position.X = center10.X - Projectile.width / 2;
            Projectile.position.Y = center10.Y - Projectile.height / 2;
            if (ScaleProgress < 0.1f)
                ScaleProgress += 0.01f;
            else
                ScaleProgress += 0.020f;

            if (ScaleProgress >= 0.95f)
                Projectile.Kill();
            //在生成一定时间之后，让触手返回索敌
            if (Projectile.GetTargetSafe(out NPC target) && Timer > 30f)
            {
                Vector2 randPos = target.Center + Vector2.UnitY.RotatedByRandom(TwoPi) * 10f * Main.rand.NextBool().ToDirectionInt();
                float angleOffset = WrapAngle(Projectile.AngleTo(randPos) - Projectile.velocity.ToRotation());
                //在这里让触手回程，但不要过度，否则表现会不符合触手的动作
                angleOffset = Clamp(angleOffset, -ToRadians(1.5f), ToRadians(1.5f));
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset + Main.rand.NextFloat(ToRadians(1f)) * Main.rand.NextBool().ToDirectionInt());
            }
            else
                InitTenctacleSpeed();
            if (ScaleProgress >= 0.95f)
                return;
            SpawnTenctacleDust();
        }

        private void InitDust()
        {
            new CrossGlow(Projectile.Center, Color.DarkOliveGreen, 40, 1f, 0.15f).Spawn();
            new ShinyOrbParticle(Projectile.Center, Vector2.Zero,Color.LightSeaGreen, 40, 0.8f).Spawn();
        }

        private void InitTenctacleSpeed()
        {
            //常规触手动作
            Projectile.velocity.X = Projectile.velocity.X + SpeedXProgress * 1.5f;
            Projectile.velocity.Y = Projectile.velocity.Y + SpeedYProgress * 1.5f;
            if (Projectile.velocity.Length() > 4.2f)
            {
                Projectile.velocity.Normalize();

                Projectile.velocity *= 4.2f;
            }
            SpeedXProgress *= 1.05f;
            SpeedYProgress *= 1.05f;

        }
        private void SpawnTenctacleDust()
        {
            
            if (Projectile.scale < 1f && Timer > 2f)
            {
                int scaleLoopCheck = 0;
                while (scaleLoopCheck < Projectile.scale * 10f)
                {
                    Vector2 spawnPos = (Projectile.position + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) + Projectile.Center) / 2f;
                    Vector2 vel = Projectile.velocity * 0.1f;
                    vel -= Projectile.velocity * (1.4f - Projectile.scale);
                    float scale = 0.7f + Projectile.scale * 0.75f;
                    Color drawColor = RandLerpColor(Color.DarkOliveGreen, Color.DarkSeaGreen);
                    scaleLoopCheck++;
                    new ShinyOrbParticle(spawnPos, vel, drawColor, (int)(60 * Projectile.scale), scale * 0.24f, glowCenter: false).Spawn();
                }
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
