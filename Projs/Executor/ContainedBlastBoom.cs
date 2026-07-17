using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class ContainedBlastBoom : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExSD()
        {
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 60;
            Projectile.height = Projectile.width = 160;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.velocity *= 0f;
        }
        public override void OnFirstFrame()
        {
            bool bigBoom = Projectile.ai[0] != 0;
            if (bigBoom)
            {
                SoundEngine.PlaySound(HJScarletSounds.Frosthammer_SnowCharge with { MaxInstances = 1, Pitch = -.85f, PitchVariance = .1f, Volume = 1f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Frostwave_Boom with { MaxInstances = 1, Pitch = 0.75f, Volume = 1f }, Projectile.Center);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(60);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 12f);
                    ECSParticle.LightntingGlow(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(20, 30), 1f, Main.rand.NextFloat(.75f, 1.1f) * .75f, 2);
                }
                for (int i = 0; i < 66; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(100);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(4.2f, 11.8f);
                    ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 55), RandRotTwoPi, .45f, 0.40f * Main.rand.NextFloat(.7f, 1.1f), true, BlendState.Additive);
                }
                for (int i = 0; i < 44; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(100);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(4.2f, 11.8f);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 55), RandRotTwoPi, .41f, 0.104f * Main.rand.NextFloat(.7f, 1.1f), BlendState.Additive);
                }
                for (int i = 0; i < 8; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(60);
                    new TurbulenceGlowOrb(pos, 6.4f, RandLerpColor(Color.White, Color.WhiteSmoke), 60, 0.3f * Main.rand.NextFloat(.7f, .9f), RandRotTwoPi).Spawn();
                }
                for (int i = 0; i < 44; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(60);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 11.8f);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), 35, .585f, 0.55f * Main.rand.NextFloat(.75f, 1f), 0.2f);
                }
                for (int i = 0; i < 26; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        float rot = Projectile.rotation + PiOver2 * j;
                        Vector2 dir = rot.ToRotationVector2();
                        Vector2 pos = Projectile.Center.ToRandCirclePosEdge(4) + dir * Main.rand.NextFloat(-1f, 16f);
                        ECSParticle.StarShape(pos, dir * Main.rand.NextFloat() * 16f, RandLerpColor(Color.White, Color.WhiteSmoke), 45, 0.40f, 0.98f, 0.85f, BlendState.Additive);
                    }
                }
                new CrossGlow(Projectile.Center, Color.White, 30, 1, 0.48f, false).Spawn();
            }
            else
            {
                SoundEngine.PlaySound(HJScarletSounds.Misc_AirFlowAlt with { MaxInstances = 1, Pitch = -.95f, Volume = .35f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Frostwave_Boom with { MaxInstances = 1, Pitch = 0.75f, Volume = .15f }, Projectile.Center);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 7f);
                    ECSParticle.LightntingGlow(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(20, 30), 1f, Main.rand.NextFloat(.75f, 1.1f) * .55f, 2);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(4.2f, 11.8f);
                    ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 55), RandRotTwoPi, .45f, 0.30f * Main.rand.NextFloat(.7f, 1.1f), true, BlendState.Additive);
                }
                for (int i = 0; i < 14; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(4.2f, 11.8f);
                    ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 55), RandRotTwoPi, .41f, 0.09f * Main.rand.NextFloat(.7f, 1.1f), BlendState.Additive);
                }
                for (int i = 0; i < 8; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(20);
                    new TurbulenceGlowOrb(pos, 6.4f, RandLerpColor(Color.White, Color.WhiteSmoke), 60, 0.23f * Main.rand.NextFloat(.7f, .9f), RandRotTwoPi).Spawn();
                }
                for (int i = 0; i < 24; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(20);
                    Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 11.8f);
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), 35, .585f, 0.45f * Main.rand.NextFloat(.75f, 1f), 0.2f);
                }
                new CrossGlow(Projectile.Center, Color.White, 30, .81f, 0.48f, false).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] != 0)
            {
                Projectile.AddExecutionTimeImmediate(ItemType<ContainedBlast>(), 5);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[2] > 0)
                modifiers.SetCrit();

        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
