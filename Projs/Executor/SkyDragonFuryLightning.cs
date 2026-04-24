using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class SkyDragonFuryLightning : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public List<Vector2> StoredCenter = [];
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 30;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 30;
            Projectile.timeLeft = 50 * 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            if (CurTarget.IsLegal())
            {
                Projectile.HomingTarget(CurTarget.Center, -1, 20, 20);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.numUpdates % 3 == 0)
            {
                new LightningParticle(Projectile.Center.ToRandCirclePos(3f), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), Main.rand.Next(25, 45), Projectile.rotation + PiOver2, Main.rand.NextFloat(0.3f, 0.44f) * 1.7f, 0).Spawn();
                new LightningGlow(Projectile.Center, Projectile.SafeDir(), Color.RoyalBlue, 30, 0.95f).Spawn();
            }
            if (Main.rand.NextBool(20))
            {
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            new LightningParticle(Projectile.Center, Vector2.Zero, Color.RoyalBlue, 40, RandRotTwoPi, 0.5f * Main.rand.NextFloat(0.75f, 0.95f), 2).Spawn();
            SoundEngine.PlaySound(SoundID.NPCHit42 with { Pitch = 0.4f }, Projectile.Center);
            Vector2 spawnPos = Projectile.Center + Projectile.SafeDir() * 10f;
            for (int i = 0; i < 2; i++)
            {
                new LightningParticle(spawnPos.ToRandCirclePos(64f), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), Main.rand.Next(30, 40), RandRotTwoPi, 0.2f).Spawn();
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = Projectile.velocity.ToSafeNormalize();
                float rotvalue = ToRadians(360f / 4f * i) * 1f;
                float scale = (i % 2 == 0) ? 0.65f : 0.65f;
                new LightningGlow(spawnPos, dir.RotatedBy(rotvalue), Color.RoyalBlue, 50, scale).Spawn();
                new LightningGlow(spawnPos, dir.RotatedBy(rotvalue), Color.RoyalBlue, 50, scale).Spawn();
            }
            Vector2 pos = spawnPos;
            for (int j = 0; j < 8; j++)
            {
                Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(30f), 1.2f, 8.8f);
                new SmokeParticle(pos.ToRandCirclePos(10f), RandVelTwoPi(-3.8f, 6.6f) + vel2 - vel2 * Main.rand.NextFloat(0.1f, 1.2f), RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), 40, RandRotTwoPi, 1f, 0.45f).SpawnToPriorityNonPreMult();
            }
            for (int j = 0; j < 4; j++)
            {
                Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(30f), 1.2f, 9.8f);
                Vector2 pos2 = pos.ToRandCirclePos(12f) + vel2 * 0.32f;
                new StarShape(pos2, vel2, RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), 0.8f, 40).Spawn();
            }
            for (int j = 0; j < 6; j++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(6f);
                new ShinyCrossStar(pos2, RandVelTwoPi(1.2f, 4f), RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, 0, 1, 0.75f, false).Spawn();
            }
            for (int j = 0; j < 6; j++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(6f);
                Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(30f), 1.2f, 9.8f);
                new ShinyCrossStar(pos2, vel2, RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, 0, 1, 0.75f, false).Spawn();
            }

            //for (int i = 0; i < 12; i++)
            //{
            //    new SmokeParticle(spawnPos.ToRandCirclePos(6f), RandVelTwoPi(1.2f, 8.4f), RandLerpColor(Color.SkyBlue, Color.CadetBlue), 40, RandRotTwoPi, 1f, 0.43f).SpawnToPriorityNonPreMult();
            //}
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
