using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class MoltenKnifeBoom :HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(-1);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
        }
        public override void OnFirstFrame()
        {
            //什么叫你写了这么多就为了处理这个特效爆炸？
            for (int i = 0; i < 45; i++)
            {
                Vector2 vel = (TwoPi / 45f * i).ToRotationVector2() * 8f * Main.rand.NextFloat(0f, 1f);

                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * Main.rand.NextFloat() * 2f;
                Color color = RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange);
                float scale = 0.40f * Main.rand.NextFloat(0.55f, 1.1f);
                ECSParticle.SmokeParticle(spawnpos, vel, color, Main.rand.Next(10, 41), RandRotTwoPi, Main.rand.NextFloat(.75f, 1f), scale, true, BlendState.Additive);
                spawnpos = Projectile.Center.ToRandCirclePos(6f) - Projectile.SafeDirByRot() * 10f;
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 4.9f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .5f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .99f * Main.rand.NextFloat(.6f, 1f), .2f);
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 4.9f);
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .5f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .15f * Main.rand.NextFloat(.6f, 1f), .5f);
            }

            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
