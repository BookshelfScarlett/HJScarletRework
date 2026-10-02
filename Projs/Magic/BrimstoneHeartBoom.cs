using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Magic
{
    public class BrimstoneHeartBoom : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.SetupImmnuity(-1);
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override void OnFirstFrame()
        {
            ScarletSound(HJScarletSounds.Gaia_Charge, Projectile.Center, 0.75f, 1, 0.4f, .1f);
            for (int i = 0; i < 35; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(2), RandVelTwoPi(0f, 16f), RandLerpColor(Color.DarkRed, Color.Crimson) * 1f, Main.rand.Next(30, 60), RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.AlphaBlend);
            }
            float glowScale = .44f;
            ECSParticle.CrossGlow(Projectile.Center, Color.Purple, 40, 1, glowScale, 0.5f);
            ECSParticle.CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, glowScale * .98f, 0.5f);
            ECSParticle.CrossGlow(Projectile.Center, Color.White, 40, 1, glowScale * .96f, 0.5f);
            //ECSParticle.Ring(Projectile.Center, Vector2.Zero, Color.Red*.68f, 45, 1, 0, glowScale*1, 0.1f,9,blendState:BlendState.NonPremultiplied);
            for (int i = 0; i < 30; i++)
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(100), Main.rand.NextFloat(1.2f, 2.4f) * 2.5f, RandLerpColor(Color.Red, Color.Crimson), 60, 1, Main.rand.NextFloat(.9f, 1.15f) * .20f, glowMult: .35f);
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePosEdge(0), RandVelTwoPi(1f, 16f), RandLerpColor(Color.DarkRed, Color.Crimson), Main.rand.Next(20, 45), 1, RandRotTwoPi, 0.4f, 1, blendstate: BlendState.Additive);
            }
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
