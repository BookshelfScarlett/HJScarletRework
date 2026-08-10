using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheSevenStarBolt : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 50;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.SetupImmnuity(-1);
        }
        public override void ProjAI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                //InitParticle(Projectile.Center);
            }
            if (Projectile.IsOutScreen())
                return;
            float scale = .51f;
            //ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(3), 0.18f, RandLerpColor(Color.SkyBlue, Color.White), Main.rand.Next(80, 90), 1, Main.rand.NextFloat(0.85f, 1.15f) * .1f * scale, glowMult: .45f);
            ECSParticle.LightntingGlow(Projectile.Center, Projectile.velocity.ToSafeNormalize(), RandLerpColor(Color.LightSkyBlue, Color.SkyBlue), 50, 1, 0.64f, 3);
            Vector2 dir = Projectile.SafeDir(); ;
            for (int i = 0; i < 1; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(3) + Projectile.SafeDir() * i * 5f, 0.18f, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), Main.rand.Next(80, 90), 1, Main.rand.NextFloat(0.85f, 1.15f) * .1f * scale, glowMult: .25f);
                ECSParticle.ShinyCrossStarECS(Projectile.Center + Projectile.SafeDir() * i * 10, RandVelTwoPi(.1f, .3f), RandLerpColor(Color.White, Color.LightSkyBlue), 80, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .32f * scale, .2f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 initPos = Projectile.oldPosition + Projectile.Size / 2f;
            //InitParticle(initPos);
            float centerGlowScale = .32f;
            ECSParticle.CrossGlow(Projectile.Center, Color.SkyBlue, 45, 1, centerGlowScale);

            for (int i = 0; i < 3; i++)
            {
                Color color = RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue);
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .1f + i * 0.032f, Projectile.whoAmI, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 10; i++)
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(16), Main.rand.NextFloat(1.2f, 2.4f) * .28f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);

        }
        public void InitParticle(Vector2 initPos)
        {
            float centerGlowScale = .16f;
            ECSParticle.CrossGlow(initPos, Color.SkyBlue, 45, 1, centerGlowScale);
            ECSParticle.CrossGlow(initPos, Color.LightSkyBlue, 45, 1, centerGlowScale * .98f);
            ECSParticle.CrossGlow(initPos, Color.White, 45, 1, centerGlowScale * .96f);
            for (int i = 0; i < 3; i++)
            {
                Color color = RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue);
                new NoiseShockRing(initPos, Vector2.Zero, color, 45, 1f, .05f + i * 0.07f, Projectile.whoAmI, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 12; i++)
                ECSParticle.TurbulenceShinyOrb(initPos.ToRandCirclePosEdge(15), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .03f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
