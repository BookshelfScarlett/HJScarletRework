using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Particles
{
    public class HRShinyOrb: BaseParticle
    {
        public bool UseRot = false;
        public override int UseBlendStateID => BlendStateID.Additive;
        public HRShinyOrb(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }

        public HRShinyOrb(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useRot)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            UseRot = useRot;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            if (!UseRot)
                Velocity *= 0.9f;
            else
                Velocity *= 0.94f;
            Scale = Lerp(Scale, 0, EaseInCubic(LifetimeRatio));

            if (UseRot)
            {
                Velocity = Velocity.RotatedBy(0.03f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_HRShinyOrb.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
