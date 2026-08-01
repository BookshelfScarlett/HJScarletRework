using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Vanity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class TitaniumShardHoming : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(Globals.Enums.VanillaAsset.Projectile, ProjectileID.TitaniumStormShard);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(-1);
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
        }
        public override void OnFirstFrame()
        {
            Projectile.frame = Main.rand.Next(0, 12);
            ScarletSound(SoundID.Item109,Projectile.Center,volume:.65f,pitch:0.6f,pitchVariance:0.3f);
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center, RandVelTwoPi(0.2f, 4.2f), Color.White, 40, 1, 0.36f);
            }
            for (int i = 0; i < 4; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center, RandVelTwoPi(0.2f, 6.2f), RandLerpColor(Color.Gray, Color.White), 40, 1, 0.9f, 0.231f, blendstate: BlendState.AlphaBlend);
            }

        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0]++;
            if (Projectile.ai[0] > Projectile.MaxUpdates * 15)
                Projectile.tileCollide = true;
            if (Projectile.GetTargetSafe(out NPC target))
            {
                Projectile.HomingTarget(target.Center, -1, 14f, 20f, 8f);
            }
            else
            {
                if (Projectile.velocity.LengthSquared() < 14f * 14f)
                    Projectile.velocity *= 1.1f;
            }
            if (Projectile.IsOutScreen())
                return;
            if(Main.rand.NextBool(6) &&(Projectile.velocity.Length() > Main.rand.NextFloat(5f,6f)))
            ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 8f, RandLerpColor(Color.Gray, Color.White), 45, 1, Main.rand.NextFloat(.75f, 1.15f) * .4f);
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 8f, RandLerpColor(Color.Gray, Color.White), 45, 1, Main.rand.NextFloat(.75f, 1.15f) * .3f, .2f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ScarletSound(HJScarletSounds.Misc_Ding, Projectile.Center, volume: .45f, pitch: .65f, pitchVariance: .2f);
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center, RandVelTwoPi(0.2f, 4.2f), Color.White, 40, 1, 0.36f);
            }
            for (int i = 0; i < 4; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center, RandVelTwoPi(0.2f, 6.2f), RandLerpColor(Color.Gray, Color.White), 40, 1, 0.9f, 0.231f, blendstate: BlendState.AlphaBlend);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(12, 1, Projectile.frame, 0);
            Vector2 ori = frame.Size() / 2;
            float rotFix = -PiOver2;
            int trailLength = Projectile.oldPos.Length;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            DrawBloomTrail(trailLength, pos);
            DrawShardTrail(trailLength, tex, frame, rotFix, ori);
            DrawShard(tex, pos, frame, ori, rotFix);
            return false;
        }

        public void DrawBloomTrail(int trailLength, Vector2 pos)
        {
            Texture2D tex = HJScarletTexture.Particle_SharpTear;
            for (int i = trailLength - 1; i >= 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float ratios = i / (float)trailLength;
                Vector2 scale = Vector2.Lerp( new Vector2(0.66f, 1.44f), new Vector2(0.25f,0.8f),ratios);
                Color drawColor = (Color.Lerp(Color.White, Color.Gray, ratios).ToAddColor(50)) * 0.94f * Projectile.Opacity * (1 - ratios);
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.PosToCenter() - Projectile.SafeDir() * 15f;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                SB.Draw(tex, trailPos, null, drawColor, oldRot, tex.Size() / 2, scale, 0, 0);
            }
        }

        public void DrawShardTrail(int trailLength, Texture2D tex, Rectangle frame,float rotFix, Vector2 ori)
        {
            for (int i = trailLength - 1; i >= 0; i--)
            {
                float ratios = i / (float)trailLength;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float trailRot = Projectile.oldRot[i];
                float opac = Lerp(0.64f, 0.88f, (1 - ratios));
                float trailScale = Lerp(.25f, 1f, (1 - ratios)) * Projectile.scale;
                Color trailC = Color.Lerp(Color.White, Color.White, ratios).ToAddColor(200) * opac;
                SB.Draw(tex, trailPos, frame, trailC, trailRot + rotFix, ori, trailScale, 0, 0);
            }

        }
        public void DrawShard(Texture2D tex, Vector2 pos, Rectangle frame, Vector2 ori, float rotFix)
        {
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, pos + (TwoPi / 8f * i).ToRotationVector2() * 2f, frame, Color.White.ToAddColor(), Projectile.rotation + rotFix, ori, Projectile.scale, 0, 0);
            SB.Draw(tex, pos, frame, Color.White.ToAddColor(210), Projectile.rotation + rotFix, ori, Projectile.scale, 0, 0);
        }
    }
}
