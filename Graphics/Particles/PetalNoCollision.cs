using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Graphics.Particles
{
    public class PetalNoCollision : BaseParticle
    {
        public float Speed = 5f;

        public int SeedOffset = 0;

        public float BeginScale = 1f;

        public int Rotdir = 1;

        // 追踪粒子是否已经着陆
        private bool FullLighting = false;
        public PetalNoCollision(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float speed, bool fullLighting = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            Speed = speed;
            FullLighting = fullLighting;
        }
        public override void OnSpawn()
        {
            SeedOffset = Main.rand.Next(0, 100000);
            Rotdir = Main.rand.NextBool() ? 1 : -1;
            Rotation = Main.rand.NextFloat(0, TwoPi);
        }

        public override void Update()
        {
            if (Speed != 0)
            {
                Vector2 idealVelocity = Vector2.UnitY.RotatedBy(Lerp(-0.94f, 0.94f, (float)Math.Sin(Time / 36f + SeedOffset) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = Lerp(0.05f, 0.1f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
                Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
                Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            Rotation += 0.1f * Rotdir;
            Scale = Lerp(BeginScale, 0, EaseInCubic(LifetimeRatio));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_Petal.Value;
            Color drawColor = FullLighting ? DrawColor * Opacity : DrawColor * Opacity * Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f));
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, drawColor , Rotation, texture.Size() / 2, Scale, 0, 0);
        }
    }
}
