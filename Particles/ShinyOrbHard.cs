using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Particles
{
    public class ShinyOrbHard : BaseParticle
    {
        public int BlendStateType;
        public override int UseBlendStateID => BlendStateType;
        public bool AffectedByGravity = false;
        public bool GlowCenter = true;
        public Color InitColor;
        public float GlowCenterScale = 0.5f;
        public ShinyOrbHard(Vector2 position, Vector2 velocity, Color color, int lifeTime, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = InitColor = color;
            Lifetime = lifeTime;
            Scale = scale;
        }
        public override void Update()
        {
            Scale *= 0.93f;
            DrawColor = Color.Lerp(InitColor, InitColor * 0.2f, (float)Math.Pow(LifetimeRatio, 30));
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = Velocity.ToRotation();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(1f, 1f) * Scale;
            Texture2D texture = HJScarletTexture.Particle_ShinyOrb.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
        }
    }
}
