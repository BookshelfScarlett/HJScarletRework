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
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ContainedBlastShockBullet : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 3;
            Projectile.height = Projectile.width = 12;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 2;
            Projectile.Opacity = 0;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.1f);
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(4);
                float scale = 0.34f * Main.rand.NextFloat(0.7f, 1.1f) * Projectile.Opacity;
                ECSParticle.LightntingGlow(pos + Projectile.SafeDir() * 5f, Projectile.velocity / 2f, RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1f, scale);
                ECSParticle.LightntingGlow(pos, Projectile.velocity / 2f, RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1f, scale);
            }
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 2f, RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1f, 0.64f * Main.rand.NextFloat(0.7f, 1.1f), 0.2f);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] > 0)
                modifiers.SetCrit();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<ContainedBlast>());
            if(Projectile.numHits > 0 && Projectile.ai[0] > 0)
            {
                Projectile.AddExecutionTimeDelayed(ItemType<ContainedBlast>());
            }
            SoundEngine.PlaySound(SoundID.DD2_GoblinBomb with { MaxInstances = 1, Pitch = 0.1f}, Projectile.Center);
            Vector2 dir = Projectile.SafeDir();
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(8);
                Vector2 vel = Projectile.SafeDir().ToRandVelocity(ToRadians(0), 1.2f, 18f);
                ECSParticle.LightntingGlow(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(20, 30), 1f, Main.rand.NextFloat(.75f, 1.1f) * .55f, 2);
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2);
                Vector2 vel = Projectile.SafeDir().ToRandVelocity(ToRadians(15), 1.2f, 11.8f);
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 55), RandRotTwoPi, .45f, 0.30f * Main.rand.NextFloat(.7f, 1.1f), true, BlendState.Additive);
            }
            for (int i = 0; i < 14; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2);
                Vector2 vel = Projectile.SafeDir().ToRandVelocity(ToRadians(15), 1.2f, 11.8f);
                ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), Main.rand.Next(35, 55), RandRotTwoPi, .41f, 0.09f * Main.rand.NextFloat(.7f, 1.1f), BlendState.Additive);
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(2);
                new TurbulenceGlowOrb(pos, 6.4f, RandLerpColor(Color.White, Color.WhiteSmoke), 60, 0.23f * Main.rand.NextFloat(.7f, .9f), RandRotTwoPi).Spawn();
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2);
                Vector2 vel = Projectile.SafeDir().ToRandVelocity(ToRadians(15), 1.2f, 11.8f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.WhiteSmoke), 35, .585f, 0.45f * Main.rand.NextFloat(.75f, 1f), 0.2f);
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            float rotFixer = PiOver2 + Pi;
            int length = Projectile.oldPos.Length / 2;
            Texture2D sharpTear = HJScarletTexture.Particle_SharpTear;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = i / (float)length;
                Vector2 lerpPos = Projectile.Center - Projectile.SafeDir() * (length - i) * 14f;
                Vector2 pos = lerpPos - Main.screenPosition;
                float rot = Projectile.oldRot[i];
                Color color = Color.Lerp(Color.White, Color.WhiteSmoke, ratios).ToAddColor() * 0.50f * Clamp(Projectile.velocity.Length(), 0f, 1f) * Projectile.Opacity;
                Vector2 scale = new Vector2(0.55f, 1f);
                SB.Draw(sharpTear, pos, null, color * .75f, rot + PiOver2, sharpTear.ToOrigin(), Projectile.scale * scale, 0, 0);
            }
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = i / (float)length;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.12f);
                Vector2 pos = lerpPos + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i];
                Color color = Color.Lerp(Color.White, Color.Transparent, ratios).ToAddColor() * 0.964f * Projectile.Opacity;
                SB.Draw(tex, pos, null, color, rot + rotFixer, tex.ToOrigin(), Projectile.scale, 0, 0);
            }
            Vector2 projPos = Projectile.Center - Main.screenPosition;
            Vector2 projScale = Projectile.scale * new Vector2(0.75f, 1.2f);
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(tex, projPos + (TwoPi / 16f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(50) * 0.5f * Projectile.Opacity, Projectile.rotation + rotFixer, tex.ToOrigin(), projScale, 0, 0);
            }
            SB.Draw(tex, projPos, null, Color.White.ToAddColor(180) * Projectile.Opacity, Projectile.rotation + rotFixer, tex.ToOrigin(), projScale, 0, 0);
            return false;
        }
    }
}
