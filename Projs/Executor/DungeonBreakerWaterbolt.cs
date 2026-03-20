using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonBreakerWaterbolt : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public int BouceTime = 0;
        public int TotalBounceTime = 10;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(14);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 10;
            Projectile.penetrate = 6;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.SetupImmnuity(30);
            Projectile.timeLeft = GetSeconds(3);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Blue);
            if (Projectile.timeLeft == 1)
                SpawnSparkle(Projectile.velocity);
            for (int i = 0; i < 2; i++)
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(4), Projectile.SafeDir() * 0.8f, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, RandRotTwoPi, 1f, 0.15f, false).Spawn();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //这边得生成一个大点的粒子盖住一下
            //不然好像这个真没什么好的解决方法
            SoundEngine.PlaySound(SoundID.Item35 with { MaxInstances = 1, Pitch = -0.5f + BouceTime * 0.05f }, Projectile.Center);
            BouceTime++;
            if (BouceTime > TotalBounceTime)
            {
                BouceTime = TotalBounceTime;
                Projectile.Kill();
            }
            new ShinyCrossStar(Projectile.Center.ToRandCirclePos(4), Vector2.Zero, Color.RoyalBlue, 40, oldVelocity.ToRotation(), 1f, 1.6f, false).Spawn();
            SpawnSparkle(oldVelocity);
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        public void SpawnSparkle(Vector2 dir)
        {
            dir = dir.ToSafeNormalize();
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(8), DustID.UnusedWhiteBluePurple);
                d.velocity = RandVelTwoPi(1f, 1.8f) + dir * Main.rand.NextFloat(0.8f, 1.1f);
                d.scale *= 1.3574f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnSparkle(Projectile.velocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            for (int i = 0; i < 7; i++)
            {
                Vector2 scale = new Vector2(0.25f, 0.65f) * 1.1f;
                SB.Draw(starShape, Projectile.Center - Main.screenPosition - Projectile.SafeDir() * 3.5f * i - Projectile.SafeDir() * 10f, null, Color.RoyalBlue.ToAddColor(50), Projectile.velocity.ToRotation() + PiOver2, starShape.ToOrigin(), scale, 0, 0);
            }
            return false;
        }
    }
}
