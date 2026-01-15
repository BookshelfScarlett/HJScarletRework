using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Particles
{
    public class FusableBall : BaseParticle
    {
        public bool UsePreMult = false;
        public override int UseBlendStateID => BlendStateID.NonPremult;
        public Vector2 Scale2;
        public FusableBall(Vector2 position, Vector2 velocity, Color color, int lifetime, float opacity, Vector2 scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale2 = scale;
            Important = true;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Scale = MathHelper.Lerp(Scale, 0, EaseInCubic(LifetimeRatio));
            Rotation = Velocity.ToRotation();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale * Scale2, SpriteEffects.None, 0);
        }
    }
}
