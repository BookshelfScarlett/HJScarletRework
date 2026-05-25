using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class DiverJellyFish : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public int TextureType = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = GetSeconds(10);
            Projectile.extraUpdates = 2;
            Projectile.width = Projectile.height = 100;
            Projectile.SetupImmnuity(20);
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            TextureType = Main.rand.NextFromList<int>(NPCID.GreenJellyfish, NPCID.BlueJellyfish, NPCID.PinkJellyfish);
        }
        public override void ProjAI()
        {
            Projectile.velocity *= 0.90f;
            UpdateParticle();
            if (Projectile.FinalUpdate())
            {
                float osci = (float)(Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
                Projectile.position.Y += osci;
            }

            Projectile.timeLeft = GetSeconds(10);
            Projectile.AddFrames(16 * Projectile.MaxUpdates, 4);
        }
        public void UpdateParticle()
        {
                switch (TextureType)
                {
                    case NPCID.GreenJellyfish:
                    ParticleHandler(Color.LimeGreen, Color.DarkGreen);
                        break;
                    case NPCID.PinkJellyfish:
                    ParticleHandler(Color.HotPink, Color.Purple);
                        break;
                    case NPCID.BlueJellyfish:
                    ParticleHandler(Color.RoyalBlue, Color.CornflowerBlue);
                    
                        break;

                }
        }

        private void GreenParticle()
        {

        }
        private void PinkParticle()
        {
        }
        private void ParticleHandler(Color color1, Color color2)
        {
            if (Projectile.FinalUpdateNextBool(6))
                new StarShape(Projectile.Center.ToRandCirclePos(30), Vector2.UnitY * Main.rand.NextFloat(1f, 3f) * .2f, RandLerpColor(color1, color2), Projectile.scale * 0.5f, 40, true).Spawn();
            //new LightningParticle(Projectile.Center.ToRandCirclePos(30), Vector2.Zero, RandLerpColor(color1, color2), 40, PiOver2+ Vector2.UnitY.RotatedByRandom(ToRadians(10f)).ToRotation(), Projectile.scale * 0.30f, 0).Spawn();
            if (Projectile.FinalUpdateNextBool(6))
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(30);
                    p.Velocity = Vector2.UnitY * Main.rand.NextFloat(1.3f, 4.8f) * .2f;
                    p.DrawColor = RandLerpColor(color1, color2);
                    p.Scale = Main.rand.NextFloat(0.9f, 1.1f) * 0.10f;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.6f;
                    p.Lifetime = 40;
                });
        }
        private void BlueParticle()
        {
            if(Projectile.FinalUpdateNextBool(6))
            new LightningParticle(Projectile.Center.ToRandCirclePos(30), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.CornflowerBlue), 40, PiOver2+ Vector2.UnitY.RotatedByRandom(ToRadians(10f)).ToRotation(), Projectile.scale * 0.30f, 0).Spawn();
            if (Projectile.FinalUpdateNextBool(6))
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = Projectile.Center.ToRandCirclePos(30);
                    p.Velocity = Vector2.UnitY * Main.rand.NextFloat(1.3f, 4.8f);
                    p.DrawColor = RandLerpColor(Color.RoyalBlue, Color.CornflowerBlue);
                    p.Scale = Main.rand.NextFloat(0.9f, 1.1f) * 0.10f;
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.6f;
                    p.Lifetime = 40;
                });
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D jellyfish = TextureAssets.Npc[TextureType].Value;
            Rectangle frame = jellyfish.Frame(1, 7, 0, Projectile.frame);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            DrawJellyfish(jellyfish, frame, ori, pos);
            DrawColorfulRing(pos);
            return false;
        }

        public void DrawJellyfish(Texture2D jellyfish, Rectangle frame, Vector2 ori, Vector2 pos)
        {
            SB.Draw(jellyfish, pos - Vector2.UnitY * 5f, frame, Color.White, Projectile.rotation, ori, Projectile.scale, 0, 0);
        }
        public void DrawColorfulRing(Vector2 pos)
        {
            switch (TextureType)
            {
                case NPCID.BlueJellyfish:
                    DrawRing(pos, Color.RoyalBlue, Color.CornflowerBlue);
                    break;
                case NPCID.GreenJellyfish:
                    DrawRing(pos, Color.LimeGreen, Color.DarkGreen);
                    break;
                case NPCID.PinkJellyfish:
                    DrawRing(pos, Color.HotPink, Color.Purple);
                    break;
            }
        }
        public void DrawRing(Vector2 pos, Color color1, Color color2)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            float scale = Projectile.scale * 0.35f;
            scale = Projectile.scale * 0.15f;
            Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
            SB.Draw(ring, pos, null, color1* 0.45f, Projectile.rotation, ring.ToOrigin(), scale, 0, 0);
            SB.Draw(ring, pos, null, color1*0.45f, Projectile.rotation + PiOver2, ring.ToOrigin(), scale, 0, 0);
            SB.Draw(ring, pos, null, color2*.45f, Projectile.rotation + Pi, ring.ToOrigin(), scale, 0, 0);
            SB.Draw(ring, pos, null, color2*.45f, Projectile.rotation - PiOver2, ring.ToOrigin(), scale, 0, 0);
            ring = HJScarletTexture.Texture_BloomRing.Value;
            scale = Projectile.scale * 0.41f;
            SB.Draw(ring, pos, null, color2*.25f, Projectile.rotation - PiOver2, ring.ToOrigin(), scale, 0, 0);
            SB.EndShaderArea();

        }
    }
}
