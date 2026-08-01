using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.General
{
    public class ChlorophyteCrystalBolt : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 50;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(-1);
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            if (Projectile.IsOutScreen())
                return;
            float scale = 1.74f;
            if(Main.rand.NextBool())
            ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(3), 0.18f, RandLerpColor(Color.LimeGreen, Color.Green), Main.rand.Next(80, 90), 1, Main.rand.NextFloat(0.85f, 1.15f) * .1f * scale, glowMult: .45f);
            ECSParticle.LightntingGlow(Projectile.Center, Projectile.velocity / 8f, RandLerpColor(Color.LimeGreen, Color.LightGreen), 50, 1, 0.4f * scale);
            Vector2 dir = Projectile.SafeDir(); ;
            if(Main.rand.NextBool(4))
            ECSParticle.ShinyCrossStarECS(Projectile.Center + dir.RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt()) * 6f, Projectile.velocity / 8f, RandLerpColor(Color.White, Color.Green), 80, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .32f * scale, .2f);
            if (Main.rand.NextBool(8))
            {
                Color c = Main.rand.NextBool() ? Color.LightGreen: Color.ForestGreen;
                if (Main.rand.NextBool())
                    c = Main.rand.NextBool() ? Color.LightPink : Color.Pink;
                ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePos(4), -Vector2.UnitY * Main.rand.NextFloat(1.2f, 4.2f) * 3.5f, c.ToAddColor(150), 100, .95f, RandRotTwoPi, Main.rand.NextFloat(0.85f, 1.15f) * .061f*scale,.71f,alterTexture:false,fullBright: true, blendState: Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 initPos = Projectile.oldPosition + Projectile.Size / 2;
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Projectile.oldVelocity.ToSafeNormalize();
                float rotvalue = ToRadians(360f / 8f * i) * 1f;
                float scale = (i % 2 == 0) ? 0.5f : 0.35f;
                for (int j = 0; j < 3; j++)
                {
                    ECSParticle.LightntingGlow(initPos + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.DarkSeaGreen, 50, 1, scale);
                    ECSParticle.LightntingGlow(initPos + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.ForestGreen, 50, 1, scale);
                }
            }
            for (int i = 0; i < 12; i++)
            {
                ECSParticle.LiliesFire(initPos + RandVelTwoPi(2f, 12.4f), RandVelTwoPi(0.8f, 5.1f), RandLerpColor(Color.Black, Color.DarkViolet), 60, RandRotTwoPi, 1f, 0.24f, true, Microsoft.Xna.Framework.Graphics.BlendState.Additive);
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = initPos.ToRandCirclePosEdge(12f);
                Vector2 vel = RandVelTwoPi(1f, 4f);
                ECSParticle.KiraStar(pos, vel, RandLerpColor(Color.ForestGreen, Color.Lime), 50, 1, 0, .07f * Main.rand.NextFloat(.85f, 1.01f), true);
                ECSParticle.KiraStar(pos, vel, Color.White, 50, 1, 0, .08f * .5f, true);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.wingTime += 50;
            Vector2 initPos = target.Center;
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = initPos.ToRandCirclePosEdge(12f);
                Vector2 vel = RandVelTwoPi(1f, 4f);
                ECSParticle.KiraStar(pos, vel, RandLerpColor(Color.ForestGreen, Color.Lime), 50, 1, 0, .07f * Main.rand.NextFloat(.85f, 1.01f), true);
                ECSParticle.KiraStar(pos, vel, Color.White, 50, 1, 0, .08f * .5f, true);
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
