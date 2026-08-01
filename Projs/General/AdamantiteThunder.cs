using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.ExecutorVanillaHead;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.General
{
    public class AdamantiteThunder: HJScarletProj
    {
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
            Projectile.SetupImmnuity(2);
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
                //new LightningParticle(Projectile.Center.ToRandCirclePos(3f), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), Main.rand.Next(25, 45), Projectile.rotation + PiOver2, Main.rand.NextFloat(0.3f, 0.44f) * 1.7f, 0).Spawn();
                Vector2 pos = Projectile.Center.ToRandCirclePos(3);
                int lifeTime = Main.rand.Next(25, 45);
                float scale = Main.rand.NextFloat(.3f, .44f) * 1.7f;
                ECSParticle.HighResolutionThunder(pos, Vector2.Zero, RandLerpColor(Color.RosyBrown, Color.DodgerBlue), lifeTime, 1f, Projectile.rotation + PiOver2, scale, 0);
                float glowScale = 1.11f;
                ECSParticle.LightntingGlow(Projectile.Center, Projectile.SafeDir(), Color.RoyalBlue, 30, 1f, glowScale);
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePos(4), Projectile.SafeDir(), Color.White, 30, 1f, glowScale*.65f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.NextBool(AdamantiteHeadExecutor.ThunderCritChance))
                modifiers.SetCrit();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ECSParticle.HighResolutionThunder(Projectile.Center, Vector2.Zero, Color.RoyalBlue, 40,1, RandRotTwoPi, 0.5f * Main.rand.NextFloat(0.75f, 0.95f), 2);
            ScarletSound(HJScarletSounds.Lightning_Quick, Projectile.Center, 0.75f, 1, 0.4f);
            Vector2 spawnPos = Projectile.Center + Projectile.SafeDir() * 10f;
            for (int i = 0; i < 2; i++)
            {
                ECSParticle.HighResolutionThunder(spawnPos.ToRandCirclePos(64f), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), Main.rand.Next(30, 40), 1, RandRotTwoPi, 0.2f,0);
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = Projectile.velocity.ToSafeNormalize();
                float rotvalue = ToRadians(360f / 4f * i) * 1f;
                float scale = .75f;
                ECSParticle.LightntingGlow(spawnPos, dir.RotatedBy(rotvalue), Color.RoyalBlue, 50, 1, scale);
                ECSParticle.LightntingGlow(spawnPos, dir.RotatedBy(rotvalue), Color.RoyalBlue, 50, 1, scale);
            }
            Vector2 pos = spawnPos;
            for (int j = 0; j < 12; j++)
            {
                Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(30f), 1.2f, 8.8f);
                ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(10f), RandVelTwoPi(-3.8f, 6.6f) + vel2 - vel2 * Main.rand.NextFloat(0.1f, 1.2f), RandLerpColor(Color.DeepSkyBlue, Color.RoyalBlue), 40, 1f, 0.45f,.2f);
            }
            for (int j = 0; j < 4; j++)
            {
                Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(30f), 1.2f, 9.8f);
                Vector2 pos2 = pos.ToRandCirclePos(12f) + vel2 * 0.32f;
                ECSParticle.StarShape(pos2, vel2, RandLerpColor(Color.DodgerBlue, Color.RoyalBlue), 40, 1, .8f);
            }
            for (int j = 0; j < 6; j++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(6f);
                ECSParticle.ShinyCrossStarECS(pos2, RandVelTwoPi(1.2f, 4f), RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), 40, 1, 0.75f, .2f);
            }
            for (int j = 0; j < 6; j++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(6f);
                Vector2 vel2 = (Projectile.velocity).ToRandVelocity(ToRadians(30f), 1.2f, 9.8f);
                ECSParticle.ShinyCrossStarECS(pos2, vel2, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), 40, 1, 0.75f, .2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
