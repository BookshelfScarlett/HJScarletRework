using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Particles
{
    public class ShinyOrbParticle : BaseParticle
    {
        public int BlendStateType;
        public override int UseBlendStateID => BlendStateType;
        public bool AffectedByGravity;
        public bool GlowCenter;
        public float FadeOut;
        public Color InitColor;
        public float GlowCenterScale;
        public ShinyOrbParticle(Vector2 position, Vector2 velocity, Color color, int lifeTime, float scale, int? blendState = null, bool affactedByGravity = false, bool glowCenter = true, float glowCenterScale = 0.5f)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = InitColor = color;
            Lifetime = lifeTime;
            Scale = scale;
            BlendStateType = blendState ?? BlendStateID.Additive;
            AffectedByGravity = affactedByGravity;
            GlowCenter = glowCenter;
            GlowCenterScale = glowCenterScale;
            FadeOut = 1f;
        }
        public override void Update()
        {
            FadeOut -= 0.05f;
            Scale *= 0.93f;
            DrawColor = Color.Lerp(InitColor, InitColor * 0.2f, (float)Math.Pow(LifetimeRatio, 30));
            Velocity *= 0.95f;
            if(Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = Velocity.ToRotation() + PiOver2;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(1f, 1f) * Scale;
            Texture2D texture = HJScarletTexture.Particle_ShinyOrb.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            if (GlowCenter)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * FadeOut, Rotation, texture.Size() * 0.5f, scale * GlowCenterScale, 0, 0f);
        }
    }
}
