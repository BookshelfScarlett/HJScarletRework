using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FierySpearFireball : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public ref float Timer => ref Projectile.ai[0];
        public int BounceTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 600;
        }
        private float SearchDistance = 300f;
        private int TotalBounceTime = 4;
        private float KillDistance = 1800f;
        public override void AI()
        {
            //如果开了灾厄，则加强索敌距离，生存时间和提供1eu
            if (HJScarletMethods.HasFuckingCalamity && Timer == 0f)
            {
                Projectile.timeLeft = 1800;
                SearchDistance = 1800;
                KillDistance = 3600;
                TotalBounceTime = 8;
                Projectile.extraUpdates = 1;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            bool additionRequire = (Timer > 15f && Projectile.HJScarlet().GlobalTargetIndex != -1) || BounceTime > 0;
            if (Projectile.GetTargetSafe(out NPC target, true, SearchDistance) && additionRequire)
                Projectile.HomingTarget(target.Center, -1, 12f, 20f);
            else
            {
                Projectile.velocity.Y += 0.18f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
            }
            //粒子
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = Projectile.Center + Projectile.SafeDirByRot() * -i * 1.2f + Main.rand.NextVector2CircularEdge(8, 8);
                Vector2 speed = Projectile.SafeDirByRot() * Main.rand.NextFloat(1.2f, 1.9f);
                Dust d = Dust.NewDustPerfect(spawnPos, DustID.Torch);
                d.velocity = speed;
                d.position += Projectile.SafeDirByRot(90) * 1.2f;
                d.scale *= 1.6f;
                d.noGravity = true;
            }

            //距离玩家过远时处死
            if (Projectile.TooAwayFromOwner(KillDistance))
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(oldVelocity.Y) > 5f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst);
                    dust.noGravity = true;
                    dust.scale *= 2f * Projectile.scale;
                }
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            BounceTime += 1;
            return BounceTime > TotalBounceTime;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Tex2DWithPath orb = HJScarletTexture.Particle_ShinyOrb;
            Tex2DWithPath ball = HJScarletTexture.Particle_HRShinyOrbMedium;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = 1f;
            Color baseColor = new Color(255, 215, 100);
            Color targetColor = Color.Orange; 
            //绘制残影
            for (int i = 1; i < 6; i++)
            {
                Vector2 projCenter = Projectile.oldPos[i] + Projectile.PosToCenter();
                scale *= 0.93f;
                float radius = (float)i / 6;
                Color color = Color.Lerp(baseColor, targetColor, radius) * 0.5f * (1 - radius);
                Vector2 trailScale = Projectile.scale * new Vector2(1.5f, scale) * 1.5f;
                Color trailColor = color with { A = 50 };
                SB.Draw(orb.Value, projCenter, null, trailColor * Projectile.Opacity, Projectile.oldRot[i], orb.Origin, trailScale * 0.6f, 0, 0f);
                SB.Draw(orb.Value, projCenter, null, targetColor with { A = 50 } * Projectile.Opacity, Projectile.oldRot[i], orb.Origin, trailScale * 0.3f, 0, 0f);
            }
            //绘制火球本身
            Vector2 projScale = Projectile.scale * 0.3f * new Vector2(1.5f, 1f);
            SB.EnterShaderArea();
            SB.Draw(ball.Value,drawPos, null, baseColor, Projectile.rotation, ball.Origin, projScale, 0, 0);
            SB.Draw(ball.Value, drawPos, null, Color.White * 0.8f, Projectile.rotation, ball.Origin, projScale / 3f, 0, 0);
            SB.EndShaderArea();
            return false;
        }
        public override bool? CanDamage()
        {
            return Timer > 20f;
        }
    }
}
