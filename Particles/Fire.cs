using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using HJScarletRework.Globals.ParticleSystem;
using Terraria;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Particles
{
    public class Fire : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public Fire(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Texture2D texture = HJScarletTexture.Particle_Fire.Value;

            Rectangle frame = HJScarletTexture.Particle_Fire.Frame(8, 8, (int)(LifetimeRatio * 64) % 8, (int)(LifetimeRatio * 8));
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * brightness * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
