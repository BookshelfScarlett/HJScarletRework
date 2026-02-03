using HJScarletRework.Assets.Registers;
using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Rarity.RarityParticles
{
    public class SakuraPetals : RaritySparkle
    {
        public float Speed = 5f;
        public float BeginScale = 1f;
        public int Rotdir = 1;

        public SakuraPetals(Vector2 position,Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float speed)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            Rotdir = Main.rand.NextBool().ToDirectionInt();
            Speed = speed;
        }
        public override void CustomUpdate()
        {
            Vector2 idealVelocity = Vector2.UnitY.RotatedBy(Lerp(-0.94f, 0.94f, (float)Math.Sin(Time / 36f + Main.rand.NextFloat(0,10000)) * 0.5f + 0.5f)) * Speed;
            float movementInterpolant = Lerp(0.05f, 0.1f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
            Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
            Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            Position += Velocity;
            Rotation += 0.05f * Rotdir;
            Scale = Lerp(BeginScale, 0, EaseInCubic(LifetimeRatio));
        }

        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            Texture2D texture = HJScarletTexture.Particle_Petal.Value;
            spriteBatch.Draw(texture, drawPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
