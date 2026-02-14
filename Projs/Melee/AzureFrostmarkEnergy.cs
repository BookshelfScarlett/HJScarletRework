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

namespace HJScarletRework.Projs.Melee
{
    public class AzureFrostmarkEnergy :HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.Particle_ShinyOrbHard.Path;
        public ref float Timer => ref Projectile.ai[0];
        public int BounceTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 150;
        }
        public override void AI()
        {
            //向上喷火的粒子，使用原版的更加合适一些。
            Vector2 pos = Projectile.Center.ToRandCirclePos(4f);
            Vector2 vel = -Projectile.velocity / 4;
            //这里的球比起雪球，更接近于霜火球
            //因此，会沿途留下烟火轨迹
            //烟的粒子会靠后一点，并且会先生成让原版的冰粒子正常绘制
            new SmokeParticle(pos - Projectile.SafeDir() * 15f, vel * 0.5f, RandLerpColor(Color.DeepSkyBlue, Color.White), 20, 0.5f,1f, 0.15f).SpawnToPriorityNonPreMult();
            //生成光球类粒子。增效
            new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity.ToRandVelocity(ToRadians(10f), -1.2f), RandLerpColor(Color.DeepSkyBlue, Color.LightBlue), 20, 0.35f).Spawn();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.98f;
            Projectile.velocity.X *= 1f;
            if (Projectile.velocity.Y < 30f)
                Projectile.velocity.Y += 0.17f;
            Timer++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            SpawnHitParticle();
            BounceTime++;
            return BounceTime > 2;
        }
        private void SpawnHitParticle()
        {
            SoundEngine.PlaySound(SoundID.Item48 with { MaxInstances = 1, Volume = 0.754f}, Projectile.Center);
            for (int i = 0; i < 10; i++)
                new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(2f), 0.8f, RandLerpColor(Color.SkyBlue, Color.AliceBlue), 40, Main.rand.NextFloat(0.1f, 0.12f), RandRotTwoPi, true).Spawn();
            for (int i = 0; i < 5; i++)
                new SmokeParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(1.2f), RandLerpColor(Color.DeepSkyBlue, Color.Gray), 20, RandRotTwoPi, 1f, 0.2f).Spawn();
        }
        public override bool? CanDamage() => Timer > 15f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, GetSeconds(5));
        }
        public override bool PreKill(int timeLeft)
        {
            //粒子。 
            SpawnHitParticle();
            return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = 6;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.90f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.LightSkyBlue, Color.Lerp(Color.LightSkyBlue, Color.Blue, rads * 0.7f), (1- rads)).ToAddColor(10) * Clamp (Projectile.velocity.Length(), 0f,1f) * (1- rads); 
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.DeepSkyBlue.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale* Projectile.scale * 0.65f, 0, 0);
            return false;
        }
    }
}
