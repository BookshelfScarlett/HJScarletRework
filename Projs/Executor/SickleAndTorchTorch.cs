using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SickleAndTorchTorch : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);

        }
        public override void ProjAI()
        {
            Projectile.rotation += Projectile.SpeedAffectRotation(39, 39) * Projectile.ai[1];
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 20 * Projectile.MaxUpdates)
            {
                Projectile.AffactedByGrav(velMult: .97f, yAdd: 0.14f);
            }
            if (Projectile.IsOutScreen())
                return;
                Vector2 pos = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * 15f * Projectile.scale;
                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(0.7f, 1.2f) * 10f;
                ECSParticle.SmokeParticle(pos.ToRandCirclePos(16), vel, RandLerpColor(Color.DarkGray, Color.WhiteSmoke), 16, RandRotTwoPi, 1, 0.215f * Main.rand.NextFloat(0.75f, 1.15f) * Projectile.scale, false, BlendState.NonPremultiplied);
            Vector2 pos2 = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * 15f * Projectile.scale;
            Vector2 vel2 = -Vector2.UnitY * Main.rand.NextFloat(0.7f, 1.2f) * 4f;
            Dust d = Dust.NewDustPerfect(pos2.ToRandCirclePos(8), DustID.Torch);
            d.velocity = vel2;
            d.scale = Main.rand.NextFloat(.95f, 1.15f);
            d.noGravity = true;
            pos2 = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * 15f * Projectile.scale;
            vel2 = -Vector2.UnitY * Main.rand.NextFloat(0.7f, 1.2f) * 4f;
            d = Dust.NewDustPerfect(pos2.ToRandCirclePos(16), DustID.Torch);
            d.velocity = vel2;
            d.scale = Main.rand.NextFloat(.95f, 1.15f);
            d.noGravity = true;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, GetSeconds(5));
            Projectile.AddExecutionTimeImmediate(ItemType<SickleAndTorch>());
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 vel = (TwoPi / 32f * i).ToRotationVector2() * 8f * Main.rand.NextFloat(0f, 1f);

                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * Main.rand.NextFloat() * 2f;
                Color color = RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange);
                float scale = 0.40f * Main.rand.NextFloat(0.55f, 1.1f);
                ECSParticle.SmokeParticle(spawnpos, vel, color, Main.rand.Next(10, 41), RandRotTwoPi, Main.rand.NextFloat(.75f, 1f), scale, true, BlendState.Additive);
            }
            for (int j = 0; j < 15; j++)
            {
                Vector2 dir = RandVelTwoPi(.1f, 4.9f);
                Vector2 pos = Projectile.Center.ToRandCirclePos(3f) + dir * Main.rand.NextFloat(0f, 3f);
                ECSParticle.ShinyCrossStarECS(pos, dir, RandLerpColor(Color.Orange, Color.OrangeRed), Main.rand.Next(15, 50), 1f, 1f * Main.rand.NextFloat(.7f, .9f), .2f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 4.9f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .5f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .99f * Main.rand.NextFloat(.6f, 1f), .2f);
            }
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 4.9f);
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .5f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .15f * Main.rand.NextFloat(.6f, 1f), .5f);
            }
            ScarletSound(SoundID.DD2_BetsyFireballImpact, Projectile.Center, 0.75f, 0, .35f);
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                for (int i = 0; i < 46; i++)

                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(16);
                    ECSParticle.StarShape(pos, Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(0.3f, 1f) * 10f, RandLerpColor(Color.Orange, Color.OrangeRed), Main.rand.Next(0, 55), 1, 0.8f * Main.rand.NextFloat(.7f, 1.1f), .89f, BlendState.Additive);
                }
                for (int i = 0; i < 2; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY.ToRandVelocity(ToRadians(10f), 9f, 13f), ProjectileType<LavaFlowBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { MaxInstances = 0, Pitch = .65f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 0, Pitch = .65f }, Projectile.Center);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
