using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Particles
{
    public class SmokeParticle : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public SmokeParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
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
            Texture2D texture = HJScarletTexture.Particle_Smoke.Value;

            Rectangle frame = HJScarletTexture.Particle_Smoke.Texture.Frame(4, 4, (int)(LifetimeRatio * 16) % 4, (int)(LifetimeRatio * 4));
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * brightness * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
