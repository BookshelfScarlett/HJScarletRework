using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Graphics.Particles
{
    public class LightningParticle : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        private int XFrames = 0;
        private int YFrames = 0;
        private int Type = 0;
        public LightningParticle(Vector2 position, Vector2 velocity, Color color, int lifeTime, float rotation, float scale, int type = 0)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifeTime;
            Rotation = rotation;
            Scale = scale;
            Type = type;
        }
        public override void OnSpawn()
        {
            switch (Type)
            {
                case 1:
                    break;
                case 2:
                    XFrames = Main.rand.Next(0, 2);
                    YFrames = Main.rand.Next(0, 2);
                    break;
                default:
                    XFrames = Main.rand.Next(0, 4);
                    YFrames = Main.rand.Next(0, 2);
                    break;
            }
        }
        public override void Update()
        {
            Opacity = Lerp(1f, 0f, EaseOutCubic(LifetimeRatio));
            Opacity = MathF.Pow(Opacity, 0.5f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Type switch
            {
                1 => HJScarletTexture.Particle_Lightning1.Value,
                2 => HJScarletTexture.Particle_Lightning2.Value,
                _ => HJScarletTexture.Particle_Lightning0.Value,
            };
            Rectangle frame = Type switch
            {
                1 => texture.Frame(),
                2 => texture.Frame(2, 2, XFrames, YFrames),
                _ => texture.Frame(4, 2, XFrames, YFrames),
            };
            Vector2 ori = frame.Size() / 2f;
            switch (Type)
            {
                case 1:
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    break;
                case 2:
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.White * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    break;
                default:
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.White * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.White * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, ori, Scale, SpriteEffects.None, 0f);
                    break;
            }


        }
    }
}
