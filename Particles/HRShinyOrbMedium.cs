using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Particles
{
    public class HRShinyOrbMedium : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;

        public float Speed = 5f;

        public int SeedOffset = 0;

        public float BeginScale = 1f;
        public HRShinyOrbMedium(Vector2 position, Vector2 velocity, Color color, int lifetime, float opacity, float scale, float speed)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            Speed = speed;
        }
        public HRShinyOrbMedium(Vector2 position, Color color, int lifetime, float scale, float speed)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            BeginScale = scale;
            Speed = speed;
        }

        public override void OnSpawn()
        {
            SeedOffset = Main.rand.Next(0, 100000);
        }

        public override void Update()
        {
            if (Speed != 0)
            {
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(Lerp(-TwoPi, TwoPi, (float)Math.Sin(Time / 36f + SeedOffset) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = Lerp(0.01f, 0.25f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
                Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
                Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            Velocity *= 0.9f;
            Scale = Lerp(BeginScale, 0, EaseOutCubic(LifetimeRatio));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_HRShinyOrbMedium.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, 0, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
