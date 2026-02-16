using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Melee
{
    public class EclipseWraithFireball : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(8, 2);
        public ref float Timer => ref Projectile.ai[0];
        public bool DontChaseToTarget = false;
        public override void ExSD()
        {
            Projectile.extraUpdates = 4;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = !HJScarletMethods.HasFuckingCalamity;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            //刚生成时不要撞墙上
            Projectile.tileCollide = Timer > 30f;
            Timer++;
            DrawParticle();
            
            //处理追踪逻辑，但也不多
            if (DontChaseToTarget)
            {

            }
            else
            {
                if (Projectile.GetTargetSafe(out NPC target, false))
                    Projectile.HomingTarget(target.Center, -1, Projectile.velocity.Length(), 20f);
            }
        }

        private void DrawParticle()
        {
            //如果超出玩家屏幕范围内，做掉所有特效
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            Vector2 dir = Projectile.SafeDirByRot();
            Vector2 basePos = Projectile.Center - dir * 10f;
            for (int k = 0; k < 1; k++)
            {
                Vector2 starShapePos = basePos + Main.rand.NextVector2CircularEdge(3f, 3f);
                Color drawColor = RandLerpColor(Color.Red, Color.DarkRed);
                new StarShape(starShapePos, dir * -2.4f, drawColor, 0.6f, 15, glowScale: 0.68f).SpawnToPriorityNonPreMult();
            }

            Color Firecolor = RandLerpColor(Color.DarkGray, Color.DarkRed);
            new Fire(basePos + Main.rand.NextVector2Circular(3, 3), dir * -Main.rand.NextFloat(1.2f, 2.3f), Firecolor, Main.rand.Next(15, 20), Main.rand.NextFloat(TwoPi), 1f, Main.rand.NextFloat(0.1f, 0.14f)).SpawnToNonPreMult();

            for (int i = 0; i < 1; i++)
            {
                new ShinyOrbParticle(basePos + Main.rand.NextVector2CircularEdge(3f, 3f), dir * -Main.rand.NextFloat(2.4f, 3.6f), RandLerpColor(Color.DarkRed, Color.Red), 15, 0.5f).Spawn();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //粒子
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 1, Pitch = 0.3f }, Projectile.Center);
            Vector2 dir = Projectile.SafeDirByRot();
            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.Black, Color.DarkRed);
                new SmokeParticle(Projectile.Center, RandVelTwoPi(0.2f, 1.2f) * 3, Firecolor, 40, RandRotTwoPi, 1f, 0.30f).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnPos = Projectile.Center + dir * -5f + Main.rand.NextVector2CircularEdge(5f, 5f);
                Vector2 velDir = dir.ToRandVelocity(ToRadians(15)) * Main.rand.NextFloat(3.2f, 6.2f);
                new ShinyOrbParticle(spawnPos, velDir, RandLerpColor(Color.Red, Color.DarkRed), 40, 0.8f).Spawn();
                new ShinyOrbParticle(spawnPos, velDir, Color.White, 40, 0.4f).Spawn();
            }
        }

        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sharpTear = HJScarletTexture.Particle_SharpTear;
            //在前端直接绘制一个箭头类型的玩意
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            for (int i = 0; i < 8; i++)
            {
                float radius = 1 - (i + 1f) / Projectile.oldPos.Length;
                Color color = Color.Lerp(Color.Red, Color.DarkRed, 1 - radius) * radius;
                Vector2 scale = Projectile.scale * new Vector2(0.8f * (radius), 1.3f) * 0.9f;
                Vector2 thedrawPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                SB.Draw(sharpTear, thedrawPos, null, color, Projectile.oldRot[i] + PiOver2, sharpTear.Size() / 2, scale, 0, 0);
                SB.Draw(sharpTear, thedrawPos, null, Color.White with { A = 0 }, Projectile.oldRot[i] + PiOver2, sharpTear.Size() / 2, scale * 0.6f, 0, 0);
            }
            return false;
        }
    }
}
